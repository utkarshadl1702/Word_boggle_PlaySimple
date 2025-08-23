using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Common")]
    public TMP_Text currentWordText;
    public TMP_Text totalScoreText;
    public TMP_Text avgScoreText;
    public TMP_Text wordsCountText;

    [Header("Levels")]
    public TMP_Text objectiveText;
    public TMP_Text timerText;
    public GameObject levelPanel;
    public GameObject endlessPanel;

    private int totalScore;
    private int wordsFormed;

    public void ResetStats()
    {
        totalScore = 0; wordsFormed = 0;
        UpdateScoreUI();
    }

    public void SetMode(bool endless)
    {
        if (endlessPanel) endlessPanel.SetActive(endless);
        if (levelPanel) levelPanel.SetActive(!endless);
    }

    public void SetCurrentWord(string w)
    {
        if (currentWordText) currentWordText.text = w;
    }

    public void AddWordScore(int add)
    {
        totalScore += add;
        wordsFormed += 1;
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (totalScoreText) totalScoreText.text = $"Score: {totalScore}";
        if (wordsCountText) wordsCountText.text = $"Words: {wordsFormed}";
        if (avgScoreText)
        {
            float avg = wordsFormed > 0 ? (float)totalScore / wordsFormed : 0f;
            avgScoreText.text = $"Avg/Word: {avg:F1}";
        }
    }

    public void SetObjective(string s) { if (objectiveText) objectiveText.text = s; }
    public void SetTimer(float sec) { if (timerText) timerText.text = $"{Mathf.CeilToInt(sec)}s"; }
   
    public int GetTotalScore() => totalScore;
    public int GetWordsCount() => wordsFormed;
}
