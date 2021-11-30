using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
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

    public void SuccessfulyUsedItem()
    {
        //Perform a succesfull use of the item
        enemyManager.inventory.currentConsumable.SuccessfullyUsedItem(enemyManager);

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
