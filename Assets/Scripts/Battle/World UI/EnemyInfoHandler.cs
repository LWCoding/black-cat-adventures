using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyInfoHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TextMeshPro _infoText;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Initialize(EnemyInfo info)
    {
        _iconRenderer.sprite = info.InfoSprite;
        _infoText.text = "<b>" + info.Name + "</b>:\n" + info.Description;
    }

    /// <summary>
    /// Makes this image appear to flash for a short duration,
    /// indicating it has been used.
    /// </summary>
    public void FlashAttack()
    {
        _animator.Play("Flash");
    }

}
