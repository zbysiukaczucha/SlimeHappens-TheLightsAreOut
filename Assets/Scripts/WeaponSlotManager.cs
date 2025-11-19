using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot headSlot;
        WeaponHolderSlot tailSlot;
        
        DamageCollider headDamageCollider;
        DamageCollider tailDamageCollider;
        
        public WeaponItem attackingWeapon;
        
        PlayerStats playerStats;
        private void Awake()
        {
            playerStats = GetComponentInParent<PlayerStats>();
            
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

            foreach (WeaponHolderSlot weaponHolderSlot in weaponHolderSlots)
            {
                if (weaponHolderSlot.isHeadSlot)
                {
                    headSlot = weaponHolderSlot;
                }
                else if (weaponHolderSlot.isTailSlot)
                {
                    tailSlot = weaponHolderSlot;
                }
            }
        }
        
        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isHeadSlot)
        {
            if (isHeadSlot)
            {
                headSlot.LoadWeaponModel(weaponItem);
                headDamageCollider = headSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
            else
            {
                tailSlot.LoadWeaponModel(weaponItem);
                tailDamageCollider = tailSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
        }

        #region Handle Damage Colliders
        public void OpenHeadDamageCollider()
        {
            headDamageCollider.EnableDamageCollider();
        }
        
        public void OpenTailDamageCollider()
        {
            tailDamageCollider.EnableDamageCollider();
        }
        
        public void CloseHeadDamageCollider()
        {
            headDamageCollider.DisableDamageCollider();
        }
        
        public void CloseTailDamageCollider()
        {
            tailDamageCollider.DisableDamageCollider();
        }
        #endregion
        
        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStaminaCost * attackingWeapon.lightAttackStaminaMultiplier));
        }
        
        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStaminaCost * attackingWeapon.heavyAttackStaminaMultiplier));
        }
    }
}