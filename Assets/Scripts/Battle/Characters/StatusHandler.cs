using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusHandler : MonoBehaviour
{

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _statusPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _statusParentTransform;

    private readonly List<StatusEffect> _effects = new();
    private List<StatusObject> _statusObjects = new();

    private CharacterHandler _characterHandler;

    private const float SPACE_BETWEEN_STATUSES = 0.15f;

    public Action OnStatusApplied = null;

    public void Initialize(CharacterHandler characterHandler)
    {
        _characterHandler = characterHandler;
    }

    /// <summary>
    /// Make this character gain a new status effect. Immediately renders
    /// any effects on apply. If the character already has the effect,
    /// increases the amplifier instead.
    /// 
    /// When a character gains an effect, instantiate a prefab to show it.
    /// Pass hideIcon: true to start the icon fully transparent (call RevealStatusIcons later to fade it in).
    /// </summary>
    public void GainStatusEffect(StatusEffect status, int amplifier, bool hideIcon = false)
    {
        OnStatusApplied?.Invoke();
        int existingStatusIdx = _effects.FindIndex((aStatus) => aStatus.Type == status.Type);
        StatusEffect statusCopy = Instantiate(status);  // Make new status effect object
        if (existingStatusIdx < 0)
        {
            GameObject statusObj = Instantiate(_statusPrefab, _statusParentTransform);
            StatusObject statusObject = statusObj.GetComponent<StatusObject>();
            statusObject.UpdateStatusInfo(statusCopy);
            if (hideIcon) { statusObject.SetHiddenForIntro(); }
            _statusObjects.Add(statusObject);
            _effects.Add(statusCopy);  // Make new status effect object
            statusCopy.ApplyEffect(_characterHandler, amplifier);
        }
        else
        {
            _effects[existingStatusIdx].CurrAmplifier += amplifier;
        }
        RerenderStatusIcons();
    }

    /// <summary>
    /// Fades in all status icons that were hidden for an intro animation.
    /// </summary>
    public void RevealStatusIcons(float duration = 0.4f)
    {
        foreach (StatusObject statusObject in _statusObjects)
        {
            statusObject.RevealForIntro(duration);
        }
    }

    /// <summary>
    /// Renders the turn-passed effect for any status effects currently on
    /// this character. Runs in reverse order.
    /// 
    /// If the status effect should go away, removes the status effect and
    /// deletes its corresponding GameObject.
    /// </summary>
    public void RenderStatusEffectEffects()
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            bool isDone = _effects[i].UpdateEffect(_characterHandler);
            if (isDone)
            {
                _effects.RemoveAt(i);
                Destroy(_statusObjects[i].gameObject);
                _statusObjects.RemoveAt(i);
            }
        }
        RerenderStatusIcons();
    }

    /// <summary>
    /// Repositions all of the objects in _statusObjects so that they
    /// are centered. Also updates any amplifier values if changed.
    /// </summary>
    public void RerenderStatusIcons()
    {
        // Calculate position based on # of letters
        float spacePerStatus = _statusPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.size.x * _statusPrefab.transform.GetChild(0).transform.localScale.x + SPACE_BETWEEN_STATUSES;
        Vector3 startingOffset;
        if (_effects.Count % 2 == 0)
        {
            // Even number
            startingOffset = new Vector3(0, -(_effects.Count / 2f) * spacePerStatus);
        }
        else
        {
            // Odd number
            startingOffset = new Vector3(0, -(_effects.Count / 2f) * spacePerStatus);
        }
        startingOffset += new Vector3(0, spacePerStatus / 2);
        for (int i = 0; i < _effects.Count; i++) {
            // Calculate position based on # of letters
            _statusObjects[i].transform.localPosition = startingOffset + new Vector3(0, spacePerStatus * i, 0);
            // Update the status information
            _statusObjects[i].UpdateStatusInfo(_effects[i]);
        }
    }

    /// <summary>
    /// Returns true if this character currently has a status effect of the given type.
    /// </summary>
    public bool HasStatus(StatusEffectType type) => _effects.Exists(e => e.Type == type);

    /// <summary>
    /// Toggle the visibility of the status effects.
    /// </summary>
    public void ToggleEffectVisibility(bool isVisible)
    {
        _statusParentTransform.gameObject.SetActive(isVisible);
    }

}
