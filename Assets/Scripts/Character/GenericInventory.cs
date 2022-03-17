using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The most simplified inventory possible for any given object
/// </summary>
public class GenericInventory : MonoBehaviour
{
    [Header("Backpack")]
    public List<ItemInventory> inventory = new List<ItemInventory>();

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
            if (inventory == null) throw new MissingComponentException("Missing Inventory on " + gameObject + "!");
            else
            {
                //Add a generated list of loot
                inventory.AddRange(LootTable.GenerateListOfLoot(inventoryItemTable));
            }
        }
    }
}
