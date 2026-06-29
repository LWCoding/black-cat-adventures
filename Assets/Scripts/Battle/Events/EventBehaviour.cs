using UnityEngine;

/// <summary>
/// Base class for all map event behaviours.
/// Subclass this and implement BeginEvent() to define the event's logic.
/// A concrete EventData subclass attaches the matching behaviour via
/// AttachBehaviour() (AddComponent&lt;T&gt;), so EventManager / LevelSpawner
/// never reference the behaviour by class-name string.
/// </summary>
public abstract class EventBehaviour : MonoBehaviour
{
    /// <summary>
    /// The EventData asset that spawned this behaviour. Set by the concrete
    /// EventData.AttachBehaviour implementation before BeginEvent is called,
    /// so the behaviour can read its own configuration without a separate
    /// InjectRefs method.
    /// </summary>
    public EventData Data { get; set; }

    /// <summary>
    /// Called immediately after this behaviour is created and its Data is set.
    /// Implement all event-specific setup here.
    /// </summary>
    public abstract void BeginEvent();
}
