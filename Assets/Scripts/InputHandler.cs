using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Slimeborne
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        
        public bool b_Input;
        public bool rb_Input;
        public bool rt_Input;
        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;
        public bool lockOn_Input;
        public bool interact_Input;
        
        public bool enableMovementInput = true;
        
        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public float rollInputTimer;
        
        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        PlayerStats playerStats;
        CameraHandler cameraHandler;
        
        Vector2 movementInput;
        Vector2 cameraInput;
        
        public LayerMask interactableLayer;
        
        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            cameraHandler = FindFirstObjectByType<CameraHandler>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            interactableLayer = LayerMask.GetMask("Interactable");
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.PlayerActions.RB.performed += i => rb_Input = true;
                inputActions.PlayerActions.RT.performed += i => rt_Input = true;
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
                inputActions.PlayerActions.LockOn.performed += i => lockOn_Input = true;
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void TickInput(float delta)
        {
            HandleMovementInput(delta);
            HandleMouseInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotsInput(delta);
            HandleLockOnInput();
            HandleInteractInput();
        }
        
        private void HandleMovementInput(float delta)
        {
            if (!enableMovementInput)
                return;
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        }
        
        private void HandleMouseInput(float delta)
        {
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
        
        private void HandleRollInput(float delta)
        {
            if (b_Input)
            {
                rollInputTimer += delta;

                if (playerStats.currentStamina <= 0)
                {
                    b_Input = false;
                    sprintFlag = false;
                }
                
                if (moveAmount > 0.5f && playerStats.currentStamina > 0)
                {
                    sprintFlag = true;
                }
            }
            else
            {
                if(rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }
                
                rollInputTimer = 0;
            }
        }
        
        private void HandleAttackInput(float delta)
        {
            if(rb_Input)
            {
                if(playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.headWeapon);
                    comboFlag = false;
                }
                else
                {
                    if (playerManager.isInteracting)
                        return;
                    playerAttacker.HandleLightAttack(playerInventory.headWeapon);
                }
            }

            if (rt_Input)
            {
                playerAttacker.HandleUltimateAttack();
            }
        }
        
        private void HandleQuickSlotsInput(float delta)
        {
            inputActions.QuickSlots.Right.performed += i => d_Pad_Right = true;
            inputActions.QuickSlots.Left.performed += i => d_Pad_Left = true;
            inputActions.QuickSlots.Up.performed += i => d_Pad_Up = true;
            inputActions.QuickSlots.Down.performed += i => d_Pad_Down = true;
            
            if(d_Pad_Right)
            {
                playerInventory.ChangeHeadWeapon();
            }
            else if(d_Pad_Left)
            {
                playerInventory.ChangeTailWeapon();
            }
        }

        private void HandleLockOnInput()
        {
            if (lockOn_Input)
            {
                cameraHandler.ClearLockOnTargets();
                if (lockOnFlag == false)
                {
                    cameraHandler.HandleLockOn();
                    Debug.Log(cameraHandler.nearestLockOnTarget);
                    if (cameraHandler.nearestLockOnTarget != null)
                    {
                        cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                        lockOnFlag = true;
                    }
                }
                else
                {
                    lockOnFlag = false;
                }
                cameraHandler.SetCameraHeight();
            }

            lockOn_Input = false;
        }

        public void LockPlayer(bool value)
        {
            enableMovementInput = !value;
            cameraHandler.ToggleCameraLock(value);
            playerAttacker.lockPlayer = value;
            if (value) moveAmount = 0;
        }
        
        private void HandleInteractInput()
        {
            inputActions.PlayerActions.Interact.performed += i => interact_Input = true;
        }

        float stepInterval = 1.12f;

        float stepTimer ;

        void Update()
        {
            if (moveAmount == 1)
            {
                stepInterval = sprintFlag ? 0.75f : 1.45f;
                stepTimer += Time.deltaTime;

                if (stepTimer >= stepInterval)
                {
                    AudioManager.PlaySound(SoundType.SnailMove, 1, 0.15f);
                    stepTimer = 0f;
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }
    }
}