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
        WordGrid.Instance.LetterTiles[4].SetTileText("C");
        WordGrid.Instance.LetterTiles[13].SetTileText("A");
        WordGrid.Instance.LetterTiles[3].SetTileText("T");

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
        };
        // When a word is shuffled, hide the tooltip
        ShuffleButton.OnClickButton += () =>
        {
            _shuffleTooltipObject.SetActive(false);
        };
    }
    
    /// <summary>
    /// Set the letters to "Q" after they've been submitted.
    /// Wait a frame before doing this to override properly.
    /// </summary>
    private IEnumerator WaitThenRenderTilesUseless()
    {
        yield return new WaitForEndOfFrame();
        WordGrid.Instance.LetterTiles[4].SetTileText("Q");
        WordGrid.Instance.LetterTiles[13].SetTileText("Q");
        WordGrid.Instance.LetterTiles[3].SetTileText("Q");
    }

}
