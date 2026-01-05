using TMPro;
using UnityEngine;
using System.Collections;

namespace Slimeborne
{
    public class PickUpItem : Interactable
    {
        public Item item;

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
            StartCoroutine(HideItemTextAfterDelay(3.5f));
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
        }
        
        private IEnumerator HideItemTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerManager.itemInteractableGameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}