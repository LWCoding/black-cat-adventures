using UnityEngine;

/// <summary>
/// In-memory result returned by SpaceData.Resolve. Flat, not persisted directly;
/// persistence uses ResolvedSpaceEntry in GameData.
/// </summary>
public class ResolvedSpace
{
    public string ResolvedTypeId;
    public string SceneToLoad;
    /// <summary>
    /// Type-specific payload (e.g. encounter id for Battle spaces).
    /// </summary>
    public string PayloadId;
}

/// <summary>
/// Abstract base for all map space types. Subclass this to add a new space type.
/// Each subclass defines what scene to load and how to resolve its payload.
/// </summary>
public abstract class SpaceData : ScriptableObject
{

    [Tooltip("Stable identifier for this space type (e.g. 'Battle', 'Unknown'). Used in ResolvedSpaceEntry.")]
    public string SpaceTypeId;
    public string DisplayName;
    [Tooltip("Label shown on the action button when the player is on this space (e.g. 'Battle!', 'Explore').")]
    public string ActionLabel;
    [Tooltip("Sprite shown on the map node when this space type is revealed.")]
    public Sprite NodeSprite;
    [Tooltip("Unity scene name loaded when the player enters this space.")]
    public string SceneToLoad;

    /// <summary>
    /// Resolves this space into a concrete payload. For deterministic results across
    /// sessions, pass a seeded System.Random derived from GameData.MapSeed.
    /// </summary>
    public abstract ResolvedSpace Resolve(System.Random rng);

}
