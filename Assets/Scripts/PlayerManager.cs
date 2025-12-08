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
        PlayerStats playerStats;
        
        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool canDoCombo;
        public bool isInvulnerable;

        // Start is called before the first frame update
        void Start()
        {
            cameraHandler = FindFirstObjectByType<CameraHandler>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            playerStats = GetComponent<PlayerStats>();
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
        }
    }
}