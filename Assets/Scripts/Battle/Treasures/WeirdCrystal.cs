using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weird Crystal", menuName = "Treasures/Weird Crystal")]
public class WeirdCrystal : Treasure
{

    public override void ActivateTreasure()
    {
        // Run next frame so the board is fully initialized before we place the wild tile.
        BattleManager.Instance.RunNextFrame(() =>
        {
            List<LetterTile> tiles = WordGrid.Instance.LetterTiles;
            if (tiles.Count == 0) { return; }
            LetterTile target = tiles[Random.Range(0, tiles.Count)];
            target.Tile.Letters = "";
            target.SetTileType(TileTypeName.WILD);
        });
    }

}
