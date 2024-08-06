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
                BattleManager.Instance.RunNextFrame(() =>
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
        BattleManager.Instance.OnPlayerAttack += () =>
        {
            if (WordPreview.Instance.CurrentTiles.Count == 7)
            {
                BattleManager.Instance.RenderAttackAgainstEnemy(7);
            }
        };
    }

}
