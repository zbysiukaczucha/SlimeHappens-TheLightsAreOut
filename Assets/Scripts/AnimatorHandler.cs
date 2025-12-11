using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class AnimatorHandler : MonoBehaviour
    {
        PlayerManager playerManager;
        public Animator anim;
        InputHandler inputHandler;
        PlayerMovement playerMovement;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerMovement = GetComponentInParent<PlayerMovement>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0;
            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement >= 0.55f)
            {
                v = 1f;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement <= -0.55f)
            {
                v = -1f;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement >= 0.55f)
            {
                h = 1f;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement <= -0.55f)
            {
                h = -1f;
            }
            else
            {
                h = 0;
            }
            #endregion
            
            if (isSprinting)
            {
                v = 2f;
                h = horizontalMovement;
            }
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }
        
        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }
        
        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }
        
        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }
        
        public void EnableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }
        
        public void DisableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }
        
        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;
            
            // float delta = Time.deltaTime;
            // playerMovement.rigidbody.drag = 0;
            // Vector3 deltaPosition = anim.deltaPosition;
            // deltaPosition.y = 0;
            // Vector3 velocity = deltaPosition / delta;
            // playerMovement.rigidbody.velocity = velocity;
            
            // Root motion tylko do obrotów lub drobnych korekt pozycji,
            // ale NIE nadpisuje velocity.
            Vector3 deltaPosition = anim.deltaPosition;

            // Projekcja na płaszczyznę powierzchni (żeby root motion działał na ścianach)
            Vector3 projected = Vector3.ProjectOnPlane(deltaPosition, playerMovement.surfaceNormal);

            // Możesz dodać delikatny ruch jeśli animacja wymaga "popychu"
            playerMovement.GetComponent<Rigidbody>().AddForce(projected * 60f, ForceMode.Acceleration);
        }
    }
}