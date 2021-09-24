using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponPickup : Interactable
{
    public WeaponItem weapon;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {
        PlayerInventory playerInventory;
        PlayerAnimatorManager playerAnimatorManager;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerAnimatorManager = playerManager.GetComponent<PlayerAnimatorManager>();

        //Plays the animation of picking up item
        playerAnimatorManager.PlayTargetAnimation("PickUp", true);
        //Add weapon to inventory
        playerInventory.weaponsInventory.Add(weapon);

        //Notify player of obtaining item
        playerManager.itemInteractableGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "You obtained " + weapon.itemName + "!";
        //Set the weapon icon to display item
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
        //Enable the game object
        playerManager.itemInteractableGameObject.SetActive(true);

        //Destroy the interactable after pick up
        Destroy(gameObject);
    }
}
