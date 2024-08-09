using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    private static TutorialManager _instance;
    public static TutorialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<TutorialManager>();
            }
            return _instance;
        }
    }
    public static bool HasTutorialPlayed = false;

    [Header("Object Assignments")]
    [SerializeField] private GameObject _boardTooltipObject;
    [SerializeField] private GameObject _shuffleTooltipObject;
    [SerializeField] private GameObject _enemyTooltipObject;
    [SerializeField] private GameObject _treasureTooltipObject;
    [SerializeField] private GameObject _statusTooltipObject;
    [SerializeField] private GameObject _endTreasureTooltipObject;
    [SerializeField] private GameObject _specialTileTooltipObject;

    private int _playerTurnTracker = 0;  // Increments by one each time the player's turn arrives
    private int _enemiesDefeated = 0;  // Increments by one each time an enemy is defeated
    private bool _isStatusAppliedYet = false;  // Becomes true after receiving effect once
    private bool _isTreasureUnlockedYet = false;  // Becomes true after unlocking treasure
    private bool _isSpecialTileUnlockedYet = false;  // Becomes true after unlocking special tiles

    private void Awake()
    {
        if (!ReferenceEquals(_instance, this)) {
            if (_instance != null)
            {
                Destroy(this);
            } else
            {
                _instance = this;
            }
        }
        _boardTooltipObject.SetActive(false);
        _shuffleTooltipObject.SetActive(false);
        _enemyTooltipObject.SetActive(false);
        _treasureTooltipObject.SetActive(false);
        _endTreasureTooltipObject.SetActive(false);
        _specialTileTooltipObject.SetActive(false);
        // If we've never played the tutorial before, hide certain objects
        if (!HasTutorialPlayed)
        {
            ShuffleButton.Instance.gameObject.SetActive(false);
            TreasureSection.Instance.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // If we've never played the tutorial before, start it now
        if (!HasTutorialPlayed)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        HasTutorialPlayed = true;
        // Initialize board with bad tiles initially
        for (int i = 0; i < WordGrid.Instance.LetterTiles.Count; i++)
        {
            WordGrid.Instance.LetterTiles[i].SetTileText("X");
        }
        WordGrid.Instance.LetterTiles[0].SetTileText("P");
        WordGrid.Instance.LetterTiles[6].SetTileText("L");
        WordGrid.Instance.LetterTiles[9].SetTileText("A");
        WordGrid.Instance.LetterTiles[15].SetTileText("Y");

        // During the player's turn, spawn tooltip
        BattleManager.Instance.OnStateChanged += OnStateChanged;
        // When a word is submitted, hide the tooltip
        SubmitButton.OnClickButton += OnSubmitButtonPressed;
        // When a word is shuffled, hide the tooltip
        ShuffleButton.OnClickButton += OnShuffleButtonPressed;
        // When a status is applied for the first time, show status tooltip
        BattleManager.Instance.PlayerHandler.StatusHandler.OnStatusApplied += OnStatusEffectApplied;
        // When we reach the last enemy (treasure), show that tooltip
        BattleManager.Instance.OnReachedLastEnemy += OnReachedLastEnemy;
        // When we have obtained the collectible treasure, hide that tooltip
        TreasureCollectible.OnCollect += OnCollectTreasure;
        // When a new enemy is set, make sure we render the OnEnemyDies function
        BattleManager.Instance.CurrEnemyHandler.HealthHandler.OnDeath += OnEnemyDies;
        BattleManager.Instance.OnNewEnemySet += OnNewEnemyArrives;
    }

    public void OnDisable()
    {
        BattleManager.Instance.OnStateChanged -= OnStateChanged;
        SubmitButton.OnClickButton -= OnSubmitButtonPressed;
        ShuffleButton.OnClickButton -= OnShuffleButtonPressed;
        BattleManager.Instance.PlayerHandler.StatusHandler.OnStatusApplied -= OnStatusEffectApplied;
        BattleManager.Instance.OnReachedLastEnemy -= OnReachedLastEnemy;
        TreasureCollectible.OnCollect -= OnCollectTreasure;
    }

    /// <summary>
    /// Make the tiles useless after they've been submitted.
    /// Wait a frame before doing this to override properly.
    /// </summary>
    private IEnumerator WaitThenRenderTilesUseless()
    {
        yield return new WaitForEndOfFrame();
        // Initialize board with bad tiles... again!
        for (int i = 0; i < WordGrid.Instance.LetterTiles.Count; i++)
        {
            if (WordGrid.Instance.LetterTiles[i].GetLetters() != "X")
            {
                WordGrid.Instance.LetterTiles[i].SetTileText("W");
            }
        }
    }

    private void OnSubmitButtonPressed()
    {
        switch (_playerTurnTracker)
        {
            case 1:
                _boardTooltipObject.SetActive(false);
                StartCoroutine(WaitThenRenderTilesUseless());
                break;
            case 3:
                _enemyTooltipObject.SetActive(false);
                break;
        }
        // If treasure is unlocked, hide tooltip when damage is dealt
        if (_isTreasureUnlockedYet)
        {
            _treasureTooltipObject.SetActive(false);
        }
        // If status is unlocked, hide tooltip when damage is dealt
        if (_isStatusAppliedYet)
        {
            _statusTooltipObject.SetActive(false);
        }
        // If special tiles are unlocked, hide tooltip when damage is dealt
        if (_isSpecialTileUnlockedYet)
        {
            _specialTileTooltipObject.SetActive(false);
        } else
        {
            // If we haven't unlocked special tiles yet, restrict player from getting any
            StartCoroutine(WaitThenRenderTilesBoring());
        }
    }

    /// <summary>
    /// Make the tiles not have tile effects after they've been submitted.
    /// Wait a frame before doing this to override properly.
    /// </summary>
    private IEnumerator WaitThenRenderTilesBoring()
    {
        yield return new WaitForEndOfFrame();
        // Initialize board with bad tiles... again!
        for (int i = 0; i < WordGrid.Instance.LetterTiles.Count; i++)
        {
            WordGrid.Instance.LetterTiles[i].SetTileType(TileTypeName.NORMAL);
        }
    }

    private void OnShuffleButtonPressed()
    {
        _shuffleTooltipObject.SetActive(false);
    }

    private void OnNewEnemyArrives(EnemyHandler currEnemy)
    {
        currEnemy.HealthHandler.OnDeath += OnEnemyDies;
    }

    private void OnEnemyDies()
    {
        _enemiesDefeated++;
        if (_enemiesDefeated == 1)  // After defeating dummy
        {
            _isTreasureUnlockedYet = true;
            TreasureSection.Instance.gameObject.SetActive(true);
            _treasureTooltipObject.SetActive(true);
        }
        if (_enemiesDefeated == 3)
        {
            _isSpecialTileUnlockedYet = true;
            _specialTileTooltipObject.SetActive(true);
            StartCoroutine(WaitThenSpawnPoisonTile());
        }
    }

    /// <summary>
    /// Make the tiles useless after they've been submitted.
    /// Wait a frame before doing this to override properly.
    /// </summary>
    private IEnumerator WaitThenSpawnPoisonTile()
    {
        yield return new WaitForEndOfFrame();
        // Spawn poison tile!
        WordGrid.Instance.LetterTiles[9].SetTileType(TileTypeName.POISON);
    }

    private void OnStatusEffectApplied()
    {
        if (!_isStatusAppliedYet)
        {
            _statusTooltipObject.SetActive(true);
            _isStatusAppliedYet = true;
        }
    }

    private void OnReachedLastEnemy()
    {
        _endTreasureTooltipObject.SetActive(true);
    }

    private void OnCollectTreasure()
    {
        _endTreasureTooltipObject.SetActive(false);
    }

    private void OnStateChanged(State state)
    {
        if (state is not PlayerTurnState) { return; }
        _playerTurnTracker++;
        switch (_playerTurnTracker)
        {
            case 1:
                _boardTooltipObject.SetActive(true);
                break;
            case 2:
                ShuffleButton.Instance.gameObject.SetActive(true);
                _shuffleTooltipObject.SetActive(true);
                break;
            case 3:
                _enemyTooltipObject.SetActive(true);
                break;
        }
    }

}
