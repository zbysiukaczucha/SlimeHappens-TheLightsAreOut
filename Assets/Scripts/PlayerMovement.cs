using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class PlayerMovement : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        // [Header("Ground & Air Detection")]
        // [SerializeField]
        // float groundDetectionRayStartPoint = 0.5f;
        // [SerializeField]
        // float minimumDistanceNeededToBeginFall = 1f; 
        // [SerializeField]
        // float groundDirectionRayDistance = 0.2f;
        // LayerMask ignoreForGroundCheck;
        // public float inAirTimer;

        [Header("Surface Detection")] [SerializeField]
        float surfaceCheckDistance = 1.5f;

        [SerializeField] LayerMask surfaceMask;
        [SerializeField] float surfaceStickForce = 50f;
        [SerializeField] float rotationSmooth = 10f;

        [Header("Movement Stats")] [SerializeField]
        float movementSpeed = 5;

        [SerializeField] float sprintSpeed = 7.5f;
        [SerializeField] float rotationSpeed = 10;

        public Vector3 surfaceNormal = Vector3.up;
        Vector3 targetUp = Vector3.up;

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            rigidbody.useGravity = false; // ślimak ma swoją grawitację
            surfaceMask = ~(1 << 8 | 1 << 11);
        }

        public void HandleSurfaceDetection(float delta)
        {
            RaycastHit hit;
            Vector3 origin = myTransform.position + myTransform.up * 0.2f;

            // Szukamy powierzchni pod ślimakiem (w jego lokalnym "dół")
            if (Physics.Raycast(origin, -myTransform.up, out hit, surfaceCheckDistance, surfaceMask))
            {
                surfaceNormal = hit.normal;
                targetUp = Vector3.Slerp(targetUp, surfaceNormal, delta * rotationSmooth);

                // Płynne dopasowanie orientacji do powierzchni
                Quaternion targetRotation = Quaternion.FromToRotation(myTransform.up, targetUp) * myTransform.rotation;
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, delta * rotationSmooth);

                // Delikatne trzymanie się powierzchni
                float distance = hit.distance;
                if (distance > 0.3f)
                {
                    rigidbody.AddForce(-hit.normal * (surfaceStickForce * (distance / surfaceCheckDistance)),
                        ForceMode.Acceleration);
                }
            }
            else
            {
                // Jeśli nie ma powierzchni pod nami, pozwól ślimakowi spaść (może to być np. zeskok)
                rigidbody.AddForce(-myTransform.up * surfaceStickForce, ForceMode.Acceleration);
            }
        }


        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 inputDir = (cameraObject.forward * inputHandler.vertical +
                                cameraObject.right * inputHandler.horizontal);
            inputDir = Vector3.ProjectOnPlane(inputDir, surfaceNormal);

            if (inputDir.sqrMagnitude < 0.01f) return;

            Quaternion targetRot = Quaternion.LookRotation(inputDir.normalized, myTransform.up);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRot, delta * rotationSpeed);
        }

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag) return;


            Vector3 inputDir = (cameraObject.forward * inputHandler.vertical +
                                cameraObject.right * inputHandler.horizontal);
            inputDir.Normalize();

            // Ruch po powierzchni (projekcja na płaszczyznę)
            Vector3 moveDir = Vector3.ProjectOnPlane(inputDir, surfaceNormal).normalized;

            float speed = movementSpeed;
            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
            }
            else
            {
                playerManager.isSprinting = false;
            }

            Vector3 targetVelocity = moveDir * speed;

            // Siła ruchu (dodaj delikatną inercję)
            Vector3 currentVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, surfaceNormal);
            Vector3 velocityChange = targetVelocity - currentVelocity;
            rigidbody.AddForce(velocityChange * 30f, ForceMode.Force);

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRolling(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
                moveDirection.Normalize();
                
                inputHandler.enableMovementInput = false;

                if (inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Dodge_Forward", true);
                    moveDirection =
                        Vector3.ProjectOnPlane(moveDirection, surfaceNormal); // ważne przy chodzeniu po ścianach
                    myTransform.rotation = Quaternion.LookRotation(moveDirection, myTransform.up);

                    // DODAJ SIŁĘ / RUCH W STRONĘ DODGE'A
                    StartCoroutine(PerformDodge(moveDirection));
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Dodge_Back", true);
                    StartCoroutine(PerformDodge(-myTransform.forward));
                }
            }
        }

        private IEnumerator PerformDodge(Vector3 direction)
        {
            float dodgeTime = 0.5f;
            float dodgeForce = 5f;

            float timer = 0;
            while (timer < dodgeTime)
            {
                timer += Time.fixedDeltaTime;
                rigidbody.AddForce(direction * dodgeForce, ForceMode.VelocityChange);
                rigidbody.AddForce(-myTransform.up * surfaceStickForce, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
            inputHandler.enableMovementInput = true;
        }

        #endregion

        #region Gravity

        public void ApplyLocalGravity(float delta)
        {
            // Przykleja ślimaka do powierzchni, symulując "grawitację" do powierzchni
            Vector3 localGravity = -myTransform.up * surfaceStickForce;
            rigidbody.AddForce(localGravity, ForceMode.Acceleration);
        }

        #endregion
    }
}