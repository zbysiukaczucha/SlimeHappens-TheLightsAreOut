using System;
using UnityEngine;
using UnityEngine.UI;

namespace Slimeborne
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void Start()
        {
            SetActive(false);
        }

        public void SetHealth(int health)
        {
            if (slider == null)
                return;
            slider.value = health;
        }
        
        public void SetMaxHealth(int maxHealth)
        {
            if (slider == null)
                return;

            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
        
        public void SetActive(bool isActive)
        {
            if (slider == null)
                return;

            slider.gameObject.SetActive(isActive);
        }
        
        private void Update()
        {
            if (slider.value <= 0)
            {
                SetActive(false);
            }
        }
    }
}