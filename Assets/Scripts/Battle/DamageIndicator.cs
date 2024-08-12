using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{

    [Header("Indicator Properties")]
    [Tooltip("Replace the damage count with %d")] public string StringLayout;
    [Header("Object Assignments")]
    [SerializeField] private GameObject _objectToShow;
    [SerializeField] private TextMeshPro _damageText;

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
        bool isWordReal = WordGenerator.Instance.IsValidWord(word);
        _objectToShow.SetActive(isWordReal);
        if (isWordReal)
        {
            int damage = DamageCalculator.CalculateDamage(WordPreview.Instance.CurrentTiles);
            int damageFromMods = DamageCalculator.CalculateDamageFromModifiers(WordPreview.Instance.CurrentTiles);
            if (damageFromMods == 0)
            {
                _damageText.text = StringLayout.Replace("%d", damage.ToString());
            } 
            else
            {
                _damageText.text = StringLayout.Replace("%d", damage.ToString() + " <color='orange'>(" + (damageFromMods > 0 ? "+" : "-") + damageFromMods.ToString() + ")</color>");
            }
        }
    }

}
