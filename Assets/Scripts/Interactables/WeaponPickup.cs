using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : ItemPickup
{

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    protected override void PickUpItem(PlayerManager playerManager)
    {
        base.PickUpItem(playerManager);

        if (item is WeaponItem weapon)
        {
            //Add weapon to inventory
            playerInventory.weaponsInventory.Add(weapon);
        }
        else
        {
            Debug.LogWarning("The item given for WeaponPickup is not a weapon, please fix, kthanks");
        }
    }
}
