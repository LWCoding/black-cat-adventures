using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Scene-level manager for the EventLevel scene. Reads the event id from
/// GameData.RecentLevelCompleted, looks up the matching EventData in the
/// EventDatabase, attaches and starts the appropriate EventBehaviour, and
/// exposes UI element references that OptionEvent (and future UI events) use
/// to populate the screen at runtime.
/// </summary>
public class EventManager : Singleton<EventManager>
{
    [Header("UI References")]
    [Tooltip("Full-screen dark background image.")]
    [SerializeField] private Image _background;
    [Tooltip("Optional sprite shown on the left side of the event panel (e.g. fountain, NPC).")]
    [SerializeField] private Image _leftImage;
    [Tooltip("Main description / flavour text label.")]
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [Tooltip("Parent transform for the option buttons spawned at runtime.")]
    [SerializeField] private Transform _optionsContainer;
    [Tooltip("Prefab used to create each clickable option button.")]
    [SerializeField] private GameObject _optionButtonPrefab;

    public Image Background => _background;
    public Image LeftImage => _leftImage;
    public TextMeshProUGUI DescriptionText => _descriptionText;
    public Transform OptionsContainer => _optionsContainer;
    public GameObject OptionButtonPrefab => _optionButtonPrefab;

    protected override void Awake()
    {
        base.Awake();

        string eventId = GameManager.GameData.RecentLevelCompleted;
        EventData eventData = GameDatabase.Events != null ? GameDatabase.Events.GetEvent(eventId) : null;
        if (eventData == null)
        {
            Debug.LogError($"[EventManager] No EventData found for id '{eventId}'.");
            return;
        }

        GameObject eventObj = new($"Event_{eventData.EventId}");
        EventBehaviour behaviour = eventData.AttachBehaviour(eventObj);
        behaviour.BeginEvent();
    }

    /// <summary>
    /// Marks the node completed, records the event as seen, saves, and loads
    /// the Map. Delegates to EventFlow so the logic isn't duplicated across
    /// event archetypes.
    /// </summary>
    public void ReturnToMap() => EventFlow.CompleteAndReturnToMap();

    /// <summary>
    /// Returns to the map after a delay — used by OptionEvent and any future
    /// UI event that wants a beat before transitioning.
    /// </summary>
    public void ReturnToMapAfterDelay(float seconds) => StartCoroutine(ReturnAfterDelay(seconds));

    private IEnumerator ReturnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EventFlow.CompleteAndReturnToMap();
    }
}
