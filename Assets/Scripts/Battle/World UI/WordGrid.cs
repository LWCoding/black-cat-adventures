using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeBoard();
    }

    /// <summary>
    /// Initializes a board of letter objects based on the
    /// number of rows and columns. Then, shuffles the board.
    /// </summary>
    public void InitializeBoard()
    {
        for (int r = 0; r < NUM_ROWS; r++)
        {
            for (int c = 0; c < NUM_COLUMNS; c++)
            {
                GameObject letter = Instantiate(_letterPrefab, _letterParentTransform, false);
                letter.transform.position += new Vector3(SPACE_BETWEEN_TILES * r, -SPACE_BETWEEN_TILES * c);

                Tile newTile = new("A", r * NUM_ROWS + c);

                letter.GetComponent<LetterTile>().InitializeTile(newTile);
                _letterTiles.Add(letter.GetComponent<LetterTile>());
            }
        }

        ShuffleBoard();  // Shuffle the board after making the objects
    }

    /// <summary>
    /// Shuffles the letters of each tile on the board.
    /// Requires the board to be initialized first.
    /// </summary>
    public void ShuffleBoard()
    {
        int vowelCount = 0;
        Dictionary<string, int> letterOccur = new();
        StringBuilder blacklistedLetters = new();  // Letters that occur >3 tiles shouldn't be added anymore

        for (int i = 0; i < _letterTiles.Count; i++) {
            LetterTile letter = _letterTiles[i];

            // Get a random letter that's not blacklisted; add one to its occurrences
            string generatedLetter = WordGenerator.Instance.GetRandomLetter(blacklistedLetters.ToString());
            if (!letterOccur.ContainsKey(generatedLetter))
            {
                letterOccur[generatedLetter] = 0;
            }
            letterOccur[generatedLetter]++;
            if (letterOccur[generatedLetter] == 3)
            {
                blacklistedLetters.Append(generatedLetter);
            }

            // Set the tile to the created tile, disregarding the tile index
            letter.SetTileText(generatedLetter);
            if (WordGenerator.Instance.IsVowel(generatedLetter)) vowelCount++;  // Add one to vowel count if we made a vowel
        }

        // Guarantee at least three vowels
        for (int i = 0; i < 3 - vowelCount; i++)
        {
            int randomIdx = Random.Range(0, NUM_ROWS * NUM_COLUMNS);
            _letterTiles[randomIdx].RandomizeVowel();
        }
    }

}
