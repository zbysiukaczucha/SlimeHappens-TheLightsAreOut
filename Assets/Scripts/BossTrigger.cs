using System;
using Slimeborne;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    UIEnemyHealthBar bossHealthBar;
    BossAISensor bossAISensor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        bossHealthBar = FindFirstObjectByType<UIEnemyHealthBar>();
        bossAISensor = FindFirstObjectByType<BossAISensor>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossHealthBar != null)
            {
                bossHealthBar.SetActive(true);
            }

            if (bossAISensor != null)
            {
                bossAISensor.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
