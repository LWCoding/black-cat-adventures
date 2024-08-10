using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Strength Tile", menuName = "Tile Type/Strength Tile")]
public class StrengthTile : TileType
{

    [Header("Strength Properties")]
    [SerializeField] private StatusEffect _strengthEffect;

    public override void ActivateTileEffects()
    {
        BattleManager.Instance.CurrEnemyHandler.StatusHandler.GainStatusEffect(_strengthEffect, 3);
    }

}
