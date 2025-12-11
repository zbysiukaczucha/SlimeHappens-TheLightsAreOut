using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class Upgrades : MonoBehaviour
{
    private TextMeshProUGUI title_1;
    private TextMeshProUGUI title_2;
    private TextMeshProUGUI title_3;
    private TextMeshProUGUI description_1;
    private TextMeshProUGUI description_2;
    private TextMeshProUGUI description_3;
    
    private Button button_1;
    private Button button_2;
    private Button button_3;

    [SerializeField] private Sprite luckImage;
    [SerializeField] private Sprite attackSpeedImage;
    [SerializeField] private Sprite movementSpeedImage;
    [SerializeField] private Sprite dashCooldownImage;
    [SerializeField] private Sprite attackDamageImage;
    [SerializeField] private Sprite maximumHealthImage;
    [SerializeField] private Sprite healAmountImage;
    [SerializeField] private Sprite HealUpImage;
    
    private GameObject PowerUpChoiceScreen;

    private GameManager gameManager;
    private GameObject player;
    private PlayerCombat playerCombat;
    private PlayerMovementLO playerMovement;
     
    private int luckThreshold;
    private int increase_1;
    private int increase_2;
    private int increase_3;
    private bool canIncreaseLuck = true;
    private float attackSpeed = 0;
    private float movementSpeed = 0;
    //private int dashCooldown = 0;
    
    private Animator anim;


    void Awake()
    {
        PowerUpChoiceScreen = FindInactiveObjectByName("PowerUpChoiceScreen");
        anim = GameObject.Find("Canvas").GetComponent<Animator>();
        title_1 = FindInactiveObjectByName("Title_1").GetComponent<TextMeshProUGUI>();
        title_2 = FindInactiveObjectByName("Title_2").GetComponent<TextMeshProUGUI>();
        title_3 = FindInactiveObjectByName("Title_3").GetComponent<TextMeshProUGUI>();
        description_1 = FindInactiveObjectByName("Description_1").GetComponent<TextMeshProUGUI>();
        description_2 = FindInactiveObjectByName("Description_2").GetComponent<TextMeshProUGUI>();
        description_3 = FindInactiveObjectByName("Description_3").GetComponent<TextMeshProUGUI>();
        button_1 = FindInactiveObjectByName("Power 1 Button").GetComponent<Button>();
        button_2 = FindInactiveObjectByName("Power 2 Button").GetComponent<Button>();
        button_3 = FindInactiveObjectByName("Power 3 Button").GetComponent<Button>();
        

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        playerCombat = player.GetComponent<PlayerCombat>();
        playerMovement = player.GetComponent<PlayerMovementLO>();
        luckThreshold = 60;
    }

    



    // UPGRADES LIST
    Upgrade[] _Upgrades = new Upgrade[]
    {
        new() {Name = "Luck", Description="Increases your chance to get better upgrades by ><", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 5, 10, 15 }},
        new() {Name = "Attack Speed", Description="Increases your attack speed by ><%", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 10, 20, 30 }},
        new() {Name = "Movement Speed", Description="Increases your movement speed by ><%", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 10, 20, 30 }},
        new() {Name = "Dash Cooldown", Description="Decreases your dash cooldown by ><%", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 10, 20, 30 }},
        new() {Name = "Attack Damage", Description="Increases your attack damage by ><", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 10, 20, 30 }},
        new() {Name = "Maximum Health", Description="Increases your Maximum Health by ><", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 25, 50, 75 }},
        new() {Name = "Heal Power", Description="Increases healing you receive by ><", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 15, 30, 45 }},
        new() {Name = "HealUp Chance", Description="Increases chance for enemies to drop HealUp by ><%", Rarity = new List<int> { 1, 2, 3 }, Increase = new List<int> { 3, 6, 9 }}
    };
    

    public class Upgrade
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public List<int> Rarity { get; set; }
        
        public List<int> Increase { get; set; }
        
        public Sprite Img {get; set;}
    }
    

    public void ShuffleList(List<int> list, List<Sprite> list2)
    {
        for(int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            
            // SWAP ITEMS
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
            (list2[randomIndex], list2[i]) = (list2[i], list2[randomIndex]);
        }
    }
    

    public static Sprite LoadImage(Sprite image)
    {
        return image;
    }
    

    public int DetermineRarity()
    {
        int rand = Random.Range(0, 100);

        if (rand < luckThreshold)
            return 0;

        else if (rand < luckThreshold + 30)
            return 1;

        return 2;
    }


    public void CreateChoice()
    {
        List<int> availableUpgrades = new();
        List<Sprite> spriteList = new()
        {
            luckImage, attackSpeedImage, movementSpeedImage, dashCooldownImage, attackDamageImage, maximumHealthImage, healAmountImage, HealUpImage
        };

        int start = 0;

        if(!canIncreaseLuck)
            start = 1; 
        
        for (int i = start; i < _Upgrades.Length; i++)
            availableUpgrades.Add(i);
        
        ShuffleList(availableUpgrades, spriteList);
        Upgrade upgrade_1 = _Upgrades[availableUpgrades[0]];
        Upgrade upgrade_2 = _Upgrades[availableUpgrades[1]];
        Upgrade upgrade_3 = _Upgrades[availableUpgrades[2]];
        
        title_1.text = upgrade_1.Name;
        title_2.text = upgrade_2.Name;
        title_3.text = upgrade_3.Name;
        
        button_1.image.sprite = spriteList[0];
        button_2.image.sprite = spriteList[1];
        button_3.image.sprite = spriteList[2];


        increase_1 = DetermineRarity();
        increase_2 = DetermineRarity();
        increase_3 = DetermineRarity();
        description_1.text = upgrade_1.Description.Replace("><",upgrade_1.Increase[increase_1].ToString());

        description_2.text = upgrade_2.Description.Replace("><",upgrade_2.Increase[increase_2].ToString());
        description_3.text = upgrade_3.Description.Replace("><",upgrade_3.Increase[increase_3].ToString());
    }
    
    public void UpgradeChosen(string name, int choice)
    {
        List<int> increases = new() {increase_1, increase_2, increase_3};

        int increase = increases[choice];

        List<float> temp;

        switch(name)
        {
            case "Luck":
                temp = new List<float> { 5, 10, 15 };
                luckThreshold -= (int)temp[increase];
                if(luckThreshold < 30)
                    canIncreaseLuck = false;
                break;
            
            case "Attack Speed":
                temp = new List<float> { 10, 20, 30 };
                attackSpeed += temp[increase];
                playerCombat.attackRate *= (100 + temp[increase]) / 100;
                break;

            case "Movement Speed":
                temp = new List<float> { 10, 20, 30 };
                movementSpeed += temp[increase];
                playerMovement.speed = playerMovement.baseSpeed * (100 + movementSpeed) / 100;
                break;

            case "Dash Cooldown":
                temp = new List<float> { 10, 20, 30 };
                playerMovement.dashCooldown *= (100 - temp[increase]) / 100;
                break;
            
            case "Attack Damage":
                temp = new List<float> { 10, 20, 30 };
                playerCombat.attackDamage += (int)temp[increase];
                break;
            
            case "Maximum Health":
                temp = new List<float> { 25, 50, 75 };
                playerCombat.maxHealth += (int)temp[increase];
                playerCombat.HealPlayer((int)temp[increase]);
                break;

            case "Heal Power":
                temp = new List<float> { 10, 20, 30 };
                gameManager.healAmount += (int)temp[increase];
                break;
                
            case "HealUp Chance":
                temp = new List<float> { 3, 6, 9 };
                gameManager.healUpChance += (int)temp[increase];
                break;
        }
    }
    
    public void Choice_1()
    {
        UpgradeChosen(PowerUpChoiceScreen.GetComponentsInChildren<Button>()[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text, 0);
        // PowerUpChoiceScreen.SetActive(false);
        StartCoroutine(ResumeTime());
    }
    
    public void Choice_2()
    {
        UpgradeChosen(PowerUpChoiceScreen.GetComponentsInChildren<Button>()[1].GetComponentsInChildren<TextMeshProUGUI>()[0].text, 1);
        // PowerUpChoiceScreen.SetActive(false);
        StartCoroutine(ResumeTime());
    }
    
    public void Choice_3()
    {
        UpgradeChosen(PowerUpChoiceScreen.GetComponentsInChildren<Button>()[2].GetComponentsInChildren<TextMeshProUGUI>()[0].text, 2);
        // PowerUpChoiceScreen.SetActive(false);
        StartCoroutine(ResumeTime());
    }
    
    
    GameObject FindInactiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None && objs[i].name == name)
                return objs[i].gameObject;
        }
        
        return null;
    }
    


    // IENUMERATORS
    
    IEnumerator ResumeTime()
    {
        anim.Rebind();
        anim.Update(0f);
        anim.enabled = true;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        anim.enabled = false;
    }
}

