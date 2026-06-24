using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{

    public static Action OnCollect = null;

    [Header("Sprites")]
    [SerializeField] private Sprite _boxClosedSprite;
    [SerializeField] private Sprite _boxOpenSprite;
    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _boxRenderer;
    [SerializeField] private SpriteRenderer _iconRenderer;

    private List<Treasure> _choices;
    private bool _isInteractable = true;

    public void SetChoices(List<Treasure> choices) => _choices = choices;

    private void Start()
    {
        _iconRenderer.gameObject.SetActive(false);
        _boxRenderer.sprite = _boxClosedSprite;
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
    /// Opens the chest, swaps to the open sprite, and shows the treasure choice overlay.
    /// </summary>
    public void OnMouseDown()
    {
        if (!_isInteractable) { return; }
        OnCollect?.Invoke();
        OnMouseExit();  // Call BEFORE setting non-interactable so cursor resets
        _isInteractable = false;
        _boxRenderer.sprite = _boxOpenSprite;
        TreasureChoiceScreen.Instance.Show(_choices, OnTreasureChosen);
    }

    private void OnTreasureChosen(Treasure chosen)
    {
        StartCoroutine(FlyTreasureToPlayerCoroutine(chosen));
    }

    /// <summary>
    /// Animates the chosen treasure icon flying from the chest to the player, then
    /// grants the treasure and transitions to the win state.
    /// </summary>
    private IEnumerator FlyTreasureToPlayerCoroutine(Treasure chosen)
    {
        _iconRenderer.sprite = chosen.TreasureIcon;
        _iconRenderer.color = Color.white;
        _iconRenderer.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        Vector3 startPosition = _iconRenderer.transform.position;
        Vector3 targetPosition = BattleManager.Instance.PlayerHandler.transform.position;
        Color startColor = _iconRenderer.color;
        Color endColor = new(startColor.r, startColor.g, startColor.b, 0);
        float currTime = 0;
        float timeToWait = 0.7f;
        while (currTime < timeToWait)
        {
            currTime += Time.deltaTime;
            _iconRenderer.transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, currTime / timeToWait));
            _iconRenderer.color = Color.Lerp(startColor, endColor, currTime / timeToWait);
            yield return null;
        }
        GameManager.GameData.UnlockedTreasures.Add(chosen);
        yield return new WaitForSeconds(0.5f);
        BattleManager.Instance.SetState(new WinState());
    }

}
