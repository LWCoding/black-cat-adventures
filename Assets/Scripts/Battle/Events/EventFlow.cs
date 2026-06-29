using UnityEngine.SceneManagement;

/// <summary>
/// Shared bookkeeping run at the end of every map event regardless of archetype:
/// marks the node completed, records the event as seen, saves, and loads the Map.
/// Both BattleEvent (Level scene) and EventManager (EventLevel scene) call this
/// instead of duplicating the logic.
/// </summary>
public static class EventFlow
{
    public static void CompleteAndReturnToMap()
    {
        string nodeId = GameManager.GameData.RecentNodeEntered;
        if (!string.IsNullOrEmpty(nodeId))
        {
            GameManager.GameData.CompletedNodeIds.Add(nodeId);
        }

        string eventId = GameManager.GameData.RecentLevelCompleted;
        if (!string.IsNullOrEmpty(eventId)
            && !GameManager.GameData.SeenEventIds.Contains(eventId))
        {
            GameManager.GameData.SeenEventIds.Add(eventId);
        }

        SaveManager.SaveGame(GameManager.GameData);
        SceneManager.LoadScene("Map");
    }
}
