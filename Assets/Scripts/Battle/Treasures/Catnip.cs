using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Catnip", menuName = "Treasures/Catnip")]
public class Catnip : Treasure
{

    public override void ActivateTreasure()
    {
        StatusEffect stunned = Resources.LoadAll<StatusEffect>("ScriptableObjects/Statuses")
            .First(s => s.name == "Stunned");

        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            BattleManager.Instance.RunNextFrame(() =>
            {
                if (!WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) || WordPreview.Instance.CurrentTiles.Count < 8) { return; }
                WordPreview.Instance.FeedbackText.enabled = true;
                if (WordPreview.Instance.FeedbackText.text != "")
                {
                    WordPreview.Instance.FeedbackText.text += " ";
                }
                WordPreview.Instance.FeedbackText.text += "Catnip!";
            });
        };

        SubmitButton.OnClickButton += () =>
        {
            if (WordPreview.Instance.CurrentTiles.Count >= 8)
            {
                BattleManager.Instance.CurrEnemyHandler.StatusHandler.GainStatusEffect(stunned, 1);
            }
        };
    }

}
