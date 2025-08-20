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
    public LevelLoader levelLoader;

    [Header("Mode")]
    public GameMode mode = GameMode.Endless;

    [Header("Endless Settings")]
    public int endlessWidth = 4;
    public int endlessHeight = 4;

    [Header("Levels Runtime")]
    private LevelData currentLevel;
    private float levelTimer;
    private bool levelActive;

    // Drag selection state
    private readonly List<LetterTile> currentPath = new();
    private HashSet<LetterTile> inPath = new();

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
        currentPath.Clear(); inPath.Clear();
    }

    public void StartLevel()
    {
        mode = GameMode.Levels;
        ui.SetMode(false);
        ui.ResetStats();

        currentLevel = levelLoader != null ? levelLoader.Load() : null;
        if (currentLevel == null)
        {
            Debug.LogError("No level data.");
            return;
        }

        grid.LoadStaticGrid(currentLevel.gridData, currentLevel.gridSize.x, currentLevel.gridSize.y);
        ui.SetObjective(LevelLogic.Describe(currentLevel));

        levelTimer = currentLevel.timeSec > 0 ? currentLevel.timeSec : 0f;
        levelActive = true;
        currentPath.Clear(); inPath.Clear();
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

        HandleInput();
    }

    private void HandleInput()
    {
        // Ignore when pointer over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            BeginSelection();
        }
        if (Input.GetMouseButton(0))
        {
            ContinueSelection(ScreenToWorld(Input.mousePosition));
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndSelection();
        }

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) BeginSelection();
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) ContinueSelection(ScreenToWorld(t.position));
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) EndSelection();
        }
#endif
    }

    private Vector3 ScreenToWorld(Vector3 screenPos)
    {
        var p = cam.ScreenToWorldPoint(screenPos);
        p.z = 0;
        return p;
    }

    private void BeginSelection()
    {
        currentPath.Clear();
        inPath.Clear();
        ui.SetCurrentWord("");
        TryAddTileAt(ScreenToWorld(Input.mousePosition));
    }

    private void ContinueSelection(Vector3 world)
    {
        TryAddTileAt(world);
        ui.SetCurrentWord(BuildWord(currentPath));
    }

    private void EndSelection()
    {
        string word = BuildWord(currentPath);
        bool usedBonus = currentPath.Exists(t => t.Type == TileType.Bonus);

        if (wordManager.IsValid(word))
        {
            int add = wordManager.ScoreWord(word, usedBonus);
            ui.AddWordScore(add);

            if (mode == GameMode.Endless)
            {
                grid.RemoveAndRefill(currentPath);
            }
            else // Levels: static grid; apply special rules
            {
                if (usedBonus && currentLevel != null && currentLevel.bugCount > 0)
                {
                    // Simple count-down of required bonuses if you want to track it
                    currentLevel.bugCount = Mathf.Max(0, currentLevel.bugCount - 1);
                }
                // Unblock rocks adjacent to word path
                grid.UnblockAdjacentsToPath(currentPath);
                CheckLevelEnd();
            }
        }

        currentPath.Clear();
        inPath.Clear();
        ui.SetCurrentWord("");
    }

    private string BuildWord(List<LetterTile> path)
    {
        System.Text.StringBuilder sb = new();
        foreach (var t in path) sb.Append(t.Letter);
        return sb.ToString();
    }

    private void TryAddTileAt(Vector3 world)
    {
        // Raycast for 2D colliders
        var hit = Physics2D.OverlapPoint(world);
        if (!hit) return;

        var tile = hit.GetComponent<LetterTile>();
        if (tile == null || !tile.IsSelectable) return;

        if (currentPath.Count == 0)
        {
            currentPath.Add(tile);
            inPath.Add(tile);
            return;
        }

        var last = currentPath[currentPath.Count - 1];

        // Allow backtracking one step (undo) if dragging over previous tile
        if (currentPath.Count >= 2 && tile == currentPath[currentPath.Count - 2])
        {
            inPath.Remove(last);
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }

        if (!inPath.Contains(tile) && grid.AreAdjacent(last, tile))
        {
            currentPath.Add(tile);
            inPath.Add(tile);
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

    // Menu hooks
    public void OnClick_Endless() => StartEndless();
    public void OnClick_Levels() => StartLevel();
}
