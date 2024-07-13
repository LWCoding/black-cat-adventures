using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CharacterData : ScriptableObject
{

    [Header("Enemy Properties")]
    public int StartingHealth;
    public Sprite AliveSprite;
    public Sprite AttackSprite;
    public Sprite DeadSprite;
    public Vector2 SpriteScale = new(1, 1);
    public Vector2 SpriteOffset = new(0, 0);

}
