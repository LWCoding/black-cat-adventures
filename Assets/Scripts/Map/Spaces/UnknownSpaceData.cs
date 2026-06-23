using UnityEngine;

/// <summary>
/// Space type that randomly becomes one of the other registered space types.
/// Resolution is weighted by SpaceResolutionRegistry and persisted via GameData so
/// the result is stable across reloads. Add new eligible types by adding entries to
/// the registry — no changes to this class are ever needed.
/// </summary>
[CreateAssetMenu(fileName = "Unknown Space", menuName = "Spaces/Unknown Space")]
public class UnknownSpaceData : SpaceData
{

    [Tooltip("Registry of all space types that an Unknown space can resolve into, with relative weights.")]
    public SpaceResolutionRegistry ResolutionRegistry;

    public override ResolvedSpace Resolve(System.Random rng)
    {
        SpaceData concrete = ResolutionRegistry != null ? ResolutionRegistry.Roll(rng) : null;
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

}
