using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelSpawner : MonoBehaviour
{

    [Header("Data Assignments")]
    [FormerlySerializedAs("_levelRegistry")]
    [FormerlySerializedAs("_encounterRegistry")]
    [SerializeField] private EncounterDatabase _encounterDatabase;
    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _treasureChestPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _enemySpawnAnchor;
    [SerializeField] private Transform _stagingAnchor;
    [SerializeField] private Transform _spawnedObjectsParent;

    private void Awake()
    {
        string encounterId = GameManager.GameData.RecentLevelCompleted;

        // Always use the tutorial encounter for the player's very first battle,
        // unless the tutorial has already been completed (e.g. on a continued save
        // or after an F7 skip that was properly persisted).
        bool needsTutorial = !GameManager.GameData.HasTutorialCompleted
            && GameManager.GameData.LevelsCompleted.Count == 0
            && _encounterDatabase.TutorialEncounter != null;
        Encounter encounter =
            needsTutorial
                ? _encounterDatabase.TutorialEncounter
                : _encounterDatabase.GetEncounter(encounterId)
                  ?? _encounterDatabase.RollNormal(new System.Random(), 0);

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

}
