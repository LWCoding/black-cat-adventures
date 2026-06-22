using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{

    public Texture2D BasicCursorSprite;
    public Texture2D SelectCursorSprite;

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
