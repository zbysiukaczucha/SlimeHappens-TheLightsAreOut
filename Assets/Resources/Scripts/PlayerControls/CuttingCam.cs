using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCam : MonoBehaviour
{
    bool lastState;

    private void Start()
    {
        lastState = true;
        GetComponent<Camera>().enabled = false;
    }


    private void Update()
    {
        if (lastState != GetComponent<Camera>().enabled)
        {
            Debug.Log("cutcam laststate=" + lastState + " enabled=" + GetComponent<Camera>().enabled);
            SetChildrenActive(GetComponent<Camera>().enabled);
            lastState = GetComponent<Camera>().enabled;
        }
    }

    private void SetChildrenActive(bool b)
    {
        foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
        {
            r.enabled = b;
        }
    }
}
