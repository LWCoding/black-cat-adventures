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
        SubmitButton.OnClickButton += () =>
        {
            if (WordPreview.Instance.CurrentTiles.Count >= 8)
            {
                BattleManager.Instance.CurrEnemyHandler.StatusHandler.GainStatusEffect(stunned, 1);
            }
        };
    }

}
