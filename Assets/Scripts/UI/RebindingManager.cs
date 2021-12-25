using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class RebindingManager : MonoBehaviour
{
    //Input handler from the player
    [HideInInspector]public InputHandler inputHandler;
    [Tooltip("The prefab for the key rebind")]
    [SerializeField] private GameObject keyRebindPrefab;
    [SerializeField] private GameObject rebindContentObject;

    private void OnEnable()
    {
        inputHandler = FindObjectOfType<InputHandler>();
        CreateKeybindingDisplay();
    }

    public void SaveKeybindings()
    {
        //get the keys
        string rebinds = inputHandler.GetInputActions().asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("keybindings", rebinds);

    }

    public void ResetKeybindings()
    {
        inputHandler.GetInputActions().asset.RemoveAllBindingOverrides();

        //get the keys
        string rebinds = inputHandler.GetInputActions().asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("keybindings", rebinds);

        CreateKeybindingDisplay();
    }
    private void CreateKeybindingDisplay()
    {
        foreach (Transform child in rebindContentObject.transform)
        {
            Destroy(child.gameObject);
        }

        //sets the map of the input actions
        var actionMap = inputHandler.GetInputActions();
        //itterates through every input in action map
        foreach (InputAction action in actionMap)
        {
            //check if it is a 2d vector composite
            if (action.bindings[0].isComposite)
            {
                int secondComposite = 0;
                //if it is a composite, check for second composite
                for (int i = 1; i < action.bindings.Count - 1; i++)
                {
                    if (action.bindings[i].isComposite)
                    {
                        secondComposite = i;
                    }
                }
                for (int i = 1; i < action.bindings.Count - secondComposite; i++)
                {
                    //create a rebind prefab in the content box 
                    GameObject createdRebindPrefab = Instantiate(keyRebindPrefab, rebindContentObject.transform);
                    //get the rebinding display component on the prefab
                    RebindingDisplay rebindingDisplay = createdRebindPrefab.GetComponent<RebindingDisplay>();
                    //if there is a rebinding display component
                    if (rebindingDisplay != null)
                        //each value between composites needs to have a respective rebinding display created for it
                        rebindingDisplay.InputStartingValues(inputHandler, action, true, i, i + secondComposite);
                }
            }
            else
            {
                //Prevents pass through controls from being modified
                //Simply, I don't want the user to be able to.
                if (action.type != InputActionType.PassThrough)
                {
                    //create a rebind prefab in the content box 
                    GameObject createdRebindPrefab = Instantiate(keyRebindPrefab, rebindContentObject.transform);
                    //get the rebinding display component on the prefab
                    RebindingDisplay rebindingDisplay = createdRebindPrefab.GetComponent<RebindingDisplay>();
                    //if there is a rebinding display component
                    if (rebindingDisplay != null)
                        //pass the values needed to the rebinding display prefab
                        rebindingDisplay.InputStartingValues(inputHandler, action);
                }
            }
        }
    }
}
