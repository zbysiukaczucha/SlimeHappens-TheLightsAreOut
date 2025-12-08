using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        
        public static CameraHandler singleton;
        
        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;
        
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        
        public Transform currentLockOnTarget;
        
        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public float maximumLockOnDistance = 30f;
        
        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindFirstObjectByType<PlayerManager>().transform;
            inputHandler = FindFirstObjectByType<InputHandler>();
        }
        
        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;
            
            HandleCameraCollisions(delta);
        }
        
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (currentLockOnTarget == null)
            {
                inputHandler.lockOnFlag = false;

                lookAngle += (mouseXInput * lookSpeed) / delta;
                pivotAngle -= (mouseYInput * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pivotAngle;
                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                Vector3 dir = currentLockOnTarget.position - myTransform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, delta / lookSpeed);

                Vector3 targetDir = currentLockOnTarget.position - cameraPivotTransform.position;
                targetDir.Normalize();

                Quaternion pivotTargetRotation = Quaternion.LookRotation(targetDir);
                Vector3 eulerAngle = pivotTargetRotation.eulerAngles;
                eulerAngle.y = 0;
                pivotTargetRotation = Quaternion.Euler(eulerAngle);

                cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, pivotTargetRotation, delta / pivotSpeed);
            }
        }
        
        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();
            
            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
        
        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                
                if (character != null && character != targetTransform.GetComponent<CharacterManager>())
                {
                    if (character.isDead)
                        continue;
                    
                    Vector3 targetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(cameraTransform.forward, targetDirection);
                    
                    if (viewableAngle is > 50 or < -50)
                        continue;
                    
                    if (distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(character);
                    }
                }
            }

            for (int k = 0; k < availableTargets.Count; k++)
            {
                float distanceFromTarget =
                    Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k].lockOnTransform;
                    Debug.Log(nearestLockOnTarget);
                }
            }
        }
        
        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }
    }
}