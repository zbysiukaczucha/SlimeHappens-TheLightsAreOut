using System;
using Slimeborne;
using UnityEngine;

namespace Slimeborne
{
    public class ŻabsonMegaSkrypt_PROWIZORKA_LVL_MAX : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        //This is going to be REALLY ugly, but I really want to sleep at least a few hours. I'm terribly sorry, future me/zbychu :(

        public Animator animator;

        [Header("Attack Animations")] public String TongueSweep;
        public String TongueSlam;
        public String TongueSnap;
        public String RollR;
        public String Charge;
        public String ArmSmash;

        public DamageCollider TongueCollider;
        public DamageCollider ArmCollider;
        public DamageCollider BodyCollider;

        void Start()
        {
            animator = GetComponent<Animator>();
            DisableTongue();
            DisableHand();
            DisableBody();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Handle damage colliders

        public void EnableTongue()
        {
            TongueCollider.EnableDamageCollider();
        }

        public void DisableTongue()
        {
            TongueCollider.DisableDamageCollider();
        }

        public void EnableHand()
        {
            ArmCollider.EnableDamageCollider();
        }

        public void DisableHand()
        {
            ArmCollider.DisableDamageCollider();
        }

        public void EnableBody()
        {
            BodyCollider.EnableDamageCollider();
        }

        public void DisableBody()
        {
            BodyCollider.DisableDamageCollider();
        }

        #endregion

    }
}