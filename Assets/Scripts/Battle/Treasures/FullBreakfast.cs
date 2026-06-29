using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Full Breakfast", menuName = "Treasures/Full Breakfast")]
public class FullBreakfast : Treasure
{

    private static readonly string[] Vowels = { "A", "E", "I", "O", "U" };

    public override void ActivateTreasure()
    {
        BattleManager.Instance.RunNextFrame(() => WordGrid.Instance.EnsureLettersPresent(Vowels));
        BattleManager.Instance.OnStateChanged += (state) =>
        {
            if (state is PlayerTurnState) { WordGrid.Instance.EnsureLettersPresent(Vowels); }
        };
    }

}
