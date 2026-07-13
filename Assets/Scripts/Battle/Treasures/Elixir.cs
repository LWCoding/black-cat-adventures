using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elixir", menuName = "Treasures/Elixir")]
public class Elixir : ActiveTreasure
{

    public override bool OnTileTargeted(LetterTile tile)
    {
        if (tile == null || tile.Tile == null) { return false; }
        tile.Tile.Letters = "E";
        tile.InitializeTile(tile.Tile);
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnPoof(tile.transform.position);
        }
        return true;
    }

}
