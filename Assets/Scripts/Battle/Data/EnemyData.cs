using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackAnimation
{
    DEFAULT = 0, NONE = 1
}

[System.Serializable]
public struct EnemyAttack
{

    public string AttackName;
    public int Damage;
    public AttackAnimation AnimType;

}

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy")]
public class EnemyData : CharacterData
{

    public List<EnemyAttack> Attacks = new();

}
