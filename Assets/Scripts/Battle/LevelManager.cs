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
        int damageDealt = word.Length;
        _enemyHealth.TakeDamage(damageDealt);
    }

}
