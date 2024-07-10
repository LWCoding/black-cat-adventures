using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubmitButton : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected TextMeshPro _letterText;
    [SerializeField] protected SpriteRenderer _bgRenderer;

    private bool _isInteractable = false;
    private readonly WordGenerator _wordChecker = new();  // Check if words are spelled right

    private void Awake()
    {
        ToggleInteractability(false);  // Start off with the button uninteractable
    }

    private void Start()
    {
        // If the currently spelled word is valid, button is interactable, else no
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            if (_wordChecker.IsValidWord(WordPreview.Instance.CurrentWord))
            {
                ToggleInteractability(true);
            }
            else
            {
                ToggleInteractability(false);
            }
        };
    }

    /// <summary>
    /// Toggle whether this tile is clickable or not.
    /// 
    /// isInteractable = true -> Button looks like it is interactable
    /// isInteractable = false -> Button looks like it isn't interactable
    /// </summary>
    private void ToggleInteractability(bool isInteractable)
    {
        _isInteractable = isInteractable;
        Color bgColor = _bgRenderer.color;
        Color letterColor = _letterText.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isInteractable ? 1 : 0.3f);
        _letterText.color = new Color(letterColor.r, letterColor.g, letterColor.b, isInteractable ? 1 : 0.3f);
    }

    private void OnMouseDown()
    {
        if (!_isInteractable) { return; }  // If not interactable, don't do anything
        LevelManager.Instance.DealDamageWithWord(WordPreview.Instance.CurrentWord);
        WordPreview.Instance.ConsumeTiles();
        LevelManager.Instance.SetState(new EnemyTurnState());
        StartCoroutine(WaitThenPlayer());  // TODO: THIS IS A TEST, REMOVE THIS
    }

    /// <summary>
    /// TODO: THIS IS A TEST REMOVE THIS
    /// TODO: THIS IS A TEST REMOVE THIS
    /// TODO: THIS IS A TEST REMOVE THIS
    /// TODO: THIS IS A TEST REMOVE THIS
    /// TODO: THIS IS A TEST REMOVE THIS
    /// </summary>
    private IEnumerator WaitThenPlayer()
    {
        yield return new WaitForSeconds(2);
        LevelManager.Instance.SetState(new PlayerTurnState());
    }

}
