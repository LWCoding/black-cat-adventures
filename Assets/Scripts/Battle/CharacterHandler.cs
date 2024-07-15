using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthHandler))]
public abstract class CharacterHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected HealthHandler _healthHandler;
    [Header("Character Properties")]
    [SerializeField] protected CharacterData _charData;

    private void Awake()
    {
        InitializeInfo(_charData);
        // Set the character's sprite to be dead when reaching zero health.
        _healthHandler.OnDeath += () =>
        {
            SetSprite(_charData.DeadSprite);
        };
    }

    public void InitializeInfo(CharacterData charInfo)
    {
        if (charInfo == null)
        {
            Debug.LogError("CharacterHandler was not supplied valid information to initialize enemy!", this);
        }
        _spriteRenderer.transform.localScale = charInfo.SpriteScale;
        _healthHandler.InitializeHealth(charInfo.StartingHealth);  // Initialize health
        SetSprite(charInfo.AliveSprite);
    }

    /// <summary>
    /// Set this character's sprite to another sprite,
    /// if not passed in a null reference.
    /// </summary>
    protected void SetSprite(SpriteInfo newSprite)
    {
        if (newSprite.Sprite == null) { return; }
        _spriteRenderer.sprite = newSprite.Sprite;
        _spriteRenderer.transform.localPosition = (Vector3)newSprite.Offset;
    }

    protected abstract void RenderAttack();
    protected abstract IEnumerator RenderAttackCoroutine(Action codeToRunAfter);

}
