using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureCollectible : MonoBehaviour
{

    public static Action OnCollect = null;

    [Header("Treasure Properties")]
    [SerializeField] private Treasure _treasureData;
    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TextMeshPro _tooltipText;
    [SerializeField] private Animator _treasureAnimator;

    private bool _isInteractable = true;

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
        if (!_isInteractable) { return; }
        CursorManager.Instance.SetPointerCursor();
    }

    public void OnMouseExit()
    {
        if (!_isInteractable) { return; }
        CursorManager.Instance.ResetCursor();
    }

    /// <summary>
    /// When the treasure is collected, add it to the player's 
    /// inventory and make the player win.
    /// 
    /// Ensure the player cannot interact with this item anymore.
    /// </summary>
    public void OnMouseDown()
    {
        if (!_isInteractable) { return; }
        OnCollect?.Invoke();
        OnMouseExit();  // Make sure to call this BEFORE setting interactable to false
        StartCoroutine(CollectAndWinCoroutine());
        _isInteractable = false;
    }

    private IEnumerator CollectAndWinCoroutine()
    {
        _treasureAnimator.Play("Collected");
        yield return new WaitForSeconds(0.4f);
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = BattleManager.Instance.PlayerHandler.transform.position;
        Color startColor = _iconRenderer.color;
        Color endColor = new(startColor.r, startColor.g, startColor.b, 0);
        float currTime = 0;
        float timeToWait = 0.7f;
        while (currTime < timeToWait)
        {
            currTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, currTime / timeToWait));
            _iconRenderer.color = Color.Lerp(startColor, endColor, currTime / timeToWait);
            yield return null;
        }
        // Make the player obtain the treasure
        GameManager.EquippedTreasures.Add(_treasureData);
        // Go to win state afterwards
        yield return new WaitForSeconds(0.5f);
        BattleManager.Instance.SetState(new WinState());
    }

}
