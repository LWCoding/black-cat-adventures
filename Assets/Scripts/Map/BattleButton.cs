using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PointerCursorOnHover))]
public class BattleButton : MonoBehaviour
{

    private static BattleButton _instance;
    public static BattleButton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<BattleButton>();
            }
            return _instance;
        }
    }

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
        SceneManager.LoadScene(LevelsManager.Instance.GetCurrentLevelString());
    }

    /// <summary>
    /// Change the battle button text to represent a specific level.
    /// </summary>
    /// <param name="levelNumber"></param>
    private void ChangeBattleTextToLevel(int levelNumber)
    {
        _battleText.text = "Play Level " + levelNumber.ToString();
    }

    /// <summary>
    /// Toggle whether this tile is clickable or not.
    /// 
    /// isInteractable = true -> Button looks like it is interactable
    /// isInteractable = false -> Button looks like it isn't interactable
    /// </summary>
    public void ToggleInteractability(bool isInteractable)
    {
        _isInteractable = isInteractable;
        Color bgColor = _bgRenderer.color;
        Color textColor = _battleText.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isInteractable ? 1 : 0.2f);
        _battleText.color = new Color(textColor.r, textColor.g, textColor.b, isInteractable ? 1 : 0.3f);
        // Toggle cursor activation based on interactability
        _pointerCursorOnHover.IsEnabled = isInteractable;
    }

}
