using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightManager : MonoBehaviour
{
    Light directionalLight;
    float intensity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        directionalLight = GetComponent<Light>();
        intensity = PlayerPrefs.GetInt("LastScore", 20)/ 3f;
        SetIntensity(intensity);
    }
    
    private void SetIntensity(float intensity)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = intensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
