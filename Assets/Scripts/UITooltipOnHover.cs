using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Object Assignments")]
    [SerializeField] private GameObject _tooltipObject;

    [HideInInspector] public bool IsInteractable = true;

    private void Awake()
    {
        _tooltipObject.SetActive(false);  // Hide tooltip initially
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) { return; }
        _tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipObject.SetActive(false);
    }

}
