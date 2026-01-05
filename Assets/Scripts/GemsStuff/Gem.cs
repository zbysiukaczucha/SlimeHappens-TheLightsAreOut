using ShineHappens;
using Slimeborne;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public bool isEnchanted;
    public int powerLevel;

    PlayerInventory playerInventory;
    TimingMinigameUI timingMinigameUI;

    private void Start()
    {
        isEnchanted = false;
        playerInventory = GameObject.Find("PlayerCharacter").GetComponent<PlayerInventory>();
        timingMinigameUI = GameObject.Find("Canvas").GetComponent<TimingMinigameUI>();
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
                    print("Enchanted the crystal");
                    isEnchanted = true;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCharacter")
        {
            transform.parent = other.transform;
            transform.localPosition = new Vector3(0, 3f, 0);
            other.GetComponent<PlayerInventory>().gems.Add(gameObject);
            gameObject.SetActive(false);
        }
    }
}
