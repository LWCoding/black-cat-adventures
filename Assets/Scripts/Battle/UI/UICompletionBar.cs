using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICompletionBar : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private Transform _playerIconTransform;
    [SerializeField] private RectTransform _fillBarTransform;
    [SerializeField] private Transform _barStart;
    [SerializeField] private Transform _barEnd;
    [Header("Image Assignments")]
    [SerializeField] private Sprite _notchSprite;

    private Vector3 _playerIconOffset;
    private float _progressBarLength;

    private int _eventsEncountered = 0;
    private int _totalNumEvents = 0;

    private readonly Dictionary<int, Image> _notchImages = new();  // Key = event encountered #

    private void Awake()
    {
        _progressBarLength = _fillBarTransform.rect.width;
        _playerIconOffset = _playerIconTransform.localPosition;

        EnemyHandler[] enemies = FindObjectsOfType<EnemyHandler>(true);
        Initialize(enemies.Length);
    }

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnNewEnemySet += OnNewEnemySet;
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnNewEnemySet -= OnNewEnemySet;
        }
    }

    /// <summary>
    /// Given the number of events that will occur throughout
    /// this level, initializes the progress bar to have that
    /// many notches.
    /// </summary>
    public void Initialize(int eventCount)
    {
        _totalNumEvents = eventCount;
        float section = (_barEnd.position.x - _barStart.position.x) / eventCount;
        // Create a notch for every possible ratio
        for (int i = 1; i <= eventCount; i++)
        {
            Image notchImage = new GameObject("Ratio").AddComponent<Image>();
            notchImage.transform.localPosition = new Vector3(_barStart.position.x + section * i, _fillBarTransform.position.y);
            notchImage.GetComponent<RectTransform>().sizeDelta = new(80, 80);
#if UNITY_WEBGL && !UNITY_EDITOR
            notchImage.GetComponent<RectTransform>().sizeDelta = new(60, 60);
#endif
            notchImage.transform.SetParent(transform, true);
            notchImage.sprite = _notchSprite;
            _notchImages.Add(i, notchImage);
        }
    }

    private void OnNewEnemySet(EnemyHandler enemy)
    {
        // Fade out already-visited events
        if (_notchImages.ContainsKey(_eventsEncountered))
        {
            _notchImages[_eventsEncountered].color = new Color(1, 1, 1, 0.5f);
        }
        _eventsEncountered++;
        GoToProgress((float)_eventsEncountered / _totalNumEvents);
    }

    /// <summary>
    /// Given a ratio, animates the progress bar to go to
    /// that ratio.
    /// </summary>
    public void GoToProgress(float ratio)
    {
        StartCoroutine(LoadProgressCoroutine(ratio));
    }

    private IEnumerator LoadProgressCoroutine(float ratio)
    {
        Vector3 currIconPos = _playerIconTransform.localPosition;
        Vector3 currBarFill = _fillBarTransform.localScale;
        Vector3 targetIconPos = new Vector3(_progressBarLength * ratio, _playerIconTransform.localPosition.y) + _playerIconOffset;
        Vector3 targetBarFill = new Vector3(ratio, _fillBarTransform.localScale.y);
        float currTime = 0;
        float timeToWait = 1;
        while (currTime < timeToWait)
        {
            currTime += Time.deltaTime;
            _playerIconTransform.localPosition = Vector3.Lerp(currIconPos, targetIconPos, currTime / timeToWait);
            _fillBarTransform.localScale = Vector3.Lerp(currBarFill, targetBarFill, currTime / timeToWait);
            yield return null;
        }
    }

}
