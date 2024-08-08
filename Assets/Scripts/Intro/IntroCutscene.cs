using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private Animator _cutsceneAnimator;
    [Header("Cutscene Properties")]
    [SerializeField] private string _sceneNameAfterCutscene;

    private void Start()
    {
        StartCoroutine(PlayCutsceneCoroutine());
    }

    /// <summary>
    /// Starts the cutscene denoted by the `Play` animation.
    /// Afterwards, switches to an `Idle` animation and waits
    /// for a click. The click will bring the player to the
    /// next scene.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCutsceneCoroutine()
    {
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

        SceneManager.LoadScene(_sceneNameAfterCutscene);
    }

}
