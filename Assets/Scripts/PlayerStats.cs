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
        
        public int ultimateMeter = 0;
        public const int maxUltimateMeter = 10;
        
        public float damageMultiplier = 1f;
        
        public HealthBar healthBar;
        public StaminaBar staminaBar;
        public UltimateBar ultimateBar;
        
        AnimatorHandler animatorHandler;
        PlayerManager playerManager;
        InputHandler inputHandler;
        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            inputHandler = GetComponent<InputHandler>();
            healthBar = FindFirstObjectByType<HealthBar>();
            staminaBar = FindFirstObjectByType<StaminaBar>();
            ultimateBar = FindFirstObjectByType<UltimateBar>();
            //PlayerPrefs.SetInt("LastScore", 45); // For testing purposes
            damageMultiplier = PlayerPrefs.GetInt("LastScore", 30) / 30f;
            if (damageMultiplier < 0.5f)
                damageMultiplier = 0.5f;
        }

        private void Start()
        {
            SetMaxHealthFromLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            
            SetMaxStaminaFromLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(Mathf.RoundToInt(maxStamina));
            
            ultimateBar.SetMaxUltMeter(maxUltimateMeter);
            ultimateBar.SetCurrentUltMeter(ultimateMeter);
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
                animatorHandler.PlayTargetAnimation("Death", true);
                inputHandler.LockPlayer(true);
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
    }
}