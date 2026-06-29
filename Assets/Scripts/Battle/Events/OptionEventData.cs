using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One option button shown to the player in a UI event. The label and description
/// form the button text; the outcome is executed when chosen; result text replaces
/// the description area after selection. Empty-pool text is shown when the outcome
/// has nothing to give (e.g. player already owns every treasure).
/// </summary>
[System.Serializable]
public struct EventOption
{
    [Tooltip("Short option title shown in the first line of the button.")]
    public string Label;
    [Tooltip("Sub-label shown below the title (smaller, tinted). Can be empty.")]
    public string Description;
    [Tooltip("What happens when the player picks this option.")]
    public EventOutcome Outcome;
    [Tooltip("Text shown in the description area after a successful outcome.")]
    public string ResultText;
    [Tooltip("Text shown when the outcome's reward pool is empty (e.g. all treasures owned).")]
    public string EmptyPoolText;
    [Tooltip("When true, this option is hidden entirely if the player owns no treasures (e.g. nothing to discard).")]
    public bool HideIfNoTreasures;
}

/// <summary>
/// EventData for a UI option event that runs in the EventLevel scene.
/// No subclass is needed per event — OptionEvent drives all events of this type
/// from this asset. Author a new OptionEventData asset and register it.
/// </summary>
[CreateAssetMenu(fileName = "New Option Event", menuName = "Events/Option Event")]
public class OptionEventData : EventData
{
    [Tooltip("Flavour text shown in the description area when the event opens.")]
    [TextArea(3, 6)]
    public string IntroText;

    [Tooltip("Sprite shown on the left panel. Leave null to hide the left image.")]
    public Sprite LeftImage;

    [Tooltip("Choices presented to the player. Each entry becomes one button.")]
    public List<EventOption> Options = new();

    public override EventBehaviour AttachBehaviour(GameObject host)
    {
        OptionEvent b = host.AddComponent<OptionEvent>();
        b.Data = this;
        return b;
    }
}
