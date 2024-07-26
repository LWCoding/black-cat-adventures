using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{

    [Header("Indicator Properties")]
    [Tooltip("Replace the damage count with %d")] public string StringLayout;
    [Header("Object Assignments")]
    [SerializeField] private GameObject _objectToShow;
    [SerializeField] private TextMeshPro _damageText;

    private WordGenerator _wordGenerator = new();

    private void Start()
    {
        _objectToShow.SetActive(false);  // Indicator should start inactive
        WordPreview.Instance.OnLetterTilesChanged += WhenDamageChanged;
    }

    /// <summary>
    /// Updates the visibility of the object that shows damage.
    /// Also updates the text to reflect the number of damage dealt.
    /// </summary>
    public void WhenDamageChanged()
    {
        string word = WordPreview.Instance.CurrentWord;
        bool isWordReal = _wordGenerator.IsValidWord(word);
        _objectToShow.SetActive(isWordReal);
        if (isWordReal)
        {
            int damage = _wordGenerator.CalculateDamage(WordPreview.Instance.CurrentTiles);
            _damageText.text = StringLayout.Replace("%d", damage.ToString());
        }
    }

}
