using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class WordPreview : MonoBehaviour
{

    public static WordPreview Instance;

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _previewLetterPrefab;

    [Header("Object Assignments")]
    public TextMeshPro FeedbackText;
    [SerializeField] private Transform _letterParentTransform;

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

    private WordGenerator _wordGenerator = new();
    private readonly List<GameObject> _previewLetterTiles = new();
    private readonly List<Tile> _currTiles = new();
    private readonly float SPACE_BETWEEN_TILES = 0.1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        FeedbackText.enabled = false;

        // Edit the feedback text based on # of letters in word, if valid
        OnLetterTilesChanged += () =>
        {
            FeedbackText.enabled = false;
            if (!_wordGenerator.IsValidWord(CurrentWord) || CurrentTiles.Count < 5) return;
            FeedbackText.enabled = true;
            if (CurrentTiles.Count >= 8)
            {
                FeedbackText.text = "Spectacular!";
            } 
            else if (CurrentTiles.Count >= 7) 
            { 
                FeedbackText.text = "Amazing!";
            } 
            else if (CurrentTiles.Count >= 6)
            {
                FeedbackText.text = "Great!";
            }
            else if (CurrentTiles.Count >= 5)
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
            WordGrid.Instance.LetterTiles[_currTiles[tileIdx].TileIndex].IsSelected = false;
            _currTiles.RemoveAt(tileIdx);
        }
        UpdatePreviewLetters();
        OnLetterTilesChanged.Invoke();
    }

    /// <summary>
    /// Remove all preview tiles currently showing at the top.
    /// Refresh the tiles in the word grid to be different letters.
    /// </summary>
    public void ConsumeTiles()
    {
        for (int i = _currTiles.Count - 1; i >= 0; i--)
        {
            Tile t = _currTiles[i];
            WordGrid.Instance.LetterTiles[t.TileIndex].InitializeTile(_wordGenerator.GetRandomTile(t.TileIndex));
            RemoveTile(t);
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

    public void ToggleTilesFromIndex(int idx, bool isVisible)
    {
        int tilesIdx = CurrentTiles.FindIndex((t) => t.TileIndex == idx);
        for (; tilesIdx < _currTiles.Count; tilesIdx++)
        {
            _previewLetterTiles[tilesIdx].GetComponent<PreviewLetterTile>().ToggleVisibility(isVisible);
        }
    }

}
