using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EncounterType
{
    Normal = 0,
    Miniboss = 1,
}

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounters/Encounter")]
public class Encounter : ScriptableObject
{

    [FormerlySerializedAs("LevelId")]
    public string EncounterId;
    public EncounterType Type;
    [FormerlySerializedAs("Encounters")]
    public List<EnemySpawn> Enemies = new();

}
