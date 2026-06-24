using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WordPreview : Singleton<WordPreview>
{

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _previewLetterPrefab;

    [Header("Object Assignments")]
    public TextMeshPro FeedbackText;
    [SerializeField] private Transform _letterParentTransform;

    [Header("Fling Animation")]
    [SerializeField] private float _staggerDelay = 0.02f;
    [SerializeField] private float _recoilDuration = 0.05f;
    [SerializeField] private float _recoilDistance = 0.3f;
    [SerializeField] private float _flightDuration = 0.16f;
    [SerializeField] private Ease _flingEase = Ease.InExpo;
    [SerializeField] private float _impactDuration = 0.25f;
    [SerializeField] private float _impactGrowScale = 1.3f;
    [SerializeField] private float _impactXOffset = 0.5f;   // How far left of enemy center letters land
    [SerializeField] private float _impactXSpread = 0.5f;   // Random X scatter around landing point
    [SerializeField] private float _impactYSpread = 0.8f;   // Random Y scatter around landing point

    public Action OnLetterTilesChanged = null;  // Called whenever chosen letters have been modified
    public List<Tile> CurrentTiles => _currTiles;
    public string CurrentWord
    {
        get
        {
            StringBuilder word = new();
            foreach (Tile t in _currTiles)
            {
                word.Append(t.Letters);
            }
            return word.ToString();
        }
    }

    private readonly List<GameObject> _previewLetterTiles = new();
    private readonly List<Tile> _currTiles = new();
    private readonly float SPACE_BETWEEN_TILES = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        FeedbackText.enabled = false;

        // Edit the feedback text based on # of letters in word, if valid
        OnLetterTilesChanged += () =>
        {
            if (!WordGenerator.Instance.IsValidWord(CurrentWord)) { return; }
            float wordDamage = DamageCalculator.CalculateDamage(CurrentTiles, true);
            if (wordDamage < 7) { return; }
            FeedbackText.enabled = true;
            if (wordDamage >= 45)
            {
                FeedbackText.text = "Otherworldly...";
            }
            else if (wordDamage >= 30)
            {
                FeedbackText.text = "Insanity!";
            }
            else if (wordDamage >= 20)
            {
                FeedbackText.text = "Spectacular!";
            } 
            else if (wordDamage >= 15) 
            { 
                FeedbackText.text = "Amazing!";
            } 
            else if (wordDamage >= 10)
            {
                FeedbackText.text = "Great!";
            }
            else if (wordDamage >= 7)
            {
                FeedbackText.text = "Good!";
            }
        };
    }

    /// <summary>
    /// Add a tile to the end of the list of chosen tiles.
    /// </summary>
    public void AppendTile(Tile tile)
    {
        tile.CurrTileType.OnTileAdded();
        _currTiles.Add(tile);
        UpdatePreviewLetters();
        OnLetterTilesChanged.Invoke();
    }

    /// <summary>
    /// Remove a specific tile from the list of preview tiles.
    /// Gets rid of all of the tiles after it, if they exist.
    /// </summary>
    public void RemoveTile(Tile tile)
    {
        int tileIdx = _currTiles.FindIndex((t) => t.TileIndex == tile.TileIndex);
        while (tileIdx < _currTiles.Count)
        {
            WordGrid.Instance.LetterTiles[_currTiles[tileIdx].TileIndex].Tile.CurrTileType.OnTileRemoved();
            WordGrid.Instance.LetterTiles[_currTiles[tileIdx].TileIndex].IsSelected = false;
            _currTiles.RemoveAt(tileIdx);
        }
        UpdatePreviewLetters();
        OnLetterTilesChanged.Invoke();
    }

    /// <summary>
    /// Remove all preview tiles currently showing at the top.
    /// Refresh the tiles in the word grid to be different letters,
    /// but discourage letters that appear twice or more.
    /// 
    /// Optionally takes a tile to spawn with the new tiles
    /// that are randomized.
    /// </summary>
    public void ConsumeTiles(TileTypeName tileToSpawn = TileTypeName.NORMAL)
    {
        int randomIdxForSpecTile = Random.Range(0, _currTiles.Count);  // Get random tile for special tile
                                                                       // if one should spawn!
        for (int i = _currTiles.Count - 1; i >= 0; i--)
        {
            Tile t = _currTiles[i];
            // Find any discouraged tiles that appear >1 time
            Dictionary<string, int> tileOccur = new();
            StringBuilder discouragedLetters = new();
            List<LetterTile> gridTiles = WordGrid.Instance.LetterTiles;
            for (int j = gridTiles.Count - 1; j >= 0; j--)
            {
                string currTileLetters = gridTiles[j].GetLetters();
                if (!tileOccur.ContainsKey(currTileLetters))
                {
                    tileOccur[currTileLetters] = 0;
                }
                tileOccur[gridTiles[j].GetLetters()] += 1;
                if (tileOccur[currTileLetters] == 2)
                {
                    discouragedLetters.Append(currTileLetters);
                }
            }
            RemoveTile(t);
            // Randomize the tile, given these discouraged letters
            if (randomIdxForSpecTile == i)
            {
                WordGrid.Instance.LetterTiles[t.TileIndex].RandomizeTile(discouragedLetters.ToString(), tileToSpawn);
            }
            else
            {
                WordGrid.Instance.LetterTiles[t.TileIndex].RandomizeTile(discouragedLetters.ToString());
            }
        }
    }

    /// <summary>
    /// Remove all preview tiles currently showing at the top.
    /// Do not initialize new tiles.
    /// </summary>
    public void EraseTiles()
    {
        for (int i = _currTiles.Count - 1; i >= 0; i--)
        {
            Tile t = _currTiles[i];
            RemoveTile(t);
        }
    }

    /// <summary>
    /// Update the preview letters to represent the currently 
    /// chosen letters.
    /// </summary>
    private void UpdatePreviewLetters()
    {
        // Get rid of all preview letters
        foreach (GameObject o in _previewLetterTiles)
        {
            Destroy(o);
        }
        // Hide preview text (this should be updated after the function)
        FeedbackText.enabled = false;
        FeedbackText.text = "";
        // Clear list of tiles
        _previewLetterTiles.Clear();
        // Calculate starting position based on # of letters
        float spacePerTile = _previewLetterPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.size.x * _previewLetterPrefab.transform.localScale.x + SPACE_BETWEEN_TILES;
        Vector3 startingOffset;
        if (_currTiles.Count % 2 == 0)
        {
            // Even number
            startingOffset = new Vector3(-(_currTiles.Count / 2f) * spacePerTile, 0);
        } else
        {
            // Odd number
            startingOffset = new Vector3(-(_currTiles.Count / 2f) * spacePerTile, 0);
        }
        startingOffset += new Vector3(spacePerTile / 2, 0);
        // Redraw current preview letters
        for (int i = 0; i < _currTiles.Count; i++)
        {
            GameObject obj = Instantiate(_previewLetterPrefab, _letterParentTransform, false);
            obj.GetComponent<LetterTile>().InitializeTile(_currTiles[i]);
            obj.transform.position = startingOffset + new Vector3(spacePerTile * i, _letterParentTransform.transform.position.y, 0);
            _previewLetterTiles.Add(obj);
        }
    }

    /// <summary>
    /// Makes all of the preview tiles from a specific index toggle
    /// visibility. Returns early if no preview tiles are found.
    /// </summary>
    public void ToggleTilesFromIndex(int idx, bool isVisible)
    {
        int tilesIdx = CurrentTiles.FindIndex((t) => t.TileIndex == idx);
        if (tilesIdx == -1) { return; }
        for (; tilesIdx < _currTiles.Count; tilesIdx++)
        {
            _previewLetterTiles[tilesIdx].GetComponent<PreviewLetterTile>().ToggleVisibility(isVisible);
        }
    }

    /// <summary>
    /// Two-phase animation:
    /// Phase 1 — each letter punches outward with a short stagger so the word
    ///           "winds up" before launching.
    /// Phase 2 — once the last pulse finishes, every letter flies to its own
    ///           scatter position on the left side of <paramref name="target"/>.
    /// <paramref name="onAllLanded"/> fires when the last letter lands.
    /// </summary>
    public void FlingTilesToEnemy(Vector3 target, System.Action onAllLanded)
    {
        // Snapshot and detach so ConsumeTiles / UpdatePreviewLetters won't Destroy these.
        List<GameObject> toFling = new(_previewLetterTiles);
        _previewLetterTiles.Clear();
        foreach (GameObject obj in toFling)
        {
            obj.transform.SetParent(null, worldPositionStays: true);
        }

        if (toFling.Count == 0)
        {
            onAllLanded?.Invoke();
            return;
        }

        // Pre-compute a unique landing position per tile: left side of enemy with random scatter.
        Vector3 landBase = target + new Vector3(-_impactXOffset, 0f, 0f);
        List<Vector3> landPositions = new(toFling.Count);
        for (int i = 0; i < toFling.Count; i++)
        {
            landPositions.Add(landBase + new Vector3(
                Random.Range(-_impactXSpread, _impactXSpread),
                Random.Range(-_impactYSpread, _impactYSpread),
                0f));
        }

        // Phase 1: staggered recoil — each letter slides away from the enemy and holds.
        for (int i = 0; i < toFling.Count; i++)
        {
            Vector3 recoilPos = toFling[i].transform.position
                + (toFling[i].transform.position - target).normalized * _recoilDistance;
            toFling[i].transform
                .DOMove(recoilPos, _recoilDuration)
                .SetDelay(i * _staggerDelay)
                .SetEase(Ease.OutQuad);
        }

        // Phase 2: once the last letter has finished recoiling, all blast to their landing spots.
        float launchDelay = (toFling.Count - 1) * _staggerDelay + _recoilDuration;
        DOVirtual.DelayedCall(launchDelay, () =>
        {
            int remaining = toFling.Count;
            for (int i = 0; i < toFling.Count; i++)
            {
                GameObject captured = toFling[i];
                Vector3 landPos = landPositions[i];
                captured.transform
                    .DOMove(landPos, _flightDuration)
                    .SetEase(_flingEase)
                    .OnComplete(() =>
                    {
                        remaining--;
                        if (remaining == 0)
                        {
                            onAllLanded?.Invoke();
                            foreach (GameObject t in toFling)
                            {
                                if (t != null)
                                {
                                    t.GetComponent<LetterTile>().PlayImpactAndDestroy(_impactDuration, _impactGrowScale);
                                }
                            }
                        }
                    });
            }
        });
    }

}
