using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : CharacterHandler
{

    [Header("Enemy Properties")]
    [SerializeField] private EnemyData _enemyInfo;
    [SerializeField] private GameObject _nextEnemyObject;  // If null, this is the last enemy

    private void Awake()
    {
        InitializeInfo(_enemyInfo);
    }

    private void Start()
    {
        if (_nextEnemyObject != null)
        {
            // If there's a "next" enemy to render, transition to it
            _healthHandler.OnDeath += TransitionToNextEnemy;
        } else
        {
            // Or else, the player has defeated all enemies in this level
            _healthHandler.OnDeath += () =>
            {
                Debug.Log("All enemies defeated!");
            };
        }
    }

    /// <summary>
    /// Animate this character dying, and make the next enemy
    /// animate in.
    /// </summary>
    public void TransitionToNextEnemy()
    {
        // TODO: Not completed yet!
    }

}
