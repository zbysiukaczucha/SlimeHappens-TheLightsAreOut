using Slimeborne;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace ShineHappens
{
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

        PlayerInventory playerInventory;

        private void Start()
        {
            expPoints = UltimateGameManager.Instance.experience;
            experienceText.text = expPoints.ToString();
            addedPointsObject = addedPointsText.gameObject;
            addedPointsObject.SetActive(false);
            addedPointsAnimator = addedPointsObject.GetComponent<Animator>();
            timingMinigamePanel.SetActive(false);
            playerInventory = GameObject.Find("PlayerCharacter").GetComponent<PlayerInventory>();
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

            // Add experience points based on enchanting score * the level of stone quality (1-5)
            //expPoints = expPoints + addedPoints * (1+(int)playerInventory.activeGem.GetComponent<Gem>().stoneLevel);
            expPoints = expPoints + addedPoints;

            experienceText.text = expPoints.ToString();
            UltimateGameManager.Instance.experience = expPoints;
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
}