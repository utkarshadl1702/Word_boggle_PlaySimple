using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    List<ScoreDot> scoreDots = new List<ScoreDot>();

    void Start()
    {
        scoreDots = this.GetComponentsInChildren<ScoreDot>().ToList();
    }

    public void IndicateScoreDots(int scoreInt)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
