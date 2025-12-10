using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for a color-adjusting minigame
public class ColorMinigame : MonoBehaviour
{
    [SerializeField]
    GameObject machineRunes;

    [SerializeField]
    GameObject colorScreen;

    [SerializeField]
    GameObject referenceColorScreen;

    Material colorScreenMaterial;
    Material referenceColorScreenMaterial;
    Material machineRunesMaterial;

    AudioSource audioSource;

    bool turningKnob;

    Vector2 mouseStartPos;
    Vector2 centerPos;

    float previousAngle;
    float angle;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 0.9f;
        colorScreenMaterial = colorScreen.GetComponent<Renderer>().material;
        machineRunesMaterial = machineRunes.GetComponent<Renderer>().material;

        referenceColorScreenMaterial = referenceColorScreen.GetComponent<Renderer>().material;

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, previousAngle);
        turningKnob = false;
        previousAngle = 0;
    }

    float lastAcceptedAngle = 0;

    void Update()
    {
        if (!GameManager.Instance.lockPlayer)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(Input.mousePosition);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    string name = hit.collider.gameObject.name;
                    //Debug.Log("Hit " + name);

                    if (hit.collider.gameObject.CompareTag("Knob"))
                    {
                        mouseStartPos = Input.mousePosition;
                        turningKnob = true;

                        // Get center position (on the screen) of the knob
                        centerPos = Camera.main.WorldToScreenPoint(transform.position);
                    }
                }
            }
        }

        if (turningKnob)
        {
            angle = previousAngle + Vector2.SignedAngle(mouseStartPos - centerPos, (Vector2)Input.mousePosition - centerPos);

            // For surpassing the SignedAngle range limits of -180 to 180
            if(angle > 0)
            {
                //angle = -180 - (180 - angle);
                angle = -360 + angle;
            }
            if(angle < -270)
            {
                //angle = 180 + (angle + 180);
                angle = 360 + angle;
            }

            if (angle < 0 && angle > -270)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);
                lastAcceptedAngle = angle;

                float normalizedAngle = angle / -270;
                SetFrequencyColor(normalizedAngle);
                audioSource.pitch = 0.9f + normalizedAngle/3;
            }
        }

        // Save the last position
        if (Input.GetMouseButtonUp(0))
        {
            if (turningKnob)
            {
                turningKnob = false;
                previousAngle = lastAcceptedAngle;

                calculatePoints();
            }
        }

    }

    void SetFrequencyColor(float hue)
    {
        Color c = Color.HSVToRGB(hue, 1f, 1f);
        colorScreenMaterial.SetColor("_Color", c);
        machineRunesMaterial.SetColor("_Color", c);
        machineRunesMaterial.SetColor("_EmissionColor", c);
        //machineRunesMaterial.EnableKeyword("_EMISSION");
    }

    float GetHue(Material material)
    {
        Color.RGBToHSV(material.color, out float h, out _, out _);
        return h;
    }

    void calculatePoints()
    {
        float refHue = GetHue(referenceColorScreenMaterial);
        float hue = GetHue(colorScreenMaterial);
        print("Mathf.Abs(hue - refHue) * 80   =   " + Mathf.Abs(hue - refHue) * 80);
        int points = 10 - (int)Mathf.Round(Mathf.Abs(hue - refHue) * 80);
        if (points < 0) points = 0;

        print("Points = " +  points);
        GameManager.Instance.knobPoints = points;
    }



}
