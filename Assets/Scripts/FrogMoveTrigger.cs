using UnityEngine;

public class FrogMoveTrigger : MonoBehaviour
{
    BossAISensor frogScript;
    private bool wasTriggered;

    private void Start()
    {
        wasTriggered = false;
        tryGetFrogScript();
    }

    bool tryGetFrogScript()
    {
        GameObject[] enemyObj = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemyObj)
        {
            if (obj.TryGetComponent(out BossAISensor script))
            {
                frogScript = script;
                return true;
            }
        }
        // If no enemy found in the scene
        if (frogScript == null)
        {
            gameObject.SetActive(false);
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wasTriggered)
        {
            return;
        }
        if (tryGetFrogScript())
        {
            wasTriggered = true;
            frogScript.ToggleStopFrog(false);
        }
    }
}
