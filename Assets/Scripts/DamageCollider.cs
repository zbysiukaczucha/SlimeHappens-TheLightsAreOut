using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Slimeborne
{
    public class DamageCollider : MonoBehaviour
    {
        [FormerlySerializedAs("damageCollider")]
        [Header("Colliders (assign in inspector or will be gathered automatically)")]
        public Collider[] damageColliders;

        [Header("Damage")]
        public int currentWeaponDamage = 25;

        [Header("Layer mapping (numbers)")]
        [Tooltip("Layer index for Enemy.")]
        public int layerEnemy = 7;
        [Tooltip("Layer index for Player.")]
        public int layerPlayer = 9;
        [Tooltip("Layer index for Boss.")]
        public int layerBoss = 11;
        [Tooltip("Layer index for EnemyParts (colliders that belong to enemies).")]
        public int layerEnemyParts = 12;
        [Tooltip("Layer index for PlayerHitbox (colliders that belong to player).")]
        public int layerPlayerHitbox = 13;

        // Tracks which root objects we've already damaged during the current activation
        private HashSet<int> damagedRoots = new HashSet<int>();

        private void Awake()
        {
            // If nothing assigned in inspector, try to gather colliders from this object and children
            if (damageColliders == null || damageColliders.Length == 0)
            {
                // najpierw GetComponent jeśli collider na tym samym obiekcie (historyczne wsparcie)
                Collider self = GetComponent<Collider>();
                if (self != null)
                {
                    damageColliders = new[] { self };
                }
                else
                {
                    // pobierz wszystkie colidery z obiektu i dzieci
                    damageColliders = GetComponentsInChildren<Collider>(includeInactive: true);
                }
            }

            if (damageColliders != null && damageColliders.Length > 0)
            {
                // ensure all are triggers and start disabled
                foreach (var col in damageColliders)
                {
                    if (col == null) continue;
                    col.isTrigger = true;
                    col.enabled = false;
                }

                DisableDamageCollider(); // clears set and ensures disabled
            }
            else
            {
                Debug.LogWarning($"[DamageCollider] No Colliders found on {gameObject.name}", this);
            }
        }

        /// <summary>
        /// Enable all colliders and clear previously damaged targets so they can be hit once this activation.
        /// Call this at the start of an attack window.
        /// </summary>
        public void EnableDamageCollider()
        {
            damagedRoots.Clear();

            if (damageColliders == null) return;

            for (int i = 0; i < damageColliders.Length; i++)
            {
                var col = damageColliders[i];
                if (col == null) continue;
                col.enabled = true;
            }
        }

        /// <summary>
        /// Disable all colliders — also clears the list to avoid holding references.
        /// Call this at the end of an attack window.
        /// </summary>
        public void DisableDamageCollider()
        {
            if (damageColliders != null)
            {
                for (int i = 0; i < damageColliders.Length; i++)
                {
                    var col = damageColliders[i];
                    if (col == null) continue;
                    col.enabled = false;
                }
            }

            damagedRoots.Clear();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision == null) return;

            // Determine root object (so multiple parts count as same target)
            Transform root = collision.transform.root;
            int rootId = root.gameObject.GetInstanceID();

            // If we've already damaged this root during this activation, skip
            if (damagedRoots.Contains(rootId)) return;
            //Debug.Log($"[DamageCollider] {gameObject.name} triggered by {collision.gameObject.name} (root: {root.name})", this);

            // Decide whether this collision is a valid target based on layers
            bool isEnemyHit = false;
            bool isPlayerHit = false;

            int otherLayer = collision.gameObject.layer;

            if (otherLayer == layerEnemy || otherLayer == layerEnemyParts || otherLayer == layerBoss)
            {
                isEnemyHit = true;
            }

            if (otherLayer == layerPlayer || otherLayer == layerPlayerHitbox)
            {
                isPlayerHit = true;
            }

            // Apply damage (only once per root per activation)
            if (isEnemyHit)
            {
                EnemyStats enemyStats = collision.GetComponentInParent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                    damagedRoots.Add(rootId);
                    Debug.Log($"[DamageCollider] {gameObject.name} damaged ENEMY {root.name} for {currentWeaponDamage}", this);
                }
                else
                {
                    damagedRoots.Add(rootId);
                    Debug.LogWarning($"[DamageCollider] {gameObject.name} hit an object on Enemy layer but no EnemyStats found on {root.name}", this);
                }
            }
            else if (isPlayerHit)
            {
                PlayerStats playerStats = collision.GetComponentInParent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                    damagedRoots.Add(rootId);
                    Debug.Log($"[DamageCollider] {gameObject.name} damaged PLAYER {root.name} for {currentWeaponDamage}", this);
                }
                else
                {
                    damagedRoots.Add(rootId);
                    Debug.LogWarning($"[DamageCollider] {gameObject.name} hit an object on Player layer but no PlayerStats found on {root.name}", this);
                }
            }
            // else: not a damageable layer -> ignore
        }
    }
}
