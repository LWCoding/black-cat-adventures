using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLetterTile : LetterTile
{

    /// <summary>
    /// When clicked, IF this tile is not selected and it is the
    /// player's turn to move, append this tile to the preview tiles
    /// and mark as selected.
    /// </summary>
    private void OnMouseDown()
    {
        if (IsSelected) return;
        if (LevelManager.Instance.CurrentState is not PlayerTurnState) return;
        WordPreview.Instance.AppendTile(_tile);
        IsSelected = true;  // Set this letter to selected
    }

}