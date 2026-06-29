using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _treasureChestPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _enemySpawnAnchor;
    [SerializeField] private Transform _stagingAnchor;
    [SerializeField] private Transform _spawnedObjectsParent;

    private void Awake()
    {
        // If the space resolves to an Event, hand off to the event behaviour
        // instead of running the normal encounter spawning path.
        if (GameManager.GameData.RecentResolvedTypeId == "Event")
        {
            AwakeEventMode();
            return;
        }

        string encounterId = GameManager.GameData.RecentLevelCompleted;
        EncounterDatabase encounterDatabase = GameDatabase.Encounters;

        // Always use the tutorial encounter for the player's very first battle,
        // unless the tutorial has already been completed (e.g. on a continued save
        // or after an F7 skip that was properly persisted).
        bool needsTutorial = !GameManager.GameData.HasTutorialCompleted
            && GameManager.GameData.LevelsCompleted.Count == 0
            && encounterDatabase.TutorialEncounter != null;
        Encounter encounter =
            needsTutorial
                ? encounterDatabase.TutorialEncounter
                : encounterDatabase.GetEncounter(encounterId)
                  ?? encounterDatabase.RollNormal(new System.Random(), 0);

        if (encounter == null)
        {
            Debug.LogError($"[LevelSpawner] No encounter found for id '{encounterId}' and no fallback available.");
            return;
        }

        List<EnemyHandler> spawnedEnemies = new();
        for (int i = 0; i < encounter.Enemies.Count; i++)
        {
            EnemySpawn spawn = encounter.Enemies[i];
            Vector3 spawnPosition = (i == 0 ? _enemySpawnAnchor.position : _stagingAnchor.position) + (Vector3)spawn.SpawnOffset;
            GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _spawnedObjectsParent);
            EnemyHandler enemyHandler = enemyObject.GetComponent<EnemyHandler>();
            enemyHandler.SetCharacterData(spawn.EnemyData);
            // Copy the list rather than aliasing the Encounter ScriptableObject's
            // own list: BattleManager consumes this list via RemoveAt while playing
            // dialogue, which would otherwise permanently empty the cached asset and
            // break dialogue (and the tutorial flow that depends on it) on replay.
            enemyHandler.DialogueToPlayOnMeet = new List<DialogueInfo>(spawn.DialogueToPlayOnMeet);
            enemyHandler.ShouldStallBeforeTurn = spawn.ShouldStallBeforeTurn;
            enemyHandler.SetTimeToNextObject(spawn.TimeToNextObject);
            if (i > 0)
            {
                enemyObject.SetActive(false);
            }
            spawnedEnemies.Add(enemyHandler);
        }

        for (int i = 0; i < spawnedEnemies.Count - 1; i++)
        {
            spawnedEnemies[i].SetNextBattleObject(spawnedEnemies[i + 1].gameObject);
        }

        EnemyHandler lastEnemy = spawnedEnemies[^1];
        List<Treasure> choices = GameManager.GameData.GetRandomUnownedTreasures(3);
        bool hasChest = choices.Count > 0;
        if (hasChest)
        {
            GameObject chestObject = Instantiate(_treasureChestPrefab, _stagingAnchor.position, Quaternion.identity, _spawnedObjectsParent);
            TreasureChest chest = chestObject.GetComponentInChildren<TreasureChest>();
            if (chest != null)
            {
                chest.SetChoices(choices);
                chestObject.SetActive(false);
                lastEnemy.SetNextBattleObject(chestObject);
            }
            else
            {
                Debug.LogError("[LevelSpawner] TreasureChest component not found on the chest prefab — check the script GUID in TreasureChest.prefab.");
                Destroy(chestObject);
                hasChest = false;
            }
        }

        int totalEvents = encounter.Enemies.Count + (hasChest ? 1 : 0);
        FindAnyObjectByType<UICompletionBar>(FindObjectsInactive.Include).Initialize(totalEvents);
        BattleManager.Instance.CurrEnemyHandler = spawnedEnemies[0];
    }

    /// <summary>
    /// Event-mode path: hides the completion bar, loads the EventData for the
    /// current payload id, attaches the EventBehaviour, supplies scene context
    /// to any behaviour that implements INeedsEventSceneContext, and calls
    /// BeginEvent. Normal encounter spawning is skipped. No per-event fields
    /// or per-event if-checks are needed on LevelSpawner.
    /// </summary>
    private void AwakeEventMode()
    {
        UICompletionBar completionBar = FindAnyObjectByType<UICompletionBar>(FindObjectsInactive.Include);
        if (completionBar != null) { completionBar.gameObject.SetActive(false); }

        string eventId = GameManager.GameData.RecentLevelCompleted;
        EventData eventData = GameDatabase.Events != null ? GameDatabase.Events.GetEvent(eventId) : null;
        if (eventData == null)
        {
            Debug.LogError($"[LevelSpawner] No EventData found for id '{eventId}'.");
            return;
        }

        GameObject eventObj = new($"Event_{eventData.EventId}");
        EventBehaviour behaviour = eventData.AttachBehaviour(eventObj);

        // Supply scene-level references to any behaviour that needs them
        // (e.g. BattleEvent). New battle events require no change here.
        if (behaviour is INeedsEventSceneContext ctx)
        {
            ctx.SetSceneContext(new EventSceneContext(
                _enemyPrefab, _enemySpawnAnchor, _stagingAnchor, _spawnedObjectsParent));
        }

        behaviour.BeginEvent();
    }

}
