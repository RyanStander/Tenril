using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : CharacterInventory
{
    public void LoadEquippedWeapons(WeaponSlotManager weaponSlotManager)
    {
            weaponSlotManager.LoadWeaponOnSlot(equippedWeapon);
    }
}
