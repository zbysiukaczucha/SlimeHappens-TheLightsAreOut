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
        
        public float maxStamina;
        public float currentStamina;
        public float staminaRegenRate = 5;
        
        public HealthBar healthBar;
        public StaminaBar staminaBar;
        
        AnimatorHandler animatorHandler;
        PlayerManager playerManager;
        
        private void Awake()
        {
            animatorHandler = GetComponent<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            healthBar = FindFirstObjectByType<HealthBar>();
            staminaBar = FindFirstObjectByType<StaminaBar>();
        }

        private void Start()
        {
            SetMaxHealthFromLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            
            SetMaxStaminaFromLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(Mathf.RoundToInt(maxStamina));
        }

        private void SetMaxHealthFromLevel()
        {
            maxHealth = 100 + level * 10;
        }
        
        private void SetMaxStaminaFromLevel()
        {
            maxStamina = 100 + level * 10;
        }
        
        public void TakeDamage(int damage)
        {
            if(playerManager.isInvulnerable)
                return;
            
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            // Play damaged animation
            //animatorHandler.PlayTargetAnimation("Damaged", true);
            
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                // Handle player death here
                // animatorHandler.PlayTargetAnimation("Death", true);
                print("Player has died.");
            }
            
        }
        
        public void TakeStaminaDamage(int damage)
        {
            currentStamina -= damage;
            staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
            if (currentStamina < 0)
                currentStamina = 0;
        }
        
        public void RegenerateStamina()
        {
            if(playerManager.isInteracting == false && playerManager.isSprinting == false && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;
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