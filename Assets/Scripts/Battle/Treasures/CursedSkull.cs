using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cursed Skull", menuName = "Treasures/Cursed Skull")]
public class CursedSkull : Treasure
{

    public override void ActivateTreasure()
    {
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            // Register synchronously so DamageIndicator (which reads at end of frame)
            // sees the correct modifier on this same tile-change event.
            bool isLong = WordPreview.Instance.CurrentWord.Length > 5;
            DamageCalculator.RegisterScaledModifier("cursedskull", isLong ? 2 : 0);

            BattleManager.Instance.RunNextFrame(() =>
            {
                if (!WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord)) { return; }
                WordPreview.Instance.FeedbackText.enabled = true;
                if (WordPreview.Instance.FeedbackText.text != "")
                {
                    WordPreview.Instance.FeedbackText.text += " ";
                }
                WordPreview.Instance.FeedbackText.text +=
                    WordPreview.Instance.CurrentWord.Length > 5 ? "Cursed!" : "Cursed...";
            });
        };
    }

}
