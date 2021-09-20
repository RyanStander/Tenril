using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : CharacterInventory
{
    private InputHandler inputHandler;

    public int Gold=250;

    [Header("Equipped Items")]
    [Tooltip("The weapon the player currently has equipped")]
    public WeaponItem equippedWeapon;
    [Tooltip("Determines the weapon that the player currently has equiped and used for swapping")]
    public bool isWieldingPrimaryWeapon = true;
    [Tooltip("The 2 weapons that a player can swap between")]
    public WeaponItem primaryWeapon, secondaryWeapon;
    [Tooltip("The spells that the player can currently cast")]
    public SpellItem[] preparedSpells = new SpellItem[8];

    [Header("Backpack")]
    public List<Item> Inventory;
    //Inventory Subcategories
    public List<WeaponItem> weaponsInventory;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

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
        if (inputHandler.drawSheathInput)
        {
            inputHandler.drawSheathInput = false;
            //if currently wielding primary weapon
            if (isWieldingPrimaryWeapon)
            {
                equippedWeapon = secondaryWeapon;
                
                LoadEquippedWeapons(weaponSlotManager);

                isWieldingPrimaryWeapon = false;
            }
            //if currently wielding secondary weapon
            else
            {
                equippedWeapon = primaryWeapon;

                LoadEquippedWeapons(weaponSlotManager);

                isWieldingPrimaryWeapon = true;
            }
        }
    }
}
