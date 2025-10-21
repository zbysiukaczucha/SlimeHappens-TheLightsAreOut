using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{

    public class Item : ScriptableObject
    {
        [Header("Item Details")]
        public Sprite itemIcon;
        public string itemName;
    }
}