using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotateable : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private Vector3 lastMousePos;
    private Transform cam;

    private void Start()
    {
        cam = GameObject.Find("CuttingCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation by dragging
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            // Camera-relative axes
            Vector3 camRight = cam.right;
            Vector3 camUp = cam.up;

            // Apply rotation relative to camera
            // Rotate around world space, not local
            transform.Rotate(camUp, -delta.x * rotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(camRight, delta.y * rotationSpeed * Time.deltaTime, Space.World);
        }

        //Rotation by keys
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(cam.forward, rotationSpeed * Time.deltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(cam.transform.forward, -rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
