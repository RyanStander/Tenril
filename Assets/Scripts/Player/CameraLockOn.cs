using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockOn : MonoBehaviour
{

    #region Serialized Fields

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

    [Tooltip("Layers that the camera will collide with and be unable to pass through")]
    [SerializeField] private LayerMask cameraCollisionLayers;
    [SerializeField] private LayerMask environmentLayer;

    [Tooltip("The speed at which the camera follows the player")]
    [SerializeField] private float followSpeed = 0.1f;
    
    [Tooltip("How close the camera is allowed to get to the player")]
    [SerializeField] private float minimumCollisionOffset = 0.2f;
    [Tooltip("The y position of the camera pivot")]
    [SerializeField] private float lockedPivotPosition = 2.25f, unlockedPivotPosition = 1.65f;
    [Tooltip("The range that the raycast extends for finding possible lock on targets, extends both ways")]
    [SerializeField] [Range(0, 180)] private float lockOnMaxAngle=90;
    [Tooltip("How far the camera can scan for targets")]
    [SerializeField] private float maximumLockOnDistance = 30;
    
    [Header("DEBUGGING")]
    [SerializeField] private bool showGizmo = false;

    #endregion

    #region Private Fields

    private InputHandler inputHandler;
    private PlayerManager playerManager;
    private PlayerAnimatorManager playerAnimatorManager;
    
    private Vector3 cameraFollowVelocity = Vector3.zero;
    
    //these positions are used to reset the position of cameras so that following works properly
    private float targetPosition, defaultPosition;

    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    private CharacterManager nearestLockOnTarget, leftLockTarget, rightLockTarget;
    internal CharacterManager currentLockOnTarget;

    //lock on cooldown
    private float lockOnSwapStamp;
    private const float LockOnSwapCooldown = 1;

    //consider deletion
    private Transform myTransform;//transform of the game object
    
    private static readonly int IsAiming = Animator.StringToHash("isAiming");
    private bool isInputHandlerNull;

    #endregion
    
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SwapToLeftLockOnTarget, OnSwapToLeftLockOnTarget);
        EventManager.currentManager.Subscribe(EventType.SwapToRightLockOnTarget, OnSwapToRightLockOnTarget);
    }

    private void Start()
    {
        SetupLockOnCamera();
        playerAnimatorManager = playerManager.GetComponent<PlayerAnimatorManager>();
        isInputHandlerNull = inputHandler == null;
    }

    private void LateUpdate()
    {
        if (isInputHandlerNull)
            return;

        if (inputHandler.lockOnFlag)
        {
            HandleCameraRotation();
            FollowTarget(Time.deltaTime);
            
            //if no target could be found, change back to exploration cam
            if (currentLockOnTarget != null) 
                return;
            EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
            inputHandler.lockOnFlag = false;
        }
        else
        {
            //lock on camera should never be on if flag is off, if for some reason it is, swap to exploration camera
            if (lockOnCamera.activeSelf)
            {
                if (playerAnimatorManager.animator.GetBool(IsAiming))
                {
                    EventManager.currentManager.AddEvent(new SwapToAimCamera());
                }
                else
                {
                    EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
                }
            }
            myTransform.position= playerTransform.position;
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
        if (Camera.main != null) 
            mainCameraTransform = Camera.main.transform;

        //set the player's transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        inputHandler = FindObjectOfType<InputHandler>();
        if (inputHandler == null)
            Debug.LogWarning(gameObject.name + " could not find an input handler, make sure you have one on your player");

        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void FollowTarget(float delta)
    {
        //performs a lerp so that the camera moves smoothly to the target
        var smoothDamp = Vector3.SmoothDamp
            (myTransform.position, playerTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = smoothDamp;

        HandleCameraCollisions(delta);
    }

    private void HandleCameraRotation()
    {
        if (currentLockOnTarget != null)
        {
            //forcing camera to rotate towards the direction of the locked on target
            var currentLockOnTargetPosition = currentLockOnTarget.transform.position;
            var direction = currentLockOnTargetPosition - transform.position;
            direction.Normalize();
            direction.y = 0;

            //faced towards the target
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;

            direction = currentLockOnTargetPosition - lockOnCameraPivotTransform.position;
            direction.Normalize();

            targetRotation = Quaternion.LookRotation(direction);
            var eulerAngle = targetRotation.eulerAngles;
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
        var direction = lockOnCameraTransform.position - lockOnCameraPivotTransform.position;
        direction.Normalize();

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        var cameraTransformPosition = Vector3.zero;
        cameraTransformPosition.z = Mathf.Lerp(lockOnCameraTransform.localPosition.z, targetPosition, delta / 0.2f);

        lockOnCameraTransform.localPosition = cameraTransformPosition;

        var playerPoint = playerManager.finisherAttackRayCastStartPointTransform.position;
        var lockOnCameraPos = myTransform.position;
        var point = new Vector3(lockOnCameraPos.x,lockOnCameraPos.y,lockOnCameraTransform.position.z);
        //if colliding with any objects in line with the camera and player, move to collision point
        if (Physics.Linecast(playerPoint, lockOnCameraTransform.position, out var hit, cameraCollisionLayers))
            lockOnCameraTransform.position = hit.point;
    }

    private void HandleLockOn()
    {
        var shortestDistance = Mathf.Infinity;
        var shortestDistanceOfLeftTarget = -Mathf.Infinity;
        var shortestDistanceOfRightTarget = Mathf.Infinity;
        availableTargets = new List<CharacterManager>();

        //get character layer
        var characterLayer = LayerMask.GetMask("Character");

        //Creates a sphere to check fo any collisions
        var colliders = Physics.OverlapSphere(mainCameraTransform.position, maximumLockOnDistance, characterLayer);

        foreach (var targetCollider in colliders)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var characterManager = targetCollider.GetComponent<CharacterManager>();

            if (characterManager == null) 
                continue;
            
            //Makes sure that the target is in the camera view to avoid locking onto targets behind camera
            var playerPosition = playerTransform.position;
            var characterTransformPosition = characterManager.transform.position;
            var lockTargetDirection = characterTransformPosition - playerPosition;
            var distanceFromTarget = Vector3.Distance(playerPosition, characterTransformPosition);
            var viewableAngle = Vector3.Angle(lockTargetDirection, mainCameraTransform.forward);

            //Prevents locking onto self, sets within view distance and makes sure its not too far from the player
            if (characterManager.transform.root == playerTransform.transform.root ||
                !(viewableAngle > -lockOnMaxAngle) || !(viewableAngle < lockOnMaxAngle) ||
                !(distanceFromTarget <= maximumLockOnDistance)) continue;
            if (!Physics.Linecast(playerManager.lockOnTransform.position, characterManager.transform.position, out var hit)) 
                continue;
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

        //search through available lock on targets
        foreach (var characterManagerTarget in availableTargets)
        {
            if (characterManagerTarget == null) 
                continue;
            var distanceFromTargets = Vector3.Distance(playerTransform.position, characterManagerTarget.transform.position);

            //check for closest target
            if (distanceFromTargets < shortestDistance)
            {
                shortestDistance = distanceFromTargets;
                nearestLockOnTarget = characterManagerTarget;
            }


            if (!inputHandler.lockOnFlag) 
                continue;
            var relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(characterManagerTarget.transform.position);
            var distanceFromTarget = relativeEnemyPosition.x;

            if (relativeEnemyPosition.x <= 0.00f && distanceFromTarget > shortestDistanceOfLeftTarget && characterManagerTarget != currentLockOnTarget)
            {
                shortestDistanceOfLeftTarget = distanceFromTarget;
                leftLockTarget = characterManagerTarget;
            }
            else if (relativeEnemyPosition.x >= 0.00f && distanceFromTarget < shortestDistanceOfRightTarget && characterManagerTarget != currentLockOnTarget)
            {
                shortestDistanceOfRightTarget = distanceFromTarget;
                rightLockTarget = characterManagerTarget;
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
        var velocity = Vector3.zero;
        var newLockedPosition = new Vector3(0, lockedPivotPosition);
        var newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

        var lockOnCameraLocalPosition = lockOnCameraTransform.transform.localPosition;
        lockOnCameraPivotTransform.transform.localPosition = currentLockOnTarget != null ? 
            Vector3.SmoothDamp(lockOnCameraLocalPosition, newLockedPosition, ref velocity, Time.deltaTime) : 
            Vector3.SmoothDamp(lockOnCameraLocalPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) 
            return;
        
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
        Gizmos.DrawSphere(mainCameraTransform.position, maximumLockOnDistance);
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
                        lockOnSwapStamp = Time.time + LockOnSwapCooldown;
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
                        lockOnSwapStamp = Time.time + LockOnSwapCooldown;
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
