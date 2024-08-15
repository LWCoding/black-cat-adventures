using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fools Gold", menuName = "Treasures/Fools Gold")]
public class FoolsGold : Treasure
{

    public override void ActivateTreasure()
    {
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            DamageCalculator.RegisterScaledModifier("foolsgold", 1 + 0.25f * CountGoldTiles(WordPreview.Instance.CurrentWord));
        };
    }

    /// <summary>
    /// Given a string, returns the number of gold tiles found
    /// within. 
    /// </summary>
    private int CountGoldTiles(string word)
    {
        int goldTiles = 0;
        foreach (char c in word)
        {
            if (Tile.GetTileDamageFromLetters(c.ToString()) == TileDamage.HIGH) {
                goldTiles++;
            }
        }
        return goldTiles;
    }

}
