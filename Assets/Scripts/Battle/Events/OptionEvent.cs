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
    private bool _proceeding     = false;

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
            if (option.HideIfNoTreasures && !GameManager.GameData.HasAnyTreasure) { continue; }

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

            case OutcomeType.SwapRandomTreasure:
                RunSwapRandom(option);
                break;

            case OutcomeType.GrantTreasureScrambleNextBattle:
                RunGrantScramble(option);
                break;

            default:
                ShowResultAndProceed(option.ResultText, option);
                break;
        }
    }

    private void RunGrantRandom(EventOption option)
    {
        List<Treasure> granted = EventOutcomes.GrantRandom(option.Outcome.Count);
        if (granted.Count == 0)
        {
            ShowResultAndProceed(option.EmptyPoolText, option);
            return;
        }
        string text = $"{option.ResultText}\n\nYou received {FormatTreasureList(granted)}.";
        ShowResultAndProceed(text, option);
    }

    /// <summary>
    /// Joins treasure names into a readable, rarity-coloured list:
    /// "A", "A and B", or "A, B, and C".
    /// </summary>
    private static string FormatTreasureList(List<Treasure> treasures)
    {
        List<string> names = new();
        foreach (Treasure t in treasures)
        {
            names.Add($"<color=#{TreasureRarityInfo.GetHexColor(t.Rarity)}>{t.TreasureName}</color>");
        }
        if (names.Count == 1) { return names[0]; }
        if (names.Count == 2) { return $"{names[0]} and {names[1]}"; }
        return string.Join(", ", names.GetRange(0, names.Count - 1)) + ", and " + names[^1];
    }

    private void RunSwapRandom(EventOption option)
    {
        (Treasure discarded, Treasure gained) = EventOutcomes.SwapRandomTreasure();
        string text;
        if (gained != null && discarded != null)
        {
            text = string.Format(option.ResultText, discarded.TreasureName, gained.TreasureName);
        }
        else if (gained != null)
        {
            text = $"You gained {gained.TreasureName}.";
        }
        else
        {
            text = option.EmptyPoolText;
        }
        ShowResultAndProceed(text, option);
    }

    private void RunGrantScramble(EventOption option)
    {
        Treasure gained = EventOutcomes.GrantTreasureScrambleNextBattle();
        string text = gained != null
            ? string.Format(option.ResultText, gained.TreasureName)
            : option.EmptyPoolText;
        ShowResultAndProceed(text, option);
    }

    private void RunChooseTreasure(EventOption option)
    {
        List<Treasure> choices = GameManager.GameData.GetRandomUnownedTreasures(option.Outcome.ChooseFrom);
        if (choices.Count == 0)
        {
            ShowResultAndProceed(option.EmptyPoolText, option);
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
            EventOption capturedOption = option;
            string label = string.IsNullOrEmpty(captured.TreasureDescription)
                ? captured.TreasureName
                : $"{captured.TreasureName}\n<size=75%><color=#cfd2ff>{captured.TreasureDescription}</color></size>";
            SpawnButton(container, prefab, label, () => OnTreasurePicked(captured, capturedOption));
        }
    }

    private void OnTreasurePicked(Treasure chosen, EventOption option)
    {
        if (_treasurePicked) { return; }
        _treasurePicked = true;

        Transform container = EventManager.Instance.OptionsContainer;
        if (container != null) { ClearContainer(container); }

        GameManager.GameData.UnlockedTreasures.Add(chosen);
        ShowResultAndProceed($"You claim {chosen.TreasureName}.", option);
    }

    // ─── Result + manual proceed ─────────────────────────────────────────────

    /// <summary>
    /// Shows the result text, then (after a short beat) spawns a single button the
    /// player must click to return to the map. The button label comes from the
    /// chosen option's ProceedLabel, defaulting to "Proceed".
    /// </summary>
    private void ShowResultAndProceed(string text, EventOption option)
    {
        SetDescriptionText(text);
        string proceedLabel = string.IsNullOrEmpty(option.ProceedLabel) ? "Proceed" : option.ProceedLabel;
        StartCoroutine(ShowProceedAfterDelay(0.4f, proceedLabel));
    }

    private IEnumerator ShowProceedAfterDelay(float delay, string proceedLabel)
    {
        yield return new WaitForSeconds(delay);

        Transform  container = EventManager.Instance.OptionsContainer;
        GameObject prefab    = EventManager.Instance.OptionButtonPrefab;
        if (container == null || prefab == null) { yield break; }

        ClearContainer(container);
        SpawnButton(container, prefab, proceedLabel, OnProceedClicked);
    }

    private void OnProceedClicked()
    {
        if (_proceeding) { return; }
        _proceeding = true;
        EventManager.Instance.ReturnToMap();
    }
}
