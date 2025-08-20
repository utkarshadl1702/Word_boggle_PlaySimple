using TMPro;
using UnityEngine;

public enum TileType { Normal = 0, Blocked = 1, Bonus = 2 }

public class LetterTile : MonoBehaviour
{
    // [SerializeField] private SpriteRenderer sr;
    // [SerializeField] private g normalSprite;
    [SerializeField] private GameObject bugSprite;
    [SerializeField] private GameObject blockedSprite;
    [SerializeField] private char letter = 'A';
    [SerializeField] private TileType tileType = TileType.Normal;
    [SerializeField] private TMP_Text LetterText;

    // Grid coordinates managed by GridManager
    public int X { get; set; }
    public int Y { get; set; }

    public char Letter
    {
        get => letter;
        set { letter = char.ToUpper(value); name = $"Tile_{letter}_{X}_{Y}"; }
    }

    public TileType Type
    {
        get => tileType;
        set { tileType = value; UpdateVisual(); }
    }

    public bool IsSelectable => tileType != TileType.Blocked;

    public void Init(int x, int y, char c, TileType type = TileType.Normal)
    {
        X = x; Y = y; Letter = c; Type = type; LetterText.text = Letter.ToString();

        UpdateVisual();
    }

    public void UpdateVisual()
    {

        // You can swap sprite via your provided prefab states (normal/blocked/bonus)
        // Here we just tint for clarity in a quick prototype:
        switch (tileType)
        {
            case TileType.Normal: break;
            case TileType.Blocked: blockedSprite.SetActive(true); break;
            case TileType.Bonus: bugSprite.SetActive(true); break;
        }
        // Optional: show letter via TextMeshPro child if your prefab has it.
    }
}
