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
            if ("jkxzq".Contains(_letters))
            {
                DamageType = TileDamage.HIGH;
            }
            else if ("bcfgmpvwy".Contains(_letters))
            {
                DamageType = TileDamage.MEDIUM;
            }
            else
            {
                DamageType = TileDamage.LOW;
            }
        }
    }

    public int TileIndex;
    public TileDamage DamageType;

    public Tile(string letters, int idx)
    {
        Letters = letters;
        TileIndex = idx; 
    }

}
