using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class LetterTile : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected TextMeshPro _letterText;
    [SerializeField] protected SpriteRenderer _bgRenderer;

    protected bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            ToggleVisibility(!_isSelected);
        }
    }

    protected bool IsVisibilityLocked = false;  // If true, visibility cannot be changed
    protected Tile _tile;

    private WordGenerator _wordGenerator = new();

    private void Start()
    {
        IsSelected = false;
    }

    /// <summary>
    /// Initializes this tile object to a specified tile. Saves the tile index.
    /// </summary>
    public void InitializeTile(Tile tile)
    {
        _tile = tile;
        _letterText.text = tile.Letters;
    }

    /// <summary>
    /// Randomly gets a tile using the WordGenerator class and sets this tile to
    /// appear like that tile. Must be initialized first.
    /// </summary>
    public void RandomizeTile()
    {
        Tile newTile = _wordGenerator.GetRandomTile(_tile.TileIndex);
        InitializeTile(newTile);
    }

    /// <summary>
    /// Toggle whether this tile looks like it's been clicked or not.
    /// 
    /// isVisible = true -> Tile looks like it is interactable
    /// isVisible = false -> Tile looks like it isn't interactable
    /// </summary>
    protected void ToggleVisibility(bool isVisible)
    {
        if (IsVisibilityLocked) { return; }
        Color bgColor = _bgRenderer.color;
        Color letterColor = _letterText.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isVisible ? 1 : 0.3f);
        _letterText.color = new Color(letterColor.r, letterColor.g, letterColor.b, isVisible ? 1 : 0.3f);
    }

}
