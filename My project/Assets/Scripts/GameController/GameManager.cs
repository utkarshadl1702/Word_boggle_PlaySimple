using System.Collections.Generic;
using UnityEngine;

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
    private bool isLevelStarted = false;

    [Header("Endless Settings")]
    public int endlessWidth = 4;
    public int endlessHeight = 4;

    [Header("Levels Runtime")]
    private LevelData currentLevel;
    private float levelTimer;
    private bool levelActive;
    private int bugsCollected = 0;
    private int randomLevel = -1;

    private Camera cam;
    private LevelDataContainer levelDataContainer;
    private int currentLevelIndex = 0;


    //For Levels
    public int numberOfBugs = 0;
    public int numberOfBlockedTiles = 0;

    private void Start()
    {
        cam = Camera.main;
        // StartEndless(); // default
        
        // if (levelDataContainer != null && levelDataContainer.data.Count > 0)
        //     StartLevel(Random.Range(0, levelDataContainer.data.Count));

        // StartEndless();

    }

    private void Awake()
    {
        LoadLevelData();
        randomLevel = Random.Range(0, levelDataContainer.data.Count);
    }

    private void LoadLevelData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("test-unity-master-Assets/Assets/levelData");
        if (jsonFile != null)
        {
            levelDataContainer = JsonUtility.FromJson<LevelDataContainer>(jsonFile.text);
            Debug.Log($"Loaded {levelDataContainer.data.Count} levels");
        }
        else
        {
            Debug.LogError("Could not load level data!");
        }
    }

    public void StartEndless()
    {
        mode = GameMode.Endless;
        ui.SetMode(true);
        ui.ResetStats();
        grid.BuildEmptyGrid(endlessWidth, endlessHeight);
        levelActive = false;
        isLevelStarted = true;
    }

    public void StartLevel(int levelIndex)
    {
        if (levelDataContainer == null || levelIndex >= levelDataContainer.data.Count)
        {
            Debug.LogError("Invalid level index or no level data!");
            return;
        }

        mode = GameMode.Levels;
        ui.SetMode(false);
        ui.ResetStats();

        currentLevel = levelDataContainer.data[levelIndex];
        currentLevelIndex = levelIndex;
        numberOfBugs = currentLevel.bugCount;
       

        // Load the grid with level data
        grid.LoadStaticGrid(currentLevel.gridData, currentLevel.gridSize.x, currentLevel.gridSize.y, numberOfBugs);

        // Set UI objective based on level type
        string objective = GetLevelObjective(currentLevel);
        ui.SetObjective(objective);

        levelTimer = currentLevel.timeSec > 0 ? currentLevel.timeSec : 0f;
        levelActive = true;

        print(currentLevel.levelType);
        isLevelStarted = true;
    }

    private string GetLevelObjective(LevelData level)
    {
        switch ((LevelType)level.levelType)
        {
            case LevelType.MakeXWords:
                return $"Make {level.wordCount} words";
            case LevelType.ReachScoreInTime:
                return $"Score {level.totalScore} points in {level.timeSec}s";
            case LevelType.MakeXWordsInTime:
                return $"Make {level.wordCount} words in {level.timeSec}s";
            case LevelType.CompleteAllTheBugs:
                return $"Collect all {level.bugCount} bugs";

            default:
                return "Complete the level";
        }
    }
    private float t = 0;

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
        if (mode == GameMode.Endless)
        {
            ui.SetTimer(t);
            t += Time.deltaTime;
        }
    }

    private void CheckLevelEnd()
    {
        if (currentLevel == null) return;

        bool timeUp = currentLevel.timeSec > 0 && levelTimer <= 0;
        int wordsCount = ui.GetWordsCount();
        int totalScore = ui.GetTotalScore();

        switch ((LevelType)currentLevel.levelType)
        {
            case LevelType.MakeXWords:
                if (wordsCount >= currentLevel.wordCount)
                    LevelWin();
                break;

            case LevelType.ReachScoreInTime:
                if (totalScore >= currentLevel.totalScore)
                    LevelWin();
                else if (timeUp)
                    LevelFail();
                break;

            case LevelType.MakeXWordsInTime:
                if (wordsCount >= currentLevel.wordCount)
                    LevelWin();
                else if (timeUp)
                    LevelFail();
                break;
            case LevelType.CompleteAllTheBugs:
                if (bugsCollected >= currentLevel.bugCount)
                    LevelWin();
                break;
            default:
                    if (timeUp)
                        LevelWin();
                break;

        }
    }

    private void LevelWin()
    {
        levelActive = false;

        wordManager.foundWords.Clear();
        ui.SetObjective("Level Complete!");
        NextLevel();
    }

    private void LevelFail()
    {
        levelActive = false;
        ui.SetObjective("Time Up!");
    }

    public void ProcessWord(string word, List<LetterTile> selectedTiles)
    {
        if (wordManager.IsValid(word))
        {
            bool usedBonus = false;
            int score = wordManager.ScoreWord(selectedTiles, usedBonus);
            ui.AddWordScore(score);

            // Check level completion after each valid word
            if (mode == GameMode.Levels && levelActive)
            {
                // grid.UnblockAdjacentsToPath(selectedTiles);

                wordManager.foundWords.Add(word);
                foreach (var tile in selectedTiles)
                {
                    if (tile.isBugTile) bugsCollected++;
                    StartCoroutine(tile.GetComponent<GridCell>().GlowCoroutine(Color.green));
                }
                grid.UnblockAdjacentsToPath(selectedTiles);
                CheckLevelEnd();
            }

            // Handle tile removal and refilling
            if (mode == GameMode.Endless)
            {
                StartCoroutine(grid.RemoveAndRefill(selectedTiles));
            }




            print($"Word '{word}' scored {score} points.");
        }
        else
        {
            foreach (var tile in selectedTiles)
            {
                StartCoroutine(tile.GetComponent<GridCell>().GlowCoroutine(Color.red));
            }

        }
        // Update UI with current word
        ui.SetCurrentWord(word);
    }









    // Menu hooks
    public void OnClick_Endless() => StartEndless();
    public void OnClick_Levels() => StartLevel(Random.Range(0, levelDataContainer.data.Count));

    // Add method to go to next level
    public void NextLevel()
    {
        //random number between 0-49
        int nextLevelIndex = Random.Range(0, levelDataContainer.data.Count);
        StartLevel(nextLevelIndex);
    }
}
