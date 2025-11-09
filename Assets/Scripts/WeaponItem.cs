using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Slimeborne
{
    [CreateAssetMenu(menuName = "Item/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [FormerlySerializedAs("LightAttack")] [Header("Attack Animations")]
        public string LightAttack1;
        public string LightAttack2;
        public string LightAttack3;
        public string HeavyAttack;
    }
}