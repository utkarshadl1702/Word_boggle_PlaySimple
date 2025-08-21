using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordDragManager : MonoBehaviour
{
    private List<GridCell> selectedCells = new List<GridCell>();
    public String wordFormed = "";
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // while holding mouse / touch
        {
            // Get UI element under pointer
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                GridCell cell = result.gameObject.GetComponent<GridCell>();
                if (cell != null && !cell.isSelected)
                {
                    cell.SelectCell();
                    selectedCells.Add(cell);
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // release finger/mouse
        {
            string finalWord = "";
            foreach (GridCell cell in selectedCells)
            {
                finalWord += cell.letter;
                cell.ResetCell(); // reset visuals
            }

            if (finalWord.Length > 0)
            {
                gameManager.ProcessWord(finalWord);
            }
            
            selectedCells.Clear();
        }
    }
}
