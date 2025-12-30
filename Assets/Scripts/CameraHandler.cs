using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimeborne
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerManager playerManager;
        
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
        
        // Zwiększamy prędkość wyrównywania do grawitacji, żeby kamera nie "szarpała" na rogach
        public float gravityAlignSpeed = 10f; 

        private float targetPosition;
        private float defaultPosition;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;
        
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;
        
        public Transform currentLockOnTarget;
        
        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public float maximumLockOnDistance = 30f;
        
        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            playerManager = FindFirstObjectByType<PlayerManager>();
            targetTransform = playerManager.transform;
            inputHandler = FindFirstObjectByType<InputHandler>();
        }
        
        public void FollowTarget(float delta)
        {
            // 1. Podążanie za pozycją (SmoothDamp)
            Vector3 targetPos = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPos;
            
            // Kolizje obsługujemy na końcu
            HandleCameraCollisions(delta);
        }
        
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (currentLockOnTarget == null)
            {
                inputHandler.lockOnFlag = false;

                // --- PIVOT (Góra/Dół) ---
                // To działa lokalnie, więc jest bezpieczne
                pivotAngle -= (mouseYInput * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);
                cameraPivotTransform.localRotation = Quaternion.Euler(pivotAngle, 0, 0);

                // --- OBRÓT KAMERY (Lewo/Prawo) ---
                // ZMIANA: Nie używamy lookAngle względem gracza. 
                // Zamiast tego obracamy kamerę wokół jej WŁASNEJ osi Y (Vector3.up).
                // Dzięki temu obrót postaci (A/D) nie wpływa na kamerę.
                
                if (Mathf.Abs(mouseXInput) > 0)
                {
                    // Obracamy "siebie" wokół osi Y (Space.Self oznacza, że używamy lokalnej osi Y kamery, która celuje w 'górę' względem ściany)
                    myTransform.Rotate(Vector3.up, mouseXInput * lookSpeed / delta, Space.Self);
                }

                // --- GRAWITACJA (Wyrównanie do podłoża) ---
                // Teraz musimy upewnić się, że "Góra" kamery zgadza się z "Górą" gracza (sufitem/ścianą),
                // ale BEZ zmieniania kierunku, w którym patrzymy (Forward).
                
                Vector3 targetUp = targetTransform.up;
                Vector3 currentUp = myTransform.up;

                // Sprawdzamy kąt, żeby uniknąć niepotrzebnych obliczeń i drgań
                if (Vector3.Angle(currentUp, targetUp) > 0.1f)
                {
                    // Obliczamy rotację, która przenosi currentUp na targetUp
                    Quaternion alignRotation = Quaternion.FromToRotation(currentUp, targetUp);
                    
                    // Aplikujemy tę rotację płynnie
                    // Używamy Slerp, żeby kamera ładnie "przepływała" z podłogi na ścianę
                    myTransform.rotation = Quaternion.Slerp(myTransform.rotation, alignRotation * myTransform.rotation, delta * gravityAlignSpeed);
                }
            }
            else
            {
                // --- LOCK ON (Bez zmian logicznych, tylko poprawka wektorów) ---
                Vector3 dir = currentLockOnTarget.position - myTransform.position;
                dir.Normalize();

                // LookRotation z uwzględnieniem Grawitacji (targetTransform.up)
                Quaternion targetRotation = Quaternion.LookRotation(dir, targetTransform.up);
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, delta / lookSpeed);

                Vector3 targetDir = currentLockOnTarget.position - cameraPivotTransform.position;
                targetDir.Normalize();

                Quaternion pivotLookRot = Quaternion.LookRotation(targetDir, targetTransform.up);
                
                cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, pivotLookRot, delta / pivotSpeed);
                
                // Zerowanie przechyłów bocznych pivota
                Vector3 euler = cameraPivotTransform.localEulerAngles;
                euler.y = 0;
                euler.z = 0;
                cameraPivotTransform.localEulerAngles = euler;
            }
        }
        
        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;

            Vector3 desiredCameraPos = cameraPivotTransform.TransformPoint(Vector3.back * Mathf.Abs(defaultPosition));
            Vector3 startPoint = targetTransform.position; 
            Vector3 direction = desiredCameraPos - startPoint;
            float distanceToCamera = direction.magnitude;
    
            // Debug.DrawLine(startPoint, startPoint + direction.normalized * distanceToCamera, Color.red);

            if (Physics.SphereCast(startPoint, cameraSphereRadius, direction.normalized, out hit, distanceToCamera, ignoreLayers))
            {
                float distanceFromPivot = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(distanceFromPivot - cameraCollisionOffset);
                if (targetPosition > -minimumCollisionOffset) targetPosition = -minimumCollisionOffset;
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        // Reszta metod (HandleLockOn, ClearLockOnTargets, SetCameraHeight) pozostaje bez zmian
        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null && character != targetTransform.GetComponent<CharacterManager>())
                {
                    if (character.isDead) continue;
                    
                    Vector3 targetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(cameraTransform.forward, targetDirection);
                    
                    if (viewableAngle is > 50 or < -50) continue;
                    
                    if (distanceFromTarget <= maximumLockOnDistance)
                    {
                        RaycastHit hit;
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            if (hit.transform.gameObject.layer == 6) continue;
                        }
                        availableTargets.Add(character);
                    }
                }
            }

            for (int k = 0; k < availableTargets.Count; k++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k].lockOnTransform;
                }
            }
        }
        
        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }
        
        public void SetCameraHeight()
        {
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);
            
            if (currentLockOnTarget != null)
                StartCoroutine(SmoothCameraHeightTransition(newLockedPosition));
            else
                StartCoroutine(SmoothCameraHeightTransition(newUnlockedPosition));
        }
        
        private IEnumerator SmoothCameraHeightTransition(Vector3 targetPosition)
        {
            Vector3 velocity = Vector3.zero;
            while (Vector3.Distance(cameraPivotTransform.localPosition, targetPosition) > 0.05f)
            {
                cameraPivotTransform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.localPosition, targetPosition, ref velocity, Time.deltaTime * 5f);
                yield return null;
            }
            cameraPivotTransform.localPosition = targetPosition;
        }
    }
}