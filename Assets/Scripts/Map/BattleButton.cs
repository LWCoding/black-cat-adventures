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
        if (resolved != null && !string.IsNullOrEmpty(resolved.SceneToLoad))
        {
            GameManager.GameData.RecentLevelCompleted = resolved.PayloadId;
            SceneManager.LoadScene(resolved.SceneToLoad);
        }
        else
        {
            GameManager.GameData.RecentLevelCompleted = LevelsManager.Instance.GetCurrentLevelString();
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
