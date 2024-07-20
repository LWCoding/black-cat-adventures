using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipOnHover : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private GameObject _objectToShowOnHover;
    [SerializeField] private float _timeToHoverBeforeShowing;

    private Coroutine _waitBeforeShowCoroutine;

    private void Awake()
    {
        _objectToShowOnHover.SetActive(false);
    }

    /// <summary>
    /// Ensure the mouse is hovering over this sprite for
    /// a specified time *before* showing the tooltip.
    /// </summary>
    private IEnumerator WaitBeforeShowCoroutine()
    {
        yield return new WaitForSeconds(_timeToHoverBeforeShowing);
        _objectToShowOnHover.SetActive(true);
    }

    private void OnMouseEnter()
    {
        _waitBeforeShowCoroutine = StartCoroutine(WaitBeforeShowCoroutine());
    }

    private void OnMouseExit()
    {
        StopCoroutine(_waitBeforeShowCoroutine);
        _objectToShowOnHover.SetActive(false);
    }

}