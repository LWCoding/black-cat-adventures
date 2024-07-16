using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class WordGrid : MonoBehaviour
{

    public static WordGrid Instance;

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _letterPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _letterParentTransform;

    [Header("Grid Properties")]
    public int NUM_ROWS;
    public int NUM_COLUMNS;
    public float SPACE_BETWEEN_TILES;

    private readonly List<LetterTile> _letterTiles = new();  // Instantiated letters
    public List<LetterTile> LetterTiles => _letterTiles;

    private WordGenerator _wordGenerator = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        InitializeBoard();
    }

    /// <summary>
    /// Initializes a board of letters based on the
    /// number of rows and columns.
    /// </summary>
    public void InitializeBoard()
    {
        for (int r = 0; r < NUM_ROWS; r++)
        {
            for (int c = 0; c < NUM_COLUMNS; c++)
            {
                GameObject letter = Instantiate(_letterPrefab, _letterParentTransform, false);
                letter.transform.position += new Vector3(SPACE_BETWEEN_TILES * r, -SPACE_BETWEEN_TILES * c);
                letter.GetComponent<LetterTile>().InitializeTile(_wordGenerator.GetRandomTile(r * NUM_ROWS + c));

                _letterTiles.Add(letter.GetComponent<LetterTile>());
            }
        }
    }

    /// <summary>
    /// Shuffles the letters of each tile on the board.
    /// Requires the board to be initialized first.
    /// </summary>
    public void ShuffleBoard()
    {
        foreach (LetterTile letterTile in _letterTiles)
        {
            letterTile.RandomizeTile();
        }
    }

}
