using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct EnemyInfo
{
    public Sprite InfoSprite;
    public string Name;
    public string Description;
}

public class EnemyInfoBox : MonoBehaviour
{

    private static EnemyInfoBox _instance;
    public static EnemyInfoBox Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<EnemyInfoBox>();
            }
            return _instance;
        }
    }

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _infoPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _attackContainer;
    [SerializeField] private TextMeshPro _enemyDescText;

    private readonly List<Tuple<string, EnemyInfoHandler>> _instantiatedAttacks = new();

    private void Awake()
    {
        if (!ReferenceEquals(_instance, this))
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }
    }

    /// <summary>
    /// Given an Enemy's data, sets the info box's description to
    /// match the enemy and creates an info prefab for each attack.
    /// </summary>
    public void SetInfo(EnemyData enemyData)
    {
        for (int i = _instantiatedAttacks.Count - 1; i >= 0; i--)
        {
            Destroy(_instantiatedAttacks[i].Item2.gameObject);
        }
        _instantiatedAttacks.Clear();
        // Compile all attacks to load UI
        List<EnemyInfo> infos = new();
        foreach (EnemyAttack attack in enemyData.Attacks)
        {
            infos.Add(new EnemyInfo()
            {
                Name = attack.AttackName,
                Description = attack.AttackDescription,
                InfoSprite = attack.IconSprite
            });
        }
        // Instantiate an instance for each piece of info
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject infoObject = Instantiate(_infoPrefab, _attackContainer, false);
            infoObject.transform.position -= new Vector3(0, 1f * i);
            infoObject.GetComponent<EnemyInfoHandler>().Initialize(infos[i]);
            _instantiatedAttacks.Add(new(infos[i].Name, infoObject.GetComponent<EnemyInfoHandler>()));
        }
        // Set description text
        _enemyDescText.text = enemyData.EnemyDescription;
    }

    /// <summary>
    /// Given the name of an attack, flashes it to indicate
    /// that the attack has been used.
    /// </summary>
    public void FlashAttackByName(string attackName)
    {
        for (int i = 0; i < _instantiatedAttacks.Count; i++)
        {
            if (_instantiatedAttacks[i].Item1 == attackName)
            {
                _instantiatedAttacks[i].Item2.FlashAttack();
            }
        }
    }

}
