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

    private readonly List<StatusEffect> _effects = new();

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
    /// any effects on apply. If the character already has the effect,
    /// increases the amplifier instead.
    /// </summary>
    public void GainStatusEffect(StatusEffect status, int amplifier)
    {
        int existingStatusIdx = _effects.FindIndex((aStatus) => aStatus.Name == status.Name);
        if (existingStatusIdx < 0)
        {
            _effects.Add(status);
            status.ApplyEffect(this, amplifier);
        } else
        {
            _effects[existingStatusIdx].CurrAmplifier += amplifier;
        }
    }

    /// <summary>
    /// Renders the turn-passed effect for any status effects currently on
    /// this character. Runs in reverse order.
    /// 
    /// If the status effect should go away, removes the status effect.
    /// </summary>
    public void RenderStatusEffectEffects()
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            bool isDone = _effects[i].UpdateEffect(this);
            if (isDone)
            {
                _effects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Given some character data, initializes this character to appear
    /// like that character and have the appropriate base statistics.
    /// </summary>
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
