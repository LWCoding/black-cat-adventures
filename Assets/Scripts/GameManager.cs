using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{

    public static GameData GameData = new();

    /// <summary>
    /// Drops the player straight into their very first tutorial battle, bypassing the
    /// map. Mirrors the GameData that <see cref="BattleButton"/> would set for the start
    /// node so the win / return-to-map flow behaves exactly as if the tutorial had been
    /// entered from the map: winning marks the start node complete, and abandoning to the
    /// map (via the pause menu) leaves it as the incomplete frontier so the player can
    /// replay it. Used only by the new-game intro paths; the map owns every later battle.
    /// </summary>
    public static void EnterFirstTutorialBattle()
    {
        // "Node_0_0" is the map's start node (col 0, row 0), which LevelsManager always
        // authors as the tutorial battle space. Recording it keeps the map's frontier and
        // completion gating consistent whether the player wins or returns to the map.
        GameData.RecentNodeEntered = "Node_0_0";
        GameData.RecentResolvedTypeId = "Battle";

        Encounter tutorial = GameDatabase.Encounters != null ? GameDatabase.Encounters.TutorialEncounter : null;
        GameData.RecentLevelCompleted = tutorial != null ? tutorial.EncounterId : "";

        SceneManager.LoadScene("Level");
    }

}
