using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public int width = 4;
    public int height = 4;
    
    [Header("Prefabs")]
    public GameObject cellPrefab;

    [Header("Letter Generation")]
    [Tooltip("If empty, random A–Z. Optionally use frequency-biased letters.")]
    public string allowedLetters = "EEEEEEEEEEEEAAAAAAAAAIIIIIIIIIOOOOOOOONNNNNNRRRRRRTTTTTLLLLSSSSUUUUDDDDGGGBBCCMMPPFFHHVVWWYYKJXQZ"; 
    public bool useFrequencyBag = true;

    private LetterTile[,] grid;
    public LetterTile[,] Grid => grid;
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    public System.Func<char> GetRandomLetter;

    private void Awake()
    {
        GetRandomLetter ??= DefaultRandomLetter;
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        
        // Set up grid layout properties
        float panelWidth = rectTransform.rect.width;
        float panelHeight = rectTransform.rect.height;
        float cellWidth = panelWidth / width;
        float cellHeight = panelHeight / height;
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }

    public void BuildEmptyGrid(int w, int h)
    {
        ClearGrid();
        width = w; 
        height = h;
        grid = new LetterTile[width, height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var tile = Instantiate(cellPrefab, transform).GetComponent<LetterTile>();
            char c = GetRandomLetter();
            tile.Init(x, y, c, TileType.Normal);
            grid[x, y] = tile;
        }
    }

    // public void LoadStaticGrid(LevelTile[] data, int w, int h)
    // {
    //     ClearGrid();
    //     width = w; 
    //     height = h;
    //     grid = new LetterTile[width, height];

    //     int i = 0;
    //     for (int y = 0; y < height; y++)
    //     for (int x = 0; x < width; x++)
    //     {
    //         var tile = Instantiate(cellPrefab, transform).GetComponent<LetterTile>();
    //         var d = data[i++];
    //         tile.Init(x, y, d.letter[0], (TileType)d.tileType);
    //         grid[x, y] = tile;
    //     }
    // }

    public bool AreAdjacent(LetterTile a, LetterTile b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        return (dx <= 1 && dy <= 1 && !(dx == 0 && dy == 0));
    }

    public void RemoveAndRefill(List<LetterTile> tilesToRemove)
    {
        foreach (var t in tilesToRemove)
        {
            if (grid[t.X, t.Y] == t)
            {
                Destroy(t.gameObject);
                grid[t.X, t.Y] = null;
            }
        }

        // Collapse per column
        for (int x = 0; x < width; x++)
        {
            int writeY = 0;
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                {
                    if (y != writeY)
                    {
                        var tile = grid[x, y];
                        grid[x, y] = null;
                        grid[x, writeY] = tile;
                        tile.Y = writeY;
                    }
                    writeY++;
                }
            }
            // Fill new tiles
            for (int y = writeY; y < height; y++)
            {
                var tile = Instantiate(cellPrefab, transform).GetComponent<LetterTile>();
                tile.Init(x, y, GetRandomLetter(), TileType.Normal);
                grid[x, y] = tile;
            }
        }
    }

    public void UnblockAdjacentsToPath(List<LetterTile> usedPath)
    {
        HashSet<(int,int)> adj = new();
        foreach (var t in usedPath)
        {
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = t.X + dx, ny = t.Y + dy;
                if (InBounds(nx, ny)) adj.Add((nx, ny));
            }
        }

        foreach (var (x, y) in adj)
        {
            var tile = grid[x, y];
            if (tile != null && tile.Type == TileType.Blocked)
            {
                tile.Type = TileType.Normal;
            }
        }
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    public void ClearGrid()
    {
        if (grid == null) return;
        foreach (var t in grid)
            if (t != null) Destroy(t.gameObject);
        grid = null;
    }

    private char DefaultRandomLetter()
    {
        if (!useFrequencyBag || string.IsNullOrEmpty(allowedLetters))
            return (char)('A' + Random.Range(0, 26));
        int i = Random.Range(0, allowedLetters.Length);
        return char.ToUpper(allowedLetters[i]);
    }
}
