using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreviewLetterTile : LetterTile
{

    private void OnMouseDown()
    {
        WordPreview.Instance.RemoveTile(_tile);
    }

}
