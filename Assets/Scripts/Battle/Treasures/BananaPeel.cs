using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Banana Peel", menuName = "Treasures/Banana Peel")]
public class BananaPeel : Treasure
{

    public override void ActivateTreasure()
    {
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            // Register synchronously so DamageIndicator (which reads at end of frame)
            // sees the correct modifier on this same tile-change event.
            bool hasTriple = HasTripleLetter(WordPreview.Instance.CurrentWord);
            DamageCalculator.RegisterScaledModifier("bananapeel", hasTriple ? 3 : 1);

            BattleManager.Instance.RunNextFrame(() =>
            {
                if (!WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) || !HasTripleLetter(WordPreview.Instance.CurrentWord)) { return; }
                WordPreview.Instance.FeedbackText.enabled = true;
                if (WordPreview.Instance.FeedbackText.text != "")
                {
                    WordPreview.Instance.FeedbackText.text += " ";
                }
                WordPreview.Instance.FeedbackText.text += "Banana!";
            });
        };
    }

    /// <summary>
    /// Returns true if any single letter appears three or more times in the word.
    /// </summary>
    private bool HasTripleLetter(string word)
    {
        Dictionary<char, int> occur = new();
        foreach (char c in word.ToLower())
        {
            if (!occur.ContainsKey(c))
            {
                occur[c] = 0;
            }
            occur[c]++;
            if (occur[c] >= 3)
            {
                return true;
            }
        }
        return false;
    }

}
