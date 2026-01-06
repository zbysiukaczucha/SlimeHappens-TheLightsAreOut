using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace Slimeborne
{
    public class EnemyStats : MonoBehaviour
    {
        public int level = 1;
        public int maxHealth;
        public int currentHealth;

        public UIEnemyHealthBar enemyHealthBar;

        private EnemyManager enemyManager;
        private Animator anim;
        private BossAISensor bossAISensor;
        private BehaviorGraphAgent behaviorGraphAgent;
        private WorldEventManager worldEventManager;
        
        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            anim = GetComponent<Animator>();
            bossAISensor = GetComponent<BossAISensor>();
            behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
            worldEventManager = FindFirstObjectByType<WorldEventManager>();
        }

        private void Start()
        {
            SetMaxHealthFromLevel();
            currentHealth = maxHealth;
            if(enemyHealthBar != null)
            {
                enemyHealthBar.SetMaxHealth(maxHealth);
                enemyHealthBar.SetHealth(currentHealth);
            }
        }

        private void SetMaxHealthFromLevel()
        {
            maxHealth = 100 + level * 10;
        }
        
        public void TakeDamage(int damage)
        {
            if (enemyManager.isDead)
                return;
            currentHealth -= damage;
            enemyHealthBar.SetHealth(currentHealth);
            // Play damaged animation
            //animatorHandler.PlayTargetAnimation("Damaged", true);
            print("Enemy took " + damage + " damage.");
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                // Handle player death here
                enemyManager.isDead = true;
                anim.applyRootMotion = true;
                anim.Play("Death");
                bossAISensor.ToggleStopFrog(true);
                bossAISensor.ToggleFrogAttack(false);
                behaviorGraphAgent.enabled = false;
                StartCoroutine(DeactivateAfterDelay(3f));
                print("Enemy has died.");
            }
        }
        
        private IEnumerator DeactivateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            worldEventManager.BossDefeated();
        }
    }
}