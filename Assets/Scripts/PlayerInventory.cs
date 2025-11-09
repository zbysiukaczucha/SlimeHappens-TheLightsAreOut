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

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }
        
        private void Start()
        {
            weaponSlotManager.LoadWeaponOnSlot(headWeapon, true);
            weaponSlotManager.LoadWeaponOnSlot(tailWeapon, false);
        }
    }
}