using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{

    [Header("Enemy Properties")]
    public int StartingHealth;
    public Sprite AliveSprite;
    public Sprite DeadSprite;

}
