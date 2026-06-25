using UnityEngine;

/// <summary>
/// Tile type applied when an enemy locks a grid tile.
/// Locked tiles cannot be added to the player's word. The lock visual,
/// countdown, and interaction block are managed by <see cref="WordGrid"/>
/// and <see cref="LetterTile"/>.
/// </summary>
[CreateAssetMenu(fileName = "Locked Tile", menuName = "Tile Type/Locked Tile")]
public class LockedTile : TileType
{

    public override void OnTileAdded() { }

    public override void OnTileRemoved() { }

    public override void ActivateTileEffects() { }

}
