using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIPointerCursorOnHover))]
public class BattleButton : MonoBehaviour, IPointerClickHandler
{

    [Header("Object Assignments")]
    [SerializeField] private TextMeshProUGUI _battleText;

    private bool _isInteractable = true;

    private UIPointerCursorOnHover _pointerCursorOnHover;

    private void Awake()
    {
        _pointerCursorOnHover = GetComponent<UIPointerCursorOnHover>();
        ToggleInteractability(false);
    }

    private void OnEnable()
    {
        if (LevelsManager.Instance != null)
        {
            LevelsManager.Instance.OnLevelChanged += OnLevelChanged;
            OnLevelChanged(LevelsManager.Instance.CurrentLevel);
        }
    }

    private void OnDisable()
    {
        if (LevelsManager.Instance != null)
        {
            LevelsManager.Instance.OnLevelChanged -= OnLevelChanged;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isInteractable) { return; }

        GameManager.GameData.RecentNodeEntered = LevelsManager.Instance.GetCurrentNodeId();

        ResolvedSpaceEntry resolved = LevelsManager.Instance.GetCurrentResolvedSpace();

        // Event nodes pick their concrete event lazily, here at entry time, so the
        // unseen-first ordering follows the player's actual path instead of the
        // order nodes resolved in at map generation.
        EventSpaceData eventSpace = GameDatabase.EventSpace;
        if (resolved != null && eventSpace != null && resolved.ResolvedTypeId == eventSpace.SpaceTypeId)
        {
            EventData chosen = eventSpace.PickEvent();
            if (chosen != null && !string.IsNullOrEmpty(chosen.SceneToLoad))
            {
                GameManager.GameData.RecentLevelCompleted = chosen.EventId;
                GameManager.GameData.RecentResolvedTypeId = resolved.ResolvedTypeId;
                SceneManager.LoadScene(chosen.SceneToLoad);
                return;
            }
        }

        if (resolved != null && !string.IsNullOrEmpty(resolved.SceneToLoad))
        {
            GameManager.GameData.RecentLevelCompleted  = resolved.PayloadId;
            GameManager.GameData.RecentResolvedTypeId  = resolved.ResolvedTypeId;
            SceneManager.LoadScene(resolved.SceneToLoad);
        }
        else
        {
            GameManager.GameData.RecentLevelCompleted = LevelsManager.Instance.GetCurrentLevelString();
            GameManager.GameData.RecentResolvedTypeId = "Battle";
            SceneManager.LoadScene("Level");
        }
    }

    private void OnLevelChanged(LevelHandler handler)
    {
        if (handler == null) { return; }

        string label = handler.AuthoredSpace != null && !string.IsNullOrEmpty(handler.AuthoredSpace.ActionLabel)
            ? handler.AuthoredSpace.ActionLabel
            : "Play";
        _battleText.text = label;
    }

    public void ToggleInteractability(bool isInteractable)
    {
        _isInteractable = isInteractable;
        _pointerCursorOnHover.IsEnabled = isInteractable;
        gameObject.SetActive(isInteractable);
    }

}
