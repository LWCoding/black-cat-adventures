using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lucky Cat Paw", menuName = "Treasures/Lucky Cat Paw")]
public class LuckyCatPaw : Treasure
{
    public override void ActivateTreasure()
    {
        DamageCalculator.RegisterFlatModifier("luckycatpaw", 1);
    }

}
