using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script to add to any objects that cause the crosshair change
public class Interactable : MonoBehaviour
{
    private void OnMouseEnter()
    {
        CursorManager.Instance.ChangeCrosshairHover();
    }

    private void OnMouseExit()
    {
        CursorManager.Instance.ChangeCrosshairBasic();
    }
}
