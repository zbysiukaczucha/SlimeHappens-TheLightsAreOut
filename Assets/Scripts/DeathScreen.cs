using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    Image deathScreenImage;
    TextMeshProUGUI deathScreenText;

    private void Awake()
    {
        deathScreenImage = GetComponentInChildren<Image>();
        deathScreenText = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }
    
    public void ShowDeathScreen()
    {
        gameObject.SetActive(true);
        AudioManager.PlaySound(SoundType.YouDied);
        StartCoroutine(DeathScreenCoroutine());
    }
    
    IEnumerator DeathScreenCoroutine()
    {
        float duration = 2f; // Duration of the fade-in effect
        float elapsed = 0f;
        
        Color initialColor = new Color(0f, 0f, 0f, 0f);
        Color targetColor = deathScreenImage.color;
        deathScreenImage.color = initialColor;
        
        Color textInitialColor = new Color(deathScreenText.color.r, deathScreenText.color.g, deathScreenText.color.b, 0f);
        Color textTargetColor = deathScreenText.color;
        deathScreenText.color = textInitialColor;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            deathScreenImage.color = Color.Lerp(initialColor, targetColor, t);
            deathScreenText.color = Color.Lerp(textInitialColor, textTargetColor, t);
            yield return null;
        }
        
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before fading out
        
        // Fade out
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            deathScreenImage.color = Color.Lerp(targetColor, initialColor, t);
            deathScreenText.color = Color.Lerp(textTargetColor, textInitialColor, t);
            yield return null;
        }
        StartCoroutine(UltimateGameManager.RestartLevelCoroutine());
    }
}
