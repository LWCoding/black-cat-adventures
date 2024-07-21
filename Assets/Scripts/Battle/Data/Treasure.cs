using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreasureType
{
    NONE = 0, CAT_PAW = 1, DUCT_TAPE = 2
}

[CreateAssetMenu(fileName = "New Treasure", menuName = "Treasure")]
public class TreasureData : ScriptableObject
{

    [Header("Treasure Properties")]
    public string TreasureName;
    public string TreasureDescription;
    public Sprite TreasureIcon;
    public TreasureType Type;

}
