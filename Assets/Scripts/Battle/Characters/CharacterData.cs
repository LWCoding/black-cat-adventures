using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpriteInfo
{
    public Sprite Sprite;
    public Vector2 Offset;
}

[System.Serializable]
public abstract class CharacterData : ScriptableObject
{

    [Header("Enemy Properties")]
    public int StartingHealth;
    public SpriteInfo AliveSprite;
    public SpriteInfo AttackSprite;
    public SpriteInfo DeadSprite;
    public Vector2 SpriteScale = new(1, 1);
    [Header("Optional")]
    [Tooltip("Optional: periodically swapped with the idle sprite while Airborne. Leave empty for no toggle.")]
    public SpriteInfo AirborneSprite;

}
