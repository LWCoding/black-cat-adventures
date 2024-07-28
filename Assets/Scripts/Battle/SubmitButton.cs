using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(PointerCursorOnHover))]
public class SubmitButton : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected TextMeshPro _letterText;
    [SerializeField] protected SpriteRenderer _bgRenderer;

    private PointerCursorOnHover _pointerCursorOnHover;
    private bool _isInteractable = false;
    private readonly WordGenerator _wordGenerator = new(); 

    public static Action OnClickButton = null;

    private void Awake()
    {
        OnClickButton = null;
        _pointerCursorOnHover = GetComponent<PointerCursorOnHover>();
        ToggleInteractability(false);  // Start off with the button uninteractable
    }

    private void Start()
    {
        // If the currently spelled word is valid, button is interactable, else no
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            if (_wordGenerator.IsValidWord(WordPreview.Instance.CurrentWord))
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
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isInteractable ? 1 : 0.2f);
        _letterText.color = new Color(letterColor.r, letterColor.g, letterColor.b, isInteractable ? 1 : 0.3f);
        // Toggle cursor activation based on interactability
        _pointerCursorOnHover.IsEnabled = isInteractable;
    }

    private void OnMouseDown()
    {
        if (!_isInteractable) { return; }  // If not interactable, don't do anything
        if (LevelManager.Instance.CurrentState is not PlayerTurnState) { return; }
        OnClickButton?.Invoke();
        // Algorithm to determine damage from word!
        int wordLength = WordPreview.Instance.CurrentWord.Length;
        int damageDealt = _wordGenerator.CalculateDamage(WordPreview.Instance.CurrentTiles);
        LevelManager.Instance.DealDamageToEnemy(damageDealt);
        LevelManager.Instance.OnPlayerAttack?.Invoke();
        WordPreview.Instance.ConsumeTiles();
    }

}
