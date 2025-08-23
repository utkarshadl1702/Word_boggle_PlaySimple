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
    [Tooltip("If empty, random A–Z. Optionally use frequency-biased letters.")]
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
                    d.tileType = 2; // Make it a bug tile if it's not blocked or bonus already
                tile.Init(x, y, d.letter[0], (TileType)d.tileType);
                
                grid[x, y] = tile;
            }
    }

    public bool AreAdjacent(LetterTile a, LetterTile b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        return (dx <= 1 && dy <= 1 && !(dx == 0 && dy == 0));
    }

    public void RemoveAndRefill(List<LetterTile> tilesToRemove)
    {
        print("Removing and refilling tiles...");
        // Step 1: Mark tiles for removal
        foreach (var tile in tilesToRemove)
        {
            if (grid[tile.X, tile.Y] == tile)
            {
                Destroy(tile.gameObject);
                grid[tile.X, tile.Y] = null;
            }
        }

        // Step 2: Process each column independently
        for (int x = 0; x < width; x++)
        {
            // Find empty spaces and shift tiles down
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    // Find the next non-null tile above
                    for (int above = y + 1; above < height; above++)
                    {
                        if (grid[x, above] != null)
                        {
                            // Move tile down
                            var tile = grid[x, above];
                            grid[x, above] = null;
                            grid[x, y] = tile;

                            // Update tile position
                            tile.Y = y;
                            tile.transform.localPosition = new Vector3(
                                tile.transform.localPosition.x,
                                -y * gridLayout.cellSize.y,
                                tile.transform.localPosition.z
                            );
                            break;
                        }
                    }
                }
            }

            // Fill empty spaces at the top with new tiles
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    var newTile = Instantiate(cellPrefab, transform).GetComponent<LetterTile>();
                    newTile.Init(x, y, GetRandomLetter(), TileType.Normal);
                    grid[x, y] = newTile;
                    newTile.transform.localPosition = new Vector3(
                        newTile.transform.localPosition.x,
                        -y * gridLayout.cellSize.y,
                        newTile.transform.localPosition.z
                    );
                }
            }
        }
    }



    public void UnblockAdjacentsToPath(List<LetterTile> usedPath)
    {
        HashSet<(int, int)> adj = new();
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

    public List<int> GetRandomPositionForSpecialTile(int n, int x)
    {

        //HashSet to make keep numbers unique
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
