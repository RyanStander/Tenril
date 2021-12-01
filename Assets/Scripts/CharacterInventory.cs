using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    [Header("Equipped Items")]
    [Tooltip("The weapon the player currently has equipped")]
    public WeaponItem equippedWeapon;

    [Tooltip("Determines the weapon that the player currently has equiped and used for swapping")]
    public bool isWieldingPrimaryWeapon = true;

    [Tooltip("The 2 weapons that a player can swap between")]
    public WeaponItem primaryWeapon, secondaryWeapon;

    [Header("Backpack")]
    public List<ItemInventory> inventory = new List<ItemInventory>();

    //this is the item referenced when actually using an item (for players this is the quickslot)
    public ConsumableItem consumableItemInUse;
}
