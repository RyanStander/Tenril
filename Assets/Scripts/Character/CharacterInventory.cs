using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the current equipment being used by a character
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    [Header("Equipped Items")]
    [Tooltip("The weapon the player currently has equipped")]
    public WeaponItem equippedWeapon = null;

    [Tooltip("Determines the weapon that the player currently has equiped and used for swapping")]
    public bool isWieldingPrimaryWeapon = true;

    [Tooltip("The 2 weapons that a player can swap between")]
    public WeaponItem primaryWeapon, secondaryWeapon;

    [Tooltip("The Ammo that the player will try to draw when using a ranged weapon")]
    public AmmunitionItem equippedAmmo;

    [Header("Backpack")]
    public List<ItemInventory> inventory = new List<ItemInventory>();

    //this is the item referenced when actually using an item (for players this is the quickslot)
    public ConsumableItem consumableItemInUse;

    //The inventory a given character will spawn with
    [Header("Inventory Table")]
    [Tooltip("Used for generating semi-random equipment")]
    public LootTable inventoryItemTable = null;

    //Populates an inventory with a given loot table
    public void PopulateInventoryItemTable()
    {
        //Load the loot table if given
        if (inventoryItemTable != null)
        {
            //Throw an error if no inventory was supplied
            if (inventory == null) throw new MissingComponentException("Missing EnemyInventory on " + gameObject + "!");
            else
            {
                //Add a generated list of loot
                inventory.AddRange(LootTable.GenerateListOfLoot(inventoryItemTable));
            }
        }
    }
}
