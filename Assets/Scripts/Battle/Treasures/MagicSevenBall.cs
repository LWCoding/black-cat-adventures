using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magic Seven Ball", menuName = "Treasures/Magic Seven Ball")]
public class MagicSevenBall : Treasure
{

    public override void ActivateTreasure()
    {
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            BattleManager.Instance.RunNextFrame(() =>
            {
                if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) && WordPreview.Instance.CurrentTiles.Count == 7)
                {
                    WordPreview.Instance.FeedbackText.enabled = true;
                    if (WordPreview.Instance.FeedbackText.text != "")
                    {
                        WordPreview.Instance.FeedbackText.text += " ";
                    }
                    WordPreview.Instance.FeedbackText.text += "Seven!";
                    DamageCalculator.RegisterFlatModifier("magicsevenball", 7);
                }
                else
                {
                    DamageCalculator.RegisterFlatModifier("magicsevenball", 0);
                }
            });
        };
    }

}
