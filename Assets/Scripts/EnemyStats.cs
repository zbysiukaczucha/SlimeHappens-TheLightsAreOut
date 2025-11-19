using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class EnemyStats : MonoBehaviour
    {
        public int level = 1;
        public int maxHealth;
        public int currentHealth;
        
        
        //AnimatorHandler animatorHandler;
        
        private void Awake()
        {
            //animatorHandler = GetComponent<AnimatorHandler>();
        }

        private void Start()
        {
            SetMaxHealthFromLevel();
            currentHealth = maxHealth;
        }

        private void SetMaxHealthFromLevel()
        {
            maxHealth = 100 + level * 10;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            // Play damaged animation
            //animatorHandler.PlayTargetAnimation("Damaged", true);
            print("Enemy took " + damage + " damage.");
            if (currentHealth < 0)
            {
                currentHealth = 0;
                // Handle player death here
                // animatorHandler.PlayTargetAnimation("Death", true);
                print("Enemy has died.");
            }
            
            //quick scuffed stuff
            if(GetComponentInParent<HealthBar>() != null)
            {
                var bar = GetComponentInParent<HealthBar>();
                bar.SetCurrentHealth(currentHealth);
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