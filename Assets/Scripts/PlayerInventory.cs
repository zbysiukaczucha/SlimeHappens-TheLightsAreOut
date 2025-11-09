using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem headWeapon;
        public WeaponItem tailWeapon;
        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInHeadSlots = new WeaponItem[1];
        public WeaponItem[] weaponsInTailSlots = new WeaponItem[1];
        
        public int currentHeadWeaponIndex = -1;
        public int currentTailWeaponIndex = -1;
        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }
        
        private void Start()
        {
            headWeapon = unarmedWeapon;
            tailWeapon = unarmedWeapon;
            // headWeapon = weaponsInHeadSlots[currentHeadWeaponIndex];
            // tailWeapon = weaponsInTailSlots[currentTailWeaponIndex];
            // weaponSlotManager.LoadWeaponOnSlot(headWeapon, true);
            // weaponSlotManager.LoadWeaponOnSlot(tailWeapon, false);
        }
        
        private WeaponItem GetNextWeapon(WeaponItem[] slots, ref int currentIndex)
        {
            int start = currentIndex;
            for (int i = 0; i < slots.Length; i++)
            {
                currentIndex = (currentIndex + 1) % slots.Length;
                if (slots[currentIndex] != null)
                    return slots[currentIndex];
            }
            return null;
        }
        
        public void ChangeHeadWeapon()
        {
            headWeapon = GetNextWeapon(weaponsInHeadSlots, ref currentHeadWeaponIndex);
            weaponSlotManager.LoadWeaponOnSlot(headWeapon, true);
        }
        
        public void ChangeTailWeapon()
        {
            tailWeapon = GetNextWeapon(weaponsInTailSlots, ref currentTailWeaponIndex);
            weaponSlotManager.LoadWeaponOnSlot(tailWeapon, false);
        }
    }
}