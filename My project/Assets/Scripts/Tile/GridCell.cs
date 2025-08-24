using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public string letter;   // The text/letter this cell holds
    [HideInInspector] public bool isSelected = false;
    public Image targetSprite;
    public bool isBlocked = false;

    public void SelectCell()
    {
        if (!isSelected)
        {
            isSelected = true;
            // Optional: change color for feedback
            targetSprite.color = Color.yellow;
        }
    }

    public IEnumerator GlowCoroutine(Color glowColor)
    {
        targetSprite.color = glowColor;
        yield return new WaitForSeconds(0.5f);
        targetSprite.color = Color.white;
       
    }

    public void ResetCell()
    {
        isSelected = false;
        targetSprite.color = Color.white;
    }
}
