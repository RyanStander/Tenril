using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : CharacterInventory
{
    private InputHandler inputHandler;

    public int Gold=250;

    [Tooltip("The spells that the player can currently cast")]
    public SpellItem[] preparedSpells = new SpellItem[8];
    [Tooltip("The quickslot items that the player has selected")]
    public List<ConsumableItem> quickslotItems;
    [Tooltip("The currently selected quickslot item")]
    public ConsumableItem currentlySelectedQuickSlotItem;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.DropItem, OnDropItem);
        EventManager.currentManager.Subscribe(EventType.AddQuickslotItem, OnAddQuickslot);
        EventManager.currentManager.Subscribe(EventType.RemoveQuickslotItem, OnRemoveQuickslot);
        EventManager.currentManager.Subscribe(EventType.RequestEquippedWeapons, OnRequestEquippedWeapons);
        EventManager.currentManager.Subscribe(EventType.CompleteDropStack, OnCompleteDropStack);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.DropItem, OnDropItem);
        EventManager.currentManager.Unsubscribe(EventType.AddQuickslotItem, OnAddQuickslot);
        EventManager.currentManager.Unsubscribe(EventType.RemoveQuickslotItem, OnRemoveQuickslot);
        EventManager.currentManager.Unsubscribe(EventType.RequestEquippedWeapons, OnRequestEquippedWeapons);
        EventManager.currentManager.Unsubscribe(EventType.CompleteDropStack, OnCompleteDropStack);
    }

    private void Awake()
    {
        EventManager.currentManager.Subscribe(EventType.RemoveItemFromInventory, OnRemoveItem);

        inputHandler = GetComponent<InputHandler>();

        if (currentlySelectedQuickSlotItem == null && quickslotItems.Count != 0)
        {
            currentlySelectedQuickSlotItem = quickslotItems[0];
            consumableItemInUse = currentlySelectedQuickSlotItem;
        }
    }

    #region Inventory Management

    public void AddItemToInventory(Item item,int amountToAdd=1)
    {
        //find all items of the specified type
        List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(item);

        bool itemAdded = false;
        //check if any items of the matching type were found
        while (amountToAdd > 0)
        {
            if (foundItems.Count > 0)
            {
                foreach (ItemInventory itemInventory in foundItems)
                {
                    //if there is space in the current itemInventory
                    if (itemInventory.item.amountPerStack > itemInventory.itemStackCount)
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
                foundItems.Add(newItem);
            }
            amountToAdd--;
            itemAdded = false;
        }
        EventManager.currentManager.AddEvent(new UpdateInventoryDisplay());
        FilterInventory();
    }

    public void RemoveItemFromInventory(Item item, int amountToBeRemoved = 1,bool dropItem=false)
    {
        if(dropItem)
            EventManager.currentManager.AddEvent(new PlayerHasDroppedItem(item, amountToBeRemoved));
        //find all items of the specified type
        List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(item);
        if (foundItems.Count > 0)
        {
            while (amountToBeRemoved > 0)
            {
                //check if any items of the matching type were found

                foreach (ItemInventory itemInventory in foundItems)
                {
                    //find the index value
                    int indexVal = inventory.IndexOf(itemInventory);

                    if (inventory[indexVal].itemStackCount > 1)
                        //decrease the stack count
                        inventory[indexVal].itemStackCount--;
                    else
                    {
                        inventory.RemoveAt(indexVal);
                        foundItems.Remove(itemInventory);
                    }

                    //exit out of foreach
                    break;
                }
                amountToBeRemoved--;
            }
            FilterInventory();
        }
        else
        {
            Debug.LogWarning("Item designated to be removed from the player's inventory was not found. This should not happen");
        }
    }

    private void FilterInventory()
    {
        //List of items that have been checked
        List<ItemInventory> itemsChecked = new List<ItemInventory>();
        //List of items that need to be removed from the inventory (if they reach 0)
        List<ItemInventory> invetoryItemsToRemove = new List<ItemInventory>();

        //itterate through the inventory to check items
        foreach (ItemInventory itemToCheck in inventory)
        {
            //if the item has already been checked, go to next loop
            if (itemsChecked.Contains(itemToCheck))
                continue;

            //If it hasnt been checked, add it to the item checked list
            itemsChecked.Add(itemToCheck);

            //Find all instances of the item
            List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(itemToCheck.item);

            //Used to determine if still need to check items
            bool hasInteratedThroughAllOfItemType = false;

            while (!hasInteratedThroughAllOfItemType)
            {
                List<ItemInventory> itemsToBeRemoved = new List<ItemInventory>();

                //if there is less than 2 items
                if (foundItems.Count < 2)
                {
                    //exit out of the while loop;
                    hasInteratedThroughAllOfItemType = true;
                    continue;
                }

                //if the first item does not have max count
                if (foundItems[0].itemStackCount < foundItems[0].item.amountPerStack)
                {
                    //decrease the 2nd and increase the first
                    foundItems[0].itemStackCount++;
                    foundItems[1].itemStackCount--;
                }
                else
                {
                    //if its full, remove it from the list to check
                    itemsToBeRemoved.Add(foundItems[0]);
                }

                //if the 2nd item is empty, remove it
                if (foundItems[1].itemStackCount < 1)
                {
                    itemsToBeRemoved.Add(foundItems[1]);
                    invetoryItemsToRemove.Add(foundItems[1]);
                }

                //if there are items to remove, do so
                if (itemsToBeRemoved.Count>0)
                {
                    foreach (ItemInventory itemToRemove in itemsToBeRemoved)
                    {
                        foundItems.Remove(itemToRemove);
                    }
                }
            }

        }
        //if any items have reached a count of 0, remove it from the inventory
        if (invetoryItemsToRemove.Count > 0)
        {
            foreach (ItemInventory itemToRemove in invetoryItemsToRemove)
            {
                inventory.Remove(itemToRemove);
            }
        }
    }

    public int GetItemStackCount(Item item)
    {
        //find all items of the specified type
        List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(item);
        //get the total item count
        int totalItemCount=0;
        foreach (ItemInventory itemInventory in foundItems)
        {
            totalItemCount += itemInventory.itemStackCount;
        }

        return totalItemCount;
    }

    /// <summary>
    /// Checks if the equipped ammunition has any arrows left
    /// </summary>
    public bool HasAmmo()
    {
        if (equippedAmmo == null)
            return false;

        if (FindAllInstancesOfItemInInventory(equippedAmmo) != null)
            return true;

        return false;
    }

    #endregion

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
                weaponSlotManager.LoadWeaponOnSlot(primaryWeapon, secondaryWeapon);
            else
                weaponSlotManager.LoadWeaponOnSlot(primaryWeapon, secondaryWeapon);
        }
        else
        {
            equippedWeapon = secondaryWeapon;
            //load secondary weapon in hand and primary in sheath
            if (equippedWeapon != null)
                weaponSlotManager.LoadWeaponOnSlot(secondaryWeapon, primaryWeapon);
            else
                weaponSlotManager.LoadWeaponOnSlot(secondaryWeapon, primaryWeapon);
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

    internal void HideWeapons(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.HideWeapons();
    }

    internal void ShowWeapons(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.ShowWeapons();
    }

    internal void DisplayQuickslotItem(WeaponSlotManager weaponSlotManager, GameObject displayObject)
    {
        weaponSlotManager.DisplayObjectInHand(displayObject);
    }

    /// <summary>
    /// displays the tool that will be used
    /// </summary>
    internal void DisplayTool(WeaponSlotManager weaponSlotManager, ToolType toolType)
    {
        switch (toolType)
        {
            case ToolType.None:
                weaponSlotManager.DisplayObjectInHand(null);
                break;
            case ToolType.Pickaxe:
                weaponSlotManager.DisplayObjectInHand(equippedPickaxe.toolPrefab);
                break;
            case ToolType.Axe:
                break;
        }
    }

    internal void HideQuickslotItem(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.HideObjectInHand();
    }

    #endregion

    #region Quickslot Management
        
    public bool CheckIfItemCanBeConsumed(Item item)
    {
        //find all items of the specified type
        List<ItemInventory> foundItems = FindAllInstancesOfItemInInventory(item);

        if (foundItems.Count<1)
            return false;
        return true;
    }

    #endregion

    private List<ItemInventory> FindAllInstancesOfItemInInventory(Item item)
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
        return foundItems;
    }

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
            //If the item is limited to 1 per stack, drop it, otherwise ask how many to be dropped
            if (dropItem.item.amountPerStack == 1)
            {
                //remove item from inventory
                RemoveItemFromInventory(dropItem.item,1,true);
                //update the inventory display
                EventManager.currentManager.AddEvent(new UpdateInventoryDisplay());
            }
            else
            {
                EventManager.currentManager.AddEvent(new InitiateDropStack(dropItem.item, GetItemStackCount(dropItem.item)));
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.EquipWeapon was received but is not of class EquipWeapon.");
        }
    }

    private void OnCompleteDropStack(EventData eventData)
    {
        if (eventData is CompleteDropStack dropStack)
        {
            RemoveItemFromInventory(dropStack.item, dropStack.amountToDrop,true);
            EventManager.currentManager.AddEvent(new UpdateInventoryDisplay());
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.CompleteDropStack was received but is not of class CompleteDropStack.");
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