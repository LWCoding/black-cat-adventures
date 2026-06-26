using UnityEngine;

/// <summary>
/// Space type that loads the Level scene and rolls a random boss encounter
/// from the database using weighted selection. Placed on the final map node
/// so every path through the map ends in a boss fight.
/// </summary>
[CreateAssetMenu(fileName = "New Boss Space", menuName = "Spaces/Boss Space")]
public class BossSpaceData : SpaceData
{

    public EncounterDatabase Database;

    public override ResolvedSpace Resolve(System.Random rng)
    {
        Encounter chosen = Database?.RollBoss(rng, GameManager.GameData.LevelsCompleted.Count);
        return new ResolvedSpace
        {
            ResolvedTypeId = SpaceTypeId,
            SceneToLoad = SceneToLoad,
            PayloadId = chosen != null ? chosen.EncounterId : string.Empty,
        };
    }

}
