using System.Collections.Generic;
using UnityEngine;

public class HarvestableResource : Interactable
{
    [Tooltip("Display icon used when a player can interact with an object")]
    public Sprite displayIcon;
    [Tooltip("The animation state that will be played when interacting")]
    public string animationToPlay= "PickUp";
    [Tooltip("The item that will be obtained when interacting")]
    public Item item;
    [Tooltip("The amount that will be received")]
    public int amountOfItem = 1;
    protected PlayerInventory playerInventory;
    [Tooltip("The amount times the recource can be harvested ")]
    public int totalHarvests = 3;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        HarvestResource(playerManager);
    }

    protected virtual void HarvestResource(PlayerManager playerManager)
    {
        PlayerAnimatorManager playerAnimatorManager;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerAnimatorManager = playerManager.GetComponent<PlayerAnimatorManager>();

        //Plays the animation of picking up item
        playerAnimatorManager.PlayTargetAnimation(animationToPlay, true);
    }

    /// <summary>
    /// Adds the items to the player's inventory on call
    /// </summary>
    public virtual void ObtainItemsFromHarvest()
    {
        //Add Item to inventory
        playerInventory.AddItemToInventory(item, amountOfItem);

        EventManager.currentManager.AddEvent(new PlayerObtainedItem(item, amountOfItem));

        EventManager.currentManager.AddEvent(new PlayerHarvestingResource(this));

        totalHarvests--;

        if (totalHarvests>0)
            Destroy(gameObject);
    }
}
