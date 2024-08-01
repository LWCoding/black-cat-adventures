using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureCollectible : MonoBehaviour
{

    [Header("Treasure Properties")]
    [SerializeField] private Treasure _treasureData;
    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TextMeshPro _tooltipText;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Registers all of the treasure stats.
    /// </summary>
    public void Initialize()
    {
        _iconRenderer.sprite = _treasureData.TreasureIcon;
        _tooltipText.text = "<b>" + _treasureData.TreasureName + "</b>:\n" + _treasureData.TreasureDescription;
    }

    public void OnMouseOver()
    {
        CursorManager.Instance.SetPointerCursor();
    }

    public void OnMouseExit()
    {
        CursorManager.Instance.ResetCursor();
    }

    /// <summary>
    /// When the treasure is collected, add it to the player's 
    /// inventory and make the player win.
    /// </summary>
    public void OnMouseDown()
    {
        StartCoroutine(CollectAndWinCoroutine());
    }

    private IEnumerator CollectAndWinCoroutine()
    {
        Debug.Log("Obtained " + _treasureData.TreasureName);
        yield return new WaitForSeconds(2);
        LevelManager.Instance.SetState(new WinState());
    }

}
