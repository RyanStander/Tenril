using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : CharacterInventory
{
    private InputHandler inputHandler;

    public int Gold=250;

    [Tooltip("The spells that the player can currently cast")]
    public SpellItem[] preparedSpells = new SpellItem[8];
    [Tooltip("The quickslot items that the player has selected")]
    public List<QuickslotItem> quickslotItems;
    [Tooltip("The currently selected quickslot item")]
    public QuickslotItem currentQuickSlotItem;

    [Header("Backpack")]
    public List<Item> Inventory;
    //Inventory Subcategories
    public List<WeaponItem> weaponsInventory;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    internal void EquipWeapon(WeaponSlotManager weaponSlotManager, WeaponItem weaponItem,bool isPrimaryWeapon)
    {
        if (isPrimaryWeapon)
        {
            primaryWeapon = weaponItem;
            EquipNewWeapon(weaponSlotManager);
        }
        else
        {
            secondaryWeapon = weaponItem;
            EquipNewWeapon(weaponSlotManager);
        }
    }

    internal void LoadEquippedWeapons(WeaponSlotManager weaponSlotManager)
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

    internal void EquipNewWeapon(WeaponSlotManager weaponSlotManager)
    {
            //if currently wielding primary weapon
            if (isWieldingPrimaryWeapon)
            {
                equippedWeapon = primaryWeapon;

                LoadEquippedWeapons(weaponSlotManager);
            }
            //if currently wielding secondary weapon
            else
            {
                equippedWeapon = secondaryWeapon;

                LoadEquippedWeapons(weaponSlotManager);
            }
    }

    internal void SwapWeapon(WeaponSlotManager weaponSlotManager)
    {
        if (inputHandler.drawSheathInput)
        {
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
