using UnityEngine;
using UnityEngine.UI;

public class ScoreDot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image visSprite;
    private Color initialColor;
    void Start()
    {
        visSprite = GetComponent<Image>();
        initialColor = visSprite.color;
    }

    // Update is called once per frame
    public void TurnOnOffSprite(bool shouldTurnOn)
    {
        if (shouldTurnOn)
        {
            visSprite.color = new Color(visSprite.color.r, visSprite.color.g, visSprite.color.b, 1);
        }
        else
        {
            visSprite.color = initialColor;
        }
    }
    void Update()
    {

    }
}
