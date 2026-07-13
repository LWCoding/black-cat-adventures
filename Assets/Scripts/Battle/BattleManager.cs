using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : Singleton<BattleManager>
{

    [Header("Object Assignments")]
    public PlayerHandler PlayerHandler;
    public EnemyHandler CurrEnemyHandler;
    [SerializeField] private EnemyInfoBox _enemyInfoBox;
    [SerializeField] private SubmitButton _submitButton;
    [SerializeField] private ShuffleButton _shuffleButton;
    public ShuffleButton ShuffleButton => _shuffleButton;

    [Header("Audio")]
    [SerializeField] private AudioClip _battleMusic;

    public State CurrentState;

    public Action OnPlayerAttack = null;
    public Action OnEnemyAttack = null;
    public Action<State> OnStateChanged = null;  // Parameter is the state to transition to
    public Action<EnemyHandler> OnNewEnemySet = null;  // Parameter is the new enemy entering
    public Action OnReachedLastEnemy = null;
    public Action OnTreasureChestSet = null;  // Fired when the treasure chest becomes the active battle object

    private void Start()
    {
        // Kick off looping battle music. The AudioManager keeps this playing
        // seamlessly across scene reloads (e.g. advancing between enemies) and
        // stops it automatically once we leave the battle scene.
        AudioManager.Instance.PlayMusic(_battleMusic);

        // Genie Lamp curse: if the previous event requested gold-etched tiles,
        // scramble half the board now (deferred one frame so InitializeBoard
        // has already run). Clear and save immediately so a retry of a lost
        // battle doesn't re-apply the penalty.
        if (GameManager.GameData.NextBattleGoldEtched && WordGrid.Instance != null)
        {
            GameManager.GameData.NextBattleGoldEtched = false;
            SaveManager.SaveGame(GameManager.GameData);
            int half = (WordGrid.Instance.NUM_ROWS * WordGrid.Instance.NUM_COLUMNS) / 2;
            RunNextFrame(() => WordGrid.Instance.ScrambleTiles(half));
        }

        // Only jump straight to the player's turn when the first enemy has no intro
        // dialogue that stalls the battle. When it does stall (e.g. the tutorial's
        // opening lines), SetNewEnemy's dialogue coroutine owns the
        // WaitState -> PlayerTurnState transition itself. Firing PlayerTurnState here
        // as well would emit a second PlayerTurnState (off-by-one for anything
        // counting turns, like the tutorial) and let the player act before the
        // dialogue has played. Mirrors EnemyHandler.TransitionToNextObject.
        if (!CurrEnemyHandler.ShouldStallBeforeTurn
            && (CurrEnemyHandler.DialogueToPlayOnMeet.Count == 0
                || !CurrEnemyHandler.DialogueToPlayOnMeet[0].ShouldStallState))
        {
            SetState(new PlayerTurnState()); // Start off as player turn
        }
        CurrEnemyHandler.ApplyStartingStatuses();
        SetNewEnemy(CurrEnemyHandler);
    }

    public void SetState(State s)
    {
        OnStateChanged?.Invoke(s);
        CurrentState?.OnExitState();  // Exit from the current state, if any
        CurrentState = s;
        CurrentState.OnEnterState();
    }

    /// <summary>
    /// QOL improvements with rendering specific actions through key presses.
    /// </summary>
    private void Update()
    {
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused) { return; }

        // If backspace is pressed, remove last letter
        if (Input.GetKeyDown(KeyCode.Backspace) && WordPreview.Instance.CurrentTiles.Count > 0)
        {
            WordPreview.Instance.RemoveTile(WordPreview.Instance.CurrentTiles[^1]);
        }

        // If return is pressed, try to submit word
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _submitButton.TrySubmitCurrentWord();
        }

        // If tab is pressed, try to shuffle
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _shuffleButton.TryShuffleBoard();
        }

        // Keys 1-5 trigger the corresponding equipped treasure slot.
        if (TreasureSection.Instance != null)
        {
            for (int i = 0; i < GameData.MaxEquippedTreasures; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    TreasureSection.Instance.TryTriggerSlot(i);
                }
            }

            // Escape cancels active treasure targeting.
            if (Input.GetKeyDown(KeyCode.Escape) && TreasureSection.Instance.IsAwaitingTarget)
            {
                TreasureSection.Instance.CancelTargeting();
            }
        }
    }

    void OnGUI()
    {
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused) { return; }

        // If a letter is pressed, try to find that letter in the grid and add it
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            char keyChar = e.keyCode.ToString()[0];
            // Look for a letter with that matching letter
            for (int i = 0; i <  WordGrid.Instance.LetterTiles.Count; i++)
            {
                string tileLetters = WordGrid.Instance.LetterTiles[i].GetLetters();
                if (tileLetters.Length > 0 && tileLetters[0] == keyChar) 
                {
                    if (((GridLetterTile)(WordGrid.Instance.LetterTiles[i])).TrySelectTile())
                    {
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Render a new enemy, updating any UI if needed.
    /// Also queues any dialogue to be played if applicable.
    /// </summary>
    public void SetNewEnemy(EnemyHandler newEnemyHandler)
    {
        OnNewEnemySet?.Invoke(newEnemyHandler);
        CurrEnemyHandler = newEnemyHandler;
        _enemyInfoBox.SetInfo((EnemyData)(newEnemyHandler.CharData));
        newEnemyHandler.StatusHandler.RevealStatusIcons();
        // If the player is dead, stop here.
        if (CurrentState is LoseState) { return; }
        // If there's any dialogue to play, play it!
        if (newEnemyHandler.DialogueToPlayOnMeet.Count > 0)
        {
            StartCoroutine(RenderDialogueCoroutine(newEnemyHandler.DialogueToPlayOnMeet));
        }
    }

    private IEnumerator RenderDialogueCoroutine(List<DialogueInfo> diList)
    {
        bool wasStalled = false;

        while (diList.Count > 0)
        {
            DialogueInfo di = diList[0];
            diList.RemoveAt(0);
            if (di.ShouldStallState)
            {
                wasStalled = true;
                SetState(new WaitState());
            }
            if (di.Speaker == Faction.PLAYER)
            {
                PlayerHandler.SayDialogue(di);
            }
            else if (di.Speaker == Faction.ENEMY)
            {
                CurrEnemyHandler.SayDialogue(di);
            }
            // Wait until the dialogue is finished, before the next one
            bool dialogueFinished = false;
            DialogueBoxHandler.OnDialogueComplete = () => {
                dialogueFinished = true; 
            };
            yield return new WaitUntil(() => dialogueFinished);
        }

        // If the dialogue was stalled, restore the state at the end
        if (wasStalled)
        {
            SetState(new PlayerTurnState());
        }
    }

    /// <summary>
    /// Deal damage (as the player) to the enemy.
    /// </summary>
    public void RenderAttackAgainstEnemy(int damage)
    {
        // Make the enemy actually take the damage.
        CurrEnemyHandler.HealthHandler.TakeDamage(damage);
    }

    /// <summary>
    /// Render damage (as the enemy) to the player.
    /// </summary>
    public void RenderAttackAgainstPlayer(EnemyAttack attack)
    {
        // If any effects should be applied, apply them.
        foreach (AttackStatus effect in attack.InflictedStatuses)
        {
            if (effect.Target == Faction.ENEMY)
            {
                CurrEnemyHandler.StatusHandler.GainStatusEffect(effect.Status, effect.Amplifier);
            }
            if (effect.Target == Faction.PLAYER)
            {
                PlayerHandler.StatusHandler.GainStatusEffect(effect.Status, effect.Amplifier);
            }
        }
        // Make the player actually take the damage.
        PlayerHandler.HealthHandler.TakeDamage(attack.Damage);
        // Apply any board effects (e.g. Scramble tiles).
        if (attack.BoardEffects != null)
        {
            foreach (AttackBoardEffect boardEffect in attack.BoardEffects)
            {
                if (boardEffect.Effect != null)
                {
                    boardEffect.Effect.Apply(boardEffect.Amplifier);
                }
            }
        }
    }

    /// <summary>
    /// Load the current scene *again* after a certain delay.
    /// </summary>
    public void ReloadCurrentLevel(int delayInSecs)
    {
        Invoke(nameof(LoadLevel), delayInSecs);
    }

    private void LoadLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    /// <summary>
    /// Run specific code after the next frame, to avoid
    /// conflicts that occur due to priority.
    /// </summary>
    public void RunNextFrame(Action codeToRunAfter)
    {
        StartCoroutine(RunNextFrameCoroutine(codeToRunAfter));
    }

    private IEnumerator RunNextFrameCoroutine(Action codeToRunAfter)
    {
        yield return new WaitForEndOfFrame();
        codeToRunAfter.Invoke();
    }

}
