using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordGenerator
{

    private List<string> _validWords = new();

    /// <summary>
    /// This function goes through the file provided in
    /// a specified file and adds each word to the set
    /// `validWords`.
    /// </summary>
    private void AssembleValidWords()
    {
        TextAsset validWordsFile = Resources.Load<TextAsset>("EnglishWords");
        string[] words = validWordsFile.text.Split('\n');
        foreach (string word in words)
        {
            _validWords.Add(word.Trim().ToLower());
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
            AssembleValidWords();
        }
        if (lettersToCheck.Length < 3) return false;
        lettersToCheck = lettersToCheck.ToLower();
        return _validWords.Contains(lettersToCheck);
    }

    /// <summary>
    /// Get a random letter based on chances from the Boggle game;
    /// some characters are prioritized in weight above others.
    /// </summary>
    public Tile GetRandomTile(int tileIdx)
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
