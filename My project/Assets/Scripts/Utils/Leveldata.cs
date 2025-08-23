using System;
using System.Collections.Generic;

[Serializable]
public class LevelDataContainer
{
    public List<LevelData> data;
}

[Serializable]
public class LevelData
{
    public int bugCount;
    public int wordCount;
    public int timeSec;
    public int totalScore;
    public GridSize gridSize;
    public int levelType;
    public List<LevelTile> gridData;
}

[Serializable]
public class GridSize
{
    public int x;
    public int y;
}

[Serializable]
public class LevelTile
{
    public string letter;
    public int tileType;
}

public enum LevelType 
{ 
    MakeXWords = 0, 
    ReachScoreInTime = 1, 
    MakeXWordsInTime = 2,
    BugCatch = 3,
    BlockedTiles = 4,
    TimedScore = 5,
    TimedWordsAndScore = 7
}
