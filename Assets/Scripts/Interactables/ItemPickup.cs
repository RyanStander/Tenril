using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPickup : Interactable
{
    public Item item;
    public int amountOfItem=1;
    protected PlayerInventory playerInventory;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    protected virtual void PickUpItem(PlayerManager playerManager)
    {
        PlayerAnimatorManager playerAnimatorManager;
        PlayerInteraction playerInteraction;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerAnimatorManager = playerManager.GetComponent<PlayerAnimatorManager>();
        playerInteraction = playerManager.GetComponent<PlayerInteraction>();

        //Plays the animation of picking up item
        playerAnimatorManager.PlayTargetAnimation("PickUp", true);

        //Add Item to inventory
        playerInventory.AddItemToInventory(item,amountOfItem);

        EventManager.currentManager.AddEvent(new PlayerObtainedItem(item,amountOfItem));

        //Enable the game object
        //playerInteraction.itemPopUp.SetActive(true);

        //Destroy the interactable after pick up
        Destroy(gameObject);
    }
}
