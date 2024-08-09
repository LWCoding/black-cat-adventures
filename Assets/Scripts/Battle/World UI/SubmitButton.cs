using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(PointerCursorOnHover))]
public class SubmitButton : MonoBehaviour
{

    private static SubmitButton _instance;
    public static SubmitButton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SubmitButton>();
            }
            return _instance;
        }
    }

    [Header("Object Assignments")]
    [SerializeField] protected TextMeshPro _letterText;
    [SerializeField] protected SpriteRenderer _bgRenderer;

    private PointerCursorOnHover _pointerCursorOnHover;
    private bool _isInteractable = false;

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
            if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord))
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
        TrySubmitCurrentWord();
    }

    public void TrySubmitCurrentWord()
    {
        if (!_isInteractable) { return; }  // If not interactable, don't do anything
        if (BattleManager.Instance.CurrentState is not PlayerTurnState) { return; }
        OnClickButton?.Invoke();
        // Algorithm to determine damage from word!
        int wordLength = WordPreview.Instance.CurrentWord.Length;
        int damageDealt = WordGenerator.Instance.CalculateDamage(WordPreview.Instance.CurrentTiles);
        // See if we should spawn any special tile in the next roll
        TileTypeName specialTile = WordGenerator.Instance.GetTileTypeNameByDamage(damageDealt);
        // For every letter, render tile effects if any exist
        foreach (Tile tile in WordPreview.Instance.CurrentTiles)
        {
            tile.CurrTileType.ActivateTileEffects();
        }
        BattleManager.Instance.RenderAttackAgainstEnemy(damageDealt);
        BattleManager.Instance.OnPlayerAttack?.Invoke();
        WordPreview.Instance.ConsumeTiles(specialTile);
    }

}
