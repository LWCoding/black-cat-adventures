using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private Animator _winScreenAnimator;

    private void Start()
    {
        LevelManager.Instance.OnStateChanged += (state) =>
        {
            if (state is WinState)
            {
                StartCoroutine(ShowWinScreenCoroutine());
            }
        };
    }

    /// <summary>
    /// Animate the win screen in.
    /// 
    /// After a set amount of time, load the level select screen.
    /// </summary>
    private IEnumerator ShowWinScreenCoroutine()
    {
        _winScreenAnimator.Play("Show");
        yield return new WaitForSeconds(4.5f);
        SceneManager.LoadScene("Map");
    }

}
