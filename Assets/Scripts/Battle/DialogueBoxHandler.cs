using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Faction
{
    PLAYER = 0, ENEMY = 1
}

[System.Serializable]
public struct DialogueInfo
{
    public string Text;
    public float Duration;
    public Faction Speaker;
    public bool ShouldStallState;  // True = state will be switched to WaitState, False = return
    public bool ShouldWinStateAfter;  // True = state will be switched to WinState, False = continue as normal
}

public class DialogueBoxHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] protected Animator _dBoxAnimator;
    [SerializeField] protected TextMeshPro _dBoxText;

    private Queue<DialogueInfo> _queuedDialogue = new();

    public static Action OnDialogueComplete = null;

    private void Awake()
    {
        ToggleDBoxVisibility(false);  // At the beginning of the game, don't show
    }

    /// <summary>
    /// Make the speech bubble appear with provided text for
    /// a certain duration.
    /// </summary>
    public void SayDialogue(DialogueInfo di)
    {
        _queuedDialogue.Enqueue(di);
        StartCoroutine(SayDialogueCoroutine());
    }

    private IEnumerator SayDialogueCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        DialogueInfo dialogue = _queuedDialogue.Dequeue();

        ToggleDBoxVisibility(true);
        _dBoxText.text = dialogue.Text;
        _dBoxAnimator.Play("Show");
        yield return new WaitForSeconds(dialogue.Duration);
        _dBoxAnimator.Play("Hide");

        // Wait until the hide animation is done
        AnimatorStateInfo animationState = _dBoxAnimator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = animationState.length;
        yield return new WaitForSeconds(animationDuration);

        OnDialogueComplete?.Invoke();
        ToggleDBoxVisibility(false);
    }

    /// <summary>
    /// Toggle whether the dialogue box is visible or not
    /// by enabling/disabling child sprites.
    /// </summary>
    private void ToggleDBoxVisibility(bool isVisible)
    {
        _dBoxText.gameObject.SetActive(isVisible);
        _dBoxAnimator.gameObject.SetActive(isVisible);
    }

}
