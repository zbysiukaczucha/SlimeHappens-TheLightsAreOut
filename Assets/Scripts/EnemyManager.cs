using System;
using Slimeborne;
using UnityEngine;

namespace Slimeborne
{
    public class EnemyManager : CharacterManager
    {
        private void Start()
        {
            SetActive(false);
        }
        
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}