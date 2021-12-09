using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager playerManager;
    public PlayerAnimatorManager playerAnimatorManager;
    private PlayerStats playerStats;
    private InputHandler inputHandler;
    private Transform cameraObject;
    private StatusEffectManager statusEffectManager;
    private CameraLockOn cameraLockOn;

    [Header("Ground & Air Detection")]
    [SerializeField] private float fallDuration=0;
    [SerializeField] private float fallDurationToPerformLand=0.5f;
    [Tooltip("The number that the environment layer is listed as")]
    [SerializeField] private int environmentLayerNumber=9;
    [SerializeField] private LayerMask EnvironmentLayer;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private float groundCheckRadius=0.3f;
    private Vector3 previousVelocity; //Used for when jumping to continue velocity until landing

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float aimSensitivity = 0.025f;
    [SerializeField] private float minimumPivot = -35, maximumPivot = 35;
    [SerializeField] private float sprintStaminaCost = 0.1f;
    [SerializeField] private float dodgeStaminaCost = 5f;

    private float lookAngle,pivotAngle;

    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerManager = GetComponent<PlayerManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStats = GetComponent<PlayerStats>();
        cameraObject = Camera.main.transform;
        statusEffectManager = GetComponent<StatusEffectManager>();
        cameraLockOn = FindObjectOfType<CameraLockOn>();

        EnvironmentLayer = 1<<environmentLayerNumber;
    }

    internal void HandleLocomotion(float delta)
    {
        HandleMovement();
        HandleRotation(delta);

        HandleFalling();
    }

    internal void HandleDodgeAndJumping()
    {
        HandleDodge();
        HandleJump();
    }
    private void HandleMovement()
    {
        //if player is locked on to a target and not sprinting
        if (inputHandler.lockOnFlag && !inputHandler.sprintFlag)
        {
            MovementType(true);
        }
        else//if player is sprinting or not locked on
        {
            MovementType(false);
        }
    }

    private void HandleRotation(float delta)
    {

        if (inputHandler.lockOnFlag)
        {
            if (!playerAnimatorManager.canRotate)
                return;

            LockOnRotation();
        }else if (playerManager.isAiming)
        {
            AimRotation(delta);
        }
        else
        {
            if (!playerAnimatorManager.canRotate)
                return;

            ExplorationRotation(delta);
        }
    }

    private void HandleDodge()
    {
        if (inputHandler.dodgeInput)
        {
            //Do not perform another dodge if already happening
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            //Do not perform a dodge if no stamina
            if (!playerStats.HasStamina())
                return;

            //Drain stamina for dodging
            playerStats.DrainStaminaWithCooldown(dodgeStaminaCost);

            //if the player is moving, roll.
            if (inputHandler.moveAmount > 0.5f)
                playerAnimatorManager.PlayTargetAnimation("Roll", true);
            //otherwise perform a backstep
            else
                playerAnimatorManager.PlayTargetAnimation("Backstep", true);
            //set dodge input to false again
        }

    }

    private void HandleJump()
    {
        if (inputHandler.jumpInput)
        {
            //Do not perform another jump if already happening
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            //perform jump animation
            playerAnimatorManager.PlayTargetAnimation("Jump", true);
        }
    }

    private void HandleFalling()
    {
        if (playerStats.isDead)
            return;

        if (!IsGrounded())
        {
            //keeps velocity while falling
            GetComponent<Rigidbody>().AddForce(new Vector3(previousVelocity.x * 25, 0, previousVelocity.z * 25));

            //count time of falling
            fallDuration += Time.deltaTime;

            //if player is currently doing another action, do not play fall animation,
            //this might need revision
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            //play fall animation
            playerAnimatorManager.PlayTargetAnimation("Falling", true);
        }
        else
        {
            //if player is falling for a long time, perform a land animation
            if (fallDuration>fallDurationToPerformLand)
            {
                //play land animation
                playerAnimatorManager.PlayTargetAnimation("Land", true);
            }
            else if (fallDuration>0)
            {
                //return to empty state
                playerAnimatorManager.PlayTargetAnimation("Empty", true);

                fallDuration = 0;
            }
            else
            {
                previousVelocity = GetComponent<Rigidbody>().velocity; 
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if the player fell for more than 1 second continue
        if (fallDuration < 1)
        {
            fallDuration = 0;
            return;
        }

        fallDuration = 0;

        //calculate fall damage based on the speed of which velocity is
        float damageMultiplier = 90 / (9.81f * 6.0f);

        ContactPoint contact = collision.contacts[0];
        Vector3 normal = contact.normal;
        Vector3 relativeVelocity = collision.relativeVelocity;
        float damage = Vector3.Dot(normal, relativeVelocity) * damageMultiplier;

        playerStats.TakeDamage(damage,false);
    }

    private bool IsGrounded()
    {
        //Check with a sphere if the player is on the ground, based on outcome, will either be set to being grounded or not
        if (Physics.CheckSphere(transform.position - raycastOffset, groundCheckRadius, EnvironmentLayer))
        {
            playerAnimatorManager.animator.SetBool("isGrounded", true);
            return true;
        }
        else
        {
            playerAnimatorManager.animator.SetBool("isGrounded", false);
            return false;
        }
    }

    private void MovementType(bool isStrafeMovement)
    {
        if (playerManager.isInteracting)
            return;

        //set the values for input
        float forwardMovement = inputHandler.forward;
        float leftMovement = inputHandler.left;
        float movementAmount = inputHandler.moveAmount;

        //if player is sprinting, double speed
        if (inputHandler.sprintFlag)
        {
            //if player does not have enough stamina, do not sprint
            if (!playerStats.HasStamina())
                movementAmount = inputHandler.moveAmount;
            //otherwise sprint
            else
            {
                movementAmount = inputHandler.moveAmount * 2;
                //Drain players stamina for sprinting
                playerStats.DrainStaminaWithCooldown(sprintStaminaCost);
            }
        }

        //if player is strafing, use both left and forward
        if (isStrafeMovement || playerManager.isAiming)
        {
            //Do not allow movement if the character is rooted
            if (statusEffectManager.GetIsRooted())
            {
                playerAnimatorManager.animator.SetFloat("Forward", 0, 0.1f, Time.deltaTime);
                playerAnimatorManager.animator.SetFloat("Left", 0, 0.1f, Time.deltaTime);
            }
            else
            {
                playerAnimatorManager.animator.SetFloat("Forward", forwardMovement, 0.1f, Time.deltaTime);
                playerAnimatorManager.animator.SetFloat("Left", -leftMovement, 0.1f, Time.deltaTime);
            }
        }
        //otherwise use move amount to work with rotations
        else
        {    //Do not allow movement if the character is rooted
            if (statusEffectManager.GetIsRooted())
            {
                playerAnimatorManager.animator.SetFloat("Forward", 0, 0.1f, Time.deltaTime);
                playerAnimatorManager.animator.SetFloat("Left", 0, 0.1f, Time.deltaTime);
            }
            else
            {
                playerAnimatorManager.animator.SetFloat("Forward", movementAmount, 0.1f, Time.deltaTime);
                //ensure left is 0 to avoid strange movement out of lock on
                playerAnimatorManager.animator.SetFloat("Left", 0, 0.1f, Time.deltaTime);
            }
        }   
    }

    private void AimRotation(float delta)
    {
        lookAngle += inputHandler.lookInput.x * aimSensitivity / delta;
        pivotAngle -= inputHandler.lookInput.y * aimSensitivity / delta;
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        GameObject cameraFollowPoint = GameObject.FindGameObjectWithTag("CameraFollowTarget");
        cameraFollowPoint.transform.localRotation = targetRotation;
    }

    private void LockOnRotation()
    {
        Vector3 targetDirection;
        //if player is locked on perform the following movements
        if (inputHandler.sprintFlag || inputHandler.dodgeInput)
        {
            targetDirection = cameraObject.forward * inputHandler.forward;
            targetDirection += cameraObject.right * inputHandler.left;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }

            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

            transform.rotation = targetRotation;
        }
        else
        {
            if (cameraLockOn.currentLockOnTarget != null)
            {
                Vector3 rotationDirection;
                rotationDirection = cameraLockOn.currentLockOnTarget.transform.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }


            /*targetDirection = cameraObject.forward;

            targetDirection.y = 0;

            transform.rotation = Quaternion.LookRotation(targetDirection);*/
        }
    }

    private void ExplorationRotation(float delta)
    {
        Vector3 targetDirection;
        //If player is in exploration mode, perform following movement

        //Sets direction in relation towards the camera
        targetDirection = cameraObject.forward * inputHandler.forward;
        targetDirection += cameraObject.right * inputHandler.left;

        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion tr = Quaternion.LookRotation(targetDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * delta);

        transform.rotation = targetRotation;
    }
}
