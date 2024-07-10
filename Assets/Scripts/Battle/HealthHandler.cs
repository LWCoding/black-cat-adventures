using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{

    [Header("Properties")]
    public int StartingHealth;
    [Header("Optional Properties")]
    public TextMeshPro TextToUpdate;

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (TextToUpdate != null)
            {
                TextToUpdate.text = value.ToString();
            }
        }
    }

    public bool IsDead => CurrentHealth <= 0;

    private int _currentHealth;
    private int _maxHealth;

    private void Awake()
    {
        CurrentHealth = StartingHealth;
    }

    /// <summary>
    /// Makes this character take a certain amount of damage.
    /// Caps damage dealt if it goes below zero.
    /// Does not allow for negative taken to be taken.
    /// </summary>
    public void TakeDamage(int damageTaken)
    {
        if (damageTaken < 0) { return; }
        CurrentHealth -= damageTaken;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }

    /// <summary>
    /// Makes this character heal a certain amount of health.
    /// Floors damage healed if it goes above the max health.
    /// </summary>
    public void HealHealth(int healthHealed)
    {
        CurrentHealth += healthHealed;
        if (CurrentHealth > _maxHealth)
        {
            CurrentHealth = _maxHealth;
        }
    }

}
