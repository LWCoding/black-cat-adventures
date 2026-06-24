using DG.Tweening;
using UnityEngine;

/// <summary>
/// Loops configurable idle animations (tilt and/or scale pulse) on a transform using DOTween.
/// Toggle each effect independently via the inspector or via the Enable* properties before the
/// component's Start() fires. Call Stop() to end all animations permanently.
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

    public bool EnableTilt { get => _enableTilt; set => _enableTilt = value; }
    public bool EnableScalePulse { get => _enableScalePulse; set => _enableScalePulse = value; }

    private Tween _tiltTween;
    private Tween _scaleTween;
    private bool _stopped;
    private bool _started;

    private void Start()
    {
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

    public void Stop(bool resetTransforms = true)
    {
        _stopped = true;
        _tiltTween?.Kill();
        _tiltTween = null;
        _scaleTween?.Kill();
        _scaleTween = null;
        if (resetTransforms)
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        _tiltTween?.Kill();
        _scaleTween?.Kill();
    }
}
