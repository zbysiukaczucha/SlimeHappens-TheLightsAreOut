using Slimeborne;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    [SerializeField]
    public GameObject clientInteractionPanel;

    BossAISensor frogScript;
    private bool wasTriggered;

    private void Start()
    {
        wasTriggered = false;

        GameObject[] enemyObj = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemyObj)
        {
            if (obj.TryGetComponent(out BossAISensor script))
            {
                frogScript = script;
            }
        }
        // If no enemy found in the scene
        if(frogScript == null)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (clientInteractionPanel.gameObject.activeSelf)
        {
            return;
        }
        if(wasTriggered)
        {
            return;
        }

        wasTriggered = true;
        clientInteractionPanel.gameObject.SetActive(true);
        clientInteractionPanel.GetComponent<FrogInteraction>().InteractWithClient();
        frogScript.ToggleFrogAttack(false);
    }
}
