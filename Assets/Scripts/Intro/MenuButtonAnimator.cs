using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Drives DOTween select/deselect animations on a UI Button: scale-up on the X axis,
/// slight Y scale-up, and an unsaturated green tint on the background Image.
/// The TMP label child is counter-scaled so only the background grows, not the text.
/// Deselect reverses the animation faster. Pointer-enter routes through EventSystem
/// selection so mouse and keyboard share the same highlighted state.
///
/// Disable the Button's built-in ColorTint transition and automatic Navigation before
/// (or via Configure) to avoid fighting these tweens.
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButtonAnimator : MonoBehaviour,
    ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private float _horizontalScale = 1.3f;
    [SerializeField] private float _verticalScale = 1.1f;

    [Header("Glow")]
    [SerializeField] private Color _glowColor = new Color(0.141f, 0.475f, 0.227f, 1f);

    [Header("Timing")]
    [SerializeField] private float _forwardDuration = 0.22f;
    [SerializeField] private float _reverseSpeedMultiplier = 2f;

    private Button _button;
    private Image _image;
    private Vector3 _baseScale;
    private Color _baseColor;

    private Transform _labelTransform;
    private Vector3 _baseLabelScale;
    private TMP_Text _label;
    private Color _baseLabelColor;

    private Tween _scaleTween;
    private Tween _colorTween;
    private Tween _labelScaleTween;
    private Tween _labelColorTween;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = _button.targetGraphic as Image;

        _baseScale = transform.localScale;
        _baseColor = _image != null ? _image.color : Color.white;

        _label = GetComponentInChildren<TMP_Text>();
        if (_label != null)
        {
            _labelTransform = _label.transform;
            _baseLabelScale = _labelTransform.localScale;
            _baseLabelColor = _label.color;
        }

        _button.transition = Selectable.Transition.None;
        _button.navigation = new Navigation { mode = Navigation.Mode.None };
    }

    public void Configure(
        float horizontalScale, float verticalScale,
        Color glowColor,
        float forwardDuration, float reverseSpeedMultiplier)
    {
        _horizontalScale = horizontalScale;
        _verticalScale = verticalScale;
        _glowColor = glowColor;
        _forwardDuration = forwardDuration;
        _reverseSpeedMultiplier = reverseSpeedMultiplier;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Only clear selection if this button is currently selected; avoids fighting keyboard nav.
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!_button.interactable) { return; }
        AnimateForward();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        AnimateReverse();
    }

    private void AnimateForward()
    {
        KillTweens();

        Vector3 targetScale = new Vector3(
            _baseScale.x * _horizontalScale,
            _baseScale.y * _verticalScale,
            _baseScale.z);

        _scaleTween = transform
            .DOScale(targetScale, _forwardDuration)
            .SetEase(Ease.OutBack);

        if (_image != null)
        {
            _colorTween = _image
                .DOColor(_glowColor, _forwardDuration)
                .SetEase(Ease.OutQuad);
        }

        // Counter-scale the label so the text stays at its original apparent size.
        if (_labelTransform != null)
        {
            Vector3 counterScale = new Vector3(
                _baseLabelScale.x / _horizontalScale,
                _baseLabelScale.y / _verticalScale,
                _baseLabelScale.z);

            _labelScaleTween = _labelTransform
                .DOScale(counterScale, _forwardDuration)
                .SetEase(Ease.OutBack);
        }

        if (_label != null)
        {
            _labelColorTween = DOTween
                .To(() => _label.color, c => _label.color = c, Color.white, _forwardDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    private void AnimateReverse()
    {
        KillTweens();

        float reverseDuration = _forwardDuration / _reverseSpeedMultiplier;

        _scaleTween = transform
            .DOScale(_baseScale, reverseDuration)
            .SetEase(Ease.OutQuad);

        if (_image != null)
        {
            _colorTween = _image
                .DOColor(_baseColor, reverseDuration)
                .SetEase(Ease.OutQuad);
        }

        if (_labelTransform != null)
        {
            _labelScaleTween = _labelTransform
                .DOScale(_baseLabelScale, reverseDuration)
                .SetEase(Ease.OutQuad);
        }

        if (_label != null)
        {
            _labelColorTween = DOTween
                .To(() => _label.color, c => _label.color = c, Color.black, reverseDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    private void KillTweens()
    {
        _scaleTween?.Kill();
        _scaleTween = null;
        _colorTween?.Kill();
        _colorTween = null;
        _labelScaleTween?.Kill();
        _labelScaleTween = null;
        _labelColorTween?.Kill();
        _labelColorTween = null;
    }

    private void OnDisable()
    {
        KillTweens();
        if (transform != null) { transform.localScale = _baseScale; }
        if (_image != null) { _image.color = _baseColor; }
        if (_labelTransform != null) { _labelTransform.localScale = _baseLabelScale; }
        if (_label != null) { _label.color = _baseLabelColor; }
    }

    private void OnDestroy()
    {
        KillTweens();
    }
}
