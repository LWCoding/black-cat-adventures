using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison Tile", menuName = "Tile Type/Poison Tile")]
public class PoisonTile : TileType
{

    [Header("Poison Properties")]
    [SerializeField] private StatusEffect _poisonEffect;

    public override void ActivateTileEffects()
    {
        BattleManager.Instance.CurrEnemyHandler.StatusHandler.GainStatusEffect(_poisonEffect, 3);
    }

    public override void OnTileAdded()
    {
        DamageCalculator.RegisterFlatModifier("poisontile", 3, true);
    }

    public override void OnTileRemoved()
    {
        DamageCalculator.RegisterFlatModifier("poisontile", -3, true);
    }

}
