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
            if (WordGenerator.Instance.IsProfaneWord(WordPreview.Instance.CurrentWord))
            {
                LevelManager.Instance.RunNextFrame(() =>
                {
                    WordPreview.Instance.FeedbackText.enabled = true;
                    if (WordPreview.Instance.FeedbackText.text != "")
                    {
                        WordPreview.Instance.FeedbackText.text += " ";
                    }
                    WordPreview.Instance.FeedbackText.text += "Profane!";
                });
            }
        };
        LevelManager.Instance.OnPlayerAttack += () =>
        {
            if (WordGenerator.Instance.IsProfaneWord(WordPreview.Instance.CurrentWord))
            {
                LevelManager.Instance.RenderAttackAgainstEnemy(WordPreview.Instance.CurrentWord.Length);
            }
        };
    }

}
