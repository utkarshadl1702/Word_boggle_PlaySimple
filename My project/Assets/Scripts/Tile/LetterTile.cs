using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private ScoreIndicator scoreDots;
    public bool isBugTile = false;
    public int tileScore = 1;




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
        GetComponent<GridCell>().letter = Letter.ToString();
        UpdateVisual();
        
        scoreDots.IndicateScoreDots(tileScore);
    }

    public void UpdateVisual()
    {

        switch (tileType)
        {
            case TileType.Normal: break;
            case TileType.Blocked: blockedSprite.SetActive(true); break;
            case TileType.Bonus: tileScore = 2; isBugTile = true; bugSprite.SetActive(true); break;
        }
       
    }



    public void ResetCell()
    {
        // Reset visual elements
        if (blockedSprite) blockedSprite.SetActive(false);
        if (bugSprite) bugSprite.SetActive(false);

        // Reset type to normal
        tileType = TileType.Normal;

        // Reset letter text
        if (LetterText) LetterText.text = "";

    }
}
