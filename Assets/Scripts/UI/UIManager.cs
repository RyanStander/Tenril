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
                inputHandler.GetInputActions().PlayerActions.Disable();
                inputHandler.GetInputActions().PlayerCombat.Disable();
                inputHandler.GetInputActions().PlayerMovement.Disable();
                inputHandler.GetInputActions().PlayerSpellcasting.Disable();
                inputHandler.GetInputActions().MenuNavigation.Enable();
                //swap to in menu screen
                InGameGUI.SetActive(false);
                MenuGUI.SetActive(true);

                if (playerAnimator != null)
                    playerAnimator.SetBool("isInMenu", true);
            }
            else
            {
                //Enable gameplay inputs and disable menu inputs
                inputHandler.GetInputActions().PlayerActions.Enable();
                inputHandler.GetInputActions().PlayerCombat.Enable();
                inputHandler.GetInputActions().PlayerMovement.Enable();
                inputHandler.GetInputActions().PlayerSpellcasting.Enable();
                inputHandler.GetInputActions().MenuNavigation.Disable();
                //swap to in game screen
                InGameGUI.SetActive(true);
                MenuGUI.SetActive(false);

                if (playerAnimator != null)
                    playerAnimator.SetBool("isInMenu", false);
            }

        }
    }
}
