using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthHandler))]
public class CharacterHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected HealthHandler _healthHandler;

    public void InitializeInfo(CharacterData charInfo)
    {
        if (charInfo == null)
        {
            Debug.LogError("CharacterHandler was not supplied valid information to initialize enemy!", this);
        }
        _spriteRenderer.sprite = charInfo.AliveSprite;
        _spriteRenderer.transform.localScale = charInfo.SpriteScale;
        _healthHandler.InitializeHealth(charInfo.StartingHealth);  // Initialize health
    }

}
