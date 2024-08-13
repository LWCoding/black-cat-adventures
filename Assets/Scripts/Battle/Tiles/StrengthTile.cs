using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Tile", menuName = "Tile Type/Heal Tile")]
public class HealTile : TileType
{

    [Header("Heal Properties")]
    [SerializeField] private StatusEffect _regenEffect;

    public override void ActivateTileEffects()
    {
        BattleManager.Instance.PlayerHandler.StatusHandler.GainStatusEffect(_regenEffect, 3);
    }

}
