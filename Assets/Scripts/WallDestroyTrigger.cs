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

    int wallNumber;

    private void Start()
    {
        objectiveText = GameObject.FindWithTag("Objective").GetComponent<TextMeshProUGUI>();
        blackoutImage = GameObject.Find("BlackoutImage").GetComponent<Image>();
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
                    UltimateGameManager.Instance.playerPosition = new Vector3(FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.position.x, FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.position.y, FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.position.z);
                    UltimateGameManager.Instance.playerRotation = new Quaternion(FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.rotation.x, FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.rotation.y, FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.rotation.z, FindFirstObjectByType<Slimeborne.PlayerMovement>().transform.rotation.w);
                    print("Adding " + gameObject.name + " to destroyed walls");
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
