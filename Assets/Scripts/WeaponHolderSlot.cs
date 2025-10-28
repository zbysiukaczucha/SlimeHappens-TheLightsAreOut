using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isHeadSlot;
        public bool isTailSlot;
        
        public GameObject currentWeaponModel;
        
        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            DestroyWeapon();

            if (weaponItem == null)
            {
                UnloadWeapon();
                return;
            }

            if (weaponItem.modelPrefab != null)
            {
                Transform parent = parentOverride != null ? parentOverride : this.transform;
                currentWeaponModel = Instantiate(weaponItem.modelPrefab, parent);
                currentWeaponModel.transform.localPosition = Vector3.zero;
                currentWeaponModel.transform.localRotation = Quaternion.identity;
            }
        }

        private void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }
        
        private void DestroyWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }
    }
}