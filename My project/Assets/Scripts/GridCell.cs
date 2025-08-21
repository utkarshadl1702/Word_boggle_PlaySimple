using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public string letter;   // The text/letter this cell holds
    [HideInInspector] public bool isSelected = false;
    public Image targetSprite;

    public void SelectCell()
    {
        if (!isSelected)
        {
            isSelected = true;
            // Optional: change color for feedback
            targetSprite.color = Color.yellow;
        }
    }

    public void ResetCell()
    {
        isSelected = false;
        targetSprite.color = Color.white;
    }
}
