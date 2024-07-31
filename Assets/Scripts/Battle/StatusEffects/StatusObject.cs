using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusObject : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _amplifierText;
    [SerializeField] private TextMeshPro _tooltipText;

    /// <summary>
    /// Update this status object's values to reflect a status's
    /// current status.
    /// </summary>
    public void UpdateStatusInfo(StatusEffect status)
    {
        _spriteRenderer.sprite = status.Icon;
        _amplifierText.text = status.CurrAmplifier.ToString();
        _tooltipText.text = "<b>" + status.Name + "</b> (" + status.CurrAmplifier.ToString() + " turn" + (status.CurrAmplifier == 1 ? "" : "s") + " left): " + status.Description.Replace("%d", status.CurrAmplifier.ToString());
    }

}
