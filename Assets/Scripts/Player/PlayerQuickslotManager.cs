using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerQuickslotManager : ConsumableManager
{
    private PlayerInventory playerInventory;
    private PlayerStats playerStats;
    private InputHandler inputHandler;
    protected AnimatorManager animatorManager;

    override internal void Awake()
    {
        //Run base getters
        base.Awake();

        inputHandler = GetComponent<InputHandler>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    internal void HandleQuickslotInputs()
    {
        HandleQuickslotUseInput();
        HandleQuickslotSelectionInputs();
    }

    private void HandleQuickslotSelectionInputs()
    {
        if (inputHandler.quickslotLeftInput)
        {
            //Get the index of the current quickslot item
            int currentIndexValue = playerInventory.quickslotItems.IndexOf(playerInventory.currentlySelectedQuickSlotItem);

            //if the value is -1 that means the quick slot item that is equipped does not exist
            if (currentIndexValue == -1)
            {
                Debug.LogWarning("The currently selected quickslot item could not be found in the list, please make sure it is set properly");
                return;
            }

            //if index value is 0, which would mean swapping would turn to -1
            if (currentIndexValue == 0)
            {
                //set it to the last item in the list
                currentIndexValue = playerInventory.quickslotItems.Count;
            }
            //sets the current quickslot item to the new value
            playerInventory.currentlySelectedQuickSlotItem = playerInventory.quickslotItems[currentIndexValue - 1];

            //Update the ui display
            EventManager.currentManager.AddEvent(new UpdateQuickslotDisplay());
        }

        if (inputHandler.quickslotRightInput)
        {
            //Get the index of the current quickslot item
            int currentIndexValue = playerInventory.quickslotItems.IndexOf(playerInventory.currentlySelectedQuickSlotItem);

            //if the value is -1 that means the quick slot item that is equipped does not exist
            if (currentIndexValue == -1)
            {
                Debug.LogWarning("The currently selected quickslot item could not be found in the list, please make sure it is set properly");
                return;
            }

            //if index value is 0, which would mean swapping would turn to -1
            if (currentIndexValue == playerInventory.quickslotItems.Count - 1)
            {
                //set it to the first item in the list
                currentIndexValue = -1;
            }
            //sets the current quickslot item to the new value
            playerInventory.currentlySelectedQuickSlotItem = playerInventory.quickslotItems[currentIndexValue + 1];

            //Update the ui display
            EventManager.currentManager.AddEvent(new UpdateQuickslotDisplay());
        }
    }

    private void HandleQuickslotUseInput()
    {
        //use selected quickslot if pressed
        if (inputHandler.quickslotUseInput)
        {
            if (playerInventory.consumableItemInUse == null)
                return;

            if (animatorManager.animator.GetBool("isInteracting"))
                return;

            if (!playerInventory.CheckIfItemCanBeConsumed(playerInventory.currentlySelectedQuickSlotItem))
                return;

            //sets the item in us to the currently selected item
            playerInventory.consumableItemInUse = playerInventory.currentlySelectedQuickSlotItem;

            //attempt using the item
            playerInventory.consumableItemInUse.AttemptToUseItem(animatorManager, this, playerStats);
            EventManager.currentManager.AddEvent(new RemoveItemFromInventory(playerInventory.consumableItemInUse));

            //Update the ui display
            EventManager.currentManager.AddEvent(new UpdateQuickslotDisplay());
        }
    }


    internal override void SuccessfulyUsedItem()
    {
        //Perform success item
        playerInventory.consumableItemInUse.SuccessfullyUsedItem(animatorManager, playerStats);

        //NOTE FOR RYAN: You probably don't need this event any more, you can directly call "HideItem(weaponManager);"
        EventManager.currentManager.AddEvent(new HideQuickslotItem());
    }
}
