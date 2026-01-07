using ShineHappens;
using Slimeborne;
using TMPro;
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

    public StoneLevel stoneLevel;  // quality of found stone material
    public int magicLevel;  // max 30 after enchanting
    public bool isEnchanted;

    PlayerInventory playerInventory;
    TimingMinigameUI timingMinigameUI;

    TextMeshProUGUI objectiveText;

    private void Start()
    {
        isEnchanted = false;
        playerInventory = GameObject.Find("PlayerCharacter").GetComponent<PlayerInventory>();
        timingMinigameUI = GameObject.Find("PlayerUI").GetComponent<TimingMinigameUI>();
        objectiveText = GameObject.FindWithTag("Objective").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (gameObject == playerInventory.activeGem)
            {
                if (isEnchanted)
                {
                    print("Nope - Crystal is already enchanted!");
                }
                else
                {
                    timingMinigameUI.ShowTimingPanel();
                    print("Enchanting the crystal");
                    isEnchanted = true;
                }
            }
        }
    }

    public void finishEnchanting(int points)
    {
        print("finishing enchanting");
        magicLevel = points;
        /*if(stoneLevel == StoneLevel.Strong)
        {
            print("adding point light");
            //Light pointLight = gameObject.AddComponent<Light>();
            Light pointLight = GetComponent<Light>();
            pointLight.intensity = points;
        }*/
        objectiveText.text = "Go further";
        UltimateGameManager.Instance.enableWallBreak = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCharacter")
        {
            transform.parent = other.transform;
            transform.localPosition = new Vector3(0, 3f, 0);

            //Set random level of the stone
            stoneLevel = (StoneLevel)Random.Range(0, 5);
            //stoneLevel = StoneLevel.Strong;
            print("Stone level: " + stoneLevel);

            //Add gem to players inventory
            other.GetComponent<PlayerInventory>().gems.Add(gameObject);
            gameObject.SetActive(false);

            objectiveText.text = "Cut the gemstone";
        }
    }
}
