using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnailControlInstructions : MonoBehaviour
{
    [SerializeField]
    GameObject instructionsPanel;

    private void Start()
    {
        instructionsPanel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(instructionsPanel.activeSelf)
            {
                instructionsPanel.SetActive(false);
            }
            else
            {
                instructionsPanel.SetActive(true);
            }
        }
    }

}
