using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{

    [Header("Data Assignments")]
    [SerializeField] private LevelRegistry _levelRegistry;
    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _rewardPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _enemySpawnAnchor;
    [SerializeField] private Transform _stagingAnchor;
    [SerializeField] private Transform _spawnedObjectsParent;

    private void Awake()
    {
        string levelId = GameManager.GameData.RecentLevelCompleted;
        LevelData levelData = _levelRegistry.GetLevelData(levelId);

        List<EnemyHandler> spawnedEnemies = new();
        for (int i = 0; i < levelData.Encounters.Count; i++)
        {
            EnemyEncounter encounter = levelData.Encounters[i];
            Vector3 spawnPosition = (i == 0 ? _enemySpawnAnchor.position : _stagingAnchor.position) + (Vector3)encounter.SpawnOffset;
            GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _spawnedObjectsParent);
            EnemyHandler enemyHandler = enemyObject.GetComponent<EnemyHandler>();
            enemyHandler.SetCharacterData(encounter.EnemyData);
            enemyHandler.DialogueToPlayOnMeet = encounter.DialogueToPlayOnMeet;
            enemyHandler.ShouldStallBeforeTurn = encounter.ShouldStallBeforeTurn;
            enemyHandler.SetTimeToNextObject(encounter.TimeToNextObject);
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
        if (levelData.RewardTreasure != null)
        {
            GameObject rewardObject = Instantiate(_rewardPrefab, _stagingAnchor.position, Quaternion.identity, _spawnedObjectsParent);
            rewardObject.GetComponentInChildren<TreasureCollectible>().SetTreasureData(levelData.RewardTreasure);
            rewardObject.SetActive(false);
            lastEnemy.SetNextBattleObject(rewardObject);
        }

        BattleManager.Instance.CurrEnemyHandler = spawnedEnemies[0];
        // The reward also carries its own EnemyHandler (see Reward.prefab) so the existing
        // transition/SetNewEnemy machinery treats it as one final "enemy" - count it to match.
        int totalEvents = levelData.Encounters.Count + (levelData.RewardTreasure != null ? 1 : 0);
        FindAnyObjectByType<UICompletionBar>(FindObjectsInactive.Include).Initialize(totalEvents);
    }

}
