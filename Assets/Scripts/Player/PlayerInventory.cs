using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : CharacterInventory
{
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
}
