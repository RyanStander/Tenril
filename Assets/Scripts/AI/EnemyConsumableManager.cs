using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConsumableManager : ConsumableManager
{
    //Relevant attached manager
    protected EnemyAgentManager enemyManager;

    override internal void Awake()
    {
        //Getter for relevant reference
        enemyManager = GetComponentInChildren<EnemyAgentManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyManager == null) throw new MissingComponentException("Missing EnemyAgentManager on " + gameObject + "!");
    }

    public bool SelectHealingItem()
    {
        //Set current item to null
        enemyManager.inventory.consumableItemInUse = null;

        //Iterate over the inventory searching for healing items
        foreach(ItemInventory item in enemyManager.inventory.inventory)
        {
            //Cast the item if it is of type healing
            if(item.item is HealingPotion healingItem)
            {
                //Check for available charges
                if(item.itemStackCount > 0)
                {
                    //Set the current consumable and break out of the foreach
                    enemyManager.inventory.consumableItemInUse = healingItem;
                    break;
                }
            }
        }

        //Return based on if an item was found
        if(enemyManager.inventory.consumableItemInUse == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Gets called by animations like "Drink" through animation events
   internal override void SuccessfulyUsedItem()
    {
        //Perform a succesfull use of the item
        enemyManager.inventory.consumableItemInUse.SuccessfullyUsedItem(enemyManager.animatorManager, enemyManager.enemyStats);

        //Remove a charge of the item if applicable
        if (enemyManager.inventory.consumableItemInUse.isConsumable)
        {
            //Remove a charge from the inventory
            enemyManager.inventory.RemoveItemFromInventory(enemyManager.inventory.consumableItemInUse);
        }

        //Hide the object used
        enemyManager.consumableManager.HideItem(enemyManager.weaponSlotManager);
    }
}
