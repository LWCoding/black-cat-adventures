using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lucky Cat Paw", menuName = "Treasures/Lucky Cat Paw")]
public class LuckyCatPaw : Treasure
{
    public override void ActivateTreasure()
    {
        BattleManager.Instance.OnPlayerAttack += () =>
        {
            BattleManager.Instance.RenderAttackAgainstEnemy(1);
        };
    }

}
