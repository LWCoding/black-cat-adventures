using UnityEngine;

/// <summary>
/// Board effect that locks a number of the player's word-grid tiles for a
/// fixed number of turns. <c>amplifier</c> (set per-attack in EnemyData)
/// controls how many tiles are locked; duration is set on this asset via
/// <c>_lockDurationTurns</c> so different enemies can lock for different lengths.
/// See <see cref="WordGrid.LockRandomTiles"/> for selection and visual logic.
/// </summary>
[CreateAssetMenu(fileName = "LockTiles", menuName = "Board Effects/Lock Tiles")]
public class LockTiles : BoardEffect
{

    [SerializeField] private int _lockDurationTurns = 2;

    public override void Apply(int amplifier) => WordGrid.Instance.LockRandomTiles(amplifier, _lockDurationTurns);

}
