using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryDisplayManager : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject inventoryItemDisplayPrefab;
    [SerializeField] private GameObject inventoryContentObject;
    [SerializeField] private TMP_Text goldDisplay;
    private CurrentlyDisplayedInventory currentlyDisplayedInventory = CurrentlyDisplayedInventory.all;

    private void OnEnable()
    {
        UpdateGoldDisplay();

        LoadAllInventoryToDisplay();

        EventManager.currentManager.Subscribe(EventType.UpdateInventoryDisplay, OnUpdateInventory);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.UpdateInventoryDisplay, OnUpdateInventory);
    }

    private void OnUpdateInventory(EventData eventData)
    {
        if (eventData is UpdateInventoryDisplay)
        {
            switch (currentlyDisplayedInventory)
            {
                case CurrentlyDisplayedInventory.all:
                    LoadAllInventoryToDisplay();
                    break;
                case CurrentlyDisplayedInventory.consumable:
                    LoadConsumableInventoryToDisplay();
                    break;
                case CurrentlyDisplayedInventory.weapon:
                    LoadWeaponInventoryToDisplay();
                    break;
                default:
                    break;
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdateInventoryDisplay was received but is not of class UpdateInventoryDisplay.");
        }
    }

    #region inventory display loading
    private void UpdateGoldDisplay()
    {
        goldDisplay.text = playerInventory.Gold.ToString();
    }

    public void LoadAllInventoryToDisplay()
    {
        //removes all previous items
        foreach (Transform child in inventoryContentObject.transform)
        {
            Destroy(child.gameObject);
        }
        currentlyDisplayedInventory = CurrentlyDisplayedInventory.all;
        foreach (ItemInventory item in playerInventory.inventory)
        {
            //create inventory item
            GameObject createdInventoryItemPrefab = Instantiate(inventoryItemDisplayPrefab, inventoryContentObject.transform);

            //Get the inventory display script
            InventoryItemDisplay inventoryItemDisplay = createdInventoryItemPrefab.GetComponent<InventoryItemDisplay>();

            //check if an inventoryitemdisplay script is present
            if (inventoryItemDisplay!=null)
            {
                inventoryItemDisplay.LoadValues(item.item,item.itemStackCount);
            }
        }
    }

    public void LoadWeaponInventoryToDisplay()
    {
        //removes all previous items
        foreach (Transform child in inventoryContentObject.transform)
        {
            Destroy(child.gameObject);
        }
        currentlyDisplayedInventory = CurrentlyDisplayedInventory.weapon;
        foreach (ItemInventory item in playerInventory.inventory)
        {
            //check if the item is a weapon item
            if (item.item is WeaponItem weapon)
            {
                //create inventory item
                GameObject createdInventoryItemPrefab = Instantiate(inventoryItemDisplayPrefab, inventoryContentObject.transform);

                //Get the inventory display script
                InventoryItemDisplay inventoryItemDisplay = createdInventoryItemPrefab.GetComponent<InventoryItemDisplay>();

                //check if an inventoryitemdisplay script is present
                if (inventoryItemDisplay != null)
                {
                    inventoryItemDisplay.LoadValues(item.item, item.itemStackCount);
                }
            }
        }
    }

    public void LoadConsumableInventoryToDisplay()
    {
        //removes all previous items
        foreach (Transform child in inventoryContentObject.transform)
        {
            Destroy(child.gameObject);
        }
        currentlyDisplayedInventory = CurrentlyDisplayedInventory.consumable;
        foreach (ItemInventory item in playerInventory.inventory)
        {
            //check if the item is a weapon item
            if (item.item is ConsumableItem consumable)
            {
                //create inventory item
                GameObject createdInventoryItemPrefab = Instantiate(inventoryItemDisplayPrefab, inventoryContentObject.transform);

                //Get the inventory display script
                InventoryItemDisplay inventoryItemDisplay = createdInventoryItemPrefab.GetComponent<InventoryItemDisplay>();

                //check if an inventoryitemdisplay script is present
                if (inventoryItemDisplay != null)
                {
                    inventoryItemDisplay.LoadValues(item.item, item.itemStackCount);
                }
            }
        }
    }

    #endregion
    private enum CurrentlyDisplayedInventory
    {
        all,
        consumable,
        weapon
    }

}
