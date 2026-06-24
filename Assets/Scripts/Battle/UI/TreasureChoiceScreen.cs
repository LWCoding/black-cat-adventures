using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Full-screen overlay that presents up to three treasure choices after opening a chest.
/// Fades in a dark overlay, lets the player pick one, dims the others, then fades out and
/// invokes the selection callback so TreasureChest can animate the chosen item.
/// </summary>
public class TreasureChoiceScreen : Singleton<TreasureChoiceScreen>
{

    [Header("Object Assignments")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TreasureChoiceSlot[] _slots;

    private Action<Treasure> _onChosen;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    /// <summary>
    /// Fades the overlay in and populates the slots with the provided treasure choices.
    /// <paramref name="onChosen"/> is invoked after the overlay fades out with the
    /// treasure the player selected.
    /// </summary>
    public void Show(List<Treasure> choices, Action<Treasure> onChosen)
    {
        _onChosen = onChosen;
        StartCoroutine(ShowCoroutine(choices));
    }

    private IEnumerator ShowCoroutine(List<Treasure> choices)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            bool hasChoice = i < choices.Count;
            _slots[i].gameObject.SetActive(hasChoice);
            if (hasChoice)
            {
                _slots[i].Initialize(choices[i], OnSlotClicked);
            }
        }
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        float elapsed = 0f;
        float duration = 0.3f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;
    }

    private void OnSlotClicked(Treasure chosen)
    {
        StartCoroutine(DismissCoroutine(chosen));
    }

    private IEnumerator DismissCoroutine(Treasure chosen)
    {
        foreach (TreasureChoiceSlot slot in _slots)
        {
            slot.SetInteractable(false);
        }
        // Dim the unchosen slots
        foreach (TreasureChoiceSlot slot in _slots)
        {
            if (slot.gameObject.activeSelf && slot.Treasure != chosen)
            {
                StartCoroutine(slot.FadeToAlpha(0.3f, 0.3f));
            }
        }
        yield return new WaitForSeconds(0.4f);
        // Fade out the entire overlay
        float elapsed = 0f;
        float duration = 0.3f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _onChosen?.Invoke(chosen);
    }

}
