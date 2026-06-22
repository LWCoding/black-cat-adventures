using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{

    public string LevelId;
    public int LevelNumber;
    public List<EnemyEncounter> Encounters = new();
    public Treasure RewardTreasure;

}
