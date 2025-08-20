using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    [Header("Dictionary")]
    public TextAsset wordList; // put /Resources/wordList and assign in Inspector
    private HashSet<string> dict;

    [Header("Scoring")]
    // Simple Scrabble-ish scoring
    private static readonly Dictionary<char, int> letterScore = new Dictionary<char, int>()
    {
        ['A']=1,['E']=1,['I']=1,['O']=1,['U']=1,['L']=1,['N']=1,['S']=1,['T']=1,['R']=1,
        ['D']=2,['G']=2,
        ['B']=3,['C']=3,['M']=3,['P']=3,
        ['F']=4,['H']=4,['V']=4,['W']=4,['Y']=4,
        ['K']=5,
        ['J']=8,['X']=8,
        ['Q']=10,['Z']=10
    };

    private void Awake()
    {
        dict = new HashSet<string>();
        if (wordList != null)
        {
            foreach (var line in wordList.text.Split('\n'))
            {
                var w = line.Trim().ToUpper();
                if (w.Length > 0) dict.Add(w);
            }
        }
        else
        {
            Debug.LogWarning("WordManager: No wordList assigned; all words will be invalid.");
        }
    }

    public bool IsValid(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length < 2) return false;
        return dict.Contains(word.ToUpper());
    }

    public int ScoreWord(string word, bool usedBonus)
    {
        int s = 0;
        foreach (var c in word.ToUpper())
        {
            if (letterScore.TryGetValue(c, out int v)) s += v;
            else s += 1;
        }
        if (usedBonus) s += 5; // simple bonus
        return s;
    }
}
