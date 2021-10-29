using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public float left, forward, moveAmount;

    private PlayerController inputActions;

    //Movement inputs
    private Vector2 movementInput;
    [HideInInspector] public bool dodgeInput, sprintInput, jumpInput;

    //Other inputs
    public bool interactInput, menuInput;

    //Combat Inputs
    public bool weakAttackInput, strongAttackInput,drawSheathInput, blockInput, parryInput, lockOnInput;
    public bool quickslotLeftInput, quickslotRightInput, quickslotUseInput;

    //Spellcasting Inputs
    public bool spellcastingModeInput;
    public bool[] castSpell = new bool[8];

    //movement flags
    public bool sprintFlag;

    //combat flags
    public bool comboFlag,lockOnFlag;

    private void Awake()
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

    private void Start()
    {
        //Gets the keybinginds saved from player prefs
        string rebinds = PlayerPrefs.GetString("keybindings", string.Empty);

        //checks if its null
        if (string.IsNullOrEmpty(rebinds))
            return;

        //loads binginds and overrides original ones
        inputActions.asset.LoadBindingOverridesFromJson(rebinds);
    }

    internal void TickInput(float delta)
    {
        //Movement inputs  
        HandleMovementInput();
        HandleSprintInput();
        HandleLockOnInput();
    }

    //Reset the input bools so that they do not queue up for animations
    internal void ResetInputs()
    {
        weakAttackInput = false;
        strongAttackInput = false;
        parryInput = false;
        drawSheathInput = false;
        dodgeInput = false;
        jumpInput = false;
        interactInput = false;
        menuInput = false;

        quickslotLeftInput = false;
        quickslotRightInput = false;
        quickslotUseInput = false;

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
        inputActions.CharacterControls.Movement.performed += movementInputActions => movementInput = movementInputActions.ReadValue<Vector2>();
        //Sprint
        inputActions.CharacterControls.Sprint.performed += i => sprintInput = true;
        inputActions.CharacterControls.Sprint.canceled += i => sprintInput = false;
        //Dodge
        inputActions.CharacterControls.Dodge.performed += i => dodgeInput = true;
        //Jump
        inputActions.CharacterControls.Jump.performed += i => jumpInput = true;

        //----------------------------------------------------------
        //                         Combat
        //----------------------------------------------------------
        //Lock On
        inputActions.CharacterControls.LockOn.performed += i => lockOnInput = true;
        //Swap weapon
        inputActions.CharacterControls.DrawSheath.performed += i => drawSheathInput = true;
        //Weak attack
        inputActions.CharacterControls.WeakAttack.performed += i => weakAttackInput = true;
        //Strong Attack
        inputActions.CharacterControls.StrongAttack.performed += i => strongAttackInput = true;
        //Block
        inputActions.CharacterControls.Block.performed += i => blockInput = true;
        inputActions.CharacterControls.Block.canceled += i => blockInput = false;
        //Parry
        inputActions.CharacterControls.Parry.performed += i => parryInput = true;
        //Quickslot left
        inputActions.CharacterControls.QuickslotLeft.performed += i => quickslotLeftInput = true;
        //Quickslot right
        inputActions.CharacterControls.QuickslotRight.performed += i => quickslotRightInput = true;
        //Quickslot use
        inputActions.CharacterControls.QuickslotUse.performed += i => quickslotUseInput = true;

        //----------------------------------------------------------
        //                         Spellcasting
        //----------------------------------------------------------
        #region Spellcasts
        //Enable Spellcasting
        inputActions.CharacterControls.SpellcastingMode.performed += i => spellcastingModeInput = true;
        inputActions.CharacterControls.SpellcastingMode.canceled += i => spellcastingModeInput = false;
        //1
        inputActions.CharacterControls.Spell1.performed += i => castSpell[0]=true;
        //2
        inputActions.CharacterControls.Spell2.performed += i => castSpell[1] = true;
        //3
        inputActions.CharacterControls.Spell3.performed += i => castSpell[2] = true;
        //4
        inputActions.CharacterControls.Spell4.performed += i => castSpell[3] = true;
        //5
        inputActions.CharacterControls.Spell5.performed += i => castSpell[4] = true;
        //6
        inputActions.CharacterControls.Spell6.performed += i => castSpell[5] = true;
        //7
        inputActions.CharacterControls.Spell7.performed += i => castSpell[6] = true;
        //8
        inputActions.CharacterControls.Spell8.performed += i => castSpell[7] = true;
        #endregion

        //----------------------------------------------------------
        //                         Other
        //----------------------------------------------------------
        //Interact
        inputActions.CharacterControls.Interact.performed += i => interactInput = true;
        //Open/Close Menu
        inputActions.CharacterControls.OpenMenu.performed += i => menuInput = true;
        inputActions.UIControls.CloseMenus.performed += i => menuInput = true;
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
            if (lockOnFlag)
                //send out event to swap to lock on camera
                EventManager.currentManager.AddEvent(new SwapToLockOnCamera());
            else
                //send out event to swap to exploration camera
                EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
        }
    }

    public PlayerController GetInputActions()
    {
        return inputActions;
    }

    #endregion
}
