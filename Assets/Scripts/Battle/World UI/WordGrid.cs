using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class WordGrid : Singleton<WordGrid>
{

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _letterPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _letterParentTransform;

    [Header("Grid Properties")]
    public int NUM_ROWS;
    public int NUM_COLUMNS;
    public float SPACE_BETWEEN_TILES;

    [Header("Scramble Effect")]
    [SerializeField] private AudioClip _scrambleSFX;  // Placeholder; assign in the Inspector
    [SerializeField] private float _scrambleSFXVolume = 1f;
    [Tooltip("Optional cloud sprite for the Scramble poof; leave empty to use plain particles.")]
    [SerializeField] private Sprite _scramblePoofSprite;

    // Rare, high-value letters a Scrambled tile is turned into (the gold-etched tier).
    private const string GoldEtchedLetters = "jkxzq";

    private readonly List<LetterTile> _letterTiles = new();  // Instantiated letters
    public List<LetterTile> LetterTiles => _letterTiles;

    protected override void Awake()
    {
        base.Awake();
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

                Tile newTile = new("A", r * NUM_ROWS + c, TileTypeName.NORMAL);

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

    /// <summary>
    /// Scrambles <paramref name="count"/> tiles on the board: turns common letters
    /// into rare gold-etched ones to mess up the player's options. This is a
    /// one-time tile effect (not a status). Tiles are chosen by etching tier,
    /// preferring the most common first: bronze (LOW) tiles, then silver (MEDIUM),
    /// then gold (HIGH). Each scrambled tile becomes a random gold-etched letter
    /// and gets a poof particle effect; a sound effect plays once for the batch.
    /// </summary>
    public void ScrambleTiles(int count)
    {
        if (count <= 0 || _letterTiles.Count == 0) { return; }

        // Build a selection pool ordered by etching tier (common letters first).
        List<LetterTile> pool = new();
        foreach (TileDamage tier in new[] { TileDamage.LOW, TileDamage.MEDIUM, TileDamage.HIGH })
        {
            List<LetterTile> inTier = _letterTiles.FindAll(t => t.Tile.DamageType == tier);
            Shuffle(inTier);
            pool.AddRange(inTier);
        }

        int toScramble = Mathf.Min(count, pool.Count);
        for (int i = 0; i < toScramble; i++)
        {
            LetterTile tile = pool[i];
            string goldLetter = GoldEtchedLetters[Random.Range(0, GoldEtchedLetters.Length)].ToString();
            tile.SetTileText(goldLetter);
            ScramblePoof.PlayAt(tile.transform.position, _scramblePoofSprite);
        }

        if (toScramble > 0)
        {
            AudioManager.Instance.PlayOneShot(_scrambleSFX, _scrambleSFXVolume);
        }
    }

    /// <summary>
    /// In-place Fisher-Yates shuffle.
    /// </summary>
    private static void Shuffle(List<LetterTile> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

}
