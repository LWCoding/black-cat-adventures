using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Misprinted Card", menuName = "Treasures/Misprinted Card")]
public class MisprintedCard : Treasure
{

    public override void ActivateTreasure()
    {
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            BattleManager.Instance.RunNextFrame(() =>
            {
                if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) && ContainsDuplicateLetters(WordPreview.Instance.CurrentWord))
                {
                    WordPreview.Instance.FeedbackText.enabled = true;
                    if (WordPreview.Instance.FeedbackText.text != "")
                    {
                        WordPreview.Instance.FeedbackText.text += " ";
                    }
                    WordPreview.Instance.FeedbackText.text += "Misprint!";
                    DamageCalculator.RegisterFlatModifier("misprintcard", 3);
                }
                else
                {
                    DamageCalculator.RegisterFlatModifier("misprintcard", 0);
                }
            });
        };
    }

    /// <summary>
    /// Returns true if there are duplicate letters in any fashion
    /// in the provided string. 
    /// 
    /// ("tenet" has duplicate letters, "tent" does not)
    /// </summary>
    private bool ContainsDuplicateLetters(string word)
    {
        Dictionary<char, int> occur = new();
        foreach (char c in word)
        {
            if (!occur.ContainsKey(c))
            {
                occur[c] = 0;
            }
            occur[c]++;
            if (occur[c] == 2)
            {
                return true;
            }
        }
        return false;
    }

}
