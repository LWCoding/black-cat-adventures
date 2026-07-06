using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Treasure : ScriptableObject
{

    [Header("Treasure Properties")]
    public string TreasureName;
    public string TreasureDescription;
    public Sprite TreasureIcon;
    public TreasureRarity Rarity;

    /// <summary>
    /// This treasure's display name wrapped in its rarity colour (TMP rich-text).
    /// Always prefer this over <see cref="TreasureName"/> anywhere the name is shown
    /// to the player, so rarity colour-coding stays consistent automatically.
    /// </summary>
    public string ColoredName => $"<color=#{TreasureRarityInfo.GetHexColor(Rarity)}>{TreasureName}</color>";

    /// <summary>
    /// Activates the treasure's effects.
    /// </summary>
    public abstract void ActivateTreasure();

}
