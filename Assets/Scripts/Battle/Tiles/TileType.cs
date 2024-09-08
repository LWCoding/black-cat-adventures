using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileTypeName
{
    NORMAL = 0, POISON = 1, HEAL = 2
}

public abstract class TileType : ScriptableObject
{

    [Header("Tile Properties")]
    public TileTypeName TileTypeName;
    public Sprite TileSprite;
    public Color TileSpriteColor = Color.white;

    /// <summary>
    /// Is called when this tile is added to the player's word.
    /// </summary>
    public abstract void OnTileAdded();

    /// <summary>
    /// Is called when this tile is removed from the word.
    /// </summary>
    public abstract void OnTileRemoved();

    /// <summary>
    /// Activates the tile's effects when used.
    /// </summary>
    public abstract void ActivateTileEffects();

}
