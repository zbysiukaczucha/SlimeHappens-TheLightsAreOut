using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        public string lastAttack;
        
        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
        }
        
        public void HandleWeaponCombo(WeaponItem weaponItem)
        {
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
            animatorHandler.PlayTargetAnimation(weaponItem.LightAttack1, true);
            lastAttack = weaponItem.LightAttack1;
        }
        
        public void HandleHeavyAttack(WeaponItem weaponItem)
        {
            animatorHandler.PlayTargetAnimation(weaponItem.HeavyAttack, true);
            lastAttack = weaponItem.HeavyAttack;
        }
    }
}