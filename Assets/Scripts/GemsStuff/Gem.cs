using ShineHappens;
using Slimeborne;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public enum StoneLevel
{
    Weak,
    Moderate,
    Average,
    Great,
    Strong
}

public class Gem : MonoBehaviour
{
    [SerializeField]
    public Material[] crystalMaterials;
    public Camera cuttingCamera;


    public StoneLevel stoneLevel;  // quality of found stone material
    public int magicLevel;  // max 30 after enchanting
    public bool isCut;
    public bool isEnchanted;
    public bool isCurrentlyCutting;

    PlayerInventory playerInventory;
    TimingMinigameUI timingMinigameUI;

    private void Start()
    {
        isCut = false;
        isEnchanted = false;
        cuttingCamera = GameObject.Find("CuttingCamera").GetComponent<Camera>();
        playerInventory = GameObject.Find("PlayerCharacter").GetComponent<PlayerInventory>();
        timingMinigameUI = GameObject.Find("PlayerUI").GetComponent<TimingMinigameUI>();

        //Set random level of the stone
        stoneLevel = (StoneLevel)Random.Range(0, 5);
        //stoneLevel = StoneLevel.Strong;
        print("Stone level: " + stoneLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCurrentlyCutting)
            {
                goBackFromCuttingMinigame();
                isCurrentlyCutting = false;
                return;
            }
            if (gameObject == playerInventory.activeGem)
            {
                if (!isCut)
                {
                    //cutting minigame
                    goToCuttingMinigame();
                    isCurrentlyCutting = true;
                    return;
                }
                if (!isEnchanted)
                {
                    //enchanting minigame
                    timingMinigameUI.ShowTimingPanel();
                    print("Enchanted the crystal");
                    isEnchanted = true;
                    return;
                }
                Debug.Log("Nothing to do, gem is cut and enchanted");
            }
        }
    }

    public void finishEnchanting(int points)
    {
        print("finishing enchanting");
        magicLevel = points;
        if(stoneLevel == StoneLevel.Strong)
        {
            print("adding point light");
            //Light pointLight = gameObject.AddComponent<Light>();
            Light pointLight = GetComponent<Light>();
            pointLight.intensity = points;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCharacter")
        {
            toInventory(other.transform);
        }
    }

    private void toInventory(Transform other)
    {
        transform.parent = other;
        transform.localPosition = new Vector3(0, 3f, 0);

        //Add gem to players inventory
        other.GetComponent<PlayerInventory>().gems.Add(gameObject);
        gameObject.SetActive(false);
    }

    public void goToCuttingMinigame()
    {
        cuttingCamera.enabled = true;
        Camera.main.enabled = false;
        transform.parent = cuttingCamera.transform;
        transform.position = cuttingCamera.transform.position;
        transform.position += new Vector3(-1, 0, 0);
        this.AddComponent<Rotateable>();
        this.AddComponent<Sliceable>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void goBackFromCuttingMinigame()
    {
        cuttingCamera.enabled = false;
        GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;
        toInventory(playerInventory.transform);
        isCut = true;
    }
}
