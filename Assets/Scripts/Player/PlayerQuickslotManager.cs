using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuickslotManager : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerStats playerStats;
    private InputHandler inputHandler;
    private QuickslotItem currentQuickSlotItem;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    internal void HandleQuickslotInputs()
    {
        HandleQuickslotUseInput();
        HandleQuickslotSelectionInputs();
    }

    private void HandleQuickslotSelectionInputs()
    {
        Debug.Log("current Index: " + playerInventory.quickslotItems.IndexOf(playerInventory.currentQuickSlotItem) + " count of:" + playerInventory.quickslotItems.Count);

        if (inputHandler.quickslotLeftInput)
        {
            //Get the index of the current quickslot item
            int currentIndexValue = playerInventory.quickslotItems.IndexOf(playerInventory.currentQuickSlotItem);

            //if the value is -1 that means the quick slot item that is equipped does not exist
            if (currentIndexValue == -1)
            {
                Debug.LogError("The currently selected quickslot item could not be found in the list, please make sure it is set properly");
                return;
            }

            //if index value is 0, which would mean swapping would turn to -1
            if (currentIndexValue == 0)
            {
                //set it to the last item in the list
                currentIndexValue = playerInventory.quickslotItems.Count;
            }
            //sets the current quickslot item to the new value
            playerInventory.currentQuickSlotItem = playerInventory.quickslotItems[currentIndexValue - 1];

        }

        if (inputHandler.quickslotRightInput)
        {
            //Get the index of the current quickslot item
            int currentIndexValue = playerInventory.quickslotItems.IndexOf(playerInventory.currentQuickSlotItem);

            //if the value is -1 that means the quick slot item that is equipped does not exist
            if (currentIndexValue == -1)
            {
                Debug.LogError("The currently selected quickslot item could not be found in the list, please make sure it is set properly");
                return;
            }

            //if index value is 0, which would mean swapping would turn to -1
            if (currentIndexValue == playerInventory.quickslotItems.Count - 1)
            {
                //set it to the first item in the list
                currentIndexValue = -1;
            }
            //sets the current quickslot item to the new value
            playerInventory.currentQuickSlotItem = playerInventory.quickslotItems[currentIndexValue + 1];
        }
    }

    private void HandleQuickslotUseInput()
    {
        //use selected quickslot if pressed
        if (inputHandler.quickslotUseInput)
        {
            if (playerInventory.currentQuickSlotItem == null)
                return;

            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            playerInventory.currentQuickSlotItem.AttemptToUseItem(playerAnimatorManager, playerStats);
            currentQuickSlotItem = playerInventory.currentQuickSlotItem;
        }
    }

    public void SuccessfulyUsedItem()
    {
        currentQuickSlotItem.SuccessfullyUsedItem(playerAnimatorManager, playerStats);
    }
}
