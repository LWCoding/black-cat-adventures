using UnityEngine;

/// <summary>
/// Space type that loads the Level scene and rolls a random miniboss encounter
/// from the database using weighted selection.
/// </summary>
[CreateAssetMenu(fileName = "New Miniboss Space", menuName = "Spaces/Miniboss Space")]
public class MinibossSpaceData : SpaceData
{

    public EncounterDatabase Database;

    public override ResolvedSpace Resolve(System.Random rng)
    {
        Encounter chosen = Database?.RollMiniboss(rng, GameManager.GameData.LevelsCompleted.Count);
        return new ResolvedSpace
        {
            ResolvedTypeId = SpaceTypeId,
            SceneToLoad = SceneToLoad,
            PayloadId = chosen != null ? chosen.EncounterId : string.Empty,
        };
    }

}
