using DG.Tweening;
using UnityEngine;

/// <summary>
/// Loops configurable idle animations (tilt, scale pulse, and/or squash &amp; stretch) on a transform
/// using DOTween. Toggle each effect independently via the inspector or via the Enable* properties
/// before the component's Start() fires. Call Stop() to end all animations permanently.
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

    private void Start()
    {
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
        Vector3 squashed = new Vector3(_baseScale.x * (1f + a), _baseScale.y * (1f - a), _baseScale.z);
        Vector3 stretched = new Vector3(_baseScale.x * (1f - a), _baseScale.y * (1f + a), _baseScale.z);
        transform.localScale = squashed;
        _squashStretchTween = transform
            .DOScale(stretched, _squashStretchDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(_squashStretchPhase);
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
        if (resetTransforms)
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale = _baseScale == Vector3.zero ? Vector3.one : _baseScale;
        }
    }

    private void OnDestroy()
    {
        _tiltTween?.Kill();
        _scaleTween?.Kill();
        _squashStretchTween?.Kill();
    }
}
