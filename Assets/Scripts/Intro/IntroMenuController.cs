using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroMenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _quitButton;
    [Header("Overwrite Confirmation")]
    [SerializeField] private GameObject _confirmOverwritePanel;
    [SerializeField] private Button _confirmYesButton;
    [SerializeField] private Button _confirmNoButton;
    [Header("Scene References")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private IntroCutscene _introCutscene;
    [SerializeField] private BypassIntro _bypassIntro;

    private bool _hasSave;

    private void Awake()
    {
        _hasSave = SaveManager.LoadGame() != null;

        _startButton.onClick.AddListener(OnStartClicked);
        _continueButton.onClick.AddListener(OnContinueClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
        _confirmYesButton.onClick.AddListener(OnConfirmYesClicked);
        _confirmNoButton.onClick.AddListener(OnConfirmNoClicked);

        _confirmOverwritePanel.SetActive(false);
        _continueButton.interactable = _hasSave;

        EnsureAnimator(_startButton);
        EnsureAnimator(_continueButton);
        EnsureAnimator(_quitButton);
        EnsureAnimator(_confirmYesButton);
        EnsureAnimator(_confirmNoButton);
    }

    private static void EnsureAnimator(Button button)
    {
        if (button.GetComponent<MenuButtonAnimator>() == null)
        {
            button.gameObject.AddComponent<MenuButtonAnimator>();
        }
    }

    private void Update()
    {
        bool down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool up   = Input.GetKeyDown(KeyCode.UpArrow)   || Input.GetKeyDown(KeyCode.W);

        if (!down && !up) { return; }

        Button[] active = GetActiveButtons();
        if (active.Length == 0) { return; }

        GameObject current = EventSystem.current.currentSelectedGameObject;
        int index = -1;
        for (int i = 0; i < active.Length; i++)
        {
            if (active[i].gameObject == current)
            {
                index = i;
                break;
            }
        }

        if (index < 0)
        {
            EventSystem.current.SetSelectedGameObject(active[0].gameObject);
            return;
        }

        int next = Mathf.Clamp(index + (down ? 1 : -1), 0, active.Length - 1);
        if (next != index)
        {
            EventSystem.current.SetSelectedGameObject(active[next].gameObject);
        }
    }

    private Button[] GetActiveButtons()
    {
        if (_confirmOverwritePanel.activeSelf)
        {
            return new[] { _confirmYesButton, _confirmNoButton };
        }

        if (_continueButton.interactable)
        {
            return new[] { _startButton, _continueButton, _quitButton };
        }

        return new[] { _startButton, _quitButton };
    }

    private void OnStartClicked()
    {
        if (_hasSave)
        {
            _confirmOverwritePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_confirmYesButton.gameObject);
        }
        else
        {
            BeginNewRun();
        }
    }

    private void OnConfirmYesClicked()
    {
        SaveManager.EraseSave();
        GameManager.GameData = new GameData();
        _hasSave = false;
        _confirmOverwritePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        BeginNewRun();
    }

    private void OnConfirmNoClicked()
    {
        _confirmOverwritePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void BeginNewRun()
    {
        _bypassIntro.IsActive = true;
        _introCutscene.BeginCutscene(() => _menuPanel.SetActive(false));
    }

    private void OnContinueClicked()
    {
        GameManager.GameData = SaveManager.LoadGame();
        SceneManager.LoadScene("Map");
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }
}
