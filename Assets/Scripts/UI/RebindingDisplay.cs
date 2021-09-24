using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindingDisplay : MonoBehaviour
{
    [Tooltip("The input you want to modify")]
    [SerializeField] private InputActionReference action;
    private InputAction setAction;
    [Tooltip("Input handler from the play")]
    [SerializeField] private InputHandler inputHandler;
    [Tooltip("The text displaying the input binding key that is currently set")]
    [SerializeField] private TMP_Text bindingDisplayNameText;
    [Tooltip("The button that will be pressed when starting a rebind")]
    [SerializeField] private GameObject startRebindObject;
    [Tooltip("The object displayed while waiting for input")]
    [SerializeField] private GameObject waitingForInputObject;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebinding()
    {
        //Change which objects are displayed so the player knows they have to press a key for input
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        //Disable the input actions to allow for modifying inputs
        inputHandler.GetInputActions().Disable();

        //Sets the key to be rebinded
        setAction = inputHandler.GetInputActions().asset.FindAction(action.name);

        //Make it so that normal inputs cant be read since this is a menu selection
        //Rebind to the key that was input
        rebindingOperation = setAction.PerformInteractiveRebinding().WithControlsExcluding("Mouse").
            OnMatchWaitForAnother(0.1f).
            OnComplete(operation => RebindComplete()).
            Start();
    }

    private void RebindComplete()
    {
        //gets the index of the control that is being used
        //currentl 0 since that would be keyboard for testing purposes
        int bindingIndex = action.action.GetBindingIndexForControl(action.action.controls[0]);

        //updates the text to display the new key
        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(setAction.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //Dispose of the allocated memory from the function
        rebindingOperation.Dispose();

        //Toggle ui back
        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);

        //Swap back to gameplay inputs
        inputHandler.GetInputActions().Enable();

    }
}
