using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindingDisplay : MonoBehaviour
{
    [Tooltip("The input you want to modify")]
    public InputAction action;
    [Tooltip("Input handler from the player")]
    public InputHandler inputHandler;
    [Tooltip("The text that displays the current keybind")]
    [SerializeField] private TMP_Text keybindText;
    [Header("Keyboard Bindings")]
    [Tooltip("The text displaying the input binding key that is currently set")]
    [SerializeField] private TMP_Text keyboardStartRebindButtonText;
    [Tooltip("The button that will be pressed when starting a rebind")]
    [SerializeField] private GameObject keyboardStartRebindButton;
    [Tooltip("The object displayed while waiting for input")]
    [SerializeField] private GameObject keyboardWaitingForInputObject;
    [Header("Controller Bindings")]
    [Tooltip("The text displaying the input binding key that is currently set")]
    [SerializeField] private TMP_Text controllerStartRebindButtonText;
    [Tooltip("The button that will be pressed when starting a rebind")]
    [SerializeField] private GameObject controllerStartRebindButton;
    [Tooltip("The object displayed while waiting for input")]
    [SerializeField] private GameObject controllerWaitingForInputObject;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        //Sets the key to be rebinded
        //setAction = inputHandler.GetInputActions().asset.FindAction(action.name);

        //setup the display text on load
        keybindText.text = action.name;
        keyboardStartRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        controllerStartRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void RebindKey(int rebindValue)
    {
        switch (rebindValue)
        {
            //Keyboard
            case 0:
                StartRebinding(keyboardStartRebindButton, keyboardStartRebindButtonText, keyboardWaitingForInputObject, 0);
                break;
            //Controller
            case 1:
                StartRebinding(controllerStartRebindButton, controllerStartRebindButtonText, controllerWaitingForInputObject, 1);
                break;
            //Mouse
            case 2:
                //Might set up later
                break;
            default:
                Debug.Log("No matching value");
                break;
        }
    }


    private void StartRebinding(GameObject startRebindButton, TMP_Text startRebindButtonText, GameObject waitingForInputObject,int inputIndex)
    {
        //Change which objects are displayed so the player knows they have to press a key for input
        startRebindButton.SetActive(false);
        waitingForInputObject.SetActive(true);

        //Disable the input actions to allow for modifying inputs
        inputHandler.GetInputActions().Disable();

        //Make it so that normal inputs cant be read since this is a menu selection
        //Rebind to the key that was input
        switch (inputIndex)
        {
            //Keyboard
            case 0:
                rebindingOperation = action.PerformInteractiveRebinding().WithControlsExcluding("<Gamepad>").WithControlsExcluding("<Mouse>").
                    OnMatchWaitForAnother(0.1f).
                    OnComplete(operation => RebindComplete(startRebindButton, startRebindButtonText, waitingForInputObject, inputIndex)).
                    Start();
                break;
            //Controller
            case 1:
                rebindingOperation = action.PerformInteractiveRebinding().WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>").
                    OnMatchWaitForAnother(0.1f).
                    OnComplete(operation => RebindComplete(startRebindButton, startRebindButtonText, waitingForInputObject, inputIndex)).
                    Start();
                break;
            //Mouse
            case 2:
                //Might set up later
                break;
            default:
                Debug.Log("No matching value");
                break;
        }

    }

    private void RebindComplete(GameObject startRebindButton, TMP_Text startRebindButtonText, GameObject waitingForInputObject, int inputIndex)
    {
        //updates the text to display the new key
        startRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[inputIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //Dispose of the allocated memory from the function
        rebindingOperation.Dispose();

        //Toggle ui back
        startRebindButton.SetActive(true);
        waitingForInputObject.SetActive(false);

        //Swap back to gameplay inputs
        inputHandler.GetInputActions().Enable();
    }
}
