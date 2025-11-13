using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class DamageCollider : MonoBehaviour
    {
        public Collider damageCollider;
        public String damageTag = "Enemy";
        
        public int currentWaeaponDamage = 25;
        
        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            DisableDamageCollider();
        }
        
        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }
        
        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            //Debug.Log($"[DamageCollider] {gameObject.name} hit {collision.name} | Tag: {collision.tag} | DamageTag: {damageTag}", this);
            
            if (collision.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWaeaponDamage);
                }
            }

            if (collision.CompareTag("Player") && damageTag == "Player")
            {
                PlayerStats playerStats = collision.GetComponentInParent<PlayerStats>();

                
                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWaeaponDamage);
                }
            }
        }
    }
}