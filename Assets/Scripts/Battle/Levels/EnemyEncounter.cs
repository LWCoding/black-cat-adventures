using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEncounter
{

    public EnemyData EnemyData;
    public List<DialogueInfo> DialogueToPlayOnMeet = new();
    public bool ShouldStallBeforeTurn;
    public float TimeToNextObject = 1.3f;
    public Vector2 SpawnOffset;

}
