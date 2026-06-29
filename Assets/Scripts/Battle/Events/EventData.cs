using UnityEngine;

/// <summary>
/// Abstract base for a single map event: its target scene and a type-safe hook
/// to attach the matching EventBehaviour. Subclass this per event type (see
/// LockedTreasureEventData, ChooseBoonEventData) so the behaviour is wired in
/// code via AddComponent&lt;T&gt; rather than a stringly-typed class name.
///
/// This mirrors the SpaceData / Treasure abstract-base pattern used elsewhere.
/// </summary>
public abstract class EventData : ScriptableObject
{
    /// <summary>
    /// Scene to load when this event runs. Battle-style events use "Level";
    /// UI events use "EventLevel".
    /// </summary>
    [Tooltip("Unity scene to load (e.g. 'Level' or 'EventLevel').")]
    public string SceneToLoad;

    /// <summary>
    /// Stable identifier used to track which events the player has already seen.
    /// The asset's own name is the id, so there is no separate field to keep in sync.
    /// </summary>
    public string EventId => name;

    /// <summary>
    /// Adds this event's concrete EventBehaviour to <paramref name="host"/> and
    /// returns it. Implemented per event type so there is no reflection or
    /// class-name string to get wrong.
    /// </summary>
    public abstract EventBehaviour AttachBehaviour(GameObject host);
}
