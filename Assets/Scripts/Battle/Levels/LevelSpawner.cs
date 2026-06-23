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
    [SerializeField] private GameObject _rewardPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _enemySpawnAnchor;
    [SerializeField] private Transform _stagingAnchor;
    [SerializeField] private Transform _spawnedObjectsParent;

    private void Awake()
    {
        string encounterId = GameManager.GameData.RecentLevelCompleted;
        Encounter encounter = _encounterDatabase.GetEncounter(encounterId);

        List<EnemyHandler> spawnedEnemies = new();
        for (int i = 0; i < encounter.Enemies.Count; i++)
        {
            EnemySpawn spawn = encounter.Enemies[i];
            Vector3 spawnPosition = (i == 0 ? _enemySpawnAnchor.position : _stagingAnchor.position) + (Vector3)spawn.SpawnOffset;
            GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _spawnedObjectsParent);
            EnemyHandler enemyHandler = enemyObject.GetComponent<EnemyHandler>();
            enemyHandler.SetCharacterData(spawn.EnemyData);
            enemyHandler.DialogueToPlayOnMeet = spawn.DialogueToPlayOnMeet;
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
        if (encounter.RewardTreasure != null)
        {
            GameObject rewardObject = Instantiate(_rewardPrefab, _stagingAnchor.position, Quaternion.identity, _spawnedObjectsParent);
            rewardObject.GetComponentInChildren<TreasureCollectible>().SetTreasureData(encounter.RewardTreasure);
            rewardObject.SetActive(false);
            lastEnemy.SetNextBattleObject(rewardObject);
        }

        BattleManager.Instance.CurrEnemyHandler = spawnedEnemies[0];
        int totalEvents = encounter.Enemies.Count + (encounter.RewardTreasure != null ? 1 : 0);
        FindAnyObjectByType<UICompletionBar>(FindObjectsInactive.Include).Initialize(totalEvents);
    }

}
