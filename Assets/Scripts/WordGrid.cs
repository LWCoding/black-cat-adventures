using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGrid : MonoBehaviour
{

    public static WordGrid Instance;

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
    [SerializeField] private GameObject _letterPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _letterParentTransform;

    [Header("Grid Properties")]
    public int NUM_ROWS;
    public int NUM_COLUMNS;
    public float SPACE_BETWEEN_TILES;

    private readonly List<LetterTile> _letterTiles = new();  // Instantiated letters
    public List<LetterTile> LetterTiles => _letterTiles;

    private void Start()
    {
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
                letter.GetComponent<LetterTile>().InitializeTile(GetRandomTile(r * NUM_ROWS + c));

                _letterTiles.Add(letter.GetComponent<LetterTile>());
            }
        }
    }

    /// <summary>
    /// Get a random letter based on chances from the Boggle game;
    /// some characters are prioritized in weight above others.
    /// </summary>
    private Tile GetRandomTile(int tileIdx)
    {
        // All the characters from Boggle but stringed together
        string chars = "AAEEGNABBJOOACHOPSAFFKPSAOOTTWCIMOTUDEILRXDELRVYDISTTYEEGHNWEEINSUEHRTVWEIOSSTELRTTYHIMNQUHLNNRZ";
        char randomChar = chars[Random.Range(0, chars.Length)];
        // Create a tile object
        Tile t = new()
        {
            Letters = randomChar.ToString(),
            TileIndex = tileIdx
        };
        return t;
    }

}
