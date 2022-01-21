using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Player Values")]
    [Tooltip("Input handler from the player")]
    [SerializeField] private InputHandler inputHandler;
    [Tooltip("The animator of the player")]
    [SerializeField] private Animator playerAnimator;

    [Header("UI Objects")]
    [SerializeField] private GameObject inGameGUI;
    [SerializeField] private GameObject mainMenu,inventoryDisplay,rebindingDisplay,playerStatsDisplay;
    [SerializeField] private GameObject dialoguePopUp,levelUpBanner;

    [Header("UI controller selections")]
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject inventroyFirstButton,playerStatsDisplayFirstButton, rebindingDisplayFirstButton,dialoguePopUpFirstButton;

    private bool isInMenuMode;
    private bool isInDialogueMode;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.InitiateDialogue, OnInitiateDialogue);
        EventManager.currentManager.Subscribe(EventType.CeaseDialogue, OnCeaseDialogue);
        EventManager.currentManager.Subscribe(EventType.PlayerLevelUp, OnPlayerLevelUp);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.InitiateDialogue, OnInitiateDialogue);
        EventManager.currentManager.Unsubscribe(EventType.CeaseDialogue, OnCeaseDialogue);
        EventManager.currentManager.Unsubscribe(EventType.PlayerLevelUp, OnPlayerLevelUp);
    }
    void Start()
    {
        if (inputHandler==null)
            inputHandler=FindObjectOfType<InputHandler>();
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

        HandleMenuInput();

        HandleInventoryInput();
        HandleStatsInput();
    }

    private void HandleMenuInput()
    {
        //if menu button has been pressed
        if (inputHandler.menuInput)
        {
            //If menu button has been pressed in dialogue mode, exit out
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
                    EnableMenuMode();

                    //swap to in menu screen
                    inGameGUI.SetActive(false);
                    mainMenu.SetActive(true);

                    SetMenuFirstButton();
                }
                else
                {
                    DisableMenuMode();
                }
            }

        }
    }

    private void HandleInventoryInput()
    {
        //If inventory button pressed
        if (inputHandler.inventoryInput && !inputHandler.spellcastingModeInput)
        {
            //Close dialogue if it is open
            if (isInDialogueMode)
                EventManager.currentManager.AddEvent(new CeaseDialogue());

            if (isInMenuMode)
            {
                DisableMenuMode();
            }
            else
            {
                EnableMenuMode();

                //Enable Invetory
                inventoryDisplay.SetActive(true);

                SetInventoryButton();
            }


        }
    }

    private void HandleStatsInput()
    {
        //If inventory button pressed
        if (inputHandler.characterStatsInput&&!inputHandler.spellcastingModeInput)
        {
            //Close dialogue if it is open
            if (isInDialogueMode)
                EventManager.currentManager.AddEvent(new CeaseDialogue());

            if (isInMenuMode)
            {
                DisableMenuMode();
            }
            else
            {
                EnableMenuMode();

                //Enable stat display
                playerStatsDisplay.SetActive(true);

                SetPlayerStatsDisplayButton();
            }


        }
    }

    private void EnableMenuMode()
    {
        //swap menu mode
        isInMenuMode = true;

        //Disable gameplay inputs and enable menu inputs
        inputHandler.GetInputActions().CharacterControls.Disable();

        inputHandler.lockOnFlag = false;
        //send out event to swap to menu camera
        EventManager.currentManager.AddEvent(new SwapToMenuCamera());

        //Display mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisableMenuMode()
    {
        //swap menu mode
        isInMenuMode = false;

        //Enable gameplay inputs and disable menu inputs
        inputHandler.GetInputActions().CharacterControls.Enable();

        //swap to in game screen
        inGameGUI.SetActive(true);
        mainMenu.SetActive(false);

        //Disable extra menus
        inventoryDisplay.SetActive(false);
        playerStatsDisplay.SetActive(false);
        rebindingDisplay.SetActive(false);

        //destroy inventory option holders
        EventManager.currentManager.AddEvent(new DestroyInventoryOptionHolders());

        //send out event to swap to exploration camera
        EventManager.currentManager.AddEvent(new SwapToExplorationCamera());

        //Hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #region Button functions

    public void ResumeGame()
    {
        DisableMenuMode();
    }

    #region Setting active buttons

    //These buttons handle setting the first selected option for easier controller menu interaction
    public void SetInventoryButton()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(inventroyFirstButton);
    }

    public void SetPlayerStatsDisplayButton()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(playerStatsDisplayFirstButton);
    }

    public void SetMenuFirstButton()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
    }

    public void SetDialogueFirstButton()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(dialoguePopUpFirstButton);
    }

    public void SetRebindingFirstButton()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(rebindingDisplayFirstButton);
    }
    #endregion

    #endregion

    #region onEvents

    private void OnInitiateDialogue(EventData eventData)
    {
        if (eventData is InitiateDialogue)
        {
            inGameGUI.SetActive(false);

            dialoguePopUp.SetActive(true);

            SetDialogueFirstButton();

            isInDialogueMode = true;

            //Display mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.InitiateDialogue was received but is not of class InitiateDialogue.");
        }
    }

    private void OnPlayerLevelUp(EventData eventData)
    {
        if (eventData is PlayerLevelUp)
        {
            levelUpBanner.SetActive(true);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.PlayerLevelUp was received but is not of class PlayerLevelUp.");
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

            //Display mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.CeaseDialogue was received but is not of class CeaseDialogue.");
        }
    }

    #endregion
}
