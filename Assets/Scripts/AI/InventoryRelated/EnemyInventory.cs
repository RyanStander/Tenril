using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : CharacterInventory
{
    //[Tooltip("The healing items the enemy has available to them")]
    //public List<EnemyConsumable> healingItems;

    //Temporary helper item for testing
    public EnemyConsumable temporaryHealingPotion;

    internal EnemyConsumable currentConsumable;

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

    public bool SelectHealingItem()
    {
        //TODO: Implement logic to check for enemy consumable charges, for now assume infinite

        //Set the current consumable to the healing potion
        currentConsumable = temporaryHealingPotion;

        //For now return true until final logic is implemented
        return true;
    }
}
