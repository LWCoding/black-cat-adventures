using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordGenerator
{

    private List<string> _validWords = new();
    private List<string> _profaneWords = new();

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
    /// Get a random letter based on modified changes from Boggle;
    /// some characters are prioritized in weight above others.
    /// </summary>
    public Tile GetRandomTile(int tileIdx)
    {
        // Modified characters from Boggle but stringed together
        string chars = "AAAAAABBCCDDDEEEEEEEEEFFGGHHHHHIIIIIIJKLLLLMMNNNNNNOOOOOOOPPQRRRRRSSSSSSTTTTTTTTTUUUVVWWWXYYYZ";
        char randomChar = chars[Random.Range(0, chars.Length)];
        // Create a tile object
        Tile t = new()
        {
            Letters = randomChar.ToString(),
            TileIndex = tileIdx
        };
        return t;
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
        Tile t = new()
        {
            Letters = randomChar.ToString(),
            TileIndex = tileIdx
        };
        return t;
    }

}
