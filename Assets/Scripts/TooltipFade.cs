using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipFade : MonoBehaviour
{

    [Header("Objects to Fade on Hover")]
    [SerializeField] private List<SpriteRenderer> _renderers;
    [SerializeField] private TextMeshPro _text;

    private void OnMouseEnter()
    {
        foreach (SpriteRenderer rend in _renderers)
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.25f);
        }
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0.25f);
    }

    private void OnMouseExit()
    {
        foreach (SpriteRenderer rend in _renderers)
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1);
        }
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1);
    }

}
