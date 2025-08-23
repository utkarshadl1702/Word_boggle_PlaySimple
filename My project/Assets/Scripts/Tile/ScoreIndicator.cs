using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreIndicator : MonoBehaviour
{
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {

    }
}
