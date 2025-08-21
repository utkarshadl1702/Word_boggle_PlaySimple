using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameMode { Endless, Levels }

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public GridManager grid;
    public WordManager wordManager;
    public UIManager ui;
    // public LevelLoader levelLoader;
    public WordDragManager wordDragManager; // Add this reference

    [Header("Mode")]
    public GameMode mode = GameMode.Endless;

    [Header("Endless Settings")]
    public int endlessWidth = 4;
    public int endlessHeight = 4;

    [Header("Levels Runtime")]
    private LevelData currentLevel;
    private float levelTimer;
    private bool levelActive;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        StartEndless(); // default
    }

    public void StartEndless()
    {
        mode = GameMode.Endless;
        ui.SetMode(true);
        ui.ResetStats();
        grid.BuildEmptyGrid(endlessWidth, endlessHeight);
        levelActive = false;
    }

    public void StartLevel()
    {
        mode = GameMode.Levels;
        ui.SetMode(false);
        ui.ResetStats();

        // currentLevel = levelLoader != null ? levelLoader.Load() : null;
        if (currentLevel == null)
        {
            Debug.LogError("No level data.");
            return;
        }

        // grid.LoadStaticGrid(currentLevel.gridData, currentLevel.gridSize.x, currentLevel.gridSize.y);
        // ui.SetObjective(LevelLogic.Describe(currentLevel));

        levelTimer = currentLevel.timeSec > 0 ? currentLevel.timeSec : 0f;
        levelActive = true;
    }

    private void Update()
    {
        // Timer for levels
        if (mode == GameMode.Levels && levelActive && currentLevel.timeSec > 0)
        {
            levelTimer -= Time.deltaTime;
            if (levelTimer < 0) levelTimer = 0;
            ui.SetTimer(levelTimer);
            if (levelTimer <= 0) CheckLevelEnd();
        }
    }

    private void CheckLevelEnd()
    {
        if (currentLevel == null) return;

        bool timeUp = currentLevel.timeSec > 0 && levelTimer <= 0;

        switch (currentLevel.levelType)
        {
            case LevelType.MakeXWords:
                if (ui.GetWordsCount() >= currentLevel.wordCount) LevelWin();
                break;
            case LevelType.ReachScoreInTime:
                if (ui.GetTotalScore() >= currentLevel.totalScore) LevelWin();
                else if (timeUp) LevelFail();
                break;
            case LevelType.MakeXWordsInTime:
                if (ui.GetWordsCount() >= currentLevel.wordCount) LevelWin();
                else if (timeUp) LevelFail();
                break;
        }
    }

    private void LevelWin()
    {
        levelActive = false;
        ui.SetObjective("Level Complete!");
    }

    private void LevelFail()
    {
        levelActive = false;
        ui.SetObjective("Time Up!");
    }

    public void ProcessWord(string word)
    {
        if (wordManager.IsValid(word))
        {
            bool usedBonus = false; // Implement bonus detection if needed
            int score = wordManager.ScoreWord(word, usedBonus);
            ui.AddWordScore(score);

            // Check level completion after each valid word
            if (mode == GameMode.Levels && levelActive)
            {
                CheckLevelEnd();
            }
            print($"Word '{word}' scored {score} points.");
        }

        // Update UI with current word
        ui.SetCurrentWord(word);
    }

    // Menu hooks
    public void OnClick_Endless() => StartEndless();
    public void OnClick_Levels() => StartLevel();
}
