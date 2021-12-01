using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConsumableManager : ConsumableManager
{
    public bool SelectHealingItem()
    {
        if(characterManager is EnemyAgentManager enemyManager)
        {
            //Set current item to null
            enemyManager.inventory.consumableItemInUse = null;

            //Iterate over the inventory searching for healing items
            foreach (ItemInventory item in enemyManager.inventory.inventory)
            {
                //Cast the item if it is of type healing
                if (item.item is HealingPotion healingItem)
                {
                    //Check for available charges
                    if (item.itemStackCount > 0)
                    {
                        //Set the current consumable and break out of the foreach
                        enemyManager.inventory.consumableItemInUse = healingItem;
                        break;
                    }
                }
            }

            //Return based on if an item was found
            if (enemyManager.inventory.consumableItemInUse == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            Debug.LogError("Wrong type of character manager being used by EnemyConsumableManager!");
            return false;
        }
    }

    //Gets called by animations like "Drink" through animation events
   internal override void SuccessfulyUsedItem()
    {
        if (characterManager is EnemyAgentManager enemyManager)
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
        else
        {
            Debug.LogError("Wrong type of character manager being used by EnemyConsumableManager!");
        }
    }
}
