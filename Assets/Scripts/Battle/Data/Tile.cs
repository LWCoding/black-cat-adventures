using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TileDamage
{
    LOW = 0, MEDIUM = 1, HIGH = 2
}

[System.Serializable]
public class Tile 
{

    private string _letters;

    public string Letters
    {
        get
        {
            if (_letters == "q")
            {
                return "Qu";
            } 
            return _letters.ToUpper();
        }
        set
        {
            _letters = value.ToLower();
            DamageType = GetTileDamageFromLetters(_letters);
        }
    }

    public int TileIndex;
    public TileDamage DamageType;
    public TileType CurrTileType;

    public Tile(string letters, int idx, TileTypeName tileTypeName)
    {
        Letters = letters;
        TileIndex = idx;
        CurrTileType = WordGenerator.TileTypes[tileTypeName];
    }

    public void SetType(TileTypeName tileTypeName)
    {
        CurrTileType = WordGenerator.TileTypes[tileTypeName];
    }

    public static TileDamage GetTileDamageFromLetters(string letters)
    {
        letters = letters.ToLower();
        if ("jkxzq".Contains(letters))
        {
            return TileDamage.HIGH;
        }
        else if ("bcfgmpvwy".Contains(letters))
        {
            return TileDamage.MEDIUM;
        }
        else
        {
            return TileDamage.LOW;
        }
    }

}
