using UnityEngine;

/// <summary>
/// Subtle idle breathing effect for battle characters. Attach to the
/// sprite child ("Image") of a player or enemy. Animates visual scale
/// only, oscillating around whatever local scale is present at start so
/// per-character SpriteScale is respected and cleanly restored.
/// </summary>
public class IdleSquashStretch : MonoBehaviour
{
    [Header("Idle Animation Tuning")]
    [SerializeField] private float squashAmount = 0.04f;
    [SerializeField] private float stretchAmount = 0.04f;
    [SerializeField] private float animationSpeed = 2f;
    [Tooltip("Offsets the wave so multiple characters don't bob in perfect sync.")]
    [SerializeField] private float phaseOffset = 0f;

    [Header("Anchoring")]
    [Tooltip("Local Y position the sprite child stays anchored at, keeping the character grounded instead of floating upward while it squashes and stretches.")]
    [SerializeField] private float imageLocalYOffset = -1f;

    private Vector3 _baseScale = Vector3.one;
    private Vector3 _basePosition;

    private void Start()
    {
        _baseScale = transform.localScale;
        // Remember the sprite's resting position and anchor its Y to the
        // intended ground offset so the squash/stretch never drifts it upward.
        _basePosition = transform.localPosition;
        _basePosition.y = imageLocalYOffset;
        transform.localPosition = _basePosition;
    }

    private void Update()
    {
        float wave = Mathf.Sin(Time.time * animationSpeed + phaseOffset); // -1..1
        // Stretch (taller) on the upswing, squash (shorter) on the downswing.
        float amount = wave >= 0f ? wave * stretchAmount : wave * squashAmount;
        // Preserve volume feel: stretch narrows X while raising Y, squash widens X while lowering Y.
        transform.localScale = new Vector3(
            _baseScale.x * (1f - amount),
            _baseScale.y * (1f + amount),
            _baseScale.z);
        // Keep the sprite grounded: scaling must not push the image upward.
        Vector3 pos = transform.localPosition;
        pos.y = _basePosition.y;
        transform.localPosition = pos;
    }

    private void OnDisable()
    {
        transform.localScale = _baseScale;
        Vector3 pos = transform.localPosition;
        pos.y = _basePosition.y;
        transform.localPosition = pos;
    }
}
