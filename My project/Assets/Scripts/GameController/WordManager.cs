using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordManager : MonoBehaviour
{
    [Header("Dictionary")]
    public TextAsset wordList; // put /Resources/wordList and assign in Inspector
    private string dictionaryPath;

    [Header("Scoring")] 
    private Dictionary<char, int> letterScore;

    private void Awake()
    {
        // Store path to dictionary file
        if (wordList != null)
        {
            dictionaryPath = Application.dataPath + "/Resources/" + wordList.name + ".txt";
        }
        else
        {
            Debug.LogWarning("WordManager: No wordList assigned; all words will be invalid.");
        }

        // Initialize empty letter scores - will be set during runtime
        letterScore = new Dictionary<char, int>();
    }

    public bool IsValid(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length < 2) return false;
        
            print($"Checking word '{word}' against dictionary at {dictionaryPath}");
        // Check word against dictionary file
        if (File.Exists(dictionaryPath))
        {
            print($"Checking word '{word}' against dictionary at {dictionaryPath}");
            string upperWord = word.ToUpper();
            foreach (string line in File.ReadLines(dictionaryPath))
            {
                if (line.Trim().ToUpper() == upperWord)
                    return true;
            }
        }
        return false;
    }

    public void SetLetterScore(char letter, int score)
    {
        letterScore[char.ToUpper(letter)] = score;
    }

    public int ScoreWord(List<LetterTile>selectedTiles, bool usedBonus)
    {
        int s = 0;
        foreach (var c in selectedTiles)
        {
                s += c.tileScore;
        }
        
        return s;
    }
}
