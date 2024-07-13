using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    #region Singleton Logic
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }
    #endregion

    [SerializeField] private HealthHandler _playerHealth;
    [SerializeField] private HealthHandler _enemyHealth;

    public State CurrentState;

    public Action<State> OnStateChanged = null;  // Parameter is the state to transition to

    private void Start()
    {
        _playerHealth.InitializeHealth(50);  // Initialize player health to static value
        SetState(new PlayerTurnState()); // Start off as player turn
    }

    public void SetState(State s)
    {
        OnStateChanged?.Invoke(s);
        CurrentState?.OnExitState();  // Exit from the current state, if any
        CurrentState = s;
        CurrentState.OnEnterState();
    }

    /// <summary>
    /// Deal damage (as the player) to the enemy using a
    /// specific word.
    /// </summary>
    public void DealDamageWithWord(string word)
    {
        int wordLength = word.Length;
        // Algorithm to determine damage from word!
        int damageDealt = (int)Mathf.Round(Mathf.Exp(wordLength * 0.6f));
        // Make the enemy actually take the damage.
        _enemyHealth.TakeDamage(damageDealt);
    }

    /// <summary>
    /// Take damage (as the player), given a specified
    /// amount of damage.
    /// </summary>
    public void MakePlayerTakeDamage(int damage)
    {
        _playerHealth.TakeDamage(damage);
    }

}
