using UnityEngine;
using UnityEngine.UI;

public class ScoreDot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image visSprite;
    private Color initialColor;
    void Awake()
    {
        visSprite = GetComponent<Image>();
        initialColor = visSprite.color;
    }

    // Update is called once per frame
    public void TurnOnOffSprite(bool shouldTurnOn)
    {
        if (shouldTurnOn)
        {
            //increase alpha to 1
            print(shouldTurnOn + "turn on sprite");
            visSprite.color = new Color(visSprite.color.r, visSprite.color.g, visSprite.color.b, 1f);
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
