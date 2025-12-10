using System;
using UnityEngine;
using UnityEngine.UI;

namespace Slimeborne
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        private Slider slider;
        float timeUntilHide = 0f;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        public void SetHealth(int health)
        {
            if (slider == null)
                return;
            slider.value = health;
            timeUntilHide = 3f;
        }
        
        public void SetMaxHealth(int maxHealth)
        {
            if (slider == null)
                return;

            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
        
        private void Update()
        {
            if (slider == null)
                return;
            
            timeUntilHide -= Time.deltaTime;

            if (timeUntilHide <= 0f)
            {
                timeUntilHide = 0f;
                slider.gameObject.SetActive(false);
            }
            else if(slider.gameObject.activeInHierarchy == false)
                slider.gameObject.SetActive(true);
            
            if (slider.value <= 0)
            {
                Destroy(slider.gameObject);
            }
        }
    }
}