using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreviewLetterTile : LetterTile
{

    private void OnMouseEnter()
    {
        WordPreview.Instance.ToggleTilesFromIndex(Tile.TileIndex, false);
    }

    private void OnMouseExit()
    {
        WordPreview.Instance.ToggleTilesFromIndex(Tile.TileIndex, true);
    }

    private void OnMouseDown()
    {
        WordPreview.Instance.RemoveTile(Tile);
    }

}
