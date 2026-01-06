using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Slimeborne
{
    public class PlayerManager : CharacterManager
    {
        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerMovement playerMovement;
        public PlayerStats playerStats;
        InteractableUI interactableUI;
        public GameObject interactableUIGameObject;
        public GameObject itemInteractableGameObject;
        
        public LightManager lightManager;
        
        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool canDoCombo;
        public bool isInvulnerable;
        public bool isUltimateAttacking;

        // Start is called before the first frame update
        void Start()
        {
            cameraHandler = FindFirstObjectByType<CameraHandler>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            playerStats = GetComponent<PlayerStats>();
            interactableUI = FindFirstObjectByType<InteractableUI>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            isInvulnerable = anim.GetBool("isInvulnerable");
            
            isSprinting = inputHandler.b_Input;
            inputHandler.TickInput(delta);
            playerMovement.HandleRolling(delta);
            
            playerStats.RegenerateStamina();
            CheckForInteractableObject();
        }
        
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
            playerMovement.HandleSurfaceDetection(delta);
            playerMovement.HandleMovement(delta);
            playerMovement.ApplyLocalGravity(delta);
            inputHandler.sprintFlag = false;
            
            
        }
        
        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.interact_Input = false;
        }
        
        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position - transform.forward * 4f, 2f, transform.forward, out hit, 4f, inputHandler.interactableLayer))
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                if (interactableObject != null)
                {
                    Debug.Log("Found interactable: " + interactableObject.name);
                    string interactionText = interactableObject.interactionPrompt;
                    interactableUI.interactionText.text = interactionText;
                    interactableUIGameObject.SetActive(true);
                    if (inputHandler.interact_Input)
                    {
                        interactableObject.Interact();
                    }
                }
            }
            else
            {
                if(interactableUIGameObject != null)
                    interactableUIGameObject.SetActive(false);
            }
        }
        // private void OnDrawGizmos()
        // {
        //     float radius = 2f;
        //     float maxDistance = 4f;
        //     RaycastHit hit;
        //     Vector3 pos = transform.position - transform.forward * maxDistance;
        //     if (Physics.SphereCast(pos, radius, transform.forward * maxDistance, out hit, maxDistance, inputHandler.interactableLayer))
        //     {
        //         Gizmos.color = Color.green;
        //         Vector3 sphereCastMidpoint = pos + (transform.forward * hit.distance);
        //         Gizmos.DrawWireSphere(sphereCastMidpoint, radius);
        //         Gizmos.DrawSphere(hit.point, 0.1f);
        //         Debug.DrawLine(pos, sphereCastMidpoint, Color.green);
        //     }
        //     else
        //     {
        //         Gizmos.color = Color.red;
        //         Vector3 sphereCastMidpoint = pos + (transform.forward * (maxDistance-radius));
        //         Gizmos.DrawWireSphere(sphereCastMidpoint, radius);
        //         Debug.DrawLine(pos, sphereCastMidpoint, Color.red);
        //     }
        // }
    }
}