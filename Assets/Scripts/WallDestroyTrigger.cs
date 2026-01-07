using Slimeborne;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WallDestroyTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject stoneWall;
    Image blackoutImage;
    float alpha = 0;

    
    TextMeshProUGUI objectiveText;

    string prevObjective;
    bool enteredTrigger;

    private void Start()
    {
        objectiveText = GameObject.FindWithTag("Objective").GetComponent<TextMeshProUGUI>();
        blackoutImage = GameObject.Find("BlackoutImage").GetComponent<Image>();
        enteredTrigger = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enteredTrigger)
            {
                if (UltimateGameManager.Instance.enableWallBreak)
                {
                    // Break the wall
                    stoneWall.SetActive(false);
                    StartCoroutine(ChangeScenes());
                }
            }
        }
    }

    IEnumerator ChangeScenes()
    {
        while (true)
        {
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.05f);
            alpha += 0.05f;
            if(alpha >= 1)
            {
                blackoutImage.color = new Color(0, 0, 0, alpha);
                break;
            }
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Game");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCharacter")
        {
            prevObjective = objectiveText.text;
            objectiveText.text = "Destroy wall (with the help of gem's magic) *press SPACE*";
            enteredTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "PlayerCharacter")
        {
            objectiveText.text = prevObjective;
            enteredTrigger = false;
        }
    }
}
