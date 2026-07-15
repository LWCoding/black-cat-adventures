using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dictionary", menuName = "Treasures/Dictionary")]
public class Dictionary : ActiveTreasure
{

    // The letters stamped onto the board, in order, when triggered.
    private static readonly string[] Suffix = { "I", "N", "G" };

    public override bool OnTrigger()
    {
        if (WordGrid.Instance == null) { return true; }

        // Only overwrite unlocked tiles so the effect can't waste itself on locked ones.
        List<LetterTile> candidates = new();
        foreach (LetterTile tile in WordGrid.Instance.LetterTiles)
        {
            if (tile != null && tile.Tile != null && !tile.Tile.IsLocked)
            {
                candidates.Add(tile);
            }
        }

        // Pick up to three distinct random tiles and stamp I, N, G onto them.
        for (int i = 0; i < Suffix.Length && candidates.Count > 0; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            LetterTile chosen = candidates[idx];
            candidates.RemoveAt(idx);

            chosen.Tile.Letters = Suffix[i];
            chosen.InitializeTile(chosen.Tile);

            // Grid and preview clones share the same Tile object, so refresh the
            // preview too in case this tile is currently staged in the word.
            if (WordPreview.Instance != null)
            {
                WordPreview.Instance.RefreshTileByIndex(chosen.Tile.TileIndex);
            }
            if (ParticleManager.Instance != null)
            {
                ParticleManager.Instance.SpawnPoof(chosen.transform.position);
            }
        }

        return true;
    }

}
