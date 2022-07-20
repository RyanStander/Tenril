using System.Collections.Generic;
using UnityEngine;
using WeaponManagement;

/// <summary>
/// Similar to the character inventory, but specialized to function with combatative AI
/// </summary>
public class EnemyInventory : CharacterInventory
{
    public void Awake()
    {
        PopulateInventoryItemTable();
        SetWeaponFromInventory();
    }

    public void SetWeaponFromInventory()
    {
        //If no weapon is currently equipted, search the inventory and use that
        if(equippedWeapon == null)
        {
            //Declare temporary list
            List<WeaponItem> weapons = new List<WeaponItem>();

            //Iterate over all items and add them to the weapons list
            foreach(ItemInventory item in inventory)
            {
                //Check if its a weapon
                if(item.item is WeaponItem weapon)
                {
                    weapons.Add(weapon);
                }
            }

            //If a weapon is found, equipt it
            if(weapons.Count > 0)
            {
                equippedWeapon = weapons[Random.Range(0, weapons.Count - 1)];
            }
            //Otherwise throw an error
            else
            {
                Debug.Log("A weapon search was conducted, but nothing was found! Did you forget to include one?");
            }
        }
    }

    public void LoadEquippedWeapons(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.LoadWeaponOnSlot(equippedWeapon);
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
