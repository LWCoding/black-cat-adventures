using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile 
{

    public string Letters;
    public int TileIndex;

    public bool IsVowel() => "aeiou".Contains(Letters.ToLower());

}
