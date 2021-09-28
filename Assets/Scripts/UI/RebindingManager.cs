using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindingManager : MonoBehaviour
{
    [Tooltip("Input handler from the player")]
    public InputHandler inputHandler;
    [Tooltip("The prefab for the key rebind")]
    [SerializeField] private GameObject keyRebindPrefab;
    [SerializeField] private GameObject rebindContentObject;

    private void Start()
    {
        //sets the map of the input actions
        var actionMap = inputHandler.GetInputActions();
        //itterates through every input in action map
        foreach (InputAction action in actionMap)
        {
            //create a rebind prefab in the content box 
            GameObject createdRebindPrefab = Instantiate(keyRebindPrefab,rebindContentObject.transform);
            //get the rebinding display component on the prefab
            RebindingDisplay rebindingDisplay = createdRebindPrefab.GetComponent<RebindingDisplay>();
            //if there is a rebinding display component
            if (rebindingDisplay!=null)
            {
                //pass the values needed to the rebinding display prefab
                rebindingDisplay.inputHandler = inputHandler;
                rebindingDisplay.action = action;
            }
        }
    }
}
