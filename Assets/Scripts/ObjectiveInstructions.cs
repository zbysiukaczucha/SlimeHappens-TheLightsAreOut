using TMPro;
using UnityEngine;

public class ObjectiveInstructions : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI objectiveInstructions;

    public void SetObjectiveInstructions(string instruction)
    {
        objectiveInstructions.text = instruction;
    }

    public void ToggleObjectiveInstructions(bool enable)
    {
        objectiveInstructions.gameObject.SetActive(enable);
    }
}
