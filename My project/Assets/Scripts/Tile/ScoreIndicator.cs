using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreIndicator : MonoBehaviour
{

    [SerializeField] private List<ScoreDot> scoreDots = new List<ScoreDot>();

    void Start()
    {
        scoreDots = this.GetComponentsInChildren<ScoreDot>().ToList();
    }

    public void IndicateScoreDots(int scoreInt)
    {
        print("Score dots to indicate: " + scoreDots.Count);
        for (int i = 0; i < scoreInt; i++)
        {
            scoreDots[i].TurnOnOffSprite(true);
        }

    }

   
}
