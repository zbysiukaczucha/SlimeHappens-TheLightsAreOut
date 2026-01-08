using TMPro;
using UnityEngine;
using System.Collections;

namespace Slimeborne
{
    public class PickUpItem : Interactable
    {
        public Item item;

        private void Awake()
        {
            playerManager = FindFirstObjectByType<PlayerManager>();
            if(UltimateGameManager.Instance.collectedPickUpItems.Contains(name))
            {
                Destroy(gameObject);
            }
        }
        public override void Interact()
        {
            base.Interact();
            PickUp();
        }

        private void PickUp()
        {
            Debug.Log("Picking up " + item.itemName);
            // Add item to player's inventory logic here

            PlayerMovement playerMovement = playerManager.GetComponent<PlayerMovement>();
            playerMovement.rigidbody.linearVelocity = Vector3.zero;
            // Destroy the item in the world after picking it up
            playerManager.itemInteractableGameObject.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
            playerManager.itemInteractableGameObject.SetActive(true);
            // Disable the item's visual representation and collider
            // add implementation to work with items with multiple mesh renderers and colliders
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }
            UltimateGameManager.Instance.collectedPickUpItems.Add(name);
            StartCoroutine(HideItemTextAfterDelay(3.5f));
        }
        
        private IEnumerator HideItemTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerManager.itemInteractableGameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}