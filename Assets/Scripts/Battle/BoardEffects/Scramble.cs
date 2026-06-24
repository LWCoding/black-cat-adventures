using UnityEngine;

/// <summary>
/// Board effect that replaces common tiles with rare gold-etched letters,
/// making the player's word options harder. <c>amplifier</c> controls how
/// many tiles are scrambled. See <see cref="WordGrid.ScrambleTiles"/> for
/// the tile-selection and visual/audio logic.
/// </summary>
[CreateAssetMenu(fileName = "Scramble", menuName = "Board Effects/Scramble")]
public class Scramble : BoardEffect
{

    public override void Apply(int amplifier) => WordGrid.Instance.ScrambleTiles(amplifier);

}
