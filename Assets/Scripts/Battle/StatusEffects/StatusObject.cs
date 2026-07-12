using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatusObject : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _amplifierText;
    [SerializeField] private TextMeshPro _tooltipText;

    private SpriteRenderer[] _cachedSprites;
    private float[] _cachedSpriteAlphas;
    private TMP_Text[] _cachedTexts;
    private float[] _cachedTextAlphas;
    private bool _isHidden;

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

    /// <summary>
    /// Captures the current alpha of all active renderers/texts then sets them to 0,
    /// so the icon is invisible but still occupies layout space.
    /// </summary>
    public void SetHiddenForIntro()
    {
        _cachedSprites = GetComponentsInChildren<SpriteRenderer>(false);
        _cachedSpriteAlphas = new float[_cachedSprites.Length];
        for (int i = 0; i < _cachedSprites.Length; i++)
        {
            _cachedSpriteAlphas[i] = _cachedSprites[i].color.a;
            Color c = _cachedSprites[i].color;
            c.a = 0f;
            _cachedSprites[i].color = c;
        }

        _cachedTexts = GetComponentsInChildren<TMP_Text>(false);
        _cachedTextAlphas = new float[_cachedTexts.Length];
        for (int i = 0; i < _cachedTexts.Length; i++)
        {
            _cachedTextAlphas[i] = _cachedTexts[i].alpha;
            _cachedTexts[i].alpha = 0f;
        }

        _isHidden = true;
    }

    /// <summary>
    /// Fades all renderers/texts back to their pre-hidden alphas over <paramref name="duration"/> seconds.
    /// No-op if SetHiddenForIntro was never called.
    /// </summary>
    public void RevealForIntro(float duration)
    {
        if (!_isHidden) { return; }
        _isHidden = false;

        for (int i = 0; i < _cachedSprites.Length; i++)
        {
            _cachedSprites[i].DOFade(_cachedSpriteAlphas[i], duration);
        }

        for (int i = 0; i < _cachedTexts.Length; i++)
        {
            float targetAlpha = _cachedTextAlphas[i];
            TMP_Text text = _cachedTexts[i];
            DOTween.To(() => text.alpha, a => text.alpha = a, targetAlpha, duration);
        }
    }

}
