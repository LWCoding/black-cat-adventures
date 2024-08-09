using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordGenerator : MonoBehaviour
{

    private static WordGenerator _instance;
    public static WordGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<WordGenerator>();
            }
            return _instance;
        }
    }

    private readonly static Dictionary<TileTypeName, TileType> _tileTypes = new();
    public static Dictionary<TileTypeName, TileType> TileTypes
    {
        get
        {
            // If we don't have any treasures, give the player the default treasure
            if (_tileTypes.Count == 0)
            {
                foreach (TileType tt in Resources.LoadAll<TileType>("ScriptableObjects/Tiles").ToList())
                {
                    _tileTypes.Add(tt.TileTypeName, tt);
                }
            }
            return _tileTypes;
        }
    }

    private readonly List<string> _validWords = new();
    private readonly List<string> _profaneWords = new();

    public void Awake()
    {
        if (!ReferenceEquals(_instance, this))
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }
        AssembleWords();  // Assemble once initialized
    }

    /// <summary>
    /// This function goes through provided files and
    /// caches all of the information inside of lists.
    /// </summary>
    private void AssembleWords()
    {
        TextAsset validWordsFile = Resources.Load<TextAsset>("EnglishWords");
        string[] words = validWordsFile.text.Split('\n');
        foreach (string word in words)
        {
            _validWords.Add(word.Trim().ToLower());
        }
        TextAsset profaneWordsFile = Resources.Load<TextAsset>("ProfaneWords");
        words = profaneWordsFile.text.Split('\n');
        foreach (string word in words)
        {
            _profaneWords.Add(word.Trim().ToLower());
        }
    }

    /// <summary>
    /// Given a word, returns True if it's valid, and
    /// False if it's not. Ignores case sensitivity.
    /// 
    /// A word is valid if it is three or more characters
    /// long, and is valid in the English dictionary.
    /// </summary>
    public bool IsValidWord(string lettersToCheck)
    {
        // If we don't have valid words, load these in once
        if (_validWords.Count == 0)
        {
            AssembleWords();
        }
        if (lettersToCheck.Length < 3) return false;
        lettersToCheck = lettersToCheck.ToLower();
        return _validWords.Contains(lettersToCheck);
    }

    /// <summary>
    /// Given a word, returns True if it's profane, and
    /// False if it's not. Ignores case sensitivity.
    /// </summary>
    public bool IsProfaneWord(string lettersToCheck)
    {
        // If we don't have words, load these in once
        if (_profaneWords.Count == 0)
        {
            AssembleWords();
        }
        lettersToCheck = lettersToCheck.ToLower();
        return _profaneWords.Contains(lettersToCheck);
    }

    /// <summary>
    /// Generate a tile object given a tile index and an optional
    /// string of banned letters.
    /// </summary>
    public Tile GetRandomTile(int tileIdx, string bannedLetters = "")
    {
        // Modified characters from Boggle but stringed together
        string randomChar = GetRandomLetter(bannedLetters);
        // Create a tile object
        Tile t = new(randomChar.ToString(), tileIdx, TileTypeName.NORMAL);
        return t;
    }

    /// <summary>
    /// Get a random letter based on modified changes from Boggle;
    /// optionally, you can provide letters (in a string) to blacklist
    /// from being generated.
    /// </summary>
    public string GetRandomLetter(string bannedLetters = "")
    {
        // Modified characters from Boggle but stringed together
        string chars = "AAAAABBCCDDDEEEEEEEEFFGGHHHHHIIIIIIJKLLLLMMNNNNNOOOOOOPPQRRRRRSSSSSSTTTTTTTUUUVVWWWXYYYZ";
        string availableChars = new(chars.Where(c => !bannedLetters.Contains(c)).ToArray());

        // If no available characters left, just return a random character
        if (availableChars == "")
        {
            return chars[Random.Range(0, chars.Length)].ToString();
        }

        // Pick a random character from the available characters
        string randomLetter = availableChars[Random.Range(0, availableChars.Length)].ToString();
        return randomLetter.ToString();
    }

    /// <summary>
    /// Get a random vowel. Skewed chance for certain letters.
    /// </summary>
    public Tile GetRandomVowel(int tileIdx)
    {
        // All the characters from Boggle but stringed together
        string chars = "AAEEIIOU";
        char randomChar = chars[Random.Range(0, chars.Length)];
        // Create a tile object
        Tile t = new(randomChar.ToString(), tileIdx, TileTypeName.NORMAL);
        return t;
    }

    /// <summary>
    /// Given an integer representing the number of damage a word
    /// deals, returns a type for a tile that spawns next.
    /// </summary>
    public TileTypeName GetTileTypeNameByDamage(int damageDealt)
    {
        if (damageDealt > 40)
        {

        } else if (damageDealt > 30)
        {

        } else if (damageDealt > 20)
        {

        } else if (damageDealt > 10)
        {

        }
        return TileTypeName.NORMAL;
    }

    /// <summary>
    /// Given a list of tiles, calculates the number of damage it
    /// would deal in total.
    /// </summary>
    public int CalculateDamage(List<Tile> tiles)
    {
        float total = 0;
        foreach (Tile tile in tiles)
        {
            switch (tile.DamageType)
            {
                case TileDamage.LOW:
                    total += 1;
                    break;
                case TileDamage.MEDIUM:
                    total += 1.4f;
                    break;
                case TileDamage.HIGH:
                    total += 1.8f;
                    break;
            }
        }
        return (int)Mathf.Round(Mathf.Exp(total * 0.4f));
    }

    /// <summary>
    /// Given a string (ideally a character), returns True if it
    /// is a vowel, else False.
    /// </summary>
    public bool IsVowel(string s) => "aeiou".Contains(s.ToLower());

}
