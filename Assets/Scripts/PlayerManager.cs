using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Slimeborne
{
    public class PlayerManager : MonoBehaviour
    {
        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerMovement playerMovement;
        
        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;

        // Start is called before the first frame update
        void Start()
        {
            cameraHandler = CameraHandler.singleton;
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            
            isSprinting = inputHandler.b_Input;
            inputHandler.TickInput(delta);
            playerMovement.HandleMovement(delta);
            playerMovement.HandleRolling(delta);
            playerMovement.HandleFalling(delta, playerMovement.moveDirection);
        }
        
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }
        
        private void LateUpdate()
        {
            playerMovement.inAirTimer += Time.deltaTime;
        }
    }
}