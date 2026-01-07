using System;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class WorldEventManager : MonoBehaviour
    {
        public List<FogWall> fogWalls;
        public UIEnemyHealthBar bossHealthBar;
        public EnemyManager boss;
        
        public bool bossFightActive = false;
        public bool bossDefeated = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            bossHealthBar = FindFirstObjectByType<UIEnemyHealthBar>();
            boss = FindFirstObjectByType<EnemyManager>();
        }

        public void ActivateBossFight()
        {
            bossFightActive = true;
            bossHealthBar.SetActive(true);
            
            foreach (FogWall fogWall in fogWalls)
            {
                fogWall.ActivateFogWall();
            }
        }
        
        public void BossDefeated()
        {
            bossFightActive = false;
            bossHealthBar.SetActive(false);
            
            foreach (FogWall fogWall in fogWalls)
            {
                fogWall.DeactivateFogWall();
            }
        }
    }
}