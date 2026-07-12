using DG.Tweening;
using UnityEngine;

/// <summary>
/// Loops configurable idle animations (tilt, scale pulse, and/or squash &amp; stretch) on a transform
/// using DOTween. Toggle each effect independently via the inspector or via the Enable* properties
/// before the component's Start() fires. Call Stop() to end all animations permanently.
///
/// When a <see cref="SpriteScaleRoot"/> is present on the same GameObject, squash &amp; stretch is
/// routed through it (so persistent scale modifiers such as Shrink compose correctly).
/// Scale Pulse is not routed — it is a deliberate uniform emphasis effect that should override
/// all modifiers, so it still tweens localScale directly.
/// </summary>
public class IdleAnimation : MonoBehaviour
{
    [Header("Tilt")]
    [SerializeField] private bool _enableTilt = true;
    [SerializeField] private float _tiltAngle = 8f;
    [SerializeField] private float _tiltDuration = 2.5f;

    [Header("Scale Pulse")]
    [SerializeField] private bool _enableScalePulse = false;
    [SerializeField] private float _scalePulseAmount = 1.08f;
    [SerializeField] private float _scalePulseDuration = 1.8f;

    [Header("Squash & Stretch")]
    [SerializeField] private bool _enableSquashStretch = false;
    [SerializeField] private float _squashStretchAmount = 0.04f;
    [SerializeField] private float _squashStretchDuration = 1.57f;
    [Tooltip("Start delay in seconds — offset multiple characters so they don't move in sync.")]
    [SerializeField] private float _squashStretchPhase = 0f;

    public bool EnableTilt { get => _enableTilt; set => _enableTilt = value; }
    public bool EnableScalePulse { get => _enableScalePulse; set => _enableScalePulse = value; }
    public bool EnableSquashStretch { get => _enableSquashStretch; set => _enableSquashStretch = value; }

    private Tween _tiltTween;
    private Tween _scaleTween;
    private Tween _squashStretchTween;
    private Vector3 _baseScale;
    private bool _stopped;
    private bool _started;

    private SpriteScaleRoot _scaler;
    private float _squashValue;

    private void Start()
    {
        _scaler = GetComponent<SpriteScaleRoot>();
        _baseScale = transform.localScale;
        _started = true;
        if (!_stopped) { Play(); }
    }

    private void OnEnable()
    {
        // Only re-play on subsequent enable events; Start handles the initial play.
        if (!_started || _stopped) { return; }
        Play();
    }

    public void Play()
    {
        if (_enableTilt) { PlayTilt(); }
        if (_enableScalePulse) { PlayScalePulse(); }
        if (_enableSquashStretch) { PlaySquashStretch(); }
    }

    private void PlayTilt()
    {
        if (_tiltTween != null && _tiltTween.IsActive()) { return; }
        transform.localRotation = Quaternion.Euler(0, 0, _tiltAngle);
        _tiltTween = transform
            .DOLocalRotate(new Vector3(0, 0, -_tiltAngle), _tiltDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void PlayScalePulse()
    {
        if (_scaleTween != null && _scaleTween.IsActive()) { return; }
        _scaleTween = transform
            .DOScale(_scalePulseAmount, _scalePulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void PlaySquashStretch()
    {
        if (_squashStretchTween != null && _squashStretchTween.IsActive()) { return; }
        float a = _squashStretchAmount;

        if (_scaler != null)
        {
            // Route through SpriteScaleRoot so other scale modifiers (e.g. Shrink) compose cleanly.
            // Animate a float from +a (squashed) to -a (stretched) in a yoyo loop.
            // SpriteScaleRoot converts this into a per-axis Vector3 modifier each frame.
            _squashValue = a;
            _scaler.SetModifier("squashstretch", new Vector3(1f + a, 1f - a, 1f));
            _squashStretchTween = DOTween.To(
                () => _squashValue,
                v =>
                {
                    _squashValue = v;
                    _scaler.SetModifier("squashstretch", new Vector3(1f + v, 1f - v, 1f));
                },
                -a, _squashStretchDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(_squashStretchPhase);
        }
        else
        {
            // Fallback: direct localScale tween for GameObjects without a SpriteScaleRoot.
            Vector3 squashed = new Vector3(_baseScale.x * (1f + a), _baseScale.y * (1f - a), _baseScale.z);
            Vector3 stretched = new Vector3(_baseScale.x * (1f - a), _baseScale.y * (1f + a), _baseScale.z);
            transform.localScale = squashed;
            _squashStretchTween = transform
                .DOScale(stretched, _squashStretchDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(_squashStretchPhase);
        }
    }

    /// <summary>
    /// Updates the base scale used by all animations. If the component is already
    /// animating, the squash-stretch and scale-pulse tweens are restarted from the
    /// new base so that the updated CharacterData.SpriteScale is respected immediately
    /// (e.g. after F4/F5 enemy cycling in the editor).
    /// </summary>
    public void SetBaseScale(Vector3 baseScale)
    {
        _baseScale = baseScale;
        transform.localScale = baseScale;

        if (!_started || _stopped) { return; }

        // Restart scale-dependent tweens around the new base.
        if (_enableSquashStretch)
        {
            _squashStretchTween?.Kill();
            _squashStretchTween = null;
            PlaySquashStretch();
        }

        if (_enableScalePulse)
        {
            _scaleTween?.Kill();
            _scaleTween = null;
            PlayScalePulse();
        }
    }

    public void Stop(bool resetTransforms = true)
    {
        _stopped = true;
        _tiltTween?.Kill();
        _tiltTween = null;
        _scaleTween?.Kill();
        _scaleTween = null;
        _squashStretchTween?.Kill();
        _squashStretchTween = null;
        _scaler?.ClearModifier("squashstretch");
        if (resetTransforms)
        {
            transform.localRotation = Quaternion.identity;
            if (_scaler == null)
            {
                transform.localScale = _baseScale == Vector3.zero ? Vector3.one : _baseScale;
            }
        }
    }

    private void OnDestroy()
    {
        _tiltTween?.Kill();
        _scaleTween?.Kill();
        _squashStretchTween?.Kill();
        _scaler?.ClearModifier("squashstretch");
    }
}
