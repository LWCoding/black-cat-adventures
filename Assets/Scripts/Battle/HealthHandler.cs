using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _fallingTextPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _fallingTextSpawnpoint;
    [Header("Optional Properties")]
    public TextMeshPro TextToUpdate;  // If there's text, update text to match the health
    public Transform FillBarToUpdate;  // If there's a bar, scale it from 0 (dead) to 1 (alive)

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (TextToUpdate != null)
            {
                TextToUpdate.text = value.ToString() + "/" + _maxHealth.ToString();
            }
            if (FillBarToUpdate != null)
            {
                FillBarToUpdate.localScale = new Vector3((float)_currentHealth / _maxHealth, FillBarToUpdate.localScale.y);
            }
        }
    }

    public Action OnDeath = null;  // Called when this enemy hits zero health

    private int _currentHealth;
    private int _maxHealth;

    public bool IsDead() => _currentHealth == 0;

    /// <summary>
    /// Initialize this character's starting health to a
    /// specified value.
    /// </summary>
    public void InitializeHealth(int startingHealth)
    {
        _maxHealth = startingHealth;
        CurrentHealth = startingHealth;
    }

    /// <summary>
    /// Makes this character take a certain amount of damage.
    /// Spawns a falling text object to show the damage taken.
    /// Caps damage dealt if it goes below zero.
    /// Does not allow for negative taken to be taken.
    /// If health reaches zero, invokes OnDeath() function.
    /// </summary>
    public void TakeDamage(int damageTaken)
    {
        if (damageTaken < 0) { return; }
        CurrentHealth -= damageTaken;
        // Spawn a falling text prefab to emphasize damage taken
        GameObject fallingText = Instantiate(_fallingTextPrefab, _fallingTextSpawnpoint);
        fallingText.GetComponent<FallingText>().Initialize("-" + damageTaken.ToString(), new Color(1, 0.05f, 0.05f));
        // Cap health at zero
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
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
