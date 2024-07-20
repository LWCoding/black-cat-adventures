using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public static CursorManager Instance;

    public Texture2D BasicCursorSprite;
    public Texture2D SelectCursorSprite;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
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
