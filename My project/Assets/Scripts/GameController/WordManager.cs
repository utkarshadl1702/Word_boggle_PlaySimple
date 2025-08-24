using System.Collections.Generic;
using UnityEngine;


public class WordManager : MonoBehaviour
{
    [Header("Dictionary")]
    public TextAsset wordList; 
    public List<string> foundWords;

    [Header("Scoring")] 
    private Dictionary<char, int> letterScore;

    

    private void Awake()
    {
        
        if (wordList == null)
        {
            Debug.LogWarning("WordManager: No wordList assigned; all words will be invalid.");
        }

        
        letterScore = new Dictionary<char, int>();
    }

    public bool IsValid(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length < 2) return false;
        if (foundWords.Contains(word)) return false; // Already found

        
        if (wordList != null)
        {
            string upperWord = word.ToUpper();
            foreach (string line in wordList.text.Split('\n'))
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
