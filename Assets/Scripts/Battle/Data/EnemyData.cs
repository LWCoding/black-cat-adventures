using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackAnimation
{
    DEFAULT = 0, NONE = 1, SAY_NOTHING = 2
}

[System.Serializable]
public struct EnemyAttack
{

    public Sprite IconSprite;
    public string AttackName;
    [SerializeField] private string _attackDescription;
    public int Damage;
    public AttackAnimation AnimType;

    // Replace %d instances with damage number
    public string AttackDescription => _attackDescription.Replace("%d", Damage.ToString());

}

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy")]
public class EnemyData : CharacterData
{

    public List<EnemyAttack> Attacks = new();

}
