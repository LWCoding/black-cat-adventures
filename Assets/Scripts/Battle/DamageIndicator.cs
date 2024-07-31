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
            int damage = WordGenerator.Instance.CalculateDamage(WordPreview.Instance.CurrentTiles);
            // If cat paw, add one damage
            if (TreasureSection.Instance.HasTreasure(TreasureType.CAT_PAW))
            {
                damage += 1;
            }
            // If profane with right treasure, add one for each letter
            if (TreasureSection.Instance.HasTreasure(TreasureType.DUCT_TAPE) && WordGenerator.Instance.IsProfaneWord(word))
            {
                damage += word.Length;
            }
            // If seven letters with right treasure, add seven damage
            if (TreasureSection.Instance.HasTreasure(TreasureType.MAGIC_7_BALL) && word.Length == 7)
            {
                damage += 7;
            }
            _damageText.text = StringLayout.Replace("%d", damage.ToString());

        }
    }

}
