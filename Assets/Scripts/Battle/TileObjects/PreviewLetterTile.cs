using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PreviewLetterTile : LetterTile
{

    private Tween _moveTween;

    public void MoveTo(Vector3 worldTarget, float duration, Ease ease, bool destroyOnArrival = false)
    {
        _moveTween?.Kill();
        _moveTween = transform.DOMove(worldTarget, duration).SetEase(ease)
            .OnComplete(() =>
            {
                _moveTween = null;
                if (destroyOnArrival) Destroy(gameObject);
            });
    }

    public void StopMove() => _moveTween?.Kill();

    private void OnDestroy()
    {
        _moveTween?.Kill();
        EndTooltipHover();
    }

    private void OnMouseEnter()
    {
        WordPreview.Instance.ToggleTilesFromIndex(Tile.TileIndex, false);
        BeginTooltipHover();
    }

    private void OnMouseExit()
    {
        WordPreview.Instance.ToggleTilesFromIndex(Tile.TileIndex, true);
        EndTooltipHover();
    }

    private void OnMouseDown()
    {
        // If a treasure is awaiting a tile target, forward the click to it instead of
        // removing this tile from the word.
        if (TreasureSection.Instance != null && TreasureSection.Instance.IsAwaitingTarget)
        {
            TreasureSection.Instance.HandleTileTargeted(this);
            return;
        }
        WordPreview.Instance.RemoveTile(Tile);
    }

}
