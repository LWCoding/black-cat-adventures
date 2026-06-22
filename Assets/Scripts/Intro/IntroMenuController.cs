using UnityEngine;
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
    }

    private void OnStartClicked()
    {
        if (_hasSave)
        {
            _confirmOverwritePanel.SetActive(true);
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
        BeginNewRun();
    }

    private void OnConfirmNoClicked()
    {
        _confirmOverwritePanel.SetActive(false);
    }

    private void BeginNewRun()
    {
        _menuPanel.SetActive(false);
        _bypassIntro.IsActive = true;
        _introCutscene.BeginCutscene();
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
