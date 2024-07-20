using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager Instance;
    public static bool HasTutorialPlayed = false;

    [Header("Object Assignments")]
    [SerializeField] private GameObject _boardTooltipObject;
    [SerializeField] private GameObject _shuffleTooltipObject;
    [SerializeField] private GameObject _enemyTooltipObject;
    [SerializeField] private GameObject _treasureTooltipObject;

    private int _playerTurnTracker = 0;  // Increments by one each time the player's turn arrives

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        _boardTooltipObject.SetActive(false);
        _shuffleTooltipObject.SetActive(false);
        _enemyTooltipObject.SetActive(false);
        _treasureTooltipObject.SetActive(false);
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
            WordGrid.Instance.LetterTiles[i].SetTileText("Q");
        }
        WordGrid.Instance.LetterTiles[0].SetTileText("P");
        WordGrid.Instance.LetterTiles[6].SetTileText("L");
        WordGrid.Instance.LetterTiles[9].SetTileText("A");
        WordGrid.Instance.LetterTiles[15].SetTileText("Y");

        // During the player's turn, spawn tooltip
        LevelManager.Instance.OnStateChanged += (state) =>
        {
            if (state is not PlayerTurnState) { return; }
            _playerTurnTracker++;
            switch (_playerTurnTracker) {
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
        };

        // When a word is submitted, hide the tooltip
        SubmitButton.OnClickButton += () =>
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
            if (TreasureSection.UnlockedTreasureSection)
            {
                _treasureTooltipObject.SetActive(false);
            }
        };
        // When a word is shuffled, hide the tooltip
        ShuffleButton.OnClickButton += () =>
        {
            _shuffleTooltipObject.SetActive(false);
        };
        // When an enemy is defeated, show treasure
        LevelManager.Instance.CurrEnemyHandler.HealthHandler.OnDeath += () =>
        {
            TreasureSection.UnlockedTreasureSection = true;
            TreasureSection.Instance.gameObject.SetActive(true);
            _treasureTooltipObject.SetActive(true);
        };
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
            WordGrid.Instance.LetterTiles[i].SetTileText("Q");
        }
    }

}
