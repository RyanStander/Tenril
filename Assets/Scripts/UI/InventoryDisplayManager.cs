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

    private void OnEnable()
    {
        UpdateGoldDisplay();

        LoadAllInventoryToDisplay();
    }

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

        foreach (Item item in playerInventory.Inventory)
        {
            //create inventory item
            GameObject createdInventoryItemPrefab = Instantiate(inventoryItemDisplayPrefab, inventoryContentObject.transform);

            //Get the inventory display script
            InventoryItemDisplay inventoryItemDisplay = createdInventoryItemPrefab.GetComponent<InventoryItemDisplay>();

            //check if an inventoryitemdisplay script is present
            if (inventoryItemDisplay!=null)
            {
                inventoryItemDisplay.LoadValues(item.itemName, item.itemIcon);
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

        foreach (Item item in playerInventory.Inventory)
        {
            //check if the item is a weapon item
            if (item is WeaponItem weapon)
            {
                //create inventory item
                GameObject createdInventoryItemPrefab = Instantiate(inventoryItemDisplayPrefab, inventoryContentObject.transform);

                //Get the inventory display script
                InventoryItemDisplay inventoryItemDisplay = createdInventoryItemPrefab.GetComponent<InventoryItemDisplay>();

                //check if an inventoryitemdisplay script is present
                if (inventoryItemDisplay != null)
                {
                    inventoryItemDisplay.LoadValues(item.itemName, item.itemIcon);
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

        foreach (Item item in playerInventory.Inventory)
        {
            //check if the item is a weapon item
            if (item is QuickslotItem consumable)
            {
                //create inventory item
                GameObject createdInventoryItemPrefab = Instantiate(inventoryItemDisplayPrefab, inventoryContentObject.transform);

                //Get the inventory display script
                InventoryItemDisplay inventoryItemDisplay = createdInventoryItemPrefab.GetComponent<InventoryItemDisplay>();

                //check if an inventoryitemdisplay script is present
                if (inventoryItemDisplay != null)
                {
                    inventoryItemDisplay.LoadValues(item.itemName, item.itemIcon);
                }
            }
        }
    }

}
