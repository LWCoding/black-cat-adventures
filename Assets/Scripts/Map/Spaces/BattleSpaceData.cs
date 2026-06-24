using System.Collections.Generic;
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
            : Database?.Roll(rng, GameManager.GameData.LevelsCompleted.Count, GetSeenEncounterIds());
        return new ResolvedSpace
        {
            ResolvedTypeId = SpaceTypeId,
            SceneToLoad = SceneToLoad,
            PayloadId = chosen != null ? chosen.EncounterId : string.Empty,
        };
    }

    /// <summary>
    /// Encounters the player has already seen: those completed plus those already
    /// assigned to other battle nodes during this map's resolution pass. Used so a
    /// freshly resolved node prefers an encounter that hasn't appeared yet.
    /// </summary>
    private HashSet<string> GetSeenEncounterIds()
    {
        HashSet<string> seen = new(GameManager.GameData.LevelsCompleted);
        foreach (ResolvedSpaceEntry entry in GameManager.GameData.ResolvedSpaces)
        {
            if (entry.ResolvedTypeId == SpaceTypeId && !string.IsNullOrEmpty(entry.PayloadId))
            {
                seen.Add(entry.PayloadId);
            }
        }
        return seen;
    }

}
