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

    //Other inputs
    public bool interactInput;

    //Combat Inputs
    public bool weakAttackInput, strongAttackInput,drawSheathInput, blockInput, parryInput, lockOnInput;

    //Spellcasting Inputs
    public bool spellcastingModeInput;
    public bool[] castSpell = new bool[8];

    //movement flags
    public bool sprintFlag;

    //combat flags
    public bool comboFlag,lockOnFlag;

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
        HandleLockOnInput();
    }

    //Reset the input bools so that they do not queue up for animations
    public void ResetInputs()
    {
        weakAttackInput = false;
        strongAttackInput = false;
        parryInput = false;
        drawSheathInput = false;
        dodgeInput = false;
        jumpInput = false;
        interactInput = false;

        for (int i = 0; i < 8; i++)
        {
            castSpell[i] = false;
        }
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
        //Lock On
        inputActions.PlayerCombat.LockOn.performed += i => lockOnInput = true;
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

        //----------------------------------------------------------
        //                         Spellcasting
        //----------------------------------------------------------
        #region Spellcasts
        //Enable Spellcasting
        inputActions.PlayerCombat.SpellcastingMode.performed += i => spellcastingModeInput = true;
        inputActions.PlayerCombat.SpellcastingMode.canceled += i => spellcastingModeInput = false;
        //1
        inputActions.PlayerSpellcasting.Spell1.performed += i => castSpell[0]=true;
        //2
        inputActions.PlayerSpellcasting.Spell2.performed += i => castSpell[1] = true;
        //3
        inputActions.PlayerSpellcasting.Spell3.performed += i => castSpell[2] = true;
        //4
        inputActions.PlayerSpellcasting.Spell4.performed += i => castSpell[3] = true;
        //5
        inputActions.PlayerSpellcasting.Spell5.performed += i => castSpell[4] = true;
        //6
        inputActions.PlayerSpellcasting.Spell6.performed += i => castSpell[5] = true;
        //7
        inputActions.PlayerSpellcasting.Spell7.performed += i => castSpell[6] = true;
        //8
        inputActions.PlayerSpellcasting.Spell8.performed += i => castSpell[7] = true;
        #endregion

        //----------------------------------------------------------
        //                         Other
        //----------------------------------------------------------
        //Interact
        inputActions.PlayerActions.Interact.performed += i => interactInput = true;
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

    private void HandleLockOnInput()
    {
        if (lockOnInput)
        {
            lockOnInput = false;
            lockOnFlag = !lockOnFlag;
        }
    }

    #endregion
}
