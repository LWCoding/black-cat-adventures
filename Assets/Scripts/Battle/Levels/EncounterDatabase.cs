using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    [FormerlySerializedAs("Entries")]
    public List<Entry> NormalEntries = new();
    public List<Entry> MinibossEntries = new();

    [Tooltip("If set, this encounter is always used for the player's very first battle and is never chosen by Roll.")]
    public Encounter TutorialEncounter;

    /// <summary>
    /// Looks up an encounter by its id. Used by LevelSpawner after the
    /// map has written the chosen id to GameData.RecentLevelCompleted.
    /// </summary>
    public Encounter GetEncounter(string encounterId)
    {
        if (TutorialEncounter != null && TutorialEncounter.EncounterId == encounterId)
        {
            return TutorialEncounter;
        }

        Entry found = NormalEntries.Find(e => e.Encounter != null && e.Encounter.EncounterId == encounterId)
                   ?? MinibossEntries.Find(e => e.Encounter != null && e.Encounter.EncounterId == encounterId);
        return found?.Encounter;
    }

    /// <summary>
    /// Picks a random normal battle encounter using weighted selection, filtered to
    /// entries whose MinEncountersCompleted prerequisite has been met.
    /// Prefers unseen encounters when <paramref name="seenEncounterIds"/> is supplied;
    /// once every eligible encounter has been seen, falls back to the full eligible pool.
    /// Returns null if no eligible entries exist.
    /// </summary>
    public Encounter RollNormal(System.Random rng, int completedCount, IEnumerable<string> seenEncounterIds = null)
        => RollFromList(NormalEntries, rng, completedCount, seenEncounterIds, preferUnseen: true);

    /// <summary>
    /// Picks a random miniboss encounter using weighted selection, filtered to entries
    /// whose MinEncountersCompleted prerequisite has been met.
    /// Returns null if no eligible entries exist.
    /// </summary>
    public Encounter RollMiniboss(System.Random rng, int completedCount)
        => RollFromList(MinibossEntries, rng, completedCount, seenEncounterIds: null, preferUnseen: false);

    private Encounter RollFromList(
        List<Entry> entries,
        System.Random rng,
        int completedCount,
        IEnumerable<string> seenEncounterIds,
        bool preferUnseen)
    {
        List<Entry> eligible = entries.FindAll(
            e => e.Encounter != null
            && e.Encounter != TutorialEncounter
            && completedCount >= e.MinEncountersCompleted);

        if (eligible.Count == 0) { return null; }

        if (preferUnseen && seenEncounterIds != null)
        {
            HashSet<string> seen = new(seenEncounterIds);
            List<Entry> unseen = eligible.FindAll(e => !seen.Contains(e.Encounter.EncounterId));
            if (unseen.Count > 0)
            {
                eligible = unseen;
            }
        }

        float totalWeight = 0f;
        foreach (Entry e in eligible) { totalWeight += e.Weight; }
        if (totalWeight <= 0f) { return null; }

        float roll = (float)(rng.NextDouble() * totalWeight);
        float cumulative = 0f;
        foreach (Entry e in eligible)
        {
            cumulative += e.Weight;
            if (roll < cumulative) { return e.Encounter; }
        }
        return eligible[^1].Encounter;
    }

}
