using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class LetterTile : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected TextMeshPro _letterText;
    [SerializeField] protected SpriteRenderer _bgRenderer;
    [SerializeField] protected SpriteRenderer _dmgIndRenderer;  // Damage indicator renderer

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
    
    [HideInInspector] public Tile Tile;

    private void Start()
    {
        IsSelected = false;
    }

    public string GetLetters() => Tile.Letters;

    /// <summary>
    /// Initializes this tile object to a specified tile. Saves the tile index.
    /// Sets the damage indicator to the correct damage.
    /// </summary>
    public void InitializeTile(Tile tile)
    {
        float savedTileBGAlpha = _bgRenderer.color.a;  // Don't override alpha value in case toggled off

        _bgRenderer.sprite = tile.CurrTileType.TileSprite;
        _bgRenderer.color = new Color(tile.CurrTileType.TileSpriteColor.r, tile.CurrTileType.TileSpriteColor.g, tile.CurrTileType.TileSpriteColor.b, savedTileBGAlpha);
        Tile = tile;
        _letterText.text = tile.Letters;

        byte savedTileIndAlpha = (byte)(255 * _dmgIndRenderer.color.a);  // Don't override alpha value in case toggled off
        switch (tile.DamageType)
        {
            case TileDamage.LOW:
                _dmgIndRenderer.color = new Color32(186, 141, 114, savedTileIndAlpha);
                break;
            case TileDamage.MEDIUM:
                _dmgIndRenderer.color = new Color32(159, 168, 168, savedTileIndAlpha);
                break;
            case TileDamage.HIGH:
                _dmgIndRenderer.color = new Color32(255, 237, 100, savedTileIndAlpha);
                break;
        }
    }

    /// <summary>
    /// Randomly gets a tile using the WordGenerator class and sets this tile to
    /// appear like that tile. May take in a list of tiles to discourage certain
    /// tiles from appearing. Must be initialized first.
    /// </summary>
    public void RandomizeTile(string discouragedLetters = "", TileTypeName tileType = TileTypeName.NORMAL)
    {
        if (discouragedLetters == "")
        {
            Tile newTile = WordGenerator.Instance.GetRandomTile(Tile.TileIndex);
            InitializeTile(newTile);
        } else {
            // If we have discouraged tiles, 40% chance to allow them to show up
            Tile newTile;
            if (Random.Range(0f, 1f) < 0.4f)
            {
                newTile = WordGenerator.Instance.GetRandomTile(Tile.TileIndex);
            }
            else
            {
                newTile = WordGenerator.Instance.GetRandomTile(Tile.TileIndex, discouragedLetters);
            }
            newTile.SetType(tileType);
            InitializeTile(newTile);
        }
    }

    /// <summary>
    /// Set the type of an already existing tile to something else.
    /// </summary>
    public void SetTileType(TileTypeName tileType)
    {
        Tile.SetType(tileType);
        InitializeTile(Tile);
    }

    /// <summary>
    /// Randomly gets a vowel using the WordGenerator class and sets this tile to
    /// appear like that tile. Must be initialized first.
    /// </summary>
    public void RandomizeVowel()
    {
        Tile newTile = WordGenerator.Instance.GetRandomVowel(Tile.TileIndex);
        InitializeTile(newTile);
    }

    public void SetTileText(string letters)
    {
        Tile newTile = WordGenerator.Instance.GetRandomTile(Tile.TileIndex);
        newTile.Letters = letters;
        InitializeTile(newTile);
    }

    /// <summary>
    /// Toggle whether this tile looks like it's been clicked or not.
    /// 
    /// isVisible = true -> Tile looks like it is interactable
    /// isVisible = false -> Tile looks like it isn't interactable
    /// </summary>
    public void ToggleVisibility(bool isVisible)
    {
        if (IsVisibilityLocked) { return; }
        Color bgColor = _bgRenderer.color;
        Color letterColor = _letterText.color;
        Color indColor = _dmgIndRenderer.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isVisible ? 1 : 0.3f);
        _letterText.color = new Color(letterColor.r, letterColor.g, letterColor.b, isVisible ? 1 : 0.3f);
        _dmgIndRenderer.color = new Color(indColor.r, indColor.g, indColor.b, isVisible ? 1 : 0.3f);
    }

}
