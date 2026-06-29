using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A single treasure option in the TreasureChoiceScreen overlay.
/// Displays the treasure icon, shows a tooltip on hover, and reports selection via callback.
/// </summary>
public class TreasureChoiceSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [Header("Object Assignments")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _tooltipObject;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private TextMeshProUGUI _rarityText;

    public Treasure Treasure { get; private set; }
    private Action<Treasure> _onSelected;
    private bool _isInteractable = false;

    public void Initialize(Treasure treasure, Action<Treasure> onSelected)
    {
        Treasure = treasure;
        _onSelected = onSelected;
        _iconImage.sprite = treasure.TreasureIcon;
        _tooltipText.text = "<b><color=#" + TreasureRarityInfo.GetHexColor(treasure.Rarity) + ">" + treasure.TreasureName + "</color></b>:\n" + treasure.TreasureDescription;
        if (_rarityText != null)
        {
            _rarityText.text = TreasureRarityInfo.GetLabel(treasure.Rarity);
            _rarityText.color = TreasureRarityInfo.GetColor(treasure.Rarity);
        }
        _tooltipObject.SetActive(false);
        _canvasGroup.alpha = 1f;
        _isInteractable = true;
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        if (!interactable) { _tooltipObject.SetActive(false); }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInteractable) { return; }
        _tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isInteractable) { return; }
        _onSelected?.Invoke(Treasure);
    }

    public IEnumerator FadeToAlpha(float target, float duration)
    {
        float start = _canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        _canvasGroup.alpha = target;
    }

}
