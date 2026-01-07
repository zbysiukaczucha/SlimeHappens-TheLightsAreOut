using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.Controls;

public class LightManager : MonoBehaviour
{
    Light directionalLight;
    float intensity;
    UltimateGameManager ultimateGameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        directionalLight = GetComponent<Light>();
        ultimateGameManager = FindFirstObjectByType<UltimateGameManager>();
        int score = ultimateGameManager != null ? ultimateGameManager.score : UltimateGameManager.defaultScore;
        intensity = score/ 2f;
        if (intensity < 10f)
        {
            intensity = 10f;
        }
        else if (intensity > 40f)
        {
            intensity = 40f;
        }
        SetIntensity(intensity);
    }
    
    private void SetIntensity(float intensity)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = intensity;
        }
    }
    
    public void TemporaryIncreaseIntensity(float amount, float duration)
    {
        if (directionalLight != null)
        {
            StartCoroutine(IncreaseIntensityCoroutine(amount, duration));
        }
    }
    
    private IEnumerator IncreaseIntensityCoroutine(float amount, float duration)
    {
        directionalLight.intensity += amount;
        yield return new WaitForSeconds(duration);
        directionalLight.intensity -= amount;
    }
}
