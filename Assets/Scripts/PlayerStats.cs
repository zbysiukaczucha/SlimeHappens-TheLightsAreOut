using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Slimeborne
{
    public class PlayerStats : MonoBehaviour
    {
        public int level = 1;
        public int maxHealth;
        public int currentHealth;
        
        public HealthBar healthBar;
        
        AnimatorHandler animatorHandler;
        
        private void Awake()
        {
            animatorHandler = GetComponent<AnimatorHandler>();
        }

        private void Start()
        {
            SetMaxHealthFromLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private void SetMaxHealthFromLevel()
        {
            maxHealth = 100 + level * 10;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            // Play damaged animation
            //animatorHandler.PlayTargetAnimation("Damaged", true);
            
            if (currentHealth < 0)
            {
                currentHealth = 0;
                // Handle player death here
                // animatorHandler.PlayTargetAnimation("Death", true);
                print("Player has died.");
            }
            
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }
}