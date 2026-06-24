using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
{
    [Header("Object Assignments")]
    [SerializeField] private Animator _cutsceneAnimator;
    [Header("Cutscene Properties")]
    [SerializeField] private string _levelIdAfterCutscene;
    [Header("Fade")]
    [SerializeField] private Image _fadeOverlay;
    [SerializeField] private float _fadeDuration = 0.4f;

    /// <summary>
    /// Fades to black, invokes <paramref name="onFadedToBlack"/> once the screen is
    /// fully covered, then plays the cutscene. The overlay stays opaque for the
    /// remainder of the cutscene; the scene load removes it naturally.
    /// </summary>
    public void BeginCutscene(System.Action onFadedToBlack = null)
    {
        StartCoroutine(PlayCutsceneCoroutine(onFadedToBlack));
    }

    private IEnumerator PlayCutsceneCoroutine(System.Action onFadedToBlack)
    {
        if (_fadeOverlay != null)
        {
            _fadeOverlay.gameObject.SetActive(true);
            Color c = _fadeOverlay.color;
            _fadeOverlay.color = new Color(c.r, c.g, c.b, 0f);
            _fadeOverlay.DOFade(1f, _fadeDuration).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(_fadeDuration);
        }

        onFadedToBlack?.Invoke();

        _cutsceneAnimator.enabled = true;
        _cutsceneAnimator.Play("Play");

        // Wait until play animation is done
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => _cutsceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        // Start idle animation
        _cutsceneAnimator.Play("Idle");

        // Wait until the player clicks down their mouse
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));  // Wait for button press

        // Start hide animation
        _cutsceneAnimator.Play("Hide");

        // Wait until hide animation is done, then load scene
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => _cutsceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        GameManager.GameData.RecentLevelCompleted = _levelIdAfterCutscene;
        SceneManager.LoadScene("Level");
    }
}
