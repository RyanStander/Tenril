using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockOn : MonoBehaviour
{
    private InputHandler inputHandler;
    private PlayerManager playerManager;

    [Header("Camera references")]
    [Tooltip("The transform of the main camera in the scene")]
    [SerializeField] private Transform mainCameraTransform;
    [Tooltip("The lockOnCamera object")]
    [SerializeField] private GameObject lockOnCamera;
    [Tooltip("The transform of the lockOnCamera")]
    [SerializeField] private Transform lockOnCameraTransform;
    [Tooltip("The transform of the lockOnCamera's pivot")]
    [SerializeField] private Transform lockOnCameraPivotTransform;

    [Tooltip("The transform which the lockOnCamera's pivot will be placed")]
    [SerializeField] private Transform playerTransform;

    private Vector3 cameraFollowVelocity = Vector3.zero;

    [Tooltip("Layers that the camera will collide with and be unable to pass through")]
    [SerializeField] private LayerMask cameraCollisionLayers;
    [SerializeField] private LayerMask environmentLayer;

    [Tooltip("The speed at which the camera follows the player")]
    [SerializeField] private float followSpeed = 0.1f;

    //these positions are used to reset the position of cameras so that following works properly
    private float targetPosition, defaultPosition;

    [Tooltip("The radius to check if the camera is collidong with any objects and will move so that the sphere is not in the radius")]
    [SerializeField] private float cameraCollisionSphereRadius = 0.2f;
    [Tooltip("The amount of distance the camera will push itself when colliding")]
    [SerializeField] private float cameraCollisionOffSet = 0.2f;
    [Tooltip("How close the camera is allowed to get to the player")]
    [SerializeField] private float minimumCollisionOffset = 0.2f;
    [Tooltip("The y position of the camera pivot")]
    [SerializeField] private float lockedPivotPosition = 2.25f, unlockedPivotPosition = 1.65f;
    [Tooltip("The range that the raycast extends for finding possible lock on targets, extends both ways")]
    [SerializeField] [Range(0, 180)] private float lockOnMaxAngle=90;
    [Tooltip("How far the camera can scan for targets")]
    [SerializeField] private float maximumLockOnDistance = 30;

    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    private CharacterManager nearestLockOnTarget, leftLockTarget, rightLockTarget;
    internal CharacterManager currentLockOnTarget;

    //lock on cooldown
    private float lockOnSwapStamp, lockOnSwapCooldown = 1;

    [Header("DEBUGGING")]
    [SerializeField] private bool showGizmo = false;



    //consider deletion
    private Transform myTransform;//transform of the game object

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SwapToLeftLockOnTarget, OnSwapToLeftLockOnTarget);
        EventManager.currentManager.Subscribe(EventType.SwapToRightLockOnTarget, OnSwapToRightLockOnTarget);
    }

    private void Start()
    {
        SetupLockOnCamera();
    }

    private void LateUpdate()
    {
        if (inputHandler == null)
            return;

        if (inputHandler.lockOnFlag)
        {
            HandleCameraRotation();
            FollowTarget(Time.deltaTime);
            //if no target could be found, change back to exploration cam
            if (currentLockOnTarget == null)
            {
                EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
                inputHandler.lockOnFlag = false;
            }
        }
        else
        {
            //lock on camera should never be on if flag is off, if for some reason it is, swap to exploration camera
            if (lockOnCamera.activeSelf)
            {
                if (playerManager.GetComponent<PlayerAnimatorManager>().animator.GetBool("isAiming"))
                {
                    EventManager.currentManager.AddEvent(new SwapToAimCamera());
                }
                else
                {
                    EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
                }

            }
            currentLockOnTarget = null;
        }
    }

    private void SetupLockOnCamera()
    {
        //set the transform of this object
        myTransform = transform;

        //set the default position of the z
        defaultPosition = lockOnCameraTransform.localPosition.z;

        //set the camera's transform
        mainCameraTransform = Camera.main.transform;

        //set the player's transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        inputHandler = FindObjectOfType<InputHandler>();
        if (inputHandler == null)
            Debug.LogWarning(gameObject.name + " could not find an input handler, make sure you have one on your player");

        playerManager = FindObjectOfType<PlayerManager>();
    }
    public void FollowTarget(float delta)
    {
        //performs a lerp so that the camera moves smoothly to the target
        Vector3 targetPosition = Vector3.SmoothDamp
            (myTransform.position, playerTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation()
    {
        if (currentLockOnTarget != null)
        {
            //forcing camera to rotate towards the direction of the locked on target
            Vector3 direction = currentLockOnTarget.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0;

            //faced towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;

            direction = currentLockOnTarget.transform.position - lockOnCameraPivotTransform.position;
            direction.Normalize();

            targetRotation = Quaternion.LookRotation(direction);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            lockOnCameraPivotTransform.localEulerAngles = eulerAngle;
        }
        else
        {
            HandleLockOn();
            currentLockOnTarget = nearestLockOnTarget;
            //if not targets found, do not lock on
            if (availableTargets.Count == 0)
            {
                inputHandler.lockOnFlag = false;
            }
        }
    }

    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = lockOnCameraTransform.position - lockOnCameraPivotTransform.position;
        direction.Normalize();
        //raycast a sphere that goes around the camera, if it intercects with any colliders, return true
        if (Physics.SphereCast(playerTransform.position, cameraCollisionSphereRadius, direction, out hit,
            Mathf.Abs(targetPosition), cameraCollisionLayers))
        {
            //if it intersects, set target position to where it would not collide with the object
            float distance = Vector3.Distance(lockOnCameraPivotTransform.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffSet);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        Vector3 cameraTransformPosition = Vector3.zero;
        cameraTransformPosition.z = Mathf.Lerp(lockOnCameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        lockOnCameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        availableTargets = new List<CharacterManager>();

        //get character layer
        int characterLayer = LayerMask.GetMask("Character");

        //Creates a sphere to check fo any collisions
        Collider[] colliders = Physics.OverlapSphere(mainCameraTransform.position, maximumLockOnDistance, characterLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager characterManager = colliders[i].GetComponent<CharacterManager>();

            if (characterManager != null)
            {
                //Makes sure that the target is in the camera view to avoid locking onto targets behind camera
                Vector3 lockTargetDirection = characterManager.transform.position - playerTransform.position;
                float distanceFromTarget = Vector3.Distance(playerTransform.position, characterManager.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, mainCameraTransform.forward);

                RaycastHit hit;
                //Prevents locking onto self, sets within view distance and makes sure its not too far from the player
                if (characterManager.transform.root != playerTransform.transform.root && viewableAngle > -lockOnMaxAngle && viewableAngle < lockOnMaxAngle && distanceFromTarget <= maximumLockOnDistance)
                {
                    if (Physics.Linecast(playerManager.lockOnTransform.position, characterManager.transform.position, out hit))
                    {
                        Debug.DrawLine(playerManager.lockOnTransform.position, characterManager.transform.position);
                        if ((environmentLayer & (1 << hit.transform.gameObject.layer)) != 0)
                        {
                            //Cannot lock on target, object in the way
                        }
                        else
                        {
                            availableTargets.Add(characterManager);
                        } 
                    }
                }
            }
        }

        //search through available lock on targets
        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTargets = Vector3.Distance(playerTransform.position, availableTargets[k].transform.position);

                //check for closest target
                if (distanceFromTargets < shortestDistance)
                {
                    shortestDistance = distanceFromTargets;
                    nearestLockOnTarget = availableTargets[k];
                }


                if (inputHandler.lockOnFlag)
                {

                    Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromTarget = relativeEnemyPosition.x;

                    if (relativeEnemyPosition.x <= 0.00f && distanceFromTarget > shortestDistanceOfLeftTarget && availableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromTarget;
                        leftLockTarget = availableTargets[k];
                    }
                    else if (relativeEnemyPosition.x >= 0.00f && distanceFromTarget < shortestDistanceOfRightTarget && availableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromTarget;
                        rightLockTarget = availableTargets[k];
                    }
                }
            }
        }
    }

    public void ClearLockOnTarget()
    {
        //safety precaution, no touchy
        availableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }

    public void SetCameraHeight()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
        Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

        if (currentLockOnTarget != null)
        {
            lockOnCameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(lockOnCameraTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
        }
        else
        {
            lockOnCameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(lockOnCameraTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
            Gizmos.DrawSphere(mainCameraTransform.position, maximumLockOnDistance);
        }
    }

    #region On Events

    private void OnSwapToLeftLockOnTarget(EventData eventData)
    {
        if (eventData is SwapToLeftLockOnTarget)
        {
            if (inputHandler.lockOnFlag)
            {
                HandleLockOn();
                if (leftLockTarget != null)
                {
                    if (lockOnSwapStamp <= Time.time)
                    {
                        lockOnSwapStamp = Time.time + lockOnSwapCooldown;
                        currentLockOnTarget = leftLockTarget;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Attempting to swap camera but is not in lock on mode");
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SwapToLeftLockOnTarget was received but is not of class SwapToLeftLockOnTarget.");
        }
    }

    private void OnSwapToRightLockOnTarget(EventData eventData)
    {
        if (eventData is SwapToRightLockOnTarget)
        {
            if (inputHandler.lockOnFlag)
            {
                HandleLockOn();
                if (rightLockTarget != null)
                {
                    if (lockOnSwapStamp <= Time.time)
                    {
                        lockOnSwapStamp = Time.time + lockOnSwapCooldown;
                        currentLockOnTarget = rightLockTarget;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Attempting to swap camera but is not in lock on mode");
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SwapToRightLockOnTarget was received but is not of class SwapToRightLockOnTarget.");
        }
    }

    #endregion
}
