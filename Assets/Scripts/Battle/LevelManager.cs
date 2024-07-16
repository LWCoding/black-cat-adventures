using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    #region Singleton Logic
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }
    #endregion

    [Header("Object Assignments")]
    [SerializeField] private PlayerHandler _playerHandler;
    [SerializeField] private EnemyHandler _currEnemyHandler;

    public State CurrentState;

    public Action OnPlayerAttack = null;
    public Action OnEnemyAttack = null;
    public Action<State> OnStateChanged = null;  // Parameter is the state to transition to

    private void Start()
    {
        SetState(new PlayerTurnState()); // Start off as player turn
        SetNewEnemy(_currEnemyHandler);
    }

    public void SetState(State s)
    {
        OnStateChanged?.Invoke(s);
        CurrentState?.OnExitState();  // Exit from the current state, if any
        CurrentState = s;
        CurrentState.OnEnterState();
    }

    /// <summary>
    /// Render a new enemy.
    /// Queues any dialogue to be played if applicable.
    /// </summary>
    public void SetNewEnemy(EnemyHandler newEnemyHandler)
    {
        _currEnemyHandler = newEnemyHandler;
        if (newEnemyHandler.DialogueToPlayOnMeet.Count > 0)
        {
            StartCoroutine(RenderDialogueCoroutine(newEnemyHandler.DialogueToPlayOnMeet));
        }
    }

    private IEnumerator RenderDialogueCoroutine(List<DialogueInfo> diList)
    {
        bool wasStalled = false;
        State previousState = CurrentState;

        while (diList.Count > 0)
        {
            DialogueInfo di = diList[0];
            diList.RemoveAt(0);
            if (di.ShouldStallState)
            {
                wasStalled = true;
                SetState(new WaitState());
            }
            if (di.Speaker == DialogueFaction.PLAYER)
            {
                _playerHandler.SayDialogue(di);
            }
            else if (di.Speaker == DialogueFaction.ENEMY)
            {
                _currEnemyHandler.SayDialogue(di);
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
            SetState(previousState);
        }
    }

    /// <summary>
    /// Deal damage (as the player) to the enemy.
    /// </summary>
    public void RenderPlayerDamage()
    {
        OnPlayerAttack?.Invoke();
        int wordLength = WordPreview.Instance.CurrentWord.Length;
        // Algorithm to determine damage from word!
        int damageDealt = (int)Mathf.Round(Mathf.Exp(wordLength * 0.4f));
        // Make the enemy actually take the damage.
        _currEnemyHandler.HealthHandler.TakeDamage(damageDealt);
    }

    /// <summary>
    /// Render damage (as the enemy) to the player.
    /// </summary>
    public void RenderEnemyDamage()
    {
        OnEnemyAttack?.Invoke();
        // Make the player actually take the damage.
        _playerHandler.HealthHandler.TakeDamage(4);
    }

}
