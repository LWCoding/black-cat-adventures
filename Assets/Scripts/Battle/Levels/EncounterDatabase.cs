using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single source of truth for all encounters in the game.
/// Handles both random selection (with prerequisite filtering) and id-based lookup.
/// </summary>
[CreateAssetMenu(fileName = "Encounter Database", menuName = "Encounters/Encounter Database")]
public class EncounterDatabase : ScriptableObject
{

    [System.Serializable]
    public class Entry
    {
        public Encounter Encounter;
        [Min(0f)]
        public float Weight = 1f;
        [Tooltip("Number of total encounters the player must have completed before this one is eligible to be chosen.")]
        [Min(0)]
        public int MinEncountersCompleted = 0;
    }

    public List<Entry> Entries = new();

    [Tooltip("If set, this encounter is always used for the player's very first battle and is never chosen by Roll.")]
    public Encounter TutorialEncounter;

    /// <summary>
    /// Looks up an encounter by its id. Used by LevelSpawner after the
    /// map has written the chosen id to GameData.RecentLevelCompleted.
    /// </summary>
    public Encounter GetEncounter(string encounterId)
        => Entries.Find(e => e.Encounter != null && e.Encounter.EncounterId == encounterId)?.Encounter;

    /// <summary>
    /// Picks a random encounter using weighted selection, filtered to entries whose
    /// MinEncountersCompleted prerequisite has been met. completedCount should be
    /// GameData.LevelsCompleted.Count at the time of resolution.
    /// The TutorialEncounter is always excluded.
    /// Returns null if no eligible entries exist.
    /// </summary>
    public Encounter Roll(System.Random rng, int completedCount)
    {
        List<Entry> eligible = Entries.FindAll(
            e => e.Encounter != null
            && e.Encounter != TutorialEncounter
            && completedCount >= e.MinEncountersCompleted);

        float totalWeight = 0f;
        foreach (Entry e in eligible)
        {
            totalWeight += e.Weight;
        }
        if (totalWeight <= 0f) { return null; }

        float roll = (float)(rng.NextDouble() * totalWeight);
        float cumulative = 0f;
        foreach (Entry e in eligible)
        {
            cumulative += e.Weight;
            if (roll < cumulative)
            {
                return e.Encounter;
            }
        }
        return eligible[^1].Encounter;
    }

}
