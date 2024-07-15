using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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

    public Action OnPlayerAttack = null;
    public Action OnEnemyAttack = null;
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
    /// Render a new enemy.
    /// </summary>
    public void SetNewEnemy(HealthHandler enemyHealthHandler)
    {
        _enemyHealth = enemyHealthHandler;
    }

    /// <summary>
    /// Deal damage (as the player) to the enemy.
    /// </summary>
    public void RenderPlayerDamage()
    {
        OnPlayerAttack?.Invoke();
        int wordLength = WordPreview.Instance.CurrentWord.Length;
        // Algorithm to determine damage from word!
        int damageDealt = (int)Mathf.Round(Mathf.Exp(wordLength * 0.4f));
        // Make the enemy actually take the damage.
        _enemyHealth.TakeDamage(damageDealt);
    }

    /// <summary>
    /// Render damage (as the enemy) to the player.
    /// </summary>
    public void RenderEnemyDamage()
    {
        OnEnemyAttack?.Invoke();
        // Make the player actually take the damage.
        _playerHealth.TakeDamage(4);
    }

}
