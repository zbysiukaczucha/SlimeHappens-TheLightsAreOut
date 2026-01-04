using System;
using UnityEngine;

namespace Slimeborne
{


    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactionPrompt;
        protected PlayerManager playerManager;
        
        private void Awake()
        {
            playerManager = FindFirstObjectByType<PlayerManager>();
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        
        public virtual void Interact()
        {
            // This method is meant to be overridden by derived classes.
            Debug.Log("Interacting with " + transform.name);
        }
    }
}