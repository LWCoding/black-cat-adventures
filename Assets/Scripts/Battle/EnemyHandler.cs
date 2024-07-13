using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthHandler))]
public class EnemyHandler : MonoBehaviour
{

    [Header("Enemy Properties")]
    [SerializeField] private EnemyData _enemyInfo;
    [SerializeField] private GameObject _nextEnemyObject;  // If null, this is the last enemy

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private HealthHandler _healthHandler;

    private void Awake()
    {
        InitializeInfo(_enemyInfo);
    }

    public void InitializeInfo(EnemyData enemyInfo)
    {
        if (enemyInfo == null)
        {
            Debug.LogError("EnemyHandler was not supplied valid information to initialize enemy!", this);
        }
        _spriteRenderer.sprite = _enemyInfo.AliveSprite;
        _spriteRenderer.transform.localScale = _enemyInfo.SpriteScale;
        _healthHandler.InitializeHealth(_enemyInfo.StartingHealth);  // Initialize health
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
