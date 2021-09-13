using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float left, forward, moveAmount;

    private PlayerController inputActions;

    //Movement inputs
    private Vector2 movementInput;
    [HideInInspector] public bool dodgeInput, sprintInput;

    //movement flags
    public bool sprintFlag;

    //combat flags
    public bool lockOnFlag=false;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            //if no input actions set, create one
            inputActions = new PlayerController();

            //Checks for inputs with input actions
            CheckInputs();
        }

        inputActions.Enable();
    }

    public void TickInput(float delta)
    {
        //Movement inputs  
        HandleMovementInput();
        HandleSprintInput();
    }

    private void CheckInputs()
    {
        //Movement
        inputActions.PlayerMovement.Movement.performed += movementInputActions => movementInput = movementInputActions.ReadValue<Vector2>();
        //Sprint
        inputActions.PlayerMovement.Sprint.performed += i => sprintInput = true;
        inputActions.PlayerMovement.Sprint.canceled += i => sprintInput = false;
        //Dodge
        inputActions.PlayerMovement.Dodge.performed += i => dodgeInput = true;
    }

    #region Movement
    private void HandleMovementInput()
    {
        left = movementInput.x;
        forward = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(left) + Mathf.Abs(forward));
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            //If character is currently moving forward
            if (moveAmount > 0.5f)
            {
                sprintFlag = true;
            }
        }
        else
        {
            sprintFlag = false;
        }
    }
    #endregion
}
