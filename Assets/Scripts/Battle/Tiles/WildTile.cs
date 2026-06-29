using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wild tiles act as any letter needed to complete a valid word.
/// The letter resolution happens in WordPreview.ResolveWildTiles() each time
/// the player's selection changes; this script just satisfies the TileType contract.
/// </summary>
[CreateAssetMenu(fileName = "Wild Tile", menuName = "Tile Type/Wild Tile")]
public class WildTile : TileType
{

    public override void OnTileAdded() { }

    public override void OnTileRemoved() { }

    public override void ActivateTileEffects() { }

}
