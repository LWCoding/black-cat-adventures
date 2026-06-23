using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerCursorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private bool _isEnabled = false;
    private bool _isHoveringOver = false;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            // If disabling while hovered, reset the cursor immediately.
            if (_isHoveringOver && !value)
            {
                CursorManager.Instance.ResetCursor();
            }
            // If enabling while hovered, apply the pointer cursor immediately.
            if (_isHoveringOver && value)
            {
                CursorManager.Instance.SetPointerCursor();
            }
            _isEnabled = value;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHoveringOver = true;
        if (!_isEnabled) { return; }
        CursorManager.Instance.SetPointerCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHoveringOver = false;
        if (!_isEnabled) { return; }
        CursorManager.Instance.ResetCursor();
    }

}
