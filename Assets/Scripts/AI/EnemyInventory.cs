using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : CharacterInventory
{
    public void LoadEquippedWeapons(WeaponSlotManager weaponSlotManager)
    {
        //if it has a secondary weapon
        if (equippedWeapon.hasSecondaryWeapon)
        {
            //load only one weapon
            weaponSlotManager.LoadWeaponOnSlot(equippedWeapon, true);
        }
        //if it has no secondary weapon
        else
        {
            //load dual weapons weapon
            weaponSlotManager.LoadWeaponOnSlot(equippedWeapon, false);
        }
    }

    public void RemoveItemFromInventory(Item item)
    {
        //Find all items of the specified type
        List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(item);

        //Check if any items of the matching type were found
        if (foundItems.Count > 0)
        {
            foreach (ItemInventory itemInventory in foundItems)
            {
                //Find the index value of the item
                int indexVal = inventory.IndexOf(itemInventory);

                //Decrease the stack count, otherwise remove the item
                if (inventory[indexVal].itemStackCount > 1) inventory[indexVal].itemStackCount--;
                else inventory.RemoveAt(indexVal);

                //Break the loop early
                break;
            }
        }
        else
        {
            Debug.LogWarning("Item designated to be removed from the enemy's inventory was not found. This should not happen");
        }
    }

    public bool CheckIfItemCanBeConsumed(Item item)
    {
        //Find all items of the specified type
        List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(item);

        if (foundItems.Count < 1)
            return false;
        return true;
    }

    private List<ItemInventory> FindAllInstancesOfItemInInventory(Item item)
    {
        //find all items of the specified type
        List<ItemInventory> foundItems = new List<ItemInventory>();
        foreach (ItemInventory itemInventory in inventory)
        {
            //check if the new item matches an existing item
            if (itemInventory.item.UID == item.UID)
            {
                //add it to the list
                foundItems.Add(itemInventory);
            }
        }
        return foundItems;
    }
}
