using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public PlayerAnimatorManager playerAnimatorManager;
    private InputHandler inputHandler;
    private Transform cameraObject;

    [SerializeField] private float rotationSpeed = 10;

    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        cameraObject = Camera.main.transform;
    }

    public void HandleLocomotion(float delta)
    {
        HandleMovement();
        HandleRotation(delta);
        HandleDodge();
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
        if (!playerAnimatorManager.canRotate)
            return;

        Vector3 targetDirection;
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

    public void HandleDodge()
    {
        if (inputHandler.dodgeInput)
        {
            inputHandler.dodgeInput = false;

            //Do not perform another dodge if already happening
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            //if the player is moving, roll.
            if (inputHandler.moveAmount > 0.5f)
                playerAnimatorManager.PlayTargetAnimation("Roll", true);
            //otherwise perform a backstep
            else
                playerAnimatorManager.PlayTargetAnimation("Backstep", true);
            //set dodge input to false again
        }

    }

    private void MovementType(bool isStafeMovement)
    {
        if (playerAnimatorManager.animator.GetBool("isInteracting"))
            return;

        //set the values for input
        float forwardMovement = inputHandler.forward;
        float leftMovement = inputHandler.left;
        float movementAmount = inputHandler.moveAmount;
        //if player is sprinting, double speed
        if (inputHandler.sprintFlag)
        {
            movementAmount = inputHandler.moveAmount * 2;
        }

        //if player is strafing, use both left and forward
        if (isStafeMovement)
        {
            playerAnimatorManager.animator.SetFloat("Forward", forwardMovement, 0.1f, Time.deltaTime);
            playerAnimatorManager.animator.SetFloat("Left", leftMovement, 0.1f, Time.deltaTime);
        }
        //otherwise use move amount to work with rotations
        else
        {
            playerAnimatorManager.animator.SetFloat("Forward", movementAmount, 0.1f, Time.deltaTime);
        }   
    }
}
