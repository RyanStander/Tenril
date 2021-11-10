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
    [SerializeField] private GameObject inGameGUI;
    [SerializeField] private GameObject mainMenu,inventoryDisplay,rebindingDisplay;
    [SerializeField] private GameObject dialoguePopUp;

    private bool isInMenuMode;
    private bool isInDialogueMode;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.InitiateDialogue, OnInitiateDialogue);
        EventManager.currentManager.Subscribe(EventType.CeaseDialogue, OnCeaseDialogue);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.InitiateDialogue, OnInitiateDialogue);
        EventManager.currentManager.Unsubscribe(EventType.CeaseDialogue, OnCeaseDialogue);
    }
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
            if (isInDialogueMode)
            {
                EventManager.currentManager.AddEvent(new CeaseDialogue());
            }
            else
            {
                //swap menu mode
                isInMenuMode = !isInMenuMode;
                //activates respective action maps depending on current mode
                if (isInMenuMode)
                {
                    //Disable gameplay inputs and enable menu inputs
                    inputHandler.GetInputActions().CharacterControls.Disable();
                    //swap to in menu screen
                    inGameGUI.SetActive(false);
                    mainMenu.SetActive(true);

                    inputHandler.lockOnFlag = false;
                    //send out event to swap to menu camera
                    EventManager.currentManager.AddEvent(new SwapToMenuCamera());
                }
                else
                {
                    //Enable gameplay inputs and disable menu inputs
                    inputHandler.GetInputActions().CharacterControls.Enable();
                    //swap to in game screen
                    inGameGUI.SetActive(true);
                    mainMenu.SetActive(false);

                    //Disable extra menus
                    inventoryDisplay.SetActive(false);
                    rebindingDisplay.SetActive(false);

                    //destroy inventory option holders
                    EventManager.currentManager.AddEvent(new DestroyInventoryOptionHolders());

                    //send out event to swap to exploration camera
                    EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
                }
            }

        }
    }

    #region onEvents

    private void OnInitiateDialogue(EventData eventData)
    {
        if (eventData is InitiateDialogue)
        {
            inGameGUI.SetActive(false);

            dialoguePopUp.SetActive(true);

            isInDialogueMode = true;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.InitiateDialogue was received but is not of class InitiateDialogue.");
        }
    }

    private void OnCeaseDialogue(EventData eventData)
    {
        if (eventData is CeaseDialogue)
        {
            inGameGUI.SetActive(true);

            dialoguePopUp.SetActive(false);

            isInDialogueMode = false;

            inputHandler.ResetInputs();

            //swap to exploration camera
            EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.CeaseDialogue was received but is not of class CeaseDialogue.");
        }
    }

    #endregion
}
