using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{

    public int CurrentHealth
    {
        get => _currentHealth;
    }

    public bool IsDead => CurrentHealth <= 0;

    private int _currentHealth;
    private int _maxHealth;

    /// <summary>
    /// Makes this character take a certain amount of damage.
    /// Caps damage dealt if it goes below zero.
    /// Does not allow for negative taken to be taken.
    /// </summary>
    public void TakeDamage(int damageTaken)
    {
        if (damageTaken < 0) { return; }
        _currentHealth -= damageTaken;
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }
    }

}
