using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elixir", menuName = "Treasures/Elixir")]
public class Elixir : ActiveTreasure
{

    public override bool OnTileTargeted(LetterTile tile)
    {
        if (tile == null || tile.Tile == null) { return false; }
        Tile data = tile.Tile;
        data.Letters = "E";

        // Grid and preview clones share the same Tile object, so refresh both visuals:
        // the grid tile by index, and the matching preview clone (if the tile is staged).
        if (WordGrid.Instance != null
            && data.TileIndex >= 0
            && data.TileIndex < WordGrid.Instance.LetterTiles.Count)
        {
            WordGrid.Instance.LetterTiles[data.TileIndex].InitializeTile(data);
        }
        if (WordPreview.Instance != null)
        {
            WordPreview.Instance.RefreshTileByIndex(data.TileIndex);
        }

        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnPoof(tile.transform.position);
        }
        return true;
    }

}
