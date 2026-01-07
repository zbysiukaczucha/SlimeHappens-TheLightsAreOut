using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShineHappens
{
    // Script to add to any objects that cause the crosshair change
    public class Interactable : MonoBehaviour
    {
        [Header("Detection")]
        public float pickupRange = 2.5f;

        [Header("References")]
        public GameObject glowCircle;

        private Transform player;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;

            if (glowCircle != null)
                glowCircle.SetActive(false);
        }

        void Update()
        {
            if (player == null || glowCircle == null)
                return;

            float distance = Vector3.Distance(player.position, transform.position);

            glowCircle.SetActive(distance <= pickupRange);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRange);
        }
#endif
    }
}