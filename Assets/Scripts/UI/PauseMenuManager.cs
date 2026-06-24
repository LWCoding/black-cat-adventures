using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Persistent singleton that owns the Pause Menu overlay. The prefab
/// instantiates itself from Resources on the first scene load, so it never
/// needs to be placed manually in any scene.
///
/// In the Level scene: Resume, Return to Map, and Return to Menu are shown.
/// In the Map scene:   Resume and Return to Menu are shown.
/// In the Intro scene: Escape is ignored entirely.
/// </summary>
public class PauseMenuManager : PersistentSingleton<PauseMenuManager>
{

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _returnToMapButton;
    [SerializeField] private Button _returnToMenuButton;

    private const float FadeDuration = 0.2f;

    public bool IsPaused { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance != null) { return; }
        GameObject prefab = Resources.Load<GameObject>("PauseMenu");
        if (prefab != null)
        {
            Instantiate(prefab);
        }
    }

    protected override void OnPersistentAwake()
    {
        _resumeButton.onClick.AddListener(Resume);
        _returnToMapButton.onClick.AddListener(ReturnToMap);
        _returnToMenuButton.onClick.AddListener(ReturnToMenu);

        SceneManager.sceneLoaded += OnSceneLoaded;

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        UpdateButtonVisibility(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(Instance, this))
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) { return; }
        if (SceneManager.GetActiveScene().name == "Intro") { return; }
        Toggle();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceClose();
        UpdateButtonVisibility(scene.name);
    }

    private void UpdateButtonVisibility(string sceneName)
    {
        _returnToMapButton.gameObject.SetActive(sceneName == "Level");
    }

    public void Toggle()
    {
        if (IsPaused) { Resume(); } else { Open(); }
    }

    public void Open()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        AudioManager.Instance?.SetMuffled(true);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, FadeDuration).SetUpdate(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.SetMuffled(false);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.DOFade(0f, FadeDuration).SetUpdate(true);
    }

    private void ForceClose()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.SetMuffled(false);
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void ReturnToMap()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.SetMuffled(false);
        SceneManager.LoadScene("Map");
    }

    private void ReturnToMenu()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.SetMuffled(false);
        SaveManager.SaveGame(GameManager.GameData);
        SceneManager.LoadScene("Intro");
    }

}
