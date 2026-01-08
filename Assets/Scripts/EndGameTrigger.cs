using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCharacter")
        {
            UltimateGameManager.Instance.isLastGem = true;
        }
    }
}
