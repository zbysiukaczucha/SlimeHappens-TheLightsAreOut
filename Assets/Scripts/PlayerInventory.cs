using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Slimeborne
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Weapons")]
        WeaponSlotManager weaponSlotManager;
        public WeaponItem headWeapon;
        public WeaponItem tailWeapon;
        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInHeadSlots = new WeaponItem[1];
        public WeaponItem[] weaponsInTailSlots = new WeaponItem[1];
        
        public int currentHeadWeaponIndex = -1;
        public int currentTailWeaponIndex = -1;

        [Header("Gems")]
        public List<GameObject> gems;
        public GameObject activeGem;
        private int lastKeyPressed = 0;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }
        
        private void Start()
        {
            // headWeapon = unarmedWeapon;
            // tailWeapon = unarmedWeapon;
            headWeapon = weaponsInHeadSlots[currentHeadWeaponIndex];
            tailWeapon = weaponsInTailSlots[currentTailWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(headWeapon, true);
            weaponSlotManager.LoadWeaponOnSlot(tailWeapon, false);
        }


        private void Update()
        {
            if (Input.anyKeyDown)
            {
                // If the pressed key is one of the numbers, show the resective gem
                if (lastKeyPressed > 47 && lastKeyPressed < 58)
                {
                    print("Pressed " + (KeyCode)lastKeyPressed);
                    int slot = Math.Abs(48 - lastKeyPressed);
                    if (gems.Count > slot) {
                        //gems[slot].SetActive(true);
                        print(gems[slot].name);
                        if(activeGem != null)
                        {
                            activeGem.SetActive(false);
                        }
                        activeGem = gems[slot];
                        activeGem.SetActive(true);
                    }
                }
                // If tilde/backquote key was pressed, hide the gem
                if(lastKeyPressed == 96)
                {
                    activeGem.SetActive(false);
                    activeGem = null;
                }
            }
        }
        
        // Get the int value of the key that was just  pressed
        void OnGUI()
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.keyCode != KeyCode.None)
                {
                    lastKeyPressed = (int)e.keyCode;
                }
            }
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