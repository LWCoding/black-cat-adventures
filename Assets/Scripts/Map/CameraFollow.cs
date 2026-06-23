using UnityEngine;

/// <summary>
/// Smoothly follows a target's x position with a configurable look-ahead bias,
/// keeping the camera's authored y and z. The bias shifts the camera right of the
/// target so the player appears in the left third of the screen, revealing upcoming
/// nodes for strategic planning. Clamp bounds are set by LevelsManager after nodes
/// are generated.
/// </summary>
public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [Tooltip("Lower values = snappier; higher values = smoother lag.")]
    [SerializeField] private float _smoothing = 5f;
    [Tooltip("Viewport fraction to shift the camera right of the target. " +
             "0 = centered. ~0.17 places the player at roughly the left third of the screen.")]
    [SerializeField] private float _leftBiasViewportFraction = 0.17f;

    private float _minX = float.NegativeInfinity;
    private float _maxX = float.PositiveInfinity;
    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    /// <summary>
    /// Called by LevelsManager once it knows the world-space x bounds of the generated graph.
    /// </summary>
    public void SetXBounds(float minX, float maxX)
    {
        _minX = minX;
        _maxX = maxX;
    }

    private void LateUpdate()
    {
        if (_target == null || _cam == null) { return; }

        // Shift the camera right by a fraction of the visible world width so the
        // player icon sits in the left third rather than dead-center.
        float worldWidth = 2f * _cam.orthographicSize * _cam.aspect;
        float desired    = _target.position.x + _leftBiasViewportFraction * worldWidth;
        float targetX    = Mathf.Clamp(desired, _minX, _maxX);

        Vector3 current = transform.position;
        float newX = Mathf.Lerp(current.x, targetX, _smoothing * Time.deltaTime);
        transform.position = new Vector3(newX, current.y, current.z);
    }

}
