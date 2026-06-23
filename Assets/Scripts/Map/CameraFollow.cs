using UnityEngine;

/// <summary>
/// Smoothly follows a target's x position, keeping the camera's authored y and z.
/// Clamp bounds are set by LevelsManager after nodes are generated so the camera
/// never scrolls past the first or last node.
/// </summary>
public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [Tooltip("Lower values = snappier; higher values = smoother lag.")]
    [SerializeField] private float _smoothing = 5f;

    private float _minX = float.NegativeInfinity;
    private float _maxX = float.PositiveInfinity;

    /// <summary>
    /// Called by LevelsManager once it knows the world-space x bounds of the generated line.
    /// </summary>
    public void SetXBounds(float minX, float maxX)
    {
        _minX = minX;
        _maxX = maxX;
    }

    private void LateUpdate()
    {
        if (_target == null) { return; }

        float targetX = Mathf.Clamp(_target.position.x, _minX, _maxX);
        Vector3 current = transform.position;
        float newX = Mathf.Lerp(current.x, targetX, _smoothing * Time.deltaTime);
        transform.position = new Vector3(newX, current.y, current.z);
    }

}
