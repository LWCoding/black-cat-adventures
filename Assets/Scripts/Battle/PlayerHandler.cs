using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : CharacterHandler
{

    [Header("Player Properties")]
    [SerializeField] private PlayerData _playerInfo;

    private void Awake()
    {
        InitializeInfo(_playerInfo);
    }

    private void Start()
    {
        //if (_nextEnemyObject != null)
        //{
        //    // If there's a "next" enemy to render, transition to it
        //    _healthHandler.OnDeath += TransitionToNextEnemy;
        //}
        //else
        //{
        //    // Or else, the player has defeated all enemies in this level
        //    _healthHandler.OnDeath += () =>
        //    {
        //        Debug.Log("All enemies defeated!");
        //    };
        //}
    }

}
