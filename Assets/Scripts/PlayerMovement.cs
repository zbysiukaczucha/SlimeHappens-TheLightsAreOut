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
        float surfaceCheckDistance = 0.5f;

        [SerializeField] LayerMask surfaceMask;
        [SerializeField] float surfaceStickForce = 50f;
        [SerializeField] float rotationSmooth = 10f;

        [Header("Movement Stats")] [SerializeField]
        float movementSpeed = 5;

        [SerializeField] float sprintSpeed = 7.5f;
        [SerializeField] float rotationSpeed = 10;

        public Vector3 surfaceNormal = Vector3.up;
        Vector3 targetUp = Vector3.up;
        
        public bool isAttachedToSurface = false;
        public float detachRotationSpeed = 5f;
        public float fallGravity = 40f;

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

        private void Update()
        {
            Debug.DrawRay(myTransform.position, -myTransform.up * 1f, isAttachedToSurface ? Color.green : Color.red);
        }

        // public void HandleSurfaceDetection(float delta)
        // {
        //     RaycastHit hit;
        //     Vector3 origin = myTransform.position + myTransform.up * 0.2f;
        //
        //     // Szukamy powierzchni pod ślimakiem (w jego lokalnym "dół")
        //     if (Physics.Raycast(origin, -myTransform.up, out hit, surfaceCheckDistance, surfaceMask))
        //     {
        //         surfaceNormal = hit.normal;
        //         targetUp = Vector3.Slerp(targetUp, surfaceNormal, delta * rotationSmooth);
        //
        //         // Płynne dopasowanie orientacji do powierzchni
        //         Quaternion targetRotation = Quaternion.FromToRotation(myTransform.up, targetUp) * myTransform.rotation;
        //         myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, delta * rotationSmooth);
        //
        //         // Delikatne trzymanie się powierzchni
        //         float distance = hit.distance;
        //         if (distance > 0.3f)
        //         {
        //             rigidbody.AddForce(-hit.normal * (surfaceStickForce * (distance / surfaceCheckDistance)),
        //                 ForceMode.Acceleration);
        //         }
        //     }
        //     else
        //     {
        //         // Jeśli nie ma powierzchni pod nami, pozwól ślimakowi spaść (może to być np. zeskok)
        //         rigidbody.AddForce(-myTransform.up * surfaceStickForce, ForceMode.Acceleration);
        //     }
        // }
        
        public void HandleSurfaceDetection(float delta)
        {
            RaycastHit hit;
            Vector3 origin = myTransform.position + myTransform.up * 0.2f;

            bool surfaceDetected = Physics.Raycast(origin, -myTransform.up, out hit, surfaceCheckDistance, surfaceMask);

            if (surfaceDetected)
            {
                // Jeśli właśnie złapaliśmy powierzchnię
                if (!isAttachedToSurface)
                {
                    isAttachedToSurface = true;
                    rigidbody.useGravity = false;
                    StopAllCoroutines();
                }

                surfaceNormal = hit.normal;

                // Obrót do normalnej powierzchni
                Quaternion surfaceAlign = Quaternion.FromToRotation(myTransform.up, surfaceNormal) * myTransform.rotation;
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, surfaceAlign, delta * rotationSmooth);

                // Trzymanie się powierzchni
                float distance = hit.distance;
                if (distance > 0.2f)
                {
                    rigidbody.AddForce(-hit.normal * (surfaceStickForce * (distance / surfaceCheckDistance)), ForceMode.Acceleration);
                }
            }
            else
            {
                // Utrata powierzchni – ślimak spada
                if (isAttachedToSurface)
                {
                    StartCoroutine(DetachFromSurfaceSmooth());
                }

                // Gdy ślimak już odczepiony, używamy tylko grawitacji świata
                if (rigidbody.useGravity)
                {
                    rigidbody.AddForce(Physics.gravity * (fallGravity / 9.81f), ForceMode.Acceleration);
                }
            }
        }



        
        private IEnumerator DetachFromSurfaceSmooth()
        {
            isAttachedToSurface = false;

            // Natychmiast zatrzymujemy ruch i obrót
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            // Przełączamy na grawitację świata
            rigidbody.useGravity = true;

            // Kierunek "przodu" na światowy poziom
            Vector3 forwardProjected = Vector3.ProjectOnPlane(myTransform.forward, Vector3.up);
            if (forwardProjected.sqrMagnitude < 0.001f)
                forwardProjected = Vector3.forward;

            Quaternion startRot = myTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(forwardProjected.normalized, Vector3.up);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * detachRotationSpeed;
                myTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            // Finalne wyrównanie
            myTransform.rotation = targetRot;
            myTransform.up = Vector3.up;
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
            if (inputHandler.rollFlag || !isAttachedToSurface) return;


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
            Vector3 currentVelocity = Vector3.ProjectOnPlane(rigidbody.linearVelocity, surfaceNormal);
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
            if (animatorHandler.anim.GetBool("isInteracting") || !isAttachedToSurface)
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

        // public void ApplyLocalGravity(float delta)
        // {
        //     // Przykleja ślimaka do powierzchni, symulując "grawitację" do powierzchni
        //     Vector3 localGravity = -myTransform.up * surfaceStickForce;
        //     rigidbody.AddForce(localGravity, ForceMode.Acceleration);
        // }
        
        public void ApplyLocalGravity(float delta)
        {
            if (!isAttachedToSurface)
                return; 

            // Lokalna grawitacja w stronę powierzchni
            Vector3 localGravity = -myTransform.up * surfaceStickForce;
            rigidbody.AddForce(localGravity, ForceMode.Acceleration);
        }


        #endregion
    }
}