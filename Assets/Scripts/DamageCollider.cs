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
            if (collision.tag == "Enemy")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWaeaponDamage);
                }
            }

            if (collision.tag == "Player" && damageTag == "Player")
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