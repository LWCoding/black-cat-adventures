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

    public override void OnTileAdded()
    {
        DamageCalculator.RegisterFlatModifier("healtile", 4, true);
    }

    public override void OnTileRemoved()
    {
        DamageCalculator.RegisterFlatModifier("healtile", -4, true);
    }

}
