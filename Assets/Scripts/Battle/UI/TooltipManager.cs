using TMPro;
using UnityEngine;

public class TooltipManager : Singleton<TooltipManager>
{

    [Header("Object Assignments")]
    [SerializeField] private GameObject _tooltipBubble;
    [SerializeField] private TextMeshPro _tooltipText;

    [Header("Settings")]
    [SerializeField] private float _verticalOffset = 0.9f;

    protected override void Awake()
    {
        base.Awake();
        _tooltipBubble.SetActive(false);
    }

    /// <summary>
    /// Shows the shared tooltip bubble at the given world position (offset downward)
    /// with the provided text.
    /// </summary>
    public void Show(string text, Vector3 worldPosition)
    {
        _tooltipText.text = text;
        _tooltipBubble.transform.position = worldPosition + Vector3.down * _verticalOffset;
        _tooltipBubble.SetActive(true);
    }

    /// <summary>
    /// Hides the shared tooltip bubble.
    /// </summary>
    public void Hide()
    {
        _tooltipBubble.SetActive(false);
    }

}
