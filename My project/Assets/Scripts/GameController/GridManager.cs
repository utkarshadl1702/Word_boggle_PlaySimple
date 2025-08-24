using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public int width = 4;
    public int height = 4;

    [Header("Prefabs")]
    public GameObject cellPrefab;

    [Header("Letter Generation")]
    public string allowedLetters = "EEEEEEEEEEEEAAAAAAAAAIIIIIIIIIOOOOOOOONNNNNNRRRRRRTTTTTLLLLSSSSUUUUDDDDGGGBBCCMMPPFFHHVVWWYYKJXQZ";// For endless
    public bool useFrequencyBag = true;

    private LetterTile[,] grid;
    public LetterTile[,] Grid => grid;
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;
    private List<int> specialTiles = new List<int>();

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

    public void BuildEmptyGrid(int w, int h)//For Endless Mode
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

    public void LoadStaticGrid(List<LevelTile> data, int w, int h, int numberOfBugs)//For Levels Mode
    {
        ClearGrid();
        width = w;
        height = h;
        grid = new LetterTile[width, height];


        // Update grid layout properties
        float panelWidth = rectTransform.rect.width;
        float panelHeight = rectTransform.rect.height;
        float cellWidth = panelWidth / width;
        float cellHeight = panelHeight / height;
        var bugPositions = GetRandomPositionForSpecialTile(width * height, numberOfBugs);
        var blockPositions = GetRandomPositionForSpecialTile(width * height, 2);
        specialTiles = bugPositions;

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

        int i = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (i >= data.Count) break;

                var tile = Instantiate(cellPrefab, transform).GetComponent<LetterTile>();
                var d = data[i++];
                if (bugPositions.Contains(i - 1) && d.tileType == 0)
                    d.tileType = 2; 

                if (blockPositions.Contains(i - 1) && d.tileType == 0)
                    d.tileType = 1;

                tile.Init(x, y, GetRandomLetter(), (TileType)d.tileType);

                grid[x, y] = tile;
            }
    }

    public bool AreAdjacent(LetterTile a, LetterTile b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        return (dx <= 1 && dy <= 1 && !(dx == 0 && dy == 0));
    }

    public IEnumerator RemoveAndRefill(List<LetterTile> tilesToRemove)
    {
        print("Removing and refilling tiles (fall-down from bottom)...");

        bool[,] empty = new bool[width, height];

        // Empty tiles
        foreach (var tile in tilesToRemove)
        {
            if (grid[tile.X, tile.Y] == tile)
            {
                tile.ResetCell();
                empty[tile.X, tile.Y] = true;
            }
        }

        yield return new WaitForSeconds(1f);

        // Step 2: process each column
        for (int x = 0; x < width; x++)
        {
            // Start from top going downward
            for (int y = height - 1; y >= 0; y--)
            {
                if (!empty[x, y]) continue;

                // Find next nonempty BELOW
                int below = y - 1;
                while (below >= 0 && empty[x, below]) below--;

                if (below >= 0)
                {
                    var src = grid[x, below];
                    var dst = grid[x, y];

                    if (src.Type == TileType.Normal) dst.tileScore = Random.Range(1,4);

                    // Copy the below tile into this one
                    dst.Init(dst.X, dst.Y, src.Letter, src.Type);
                    
                    // Reset the below (now becomes empty)
                    src.ResetCell();
                    empty[x, below] = true;
                    empty[x, y] = false;
                }
                else
                {
                    // Nothing below assign fresh letter
                    var dst = grid[x, y];
                    dst.tileScore = 1;
                    dst.Init(dst.X, dst.Y, GetRandomLetter(), TileType.Normal);
                    empty[x, y] = false;
                }

                yield return new WaitForSeconds(0.2f); // smoother animation
            }
        }
    }




    public void UnblockAdjacentsToPath(List<LetterTile> usedPath)
    {
        HashSet<LetterTile> adjacents = new HashSet<LetterTile>();
        
        
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        foreach (var tile in usedPath)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = tile.X + dx[i];
                int ny = tile.Y + dy[i];
                if (InBounds(nx, ny))
                {
                    var adjTile = grid[nx, ny];
                    if (adjTile != null && adjTile.Type == TileType.Blocked)
                    {
                        adjacents.Add(adjTile);
                    }
                }
            }
        }
        print($"Unblocking {adjacents.Count} adjacent blocked tiles...");

        foreach (var blockedTile in adjacents)
        {
            blockedTile.Type = TileType.Normal;
            blockedTile.isBlocked = false;
            blockedTile.UnBlockTile();
            blockedTile.UpdateVisual();
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

    public List<int> GetRandomPositionForSpecialTile(int n, int x)
    {

        var result = new HashSet<int>();
        var random = new System.Random();

        while (result.Count < x)
        {
            var num = random.Next(n);
            if (specialTiles.Contains(num)) continue;
            result.Add(num);
        }

        return result.ToList();
    }
}
