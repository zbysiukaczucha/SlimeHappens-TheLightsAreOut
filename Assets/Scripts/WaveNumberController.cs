using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WaveNumberController : MonoBehaviour
{
    public Image waveImage; // Reference to the Image component of WaveNumber
    public Sprite[] waveSprites; // Array to hold sprites for each wave
    public float fadeDuration = 1.0f; // Duration of the fade animation

    private int currentWave = 0;

    private void Start()
    {
        if (waveImage == null)
        {
            waveImage = GetComponent<Image>();
        }

        if (waveSprites.Length > 0)
        {
            waveImage.sprite = waveSprites[currentWave];
        }
    }

    public void ChangeWave(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < waveSprites.Length)
        {
            currentWave = waveIndex;
            StartCoroutine(FadeToNewSprite(waveSprites[waveIndex]));
        }
        else
        {
            Debug.LogWarning("Invalid wave index.");
        }
    }

    private System.Collections.IEnumerator FadeToNewSprite(Sprite newSprite)
    {
        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }

        // Change sprite
        waveImage.sprite = newSprite;

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = waveImage.color;
        color.a = alpha;
        waveImage.color = color;
    }
}
