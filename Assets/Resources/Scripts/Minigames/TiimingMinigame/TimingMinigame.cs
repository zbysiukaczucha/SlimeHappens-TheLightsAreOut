using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Limits
{
    public float max;
    public float min;
}

public class TimingMinigame : MonoBehaviour
{
    [SerializeField]
    GameObject container, greenArea, yellowArea, redAreaBottom, redAreaTop, orangeArea;

    Limits yLimits;
    Limits greenLimits;
    Limits yellowLimits;
    Limits orangeLimits;
    //Limits redLimits;
    float step = 10;
    float stepChange = 0.1f;
    float center;

    bool goDown = false;
    bool stop = false;

    float deltaTime;

    int points = 0;
    int multiplier = 1;

    int triesCounter = 0;

    GemAnimationScript gemAnimation;

    void Start()
    {
        center = transform.position.y;
        float containerHeight = container.GetComponent<RectTransform>().rect.height;
        yLimits.max = transform.position.y + containerHeight / 2;
        yLimits.min = transform.position.y - containerHeight / 2;
        float greenHeight = greenArea.GetComponent<RectTransform>().rect.height;
        greenLimits.max = transform.position.y + greenHeight / 2;
        greenLimits.min = transform.position.y - greenHeight / 2;
        float yellowHeight = yellowArea.GetComponent<RectTransform>().rect.height;
        yellowLimits.max = transform.position.y + yellowHeight / 2;
        yellowLimits.min = transform.position.y - yellowHeight / 2;
        float orangeHeight = orangeArea.GetComponent<RectTransform>().rect.height;
        orangeLimits.max = transform.position.y + orangeHeight / 2;
        orangeLimits.min = transform.position.y - orangeHeight / 2;

        gemAnimation = GameManager.Instance.currentGem.GetComponent<GemAnimationScript>();
    }

    void Update()
    {
        // 4 times, the max points is 30
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (stop)
            {
                stop = false;
            }
            else
            {
                triesCounter++;
                Color color = Color.white;
                stop = true;
                int newPoints = 0;
                if(transform.position.y < greenLimits.max && transform.position.y > greenLimits.min)
                {
                    newPoints = 2 * multiplier;
                    points += newPoints;
                    multiplier += multiplier;
                    color = Color.green;

                    //print("<color=lime>GREAT :D</color>");
                    gemAnimation.switchAnimation(GemStabilityLevel.Stable);
                    //GameManager.Instance.gemParticles.PlayGemSuddenStable();
                }
                else if(transform.position.y < yellowLimits.max && transform.position.y > yellowLimits.min)
                {
                    newPoints = 1 * multiplier;
                    points += newPoints;
                    multiplier += multiplier;
                    color = new Color(255, 242, 0);

                    //print("<color=#FFF200>Good :)</color>");
                    gemAnimation.switchAnimation(GemStabilityLevel.Wavering);
                }
                else if (transform.position.y < orangeLimits.max && transform.position.y > orangeLimits.min)
                {
                    //print("<color=orange>Well... Could be worse</color>");
                    gemAnimation.switchAnimation(GemStabilityLevel.Disrupted);
                }
                else
                {
                    if(multiplier >= 2) 
                        multiplier -= multiplier / 2;

                    //print("<color=red>Ouch :(</color>");
                    gemAnimation.switchAnimation(GemStabilityLevel.Unstable);
                    //GameManager.Instance.gemParticles.PlayGemSuddenStable();
                }

                if(newPoints != 0)
                {
                    GameManager.Instance.timingMinigameUI.setAddedPointsText(newPoints, color);
                    GameManager.Instance.timingMinigameUI.setScoreText(points);
                }
                if(triesCounter == 4)
                {
                    GameManager.Instance.timingMinigameUI.HideTimingPanel();
                    print($"<color=lime>You scored {points}/30</color>");
                    resetMinigame();
                }
            }

        }

        deltaTime = Time.deltaTime * 100;

        if (stop)
        {
            return;
        }

        if (goDown)
        {
            if (transform.position.y > center)
            {
                step += stepChange * deltaTime;
            }
            else
            {
                step -= stepChange * deltaTime;
            }

            if (transform.position.y > yLimits.min)
            {
                transform.position -= new Vector3(0, step) * deltaTime;
            }
            else
            {
                goDown = false;
            }
        }
        else
        {
            if (transform.position.y < center)
            {
                step += stepChange * deltaTime;
            }
            else
            {
                step -= stepChange * deltaTime;
            }

            if (transform.position.y < yLimits.max)
            {
                transform.position += new Vector3(0, step) * deltaTime;
            }
            else
            {
                goDown = true;
            }

        }
    }

    void resetMinigame()
    {
        points = 0;
        multiplier = 1;
        triesCounter = 0;
    }

}
