using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : CharacterInventory
{
    public int Gold=250;

    [Header("Equipped Items")]
    [Tooltip("The weapon the player currently has equiped")]
    public WeaponItem equippedWeapon;
    [Tooltip("Determines the weapon that the player currently has equiped and used for swapping")]
    public bool isWieldingPrimaryWeapon = true;
    [Tooltip("The 2 weapons that a player can swap between")]
    public WeaponItem primaryWeapon, secondaryWeapon;

    [Header("Backpack")]
    public List<Item> Inventory;
    //Inventory Subcategories
    public List<WeaponItem> weaponsInventory;

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

    public void SwapWeapon(WeaponSlotManager weaponSlotManager)
    {
        //if currently wielding primary weapon
        if (isWieldingPrimaryWeapon)
        {
            //if it has a secondary weapon
            if (equippedWeapon.hasSecondaryWeapon)
            {
                //load only one weapon
                weaponSlotManager.LoadWeaponOnSlot(secondaryWeapon, true);
            }
            //if it has no secondary weapon
            else
            {
                //load dual weapons weapon
                weaponSlotManager.LoadWeaponOnSlot(secondaryWeapon, false);
            }
        }
        //if currently wielding secondary weapon
        else
        {
            //if it has a secondary weapon
            if (equippedWeapon.hasSecondaryWeapon)
            {
                //load only one weapon
                weaponSlotManager.LoadWeaponOnSlot(primaryWeapon, true);
            }
            //if it has no secondary weapon
            else
            {
                //load dual weapons weapon
                weaponSlotManager.LoadWeaponOnSlot(primaryWeapon, false);
            }

        }
    }
}
