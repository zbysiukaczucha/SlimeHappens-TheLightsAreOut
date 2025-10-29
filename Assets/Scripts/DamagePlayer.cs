using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class DamagePlayer : MonoBehaviour
    {
        public int damage = 10;
        private void OnTriggerEnter(Collider other)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            
        }
    }
}