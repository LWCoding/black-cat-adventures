using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Generic EventBehaviour for UI option events that run in the EventLevel scene.
/// Reads all configuration (intro text, left sprite, option list, outcomes, result
/// text) from an OptionEventData asset. No subclass is needed per event; author a
/// new OptionEventData asset and register it in _EventDatabase.
/// </summary>
public class OptionEvent : EventBehaviour
{
    private bool _optionChosen   = false;
    private bool _treasurePicked = false;

    public override void BeginEvent()
    {
        OptionEventData data = (OptionEventData)Data;

        SetupLeftImage(data.LeftImage);
        SetDescriptionText(data.IntroText);
        StartCoroutine(ShowOptionsAfterDelay(0.4f, data.Options));
    }

    // ─── UI setup ────────────────────────────────────────────────────────────

    private static void SetupLeftImage(Sprite sprite)
    {
        Image left = EventManager.Instance.LeftImage;
        if (left == null) { return; }

        if (sprite == null)
        {
            left.gameObject.SetActive(false);
            return;
        }

        left.sprite = sprite;
        left.color  = Color.white;
        left.gameObject.SetActive(true);

        left.transform
            .DOLocalMoveY(left.transform.localPosition.y + 12f, 1.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private static void SetDescriptionText(string text)
    {
        TextMeshProUGUI desc = EventManager.Instance.DescriptionText;
        if (desc != null) { desc.text = text; }
    }

    private IEnumerator ShowOptionsAfterDelay(float delay, List<EventOption> options)
    {
        yield return new WaitForSeconds(delay);
        BuildOptionButtons(options);
    }

    private void BuildOptionButtons(List<EventOption> options)
    {
        Transform  container = EventManager.Instance.OptionsContainer;
        GameObject prefab    = EventManager.Instance.OptionButtonPrefab;
        if (container == null || prefab == null) { return; }

        ClearContainer(container);

        foreach (EventOption option in options)
        {
            EventOption captured = option;
            string label = string.IsNullOrEmpty(captured.Description)
                ? captured.Label
                : $"{captured.Label}\n<size=80%><color=#cccccc>{captured.Description}</color></size>";
            SpawnButton(container, prefab, label, () => OnOptionChosen(captured));
        }
    }

    private static void ClearContainer(Transform container)
    {
        foreach (Transform child in container) { Destroy(child.gameObject); }
    }

    private static void SpawnButton(Transform parent, GameObject prefab, string label,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject obj = Instantiate(prefab, parent);
        TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) { tmp.text = label; }
        Button btn = obj.GetComponent<Button>();
        if (btn != null) { btn.onClick.AddListener(onClick); }
    }

    // ─── Option execution ────────────────────────────────────────────────────

    private void OnOptionChosen(EventOption option)
    {
        if (_optionChosen) { return; }
        _optionChosen = true;

        Transform container = EventManager.Instance.OptionsContainer;
        if (container != null) { ClearContainer(container); }

        switch (option.Outcome.Type)
        {
            case OutcomeType.GrantRandomTreasures:
                RunGrantRandom(option);
                break;

            case OutcomeType.ChooseTreasure:
                RunChooseTreasure(option);
                break;

            default:
                ShowResultAndReturn(option.ResultText, 2f);
                break;
        }
    }

    private void RunGrantRandom(EventOption option)
    {
        List<Treasure> granted = EventOutcomes.GrantRandom(option.Outcome.Count);
        string text = granted.Count > 0 ? option.ResultText : option.EmptyPoolText;
        ShowResultAndReturn(text, 2.5f);
    }

    private void RunChooseTreasure(EventOption option)
    {
        List<Treasure> choices = GameManager.GameData.GetRandomUnownedTreasures(option.Outcome.ChooseFrom);
        if (choices.Count == 0)
        {
            ShowResultAndReturn(option.EmptyPoolText, 2f);
            return;
        }

        // Use the ResultText as a prompt for the selection phase.
        SetDescriptionText(option.ResultText);

        Transform  container = EventManager.Instance.OptionsContainer;
        GameObject prefab    = EventManager.Instance.OptionButtonPrefab;
        if (container == null || prefab == null) { return; }

        foreach (Treasure treasure in choices)
        {
            Treasure captured = treasure;
            string label = string.IsNullOrEmpty(captured.TreasureDescription)
                ? captured.TreasureName
                : $"{captured.TreasureName}\n<size=75%><color=#cfd2ff>{captured.TreasureDescription}</color></size>";
            SpawnButton(container, prefab, label, () => OnTreasurePicked(captured));
        }
    }

    private void OnTreasurePicked(Treasure chosen)
    {
        if (_treasurePicked) { return; }
        _treasurePicked = true;

        Transform container = EventManager.Instance.OptionsContainer;
        if (container != null) { ClearContainer(container); }

        GameManager.GameData.UnlockedTreasures.Add(chosen);
        SetDescriptionText($"You claim {chosen.TreasureName}.");
        EventManager.Instance.ReturnToMapAfterDelay(2f);
    }

    private static void ShowResultAndReturn(string text, float delay)
    {
        SetDescriptionText(text);
        EventManager.Instance.ReturnToMapAfterDelay(delay);
    }
}
