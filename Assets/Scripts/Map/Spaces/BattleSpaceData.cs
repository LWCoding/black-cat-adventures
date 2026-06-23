using UnityEngine;

/// <summary>
/// Space type that loads the Level scene and runs a battle encounter.
/// Assign either a specific PinnedEncounter (always plays that one) or a Database
/// (rolls a random eligible encounter based on prerequisites at resolve time).
/// If both are assigned, PinnedEncounter takes priority.
/// </summary>
[CreateAssetMenu(fileName = "New Battle Space", menuName = "Spaces/Battle Space")]
public class BattleSpaceData : SpaceData
{

    [Tooltip("A pinned encounter. Takes priority over Database if assigned.")]
    public Encounter PinnedEncounter;
    [Tooltip("Database to draw from when no pinned encounter is set. Respects MinEncountersCompleted prerequisites.")]
    public EncounterDatabase Database;

    public override ResolvedSpace Resolve(System.Random rng)
    {
        Encounter chosen = PinnedEncounter != null
            ? PinnedEncounter
            : Database?.Roll(rng, GameManager.GameData.LevelsCompleted.Count);
        return new ResolvedSpace
        {
            ResolvedTypeId = SpaceTypeId,
            SceneToLoad = SceneToLoad,
            PayloadId = chosen != null ? chosen.EncounterId : string.Empty,
        };
    }

}
