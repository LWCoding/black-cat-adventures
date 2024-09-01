using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Censorship Tape", menuName = "Treasures/Censorship Tape")]
public class ProfanityTape : Treasure
{

    public override void ActivateTreasure()
    {
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {

            BattleManager.Instance.RunNextFrame(() =>
            {
                if (WordGenerator.Instance.IsProfaneWord(WordPreview.Instance.CurrentWord))
                {
                    WordPreview.Instance.FeedbackText.enabled = true;
                    if (WordPreview.Instance.FeedbackText.text != "")
                    {
                        WordPreview.Instance.FeedbackText.text += " ";
                    }
                    WordPreview.Instance.FeedbackText.text += "Profane!";

                    DamageCalculator.RegisterFlatModifier("profanitytape", WordPreview.Instance.CurrentWord.Length);
                }
                else
                {
                    DamageCalculator.RegisterFlatModifier("profanitytape", 0);
                }
            });
        };
    }

}
