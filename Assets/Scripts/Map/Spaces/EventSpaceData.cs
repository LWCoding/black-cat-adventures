using UnityEngine;

/// <summary>
/// Space type that triggers a random map event from the EventDatabase,
/// preferring events the player hasn't seen yet (unseen-first).
///
/// The concrete event is chosen at ENTRY time (see <see cref="PickEvent"/> /
/// BattleButton), not at map-resolution time. Resolving the event up-front would
/// bake an event ordering based on the order nodes happen to resolve in, which
/// ignores the player's actual path and can repeat an event before every event
/// has been seen. So Resolve only marks the node as an Event-type node; the
/// PayloadId (event id) and SceneToLoad are filled in when the player enters.
/// </summary>
[CreateAssetMenu(fileName = "New Event Space", menuName = "Spaces/Event Space")]
public class EventSpaceData : SpaceData
{
    public override ResolvedSpace Resolve(System.Random rng)
    {
        return new ResolvedSpace
        {
            ResolvedTypeId = SpaceTypeId,
            SceneToLoad    = string.Empty,
            PayloadId      = string.Empty,
        };
    }

    /// <summary>
    /// Chooses the event to run when the player enters this Event node, preferring
    /// events they have not seen yet. Called at entry time so the unseen-first
    /// ordering follows the player's real traversal rather than the order map
    /// nodes resolved in. Returns null if the event database is empty.
    /// </summary>
    public EventData PickEvent()
    {
        return GameDatabase.Events != null
            ? GameDatabase.Events.RollEvent(new System.Random(), GameManager.GameData.SeenEventIds)
            : null;
    }
}
