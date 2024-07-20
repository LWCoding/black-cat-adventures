using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerCursorOnHover : MonoBehaviour
{

    private bool _isEnabled = false;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            // If we're disabling, and the mouse is hovering over this,
            // immediately call OnMouseExit()
            if (_isHoveringOver && !value)
            {
                OnMouseExit();
            }
            // If we're enabling, and the mouse is hovering over this,
            // immediately call OnMouseEnter()
            if (_isHoveringOver && value)
            {
                OnMouseEnter();
            }
            _isEnabled = value;
        }
    }

    private bool _isHoveringOver = false;

    public void OnMouseEnter()
    {
        _isHoveringOver = true;
        if (!IsEnabled) { return; }
        CursorManager.Instance.SetPointerCursor();
    }

    public void OnMouseExit() 
    {
        _isHoveringOver = false;
        if (!IsEnabled) { return; }
        CursorManager.Instance.ResetCursor();
    }

}
