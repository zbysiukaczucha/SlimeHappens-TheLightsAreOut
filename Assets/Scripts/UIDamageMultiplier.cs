using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Slimeborne
{
    public class UIDamageMultiplier : MonoBehaviour
    {
        PlayerStats playerStats;
        Image icon;
        public Texture2D buffIcon;
        public Texture2D debuffIcon;
        TextMeshProUGUI multiplierText;
        

        private void Awake()
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            icon = GetComponentInChildren<Image>();
            multiplierText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            float multiplier = playerStats.damageMultiplier;
            multiplierText.text = "x" + multiplier.ToString("F2");
            if (multiplier > 1f)
            {
                icon.sprite = Sprite.Create(buffIcon, new Rect(0, 0, buffIcon.width, buffIcon.height), new Vector2(0.5f, 0.5f));
                icon.color = Color.darkGreen;
            }
            else if (multiplier < 1f)
            {
                icon.sprite = Sprite.Create(debuffIcon, new Rect(0, 0, debuffIcon.width, debuffIcon.height),
                    new Vector2(0.5f, 0.5f));
                icon.color = Color.darkRed;
            }
            else 
            {
                icon.enabled = false;
                multiplierText.enabled = false;
            }
            
            Debug.Log("Damage Multiplier: " + multiplier);
        }
    }
}