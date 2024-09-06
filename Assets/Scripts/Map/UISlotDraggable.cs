using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISlotDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [HideInInspector] public Action OnDragStart = null;
    [HideInInspector] public Action<Vector3> OnDragging = null;
    [HideInInspector] public Action<Vector3> OnDragEnd = null;

    [Header("Object Assignments")]
    [SerializeField] private CanvasGroup _canvasGroup;

    private Vector3 _originalPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _originalPosition = transform.position;
        OnDragStart?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        OnDragging?.Invoke(transform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        transform.position = _originalPosition;
        OnDragEnd?.Invoke(transform.position);
    }

}
