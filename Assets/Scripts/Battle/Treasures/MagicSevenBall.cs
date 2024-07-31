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
            if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) && WordPreview.Instance.CurrentTiles.Count == 7)
            {
                LevelManager.Instance.RunNextFrame(() =>
                {
                    WordPreview.Instance.FeedbackText.enabled = true;
                    if (WordPreview.Instance.FeedbackText.text != "")
                    {
                        WordPreview.Instance.FeedbackText.text += " ";
                    }
                    WordPreview.Instance.FeedbackText.text += "Seven!";
                });
            }
        };
        LevelManager.Instance.OnPlayerAttack += () =>
        {
            if (WordPreview.Instance.CurrentTiles.Count == 7)
            {
                LevelManager.Instance.RenderAttackAgainstEnemy(7);
            }
        };
    }

}
