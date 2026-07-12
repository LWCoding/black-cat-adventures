using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackAnimation
{
    DEFAULT = 0, NONE = 1, SAY_NOTHING = 2, PROJECTILE = 3
}

[System.Serializable]
public struct EnemyAttack
{

    public Sprite IconSprite;
    public string AttackName;
    [SerializeField] private string _attackDescription;
    public int Damage;
    public AttackAnimation AnimType;
    public List<AttackStatus> InflictedStatuses;
    public List<AttackBoardEffect> BoardEffects;
    [Tooltip("If set, this attack is skipped while the caster already has this status.")]
    public StatusEffect AvoidIfSelfHasStatus;
    [Tooltip("If set, this attack is skipped while the player already has this status.")]
    public StatusEffect AvoidIfTargetHasStatus;
    [Tooltip("Sprite shown as the projectile. If unset, falls back to IconSprite. Only used when AnimType is PROJECTILE.")]
    public Sprite ProjectileSprite;
    [Tooltip("Travel speed in world units per second. If 0, uses a default of 12 u/s. Only used when AnimType is PROJECTILE.")]
    public float ProjectileSpeed;

    // Replace %d instances with damage number
    public string AttackDescription => _attackDescription.Replace("%d", Damage.ToString());

}

[System.Serializable]
public struct StartingStatus
{
    public StatusEffect Status;
    public int Amplifier;
}

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy")]
public class EnemyData : CharacterData
{

    [Header("Enemy-Specific Information")]
    [TextArea(2, 3)] public string EnemyDescription;
    public List<EnemyAttack> Attacks = new();
    [Header("Starting Statuses")]
    [Tooltip("Status effects applied to this enemy when it enters the battlefield.")]
    public List<StartingStatus> StartingStatuses = new();

}
