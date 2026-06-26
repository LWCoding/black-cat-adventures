using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounters/Encounter")]
public class Encounter : ScriptableObject
{

    [FormerlySerializedAs("LevelId")]
    public string EncounterId;
    [FormerlySerializedAs("Encounters")]
    public List<EnemySpawn> Enemies = new();

}
