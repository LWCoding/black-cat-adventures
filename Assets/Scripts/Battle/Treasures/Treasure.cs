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
    /// Activates the treasure's effects.
    /// </summary>
    public abstract void ActivateTreasure();

}
