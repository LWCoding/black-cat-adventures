using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registry of all map events. Supports unseen-first selection so players
/// encounter events they haven't seen before falling back to the full pool.
/// Place a single instance at Resources/ScriptableObjects/Events/_EventDatabase.
/// </summary>
[CreateAssetMenu(fileName = "Event Database", menuName = "Events/Event Database")]
public class EventDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public EventData Event;
        [Min(0f)]
        public float Weight = 1f;
    }

    public List<Entry> Events = new();

    /// <summary>
    /// Looks up an event by its stable id.
    /// </summary>
    public EventData GetEvent(string eventId)
    {
        Entry found = Events.Find(e => e.Event != null && e.Event.EventId == eventId);
        return found?.Event;
    }

    /// <summary>
    /// Picks a random event, preferring ones whose EventId is not in
    /// <paramref name="seenEventIds"/>. Falls back to the full eligible list
    /// once all events have been seen, mirroring EncounterDatabase.RollFromList.
    /// </summary>
    public EventData RollEvent(System.Random rng, IEnumerable<string> seenEventIds = null)
    {
        List<Entry> eligible = Events.FindAll(e => e.Event != null);
        if (eligible.Count == 0) { return null; }

        if (seenEventIds != null)
        {
            HashSet<string> seen = new(seenEventIds);
            List<Entry> unseen = eligible.FindAll(e => !seen.Contains(e.Event.EventId));
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
            if (roll < cumulative) { return e.Event; }
        }
        return eligible[^1].Event;
    }
}
