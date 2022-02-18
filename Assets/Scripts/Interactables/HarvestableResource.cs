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
    [Tooltip("The time it takes to fully harvest the item")]
    public float harvestDuration=4f;
    protected PlayerInventory playerInventory;

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

        //Add Item to inventory
        playerInventory.AddItemToInventory(item, amountOfItem);

        EventManager.currentManager.AddEvent(new PlayerObtainedItem(item, amountOfItem));

        //Destroy the interactable after pick up
        Destroy(gameObject);
    }
}
