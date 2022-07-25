using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputHandler : MonoBehaviour
    {
        //The input device that is being used by the player
        public InputDeviceType activeInputDevice = InputDeviceType.KeyboardMouse;

        public float left, forward, moveAmount;

        private PlayerController inputActions;

        public DeviceDisplayConfigurator deviceDisplayConfigurator;

        //Movement inputs
        public Vector2 lookInput;
        private Vector2 movementInput;
        [HideInInspector] public bool dodgeInput, sprintInput, jumpInput;
        [HideInInspector] public float lockOnTargetInput;

        //Other inputs
        public bool interactInput, menuInput, mapInput, inventoryInput, characterStatsInput, alternateInteraction;

        //Combat Inputs
        public bool weakAttackInput,
            attackLetGoInput,
            strongAttackInput,
            drawSheathInput,
            blockInput,
            parryInput,
            lockOnInput;

        public bool quickslotLeftInput, quickslotRightInput, quickslotUseInput;

        //Spellcasting Inputs
        public bool spellcastingModeInput;
        public bool[] castSpell = new bool[8];

        //movement flags
        public bool sprintFlag;

        //combat flags
        public bool comboFlag, lockOnFlag;

        private void OnEnable()
        {
            InputSystem.onActionChange += OnInputDeviceChange;
        }

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
            //Gets the keybindings saved from player prefs
            var rebinds = PlayerPrefs.GetString("keybindings", string.Empty);

            //checks if its null
            if (string.IsNullOrEmpty(rebinds))
                return;

            //loads bindings and overrides original ones
            inputActions.asset.LoadBindingOverridesFromJson(rebinds);
        }

        internal void TickInput()
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
            characterStatsInput = false;
            inventoryInput = false;
            mapInput = false;
            alternateInteraction = false;

            quickslotLeftInput = false;
            quickslotRightInput = false;
            quickslotUseInput = false;

            for (var i = 0; i < 8; i++)
            {
                castSpell[i] = false;
            }
        }

        internal void ResetMovementValues()
        {
            forward = 0;
            left = 0;
            moveAmount = 0;
            movementInput = Vector2.zero;
        }

        private void CheckInputs()
        {
            //----------------------------------------------------------
            //                         Locomotion
            //----------------------------------------------------------

            #region Locomotion

            //Look
            inputActions.CharacterControls.Look.performed +=
                lookInputActions => lookInput = lookInputActions.ReadValue<Vector2>();
            //Movement
            inputActions.CharacterControls.Movement.performed += movementInputActions =>
                movementInput = movementInputActions.ReadValue<Vector2>();
            //Sprint
            inputActions.CharacterControls.Sprint.performed += i => sprintInput = true;
            inputActions.CharacterControls.Sprint.canceled += i => sprintInput = false;
            //Dodge
            inputActions.CharacterControls.Dodge.performed += i => dodgeInput = true;
            //Jump
            inputActions.CharacterControls.Jump.performed += i => jumpInput = true;

            #endregion

            //----------------------------------------------------------
            //                         Combat
            //----------------------------------------------------------

            #region Combat

            //Lock On
            inputActions.CharacterControls.LockOn.performed += i => lockOnInput = true;
            inputActions.CharacterControls.LockOnSwapTargetInput.performed += lockOnTargetInputActions =>
                lockOnTargetInput = lockOnTargetInputActions.ReadValue<float>();
            //Swap weapon
            inputActions.CharacterControls.WeaponSwap.performed += i => drawSheathInput = true;
            //Weak attack
            inputActions.CharacterControls.WeakAttack.performed += i => weakAttackInput = true;
            //attack let go
            inputActions.CharacterControls.WeakAttack.canceled += i => attackLetGoInput = true;
            inputActions.CharacterControls.StrongAttack.canceled += i => attackLetGoInput = true;
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

            #endregion

            //----------------------------------------------------------
            //                         Spellcasting
            //----------------------------------------------------------

            #region Spellcasts

            //Enable Spellcasting
            inputActions.CharacterControls.SpellcastingMode.performed += i => spellcastingModeInput = true;
            inputActions.CharacterControls.SpellcastingMode.canceled += i => spellcastingModeInput = false;
            //1
            inputActions.CharacterControls.Spell1.performed += i => castSpell[0] = true;
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
            //Alternate Interaction
            inputActions.CharacterControls.AlternateInteraction.performed += i => alternateInteraction = true;
            //Menu
            inputActions.CharacterControls.Menu.performed += i => menuInput = true;
            inputActions.UIControls.Menu.performed += i => menuInput = true;
            //UI interactions
            inputActions.CharacterControls.Inventory.performed += i => inventoryInput = true;
            inputActions.UIControls.Inventory.performed += i => inventoryInput = true;
            inputActions.CharacterControls.OpenStatSheet.performed += i => characterStatsInput = true;
            inputActions.UIControls.OpenStatSheet.performed += i => characterStatsInput = true;
            inputActions.CharacterControls.Map.performed += i => mapInput = true;
            inputActions.UIControls.Map.performed += i => mapInput = true;
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

            switch (activeInputDevice)
            {
                case InputDeviceType.KeyboardMouse:
                    HandleKeyboardMouseLockOnInputSwapping();
                    break;
                case InputDeviceType.GeneralGamepad:
                    HandleControllerLockOnInputSwapping();
                    break;
                case InputDeviceType.PlayStation:
                    HandleControllerLockOnInputSwapping();
                    break;
                case InputDeviceType.Xbox:
                    HandleControllerLockOnInputSwapping();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleControllerLockOnInputSwapping()
        {
            //Move to the next left target
            if (lockOnFlag && lockOnTargetInput < -0.7f)
            {
                EventManager.currentManager.AddEvent(new SwapToLeftLockOnTarget());
            } //Move to the next right target
            else if (lockOnFlag && lockOnTargetInput > 0.7f)
            {
                EventManager.currentManager.AddEvent(new SwapToRightLockOnTarget());
            }
        }

        private void HandleKeyboardMouseLockOnInputSwapping()
        {
            //Move to the next left target
            if (lockOnFlag && lockOnTargetInput < -5f)
            {
                EventManager.currentManager.AddEvent(new SwapToLeftLockOnTarget());
            } //Move to the next right target
            else if (lockOnFlag && lockOnTargetInput > 5f)
            {
                EventManager.currentManager.AddEvent(new SwapToRightLockOnTarget());
            }
        }

        public PlayerController GetInputActions()
        {
            return inputActions;
        }

        #endregion


        #region OnEvents

        private void OnInputDeviceChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;
            var inputAction = (InputAction) obj;
            var lastControl = inputAction.activeControl;
            var lastDevice = lastControl.device;

            //Some controller joysticks spam these values, this function prevents from reading 0 values
            if (inputAction.name == "Navigate" || inputAction.name == "Movement" || inputAction.name == "Look")
                if (inputAction.ReadValue<Vector2>().magnitude < 0.001f)
                    return;

            var currentInputDevice = GetInputDeviceType(lastDevice.device.layout);

            //Check if the active device is different from the one that was just inputed
            if (currentInputDevice == activeInputDevice) return;
            //if it is change it to be the same
            activeInputDevice = currentInputDevice;
            EventManager.currentManager.AddEvent(new PlayerChangedInputDevice(activeInputDevice));
            Debug.Log($"Device changed to: {currentInputDevice}");
        }

        private static InputDeviceType GetInputDeviceType(string deviceLayout)
        {
            //If we find more controllers we can expand on this

            //Checks for the layout name of the device and returns an input device
            return deviceLayout switch
            {
                "XInputControllerWindows" => InputDeviceType.Xbox,
                "DualShock4GamepadHID" => InputDeviceType.PlayStation,
                "DualShock3GamepadHID" => InputDeviceType.PlayStation,
                _ => InputDeviceType.KeyboardMouse
            };
        }

        #endregion
    }
}