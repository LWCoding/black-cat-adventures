using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureItem : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TextMeshPro _tooltipText;
    [Header("Treasure Properties")]
    public TreasureData TreasureData;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Store an index into this treasure item, so that it can properly
    /// select itself. Also registers all of the treasure stats.
    /// </summary>
    public void Initialize()
    {
        _iconRenderer.sprite = TreasureData.TreasureIcon;
        _tooltipText.text = "<b>" + TreasureData.TreasureName + "</b>:\n" + TreasureData.TreasureDescription;
        switch (TreasureData.Type)
        {
            case TreasureType.CAT_PAW:
                LevelManager.Instance.OnPlayerAttack += () =>
                {
                    LevelManager.Instance.RenderAttackAgainstEnemy(1);
                };
                break;
            case TreasureType.DUCT_TAPE:
                WordPreview.Instance.OnLetterTilesChanged += () =>
                {
                    if (WordGenerator.Instance.IsProfaneWord(WordPreview.Instance.CurrentWord))
                    {
                        StartCoroutine(RunNextFrameCoroutine(() =>
                        {
                            WordPreview.Instance.FeedbackText.enabled = true;
                            if (WordPreview.Instance.FeedbackText.text != "")
                            {
                                WordPreview.Instance.FeedbackText.text += " ";
                            }
                            WordPreview.Instance.FeedbackText.text += "Profane!";
                        }));
                    }
                };
                LevelManager.Instance.OnPlayerAttack += () =>
                {
                    if (WordGenerator.Instance.IsProfaneWord(WordPreview.Instance.CurrentWord))
                    {
                        LevelManager.Instance.RenderAttackAgainstEnemy(WordPreview.Instance.CurrentWord.Length);
                    }
                };
                break;
            case TreasureType.MAGIC_7_BALL:
                WordPreview.Instance.OnLetterTilesChanged += () =>
                {
                    if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) && WordPreview.Instance.CurrentTiles.Count == 7)
                    {
                        StartCoroutine(RunNextFrameCoroutine(() =>
                        {
                            WordPreview.Instance.FeedbackText.enabled = true;
                            if (WordPreview.Instance.FeedbackText.text != "")
                            {
                                WordPreview.Instance.FeedbackText.text += " ";
                            }
                            WordPreview.Instance.FeedbackText.text += "Seven!";
                        }));
                    }
                };
                LevelManager.Instance.OnPlayerAttack += () =>
                {
                    if (WordPreview.Instance.CurrentTiles.Count == 7)
                    {
                        LevelManager.Instance.RenderAttackAgainstEnemy(7);
                    }
                };
                break;
        }
        OnMouseExit();
    }

    public void OnMouseEnter()
    {
        EnableTreasure();
    }

    public void OnMouseExit()
    {
        DisableTreasure();
    }

    /// <summary>
    /// Make this treasure animate to its selected phase.
    /// </summary>
    public void EnableTreasure()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.Play("Selected");
    }

    /// <summary>
    /// Make this treasure animate to its unselected phase.
    /// </summary>
    public void DisableTreasure()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.Play("Unselected");
    }

    /// <summary>
    /// Run specific code after the next frame, to avoid
    /// conflicts that occur due to priority.
    /// </summary>
    private IEnumerator RunNextFrameCoroutine(Action codeToRunAfter)
    {
        yield return new WaitForEndOfFrame();
        codeToRunAfter.Invoke();
    }

}
