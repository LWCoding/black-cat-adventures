using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison Tile", menuName = "Tile Type/Poison Tile")]
public class PoisonTile : TileType
{

    public override void ActivateTileEffects()
    {
        Debug.Log("used poison tile lol");
    }

}
