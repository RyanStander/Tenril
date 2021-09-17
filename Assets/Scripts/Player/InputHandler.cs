using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float left, forward, moveAmount;

    private PlayerController inputActions;

    //Movement inputs
    private Vector2 movementInput;
    [HideInInspector] public bool dodgeInput, sprintInput, jumpInput;

    //Combat Inputs
    public bool weakAttackInput, strongAttackInput,drawSheathInput, blockInput, parryInput;

    //movement flags
    public bool sprintFlag;

    //combat flags
    public bool lockOnFlag=false,comboFlag;

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
        //----------------------------------------------------------
        //                         Locomotion
        //----------------------------------------------------------
        //Movement
        inputActions.PlayerMovement.Movement.performed += movementInputActions => movementInput = movementInputActions.ReadValue<Vector2>();
        //Sprint
        inputActions.PlayerMovement.Sprint.performed += i => sprintInput = true;
        inputActions.PlayerMovement.Sprint.canceled += i => sprintInput = false;
        //Dodge
        inputActions.PlayerMovement.Dodge.performed += i => dodgeInput = true;
        //Jump
        inputActions.PlayerMovement.Jump.performed += i => jumpInput = true;

        //----------------------------------------------------------
        //                         Combat
        //----------------------------------------------------------
        //Swap weapon
        inputActions.PlayerActions.DrawSheath.performed += i => drawSheathInput = true;
        //Weak attack
        inputActions.PlayerCombat.WeakAttack.performed += i => weakAttackInput = true;
        //Strong Attack
        inputActions.PlayerCombat.StrongAttack.performed += i => strongAttackInput = true;
        //Block
        inputActions.PlayerCombat.Block.performed += i => blockInput = true;
        inputActions.PlayerCombat.Block.canceled += i => blockInput = false;
        //Parry
        inputActions.PlayerCombat.Parry.performed += i => parryInput = true;
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

    #region Combat

    #endregion
}
