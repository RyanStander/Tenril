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
    public List<ItemInventory> inventory= new List<ItemInventory>();

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.DropItem, OnDropItem);
        EventManager.currentManager.Subscribe(EventType.AddQuickslotItem, OnAddQuickslot);
        EventManager.currentManager.Subscribe(EventType.RemoveQuickslotItem, OnRemoveQuickslot);
        EventManager.currentManager.Subscribe(EventType.RequestEquippedWeapons, OnRequestEquippedWeapons);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.DropItem, OnDropItem);
        EventManager.currentManager.Unsubscribe(EventType.AddQuickslotItem, OnAddQuickslot);
        EventManager.currentManager.Unsubscribe(EventType.RemoveQuickslotItem, OnRemoveQuickslot);
        EventManager.currentManager.Unsubscribe(EventType.RequestEquippedWeapons, OnRequestEquippedWeapons);
    }

    private void Awake()
    {
        EventManager.currentManager.Subscribe(EventType.RemoveItemFromInventory, OnRemoveItem);

        inputHandler = GetComponent<InputHandler>();

        if (currentlySelectedQuickSlotItem == null && quickslotItems.Count != 0)
        {
            currentlySelectedQuickSlotItem = quickslotItems[0];
            quickslotItemInUse = currentlySelectedQuickSlotItem;
        }
    }

    public void AddItemToInventory(Item item)
    {
        List<ItemInventory> foundItems = new List<ItemInventory>();
        foreach (ItemInventory itemInventory in inventory)
        {
            //check if the new item matches an existing item
            if (itemInventory.item.UID==item.UID)
            {
                //add it to the list
                foundItems.Add(itemInventory);
            }
        }

        bool itemAdded = false;
        //check if any items of the matching type were found
        if (foundItems.Count>0)
        {
            foreach (ItemInventory itemInventory in foundItems)
            {
                //if there is space in the current itemInventory
                if (itemInventory.item.amountPerStack>itemInventory.itemStackCount)
                {
                    //find the index value
                    int indexVal = inventory.IndexOf(itemInventory);
                    //increase the stack count
                    inventory[indexVal].itemStackCount++;
                 
                    itemAdded = true;
                    //exit out of foreach
                    break;
                }
            }
        }
        //if no item was added
        if (!itemAdded)
        {
            //create a new one and set its stack count to 1
            ItemInventory newItem = new ItemInventory();
            newItem.item = item;
            newItem.itemStackCount = 1;
            inventory.Add(newItem);
        }

        EventManager.currentManager.AddEvent(new UpdateInventoryDisplay());
    }

    public void RemoveItemFromInventory(Item item, int amountToBeRemove=1)
    {
        //TO DO: currently does not make use of the amount to be removed, needs functionality

        List<ItemInventory> foundItems = new List<ItemInventory>();
        foreach (ItemInventory itemInventory in inventory)
        {
            //check if the new item matches an existing item
            if (itemInventory.item.UID == item.UID)
            {
                //add it to the list
                foundItems.Add(itemInventory);
            }
        }

        //check if any items of the matching type were found
        if (foundItems.Count > 0)
        {
            foreach (ItemInventory itemInventory in foundItems)
            {
                //find the index value
                int indexVal = inventory.IndexOf(itemInventory);

                if (inventory[indexVal].itemStackCount > 1)
                    //decrease the stack count
                    inventory[indexVal].itemStackCount--;
                else
                    inventory.RemoveAt(indexVal);

                //exit out of foreach
                break;
            }
        }
        else
        {
            Debug.LogWarning("Item designated to be removed from the player's inventory was not found. This should not happen");
        }
    }

    public int GetItemStackCount(Item item)
    {
        //find all items of the specified type
        List<ItemInventory> foundItems = new List<ItemInventory>();
        foreach (ItemInventory itemInventory in inventory)
        {
            //check if the new item matches an existing item
            if (itemInventory.item.UID == item.UID)
            {
                //add it to the list
                foundItems.Add(itemInventory);
            }
        }
        //get the total item count
        int totalItemCount=0;
        foreach (ItemInventory itemInventory in foundItems)
        {
            totalItemCount += itemInventory.itemStackCount;
        }

        return totalItemCount;
    }

    #region Weapon Management
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
        if (isWieldingPrimaryWeapon)
        {
            equippedWeapon = primaryWeapon;
            //load primary weapon in hand and secondary in sheath
            if(equippedWeapon!=null)
                weaponSlotManager.LoadWeaponOnSlot(primaryWeapon, equippedWeapon.hasSecondaryWeapon, secondaryWeapon);
            else
                weaponSlotManager.LoadWeaponOnSlot(primaryWeapon, false, secondaryWeapon);
        }
        else
        {
            equippedWeapon = secondaryWeapon;
            //load secondary weapon in hand and primary in sheath
            if (equippedWeapon != null)
                weaponSlotManager.LoadWeaponOnSlot(secondaryWeapon, equippedWeapon.hasSecondaryWeapon, primaryWeapon);
            else
                weaponSlotManager.LoadWeaponOnSlot(secondaryWeapon, false, primaryWeapon);
        }

        //send out event to update ui
        EventManager.currentManager.AddEvent(new UpdateWeaponDisplay(primaryWeapon, secondaryWeapon, isWieldingPrimaryWeapon));
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
                //change to secondary weapon
                equippedWeapon = secondaryWeapon;

                isWieldingPrimaryWeapon = false;

                LoadEquippedWeapons(weaponSlotManager);
            }
            //if currently wielding secondary weapon
            else
            {
                //change to primary weapon
                equippedWeapon = primaryWeapon;

                isWieldingPrimaryWeapon = true;

                LoadEquippedWeapons(weaponSlotManager);
            }
        }
    }

    #endregion

    #region On Events
    private void OnRequestEquippedWeapons(EventData eventData)
    {
        if (eventData is RequestEquippedWeapons)
        {
            EventManager.currentManager.AddEvent(new UpdateWeaponDisplay(primaryWeapon, secondaryWeapon, isWieldingPrimaryWeapon));
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.RequestEquippedWeapons was received but is not of class RequestEquippedWeapons.");
        }
    }

    private void OnDropItem(EventData eventData)
    {
        if (eventData is DropItem dropItem)
        {
            //remove item from inventory
            RemoveItemFromInventory(dropItem.item);
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

    private void OnRemoveItem(EventData eventData)
    {
        if (eventData is RemoveItemFromInventory removeItem)
        {
            RemoveItemFromInventory(removeItem.item, removeItem.amountToBeRemoved);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.RemoveItemFromInventory was received but is not of class RemoveItemFromInventory.");
        }
    }

    #endregion
}
[System.Serializable]
public class ItemInventory
{
    /// <summary>
    /// The item in the inventory slot
    /// </summary>
    public Item item;
    /// <summary>
    /// How many of the item is in the stack
    /// </summary>
    public int itemStackCount;
}