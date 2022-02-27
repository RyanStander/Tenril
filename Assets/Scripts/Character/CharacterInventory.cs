using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the current equipment being used by a character
/// </summary>
public class CharacterInventory : MonoBehaviour
{
    [Header("Equipped Items")]
    [Tooltip("The weapon the player currently has equipped")]
    public WeaponItem equippedWeapon = null;

    [Tooltip("Determines the weapon that the player currently has equiped and used for swapping")]
    public bool isWieldingPrimaryWeapon = true;

    [Tooltip("The 2 weapons that a player can swap between")]
    public WeaponItem primaryWeapon, secondaryWeapon;

    [Tooltip("The Ammo that the player will try to draw when using a ranged weapon")]
    public AmmunitionItem equippedAmmo;
    
    [Header("Tools")]
    [Tooltip("The pickaxe that the player will try to use when mining")]
    public ToolItem equippedPickaxe;
    [Tooltip("The axe that the player will try to use when chopping wood")]
    public ToolItem equippedAxe;

    [Header("Backpack")]
    public List<ItemInventory> inventory = new List<ItemInventory>();

    //this is the item referenced when actually using an item (for players this is the quickslot)
    public ConsumableItem consumableItemInUse;
}
