using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Space type that randomly becomes one of the other registered space types.
/// Resolution is weighted by Entries and persisted via GameData so the result
/// is stable across reloads.
/// </summary>
[CreateAssetMenu(fileName = "Unknown Space", menuName = "Spaces/Unknown Space")]
public class UnknownSpaceData : SpaceData
{

    [System.Serializable]
    public class Entry
    {
        [Tooltip("A concrete (non-Unknown) SpaceData asset.")]
        public SpaceData Space;
        [Min(0f)]
        public float Weight = 1f;
    }

    [Tooltip("Space types this Unknown can resolve into, with relative weights.")]
    public List<Entry> Entries = new();

    public override ResolvedSpace Resolve(System.Random rng)
    {
        SpaceData concrete = Roll(rng);
        if (concrete == null)
        {
            return new ResolvedSpace
            {
                ResolvedTypeId = SpaceTypeId,
                SceneToLoad = SceneToLoad,
                PayloadId = string.Empty,
            };
        }
        return concrete.Resolve(rng);
    }

    private SpaceData Roll(System.Random rng)
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
