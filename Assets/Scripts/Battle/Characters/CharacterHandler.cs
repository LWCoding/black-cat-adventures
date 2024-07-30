using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(HealthHandler))]
public abstract class CharacterHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    public HealthHandler HealthHandler;
    [Header("Character Properties")]
    [SerializeField] private CharacterData _charData;
    [Header("Optional Assignments")]
    [SerializeField] private DialogueBoxHandler _dBoxHandler;

    private List<IStatusEffect> _effects;

    public CharacterData CharData
    {
        get => _charData;
        protected set
        {
            _charData = value;
        }
    }

    private void Awake()
    {
        InitializeInfo(_charData);
        // Set the character's sprite to be dead when reaching zero health.
        HealthHandler.OnDeath += () =>
        {
            SetSprite(_charData.DeadSprite);
        };
    }

    /// <summary>
    /// Make this character gain a new status effect. Immediately renders
    /// any effects on apply.
    /// </summary>
    public void GainStatusEffect(IStatusEffect status)
    {
        _effects.Add(status);
        status.ApplyEffect(this);
    }

    public void InitializeInfo(CharacterData charInfo)
    {
        if (charInfo == null)
        {
            Debug.LogError("CharacterHandler was not supplied valid information to initialize enemy!", this);
        }
        _spriteRenderer.transform.localScale = charInfo.SpriteScale;
        HealthHandler.InitializeHealth(charInfo.StartingHealth);  // Initialize health
        SetSprite(charInfo.AliveSprite);
    }

    /// <summary>
    /// Queues dialogue if a dialogue box handler was assigned.
    /// If not, does nothing.
    /// </summary>
    public void SayDialogue(DialogueInfo di)
    {
        _dBoxHandler.SayDialogue(di);
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
