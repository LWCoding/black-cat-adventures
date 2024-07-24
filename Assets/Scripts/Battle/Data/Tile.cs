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
        get => _letters;
        set
        {
            _letters = value;
            // AAAAABBCCDDDEEEEEEEEFFGGHHHHHIIIIIIJKLLLLMMNNNNNOOOOOOPPQRRRRRSSSSSSTTTTTTTUUUVVWWWXYYYZ
            if ("JKQXZ".Contains(value))
            {
                DamageType = TileDamage.HIGH;
            }
            else if ("BCDFGLMPUVWY".Contains(value))
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
