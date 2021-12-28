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

    private bool isComposite = false;
    private int compositeSpecificBindingKeyboard=0, compositeSpecificBindingController=1;

    private void Start()
    {
        //Sets the key to be rebinded
        //setAction = inputHandler.GetInputActions().asset.FindAction(action.name);

        //Composites have large binding sizes so it requires some extra coding work to find the correct on
        if (isComposite)
        {
            //setup the display text on load
            keybindText.text =  action.name+ " " +action.bindings[compositeSpecificBindingKeyboard].name;
            keyboardStartRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[compositeSpecificBindingKeyboard].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            controllerStartRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[compositeSpecificBindingController].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
        {
            //setup the display text on load
            keybindText.text = action.name;
            keyboardStartRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            controllerStartRebindButtonText.text = InputControlPath.ToHumanReadableString(action.bindings[1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            Debug.Log(action.bindings[compositeSpecificBindingController].effectivePath);
            Debug.Log(action.name);
        }
    }

    public void InputStartingValues(InputHandler inputHandler,InputAction action, bool isComposite=false,int compositeSpecificBindingKeyboard=0, int compositeSpecificBindingController=1)
    {
        this.inputHandler = inputHandler;
        this.action = action;
        this.isComposite = isComposite;
        this.compositeSpecificBindingKeyboard = compositeSpecificBindingKeyboard;
        this.compositeSpecificBindingController = compositeSpecificBindingController;
    }

    public void RebindKey(int rebindValue)
    {
        switch (rebindValue)
        {
            //Keyboard
            case 0:
                if (isComposite)
                    StartRebinding(keyboardStartRebindButton, keyboardStartRebindButtonText, keyboardWaitingForInputObject,rebindValue, compositeSpecificBindingKeyboard);
                else
                    StartRebinding(keyboardStartRebindButton, keyboardStartRebindButtonText, keyboardWaitingForInputObject, rebindValue, 0);
                break;
            //Controller
            case 1:
                if (isComposite)
                    StartRebinding(controllerStartRebindButton, controllerStartRebindButtonText, controllerWaitingForInputObject, rebindValue, compositeSpecificBindingController);
                else
                    StartRebinding(controllerStartRebindButton, controllerStartRebindButtonText, controllerWaitingForInputObject, rebindValue, 1);
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


    private void StartRebinding(GameObject startRebindButton, TMP_Text startRebindButtonText, GameObject waitingForInputObject, int rebindValue, int inputIndex)
    {
        //Change which objects are displayed so the player knows they have to press a key for input
        startRebindButton.SetActive(false);
        waitingForInputObject.SetActive(true);

        //Disable the input actions to allow for modifying inputs
        inputHandler.GetInputActions().UIControls.Disable();

        //Rebind to the key that was input
        switch (rebindValue)
        {
            //Keyboard
            case 0:
                if (isComposite)
                    rebindingOperation = action.PerformInteractiveRebinding(compositeSpecificBindingKeyboard).WithControlsExcluding("<Gamepad>").WithControlsExcluding("<Mouse>").
                        OnMatchWaitForAnother(0.1f).
                        OnComplete(operation => RebindComplete(startRebindButton, startRebindButtonText, waitingForInputObject, inputIndex)).
                        Start();
                else
                    rebindingOperation = action.PerformInteractiveRebinding().WithControlsExcluding("<Gamepad>").WithControlsExcluding("<Mouse>").
                        OnMatchWaitForAnother(0.1f).
                        OnComplete(operation => RebindComplete(startRebindButton, startRebindButtonText, waitingForInputObject, inputIndex)).
                        Start();
                break;
            //Controller
            case 1:
                if (isComposite)
                    rebindingOperation = action.PerformInteractiveRebinding(compositeSpecificBindingController).WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>").
                        OnMatchWaitForAnother(0.1f).
                        OnComplete(operation => RebindComplete(startRebindButton, startRebindButtonText, waitingForInputObject, inputIndex)).
                        Start();
                else
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
        inputHandler.GetInputActions().UIControls.Enable();
    }
}
