using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weighted registry of concrete SpaceData types that Unknown spaces can resolve into.
/// To make a space type eligible for Unknown resolution, add an entry here with a
/// relative weight. Chance = entry.Weight / sum(all weights).
/// </summary>
[CreateAssetMenu(fileName = "Space Resolution Registry", menuName = "Spaces/Space Resolution Registry")]
public class SpaceResolutionRegistry : ScriptableObject
{

    [System.Serializable]
    public class Entry
    {
        [Tooltip("A concrete (non-Unknown) SpaceData asset.")]
        public SpaceData Space;
        [Min(0f)]
        public float Weight = 1f;
    }

    public List<Entry> Entries = new();

    /// <summary>
    /// Picks a random SpaceData weighted by Entry.Weight.
    /// Returns null if the registry is empty or all weights are zero.
    /// </summary>
    public SpaceData Roll(System.Random rng)
    {
        float totalWeight = 0f;
        foreach (Entry e in Entries)
        {
            if (e.Space != null)
            {
                totalWeight += e.Weight;
            }
        }
        if (totalWeight <= 0f) { return null; }

        float roll = (float)(rng.NextDouble() * totalWeight);
        float cumulative = 0f;
        foreach (Entry e in Entries)
        {
            if (e.Space == null) { continue; }
            cumulative += e.Weight;
            if (roll < cumulative)
            {
                return e.Space;
            }
        }
        return Entries[^1].Space;
    }

}
