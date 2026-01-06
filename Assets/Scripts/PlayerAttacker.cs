using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        PlayerStats playerStats;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerManager playerManager;
        PlayerMovement playerMovement;
        public string lastAttack;

        public bool lockPlayer;
        
        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerStats = GetComponent<PlayerStats>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
            playerManager = GetComponent<PlayerManager>();
            playerMovement = GetComponent<PlayerMovement>();
        }
        
        public void HandleWeaponCombo(WeaponItem weaponItem)
        {
            if(weaponItem.isUnarmed || playerStats.currentStamina <= 0 || lockPlayer)
                return;
            if (inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == weaponItem.LightAttack1)
                {
                    animatorHandler.PlayTargetAnimation(weaponItem.LightAttack2, true);
                    lastAttack = weaponItem.LightAttack2;
                }
                else if (lastAttack == weaponItem.LightAttack2)
                {
                    animatorHandler.PlayTargetAnimation(weaponItem.LightAttack3, true);
                    lastAttack = weaponItem.LightAttack3;
                }
            }
        }
        
        public void HandleLightAttack(WeaponItem weaponItem)
        {
            if (playerStats.currentStamina <= 0 || lockPlayer)
                return;
            
            weaponSlotManager.attackingWeapon = weaponItem;
            if (weaponItem.isUnarmed || animatorHandler.anim.GetBool("isInteracting"))
                return;
            animatorHandler.PlayTargetAnimation(weaponItem.LightAttack1, true);
            lastAttack = weaponItem.LightAttack1;
        }
        
        public void HandleUltimateAttack()
        {
            // Add checking for enough ultimate energy here later
            if (playerStats.ultimateMeter < PlayerStats.maxUltimateMeter || animatorHandler.anim.GetBool("isInteracting") || playerMovement.isAttachedToSurface == false || lockPlayer)
                return;
            animatorHandler.PlayTargetAnimation("Ult", true);
            playerManager.isUltimateAttacking = true;
            weaponSlotManager.HideWeapons();
            playerMovement.rigidbody.linearVelocity = Vector3.zero;
            playerMovement.rigidbody.angularVelocity = Vector3.zero;
            lastAttack = null;
            playerStats.ultimateMeter = 0;
            playerStats.ultimateBar.SetCurrentUltMeter(playerStats.ultimateMeter);
        }
    }
}