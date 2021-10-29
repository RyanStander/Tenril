using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    [Header("Player Values")]
    [Tooltip("Input handler from the player")]
    [SerializeField] private InputHandler inputHandler;
    [Tooltip("The animator of the player")]
    [SerializeField] private Animator playerAnimator;

    [Header("UI Objects")]
    [SerializeField] private GameObject InGameGUI, MenuGUI;
    [SerializeField] private List<GameObject> ExtraMenuGUIs;

    private bool isInMenuMode;
    void Start()
    {
        if (inputHandler==null)
            Debug.Log("Input handler for the UI manager was not set. Drag it into the inspector");
    }

    private void Update()
    {
        ManageActionMaps();
    }

    private void ManageActionMaps()
    {
        //checks if input handler was assigned
        if (inputHandler == null)
            return;
        //if menu button has been pressed
        if (inputHandler.menuInput)
        {
            //swap menu mode
            isInMenuMode = !isInMenuMode;
            //activates respective action maps depending on current mode
            if (isInMenuMode)
            {
                //Disable gameplay inputs and enable menu inputs
                inputHandler.GetInputActions().CharacterControls.Disable();
                inputHandler.GetInputActions().UIControls.Enable();
                //swap to in menu screen
                InGameGUI.SetActive(false);
                MenuGUI.SetActive(true);

                inputHandler.lockOnFlag = false;
                //send out event to swap to menu camera
                EventManager.currentManager.AddEvent(new SwapToMenuCamera());
            }
            else
            {
                //Enable gameplay inputs and disable menu inputs
                inputHandler.GetInputActions().CharacterControls.Enable();
                inputHandler.GetInputActions().UIControls.Disable();
                //swap to in game screen
                InGameGUI.SetActive(true);
                MenuGUI.SetActive(false);

                //Disable extra menus
                foreach (GameObject menuToDisable in ExtraMenuGUIs)
                {
                    menuToDisable.SetActive(false);
                }

                //destroy inventory option holders
                EventManager.currentManager.AddEvent(new DestroyInventoryOptionHolders());

                //send out event to swap to exploration camera
                EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
            }

        }
    }
}
