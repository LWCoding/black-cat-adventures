using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Credit Card", menuName = "Treasures/Credit Card")]
public class CreditCard : Treasure
{

    private static bool _freeShuffleAvailable;

    public override void ActivateTreasure()
    {
        _freeShuffleAvailable = true;
    }

    public static bool IsShuffleFree()
    {
        return TreasureSection.Instance != null
            && TreasureSection.Instance.HasTreasure(typeof(CreditCard))
            && _freeShuffleAvailable;
    }

    public static bool ConsumeFreeShuffle()
    {
        if (!IsShuffleFree()) { return false; }
        _freeShuffleAvailable = false;
        return true;
    }

}
