using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PointerCursorOnHover))]
public class BattleButton : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private TextMeshPro _battleText;
    [SerializeField] private SpriteRenderer _bgRenderer;

    private bool _isInteractable = true;

    private PointerCursorOnHover _pointerCursorOnHover;

    private void Awake()
    {
        _pointerCursorOnHover = GetComponent<PointerCursorOnHover>();
        ToggleInteractability(false);
    }

    private void OnEnable()
    {
        if (LevelsManager.Instance != null)
        {
            LevelsManager.Instance.OnLevelChanged += ChangeBattleTextToLevel;
        }
    }

    private void OnDisable()
    {
        if (LevelsManager.Instance != null)
        {
            LevelsManager.Instance.OnLevelChanged -= ChangeBattleTextToLevel;
        }
    }

    private void OnMouseDown()
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

    private void ChangeBattleTextToLevel(int levelNumber)
    {
        _battleText.text = "Play Level " + levelNumber.ToString();
    }

    public void ToggleInteractability(bool isInteractable)
    {
        _isInteractable = isInteractable;
        Color bgColor = _bgRenderer.color;
        Color textColor = _battleText.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isInteractable ? 1 : 0.2f);
        _battleText.color = new Color(textColor.r, textColor.g, textColor.b, isInteractable ? 1 : 0.3f);
        _pointerCursorOnHover.IsEnabled = isInteractable;
    }

}
