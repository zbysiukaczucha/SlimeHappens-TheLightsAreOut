using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimingMinigameUI : MonoBehaviour
{
    [SerializeField]
    GameObject timingBar;

    [SerializeField]
    TextMeshProUGUI addedPointsText;
    GameObject addedPointsObject;
    Animator addedPointsAnimator;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    GameObject timingMinigamePanel;

    [SerializeField]
    TextMeshProUGUI experienceText;
    int expPoints = 0;

    private void Start()
    {
        addedPointsObject = addedPointsText.gameObject;
        addedPointsObject.SetActive(false);
        addedPointsAnimator = addedPointsObject.GetComponent<Animator>();
        timingMinigamePanel.SetActive(false);
    }


    public void ShowTimingPanel()
    {
        timingMinigamePanel.SetActive(true);
        scoreText.text = "0";
    }
    public void HideTimingPanel()
    {
        timingMinigamePanel.SetActive(false);
    }

    public void setScoreText(int points)
    {
        scoreText.text = points.ToString();
    }

    public void setAddedPointsText(int addedPoints, Color color)
    {
        addedPointsObject.SetActive(true);
        addedPointsText.color = color;
        addedPointsText.text = $"+" + addedPoints;
        expPoints = expPoints + addedPoints;
        experienceText.text = expPoints.ToString();
        //print("Playing animation");
        addedPointsAnimator.Play("Base Layer.AddedPoints");
        StartCoroutine(waitForAnimEnd(addedPointsAnimator));
    }


    IEnumerator waitForAnimEnd(Animator animator)
    {
        AnimatorStateInfo stateInfo;
        //print("Waiting for animation to end...");
        while (true)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("End"))
            {
                break;
            }
        }
        addedPointsObject.SetActive(false);
        //print("Animation finished");
    }

}
