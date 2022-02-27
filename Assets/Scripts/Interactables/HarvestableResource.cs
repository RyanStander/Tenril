using System.Collections.Generic;
using UnityEngine;

public class HarvestableResource : Interactable
{
    [Tooltip("Display icon used when a player can interact with an object")]
    public Sprite displayIcon;
    [Tooltip("The animation state that will be played when interacting")]
    public string animationToPlay= "PickUp";
    [Tooltip("References the tool needed to harvest")]
    public ToolType toolRequired;
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

        //Plays the animation of harvesting the item
        playerManager.DisplayTool(toolRequired);
        playerAnimatorManager.PlayTargetAnimation(animationToPlay, true);
        playerAnimatorManager.animator.SetBool("isHarvestingResource", true);
    }

    /// <summary>
    /// Adds the items to the player's inventory on call. Returns true when there are still resources to harvest, returns false when there are none left.
    /// </summary>
    public virtual bool ObtainItemsFromHarvest()
    {
        //Add Item to inventory
        if (playerInventory != null)
            playerInventory.AddItemToInventory(item, amountOfItem);

        EventManager.currentManager.AddEvent(new PlayerObtainedItem(item, amountOfItem));

        totalHarvests--;

        if (totalHarvests < 1)
        {
            Destroy(gameObject);

            return false;
        }
        else
        {
            return true;
        }

    }
}
