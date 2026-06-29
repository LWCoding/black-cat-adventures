using UnityEngine;

/// <summary>
/// Bundles the scene-level references that battle-style event behaviours need
/// (prefab + transform anchors from LevelSpawner) into one struct so they can
/// be passed through a single interface call rather than per-event InjectRefs.
/// </summary>
public struct EventSceneContext
{
    public GameObject EnemyPrefab;
    public Transform  EnemySpawnAnchor;
    public Transform  StagingAnchor;
    public Transform  SpawnedObjectsParent;

    public EventSceneContext(
        GameObject enemyPrefab,
        Transform  enemySpawnAnchor,
        Transform  stagingAnchor,
        Transform  spawnedObjectsParent)
    {
        EnemyPrefab          = enemyPrefab;
        EnemySpawnAnchor     = enemySpawnAnchor;
        StagingAnchor        = stagingAnchor;
        SpawnedObjectsParent = spawnedObjectsParent;
    }
}

/// <summary>
/// Implemented by EventBehaviour subclasses that run in the Level scene and
/// need access to LevelSpawner's scene references. LevelSpawner.AwakeEventMode
/// checks for this interface and injects the context before calling BeginEvent,
/// so no per-event InjectRefs method or LevelSpawner fields are needed.
/// </summary>
public interface INeedsEventSceneContext
{
    void SetSceneContext(EventSceneContext ctx);
}
