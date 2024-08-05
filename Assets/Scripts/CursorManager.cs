using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    private static CursorManager _instance;
    public static CursorManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<CursorManager>();
            }
            return _instance;
        }
    }

    public Texture2D BasicCursorSprite;
    public Texture2D SelectCursorSprite;

    private void Awake()
    {
        if (!ReferenceEquals(_instance, this))
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }
    }

    // This method sets the cursor sprite
    public void SetPointerCursor()
    {
        Cursor.SetCursor(SelectCursorSprite, Vector2.zero, CursorMode.Auto);
    }

    // This method resets the cursor to the default
    public void ResetCursor()
    {
        Cursor.SetCursor(BasicCursorSprite, Vector2.zero, CursorMode.Auto);
    }

}
