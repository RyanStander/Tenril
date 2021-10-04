using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPickup : Interactable
{
    public Item item;
    protected PlayerInventory playerInventory;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    protected virtual void PickUpItem(PlayerManager playerManager)
    {
        PlayerAnimatorManager playerAnimatorManager;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerAnimatorManager = playerManager.GetComponent<PlayerAnimatorManager>();

        //Plays the animation of picking up item
        playerAnimatorManager.PlayTargetAnimation("PickUp", true);
        
        //Add Item to inventory

        //Notify player of obtaining item
        playerManager.itemInteractableGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "You obtained " + item.itemName + "!";
        //Set the weapon icon to display item
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture;
        //Enable the game object
        playerManager.itemInteractableGameObject.SetActive(true);

        //Destroy the interactable after pick up
        Destroy(gameObject);
    }
}
