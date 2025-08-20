using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public int width = 4;
    public int height = 4;
    public float spacing = 1.2f;
    public Vector2 origin = new Vector2(-2f, -2f);

    [Header("Prefabs")]
    public LetterTile tilePrefab;

    [Header("Letter Generation")]
    [Tooltip("If empty, random A–Z. Optionally use frequency-biased letters.")]
    public string allowedLetters = "EEEEEEEEEEEEAAAAAAAAAIIIIIIIIIOOOOOOOONNNNNNRRRRRRTTTTTLLLLSSSSUUUUDDDDGGGBBCCMMPPFFHHVVWWYYKJXQZ"; 
    public bool useFrequencyBag = true;

    private LetterTile[,] grid;
    public LetterTile[,] Grid => grid;

    public System.Func<char> GetRandomLetter; // injectable by GameManager if needed

    private void Awake()
    {
        GetRandomLetter ??= DefaultRandomLetter;
        // Center the grid by calculating the origin based on width and height
        origin = new Vector2(
            -(width * spacing) / 2f,
            -(height * spacing) / 2f
        );
    }

    public void BuildEmptyGrid(int w, int h)
    {
        ClearGrid();
        width = w; height = h;
        grid = new LetterTile[width, height];

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            var tile = Instantiate(tilePrefab, CellToWorld(x, y), Quaternion.identity, transform);
            char c = GetRandomLetter();
            tile.Init(x, y, c, TileType.Normal);
            grid[x, y] = tile;
        }
    }

    public void LoadStaticGrid(LevelTile[] data, int w, int h)
    {
        ClearGrid();
        width = w; height = h;
        grid = new LetterTile[width, height];

        int i = 0;
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var tile = Instantiate(tilePrefab, CellToWorld(x, y), Quaternion.identity, transform);
            var d = data[i++];
            tile.Init(x, y, d.letter[0], (TileType)d.tileType);
            grid[x, y] = tile;
        }
    }

    public Vector3 CellToWorld(int x, int y)
    {
        // Apply proper spacing between tiles
        float xPos = origin.x + (x * spacing);
        float yPos = origin.y + (y * spacing);
        return new Vector3(xPos, yPos, 0);
    }

    public bool AreAdjacent(LetterTile a, LetterTile b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        return (dx <= 1 && dy <= 1 && !(dx == 0 && dy == 0));
    }

    // Remove selected tiles (set to null), then collapse columns and refill
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
                        tile.transform.position = CellToWorld(x, writeY);
                    }
                    writeY++;
                }
            }
            // Fill new from top
            for (int y = writeY; y < height; y++)
            {
                var tile = Instantiate(tilePrefab, CellToWorld(x, y), Quaternion.identity, transform);
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
                tile.Type = TileType.Normal; // unblocked
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
