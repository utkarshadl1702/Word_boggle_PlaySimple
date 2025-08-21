using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int bugCount;
    public int wordCount;
    public int timeSec;
    public int totalScore;
    public Vector2Int gridSize;
    public LevelTile[] gridData;

    // Type of win condition (derived):
    public LevelType levelType; // MakeXWords, ReachScoreInTime, MakeXWordsInTime
}

public enum LevelType { MakeXWords = 0, ReachScoreInTime = 1, MakeXWordsInTime = 2 }

[Serializable]
public class LevelTile
{
    public int tileType; // 0=Normal,1=Blocked,2=Bonus
    public string letter;
}
