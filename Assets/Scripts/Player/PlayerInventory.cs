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
    public QuickslotItem currentlySelectedQuickSlotItem;
    //this is the item referenced when actually using a quickslot item
    public QuickslotItem quickslotItemInUse;

    [Header("Backpack")]
    public List<Item> inventory;
    //Inventory Subcategories
    public List<WeaponItem> weaponsInventory;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.DropItem, OnDropItem);
        EventManager.currentManager.Subscribe(EventType.AddQuickslotItem, OnAddQuickslot);
        EventManager.currentManager.Subscribe(EventType.RemoveQuickslotItem, OnRemoveQuickslot);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.DropItem, OnDropItem);
        EventManager.currentManager.Unsubscribe(EventType.AddQuickslotItem, OnAddQuickslot);
        EventManager.currentManager.Unsubscribe(EventType.RemoveQuickslotItem, OnRemoveQuickslot);
    }

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();

        if (currentlySelectedQuickSlotItem==null&&quickslotItems.Count!=0)
        {
            currentlySelectedQuickSlotItem = quickslotItems[0];
            quickslotItemInUse = currentlySelectedQuickSlotItem;
        }
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

    #region On Events
    private void OnDropItem(EventData eventData)
    {
        if (eventData is DropItem dropItem)
        {
            //remove item from inventory
            inventory.Remove(dropItem.item);
            //update the inventory display
            EventManager.currentManager.AddEvent(new UpdateInventoryDisplay());
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.EquipWeapon was received but is not of class EquipWeapon.");
        }
    }

    private void OnAddQuickslot(EventData eventData)
    {
        if (eventData is AddQuickslotItem addQuickslot)
        {
            //check if the list does not contain the item
            if (!quickslotItems.Contains(addQuickslot.quickslotItem))
            {
                //if there are no quickslot items
                if (quickslotItems.Count == 0)
                    //set the current quickslot item to be the newly added one
                    currentlySelectedQuickSlotItem = addQuickslot.quickslotItem;
                //add item to the quickslot list
                quickslotItems.Add(addQuickslot.quickslotItem);
                //update quickslot display
                EventManager.currentManager.AddEvent(new UpdateQuickslotDisplay());
            }
            else
            {
                //give feedback that the item is already there
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.AddQuickslotItem was received but is not of class AddQuickslotItem.");
        }
    }

    private void OnRemoveQuickslot(EventData eventData)
    {
        if (eventData is RemoveQuickslotItem removeQuickslot)
        {
            //check if the list contains the item
            if (quickslotItems.Contains(removeQuickslot.quickslotItem))
            {
                //remove item from the quickslot list
                quickslotItems.Remove(removeQuickslot.quickslotItem);
                //check if the item is currently selected, if so remove it
                if (currentlySelectedQuickSlotItem==removeQuickslot.quickslotItem)
                {
                    //check the size of the list
                    if (quickslotItems.Count>0)
                    {
                        //set it to the first one in the list
                        currentlySelectedQuickSlotItem = quickslotItems[0];
                    }
                    else
                    {
                        //set quickslot item to null if there are none
                        currentlySelectedQuickSlotItem = null;
                    }

                }
                //update quickslot display
                EventManager.currentManager.AddEvent(new UpdateQuickslotDisplay());
            }
            else
            {
                //give feedback that the item does not exist
                Debug.LogWarning("the item to be removed was not found");
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.RemoveQuickslotItem was received but is not of class RemoveQuickslotItem.");
        }
    }


    #endregion
}
