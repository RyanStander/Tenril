using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConsumableManager : MonoBehaviour
{
    //Relevant attached manager
    protected EnemyAgentManager enemyManager;

    private void Awake()
    {
        //Getter for relevant reference
        enemyManager = GetComponentInChildren<EnemyAgentManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyManager == null) throw new MissingComponentException("Missing EnemyAgentManager on " + gameObject + "!");
    }

    public bool SelectHealingItem()
    {
        //Set current item to null
        enemyManager.inventory.currentConsumable = null;

        //Iterate over the inventory searching for healing items
        foreach(ItemInventory item in enemyManager.inventory.inventory)
        {
            //Cast the item if it is of type healing
            if(item.item is EnemyHealingItem healingItem)
            {
                //Check for available charges
                if(item.itemStackCount > 0)
                {
                    //Set the current consumable and break out of the foreach
                    enemyManager.inventory.currentConsumable = healingItem;
                    break;
                }
            }
        }

        //Return based on if an item was found
        if(enemyManager.inventory.currentConsumable == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Gets called by animations like "Drinking" through animation events
    public void SuccessfulyUsedItem()
    {
        //Perform a succesfull use of the item
        enemyManager.inventory.currentConsumable.SuccessfullyUsedItem(enemyManager);

        //Remove a charge of the item if applicable
        if (enemyManager.inventory.currentConsumable.isConsumable)
        {
            //Remove a charge from the inventory
            enemyManager.inventory.RemoveItemFromInventory(enemyManager.inventory.currentConsumable);
        }

        //Hide the object used
        enemyManager.consumableManager.HideItem(enemyManager.weaponSlotManager);
    }

    public void DisplayItem(WeaponSlotManager weaponSlotManager, GameObject displayObject)
    {
        weaponSlotManager.DisplayObjectInHand(displayObject);
    }

    internal void HideItem(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.HideObjectInHand();
    }
}
