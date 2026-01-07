using Slimeborne;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallDestroyTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject stoneWall;

    TextMeshProUGUI objectiveText;

    string prevObjective;
    bool enteredTrigger;

    int wallNumber;

    private void Start()
    {
        objectiveText = GameObject.FindWithTag("Objective").GetComponent<TextMeshProUGUI>();
        enteredTrigger = false;
        GameObject[] walls = GameObject.Find("WorldEventManager").GetComponent<WorldEventManager>().wallsToBeDestroyed;
        for(int i = 0; i < walls.Length; i++)
        {
            if (walls[i] == gameObject)
            {
                wallNumber = i;
            }
        }
        if( UltimateGameManager.Instance.destroyedWallNumbers.Contains(wallNumber))
        {
            gameObject.SetActive(false);
            print(gameObject.name + " is in destroyed walls");
        }

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
                    UltimateGameManager.Instance.destroyedWallNumbers.Add(wallNumber);
                    print("Adding " + gameObject.name + " to destroyed walls");
                    StartCoroutine(ChangeScenes());
                }
            }
        }
    }

    IEnumerator ChangeScenes()
    {
        yield return new WaitForSeconds(1);
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
