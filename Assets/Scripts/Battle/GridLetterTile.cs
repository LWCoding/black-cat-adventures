using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLetterTile : LetterTile
{

    private void OnMouseDown()
    {
        if (IsSelected) return;
        WordPreview.Instance.AppendTile(_tile);
        IsSelected = true;  // Set this letter to selected
    }

}