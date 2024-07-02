using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubmitButton : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected TextMeshPro _letterText;
    [SerializeField] protected SpriteRenderer _bgRenderer;

    private WordChecker _wordChecker = new();

    private void Start()
    {
        // If the currently spelled word is valid, button is interactable, else no
        WordPreview.Instance.OnLetterTilesChanged += () =>
        {
            if (_wordChecker.IsValidWord(WordPreview.Instance.CurrentWord))
            {
                ToggleVisibility(true);
            }
            else
            {
                ToggleVisibility(false);
            }
        };
    }

    /// <summary>
    /// Toggle whether this tile looks like it's been clicked or not.
    /// 
    /// isVisible = true -> Button looks like it is interactable
    /// isVisible = false -> Button looks like it isn't interactable
    /// </summary>
    private void ToggleVisibility(bool isVisible)
    {
        Color bgColor = _bgRenderer.color;
        Color letterColor = _letterText.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isVisible ? 1 : 0.3f);
        _letterText.color = new Color(letterColor.r, letterColor.g, letterColor.b, isVisible ? 1 : 0.3f);
    }

}
