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
