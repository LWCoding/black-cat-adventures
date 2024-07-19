using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreviewLetterTile : LetterTile
{

    private void OnMouseEnter()
    {
        WordPreview.Instance.ToggleTilesFromIndex(_tile.TileIndex, false);
    }

    private void OnMouseExit()
    {
        WordPreview.Instance.ToggleTilesFromIndex(_tile.TileIndex, true);
    }

    private void OnMouseDown()
    {
        WordPreview.Instance.RemoveTile(_tile);
    }

}
