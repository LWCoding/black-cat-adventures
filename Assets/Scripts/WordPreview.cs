using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WordPreview : MonoBehaviour
{

    public static WordPreview Instance;

    #region Singleton Logic
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }
    #endregion

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _previewLetterPrefab;

    [Header("Object Assignments")]
    [SerializeField] private Transform _letterParentTransform;

    private readonly List<GameObject> _previewLetterTiles = new();
    private readonly List<Tile> _currTiles = new();

    private readonly float SPACE_BETWEEN_TILES = 0.1f;

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
    /// Remove a specific tile from the list of chosen tiles.
    /// </summary>
    public void RemoveTile(Tile tile)
    {
        int tileIdx = _currTiles.FindIndex((t) => t.TileIndex == tile.TileIndex);
        WordGrid.Instance.LetterTiles[tile.TileIndex].IsSelected = false;
        _currTiles.RemoveAt(tileIdx);
        // TODO: When we get rid of a letter, get rid of all letters after it
        //while (_currTiles.Count >= tileIdx)
        //{
        //    _currTiles.RemoveAt(tileIdx)
        //}
        UpdatePreviewLetters();
        OnLetterTilesChanged.Invoke();
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

}
