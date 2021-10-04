using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotUIManager : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private Image quickSlotItemRight,quickSlotItemLeft,currentQuickSlotItem;

    private void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        }
        LoadQuickslotItemsToDisplay();
    }

    private void LoadQuickslotItemsToDisplay()
    {
        //Get current quickslotItem
        int currentIndexValue = playerInventory.quickslotItems.IndexOf(playerInventory.currentQuickSlotItem);

        //used to hide ui elements
        Color transperant = new Color(1, 1, 1, 0);
        //used to show ui elements
        Color nonTransperant = new Color(1, 1, 1, 1);

        //if the value is -1 that means the quick slot item that is equipped does not exist
        if (currentIndexValue == -1)
        {
            Debug.LogError("The currently selected quickslot item could not be found in the list, please make sure it is set properly");
            currentQuickSlotItem.color = transperant;
            quickSlotItemLeft.color = transperant;
            quickSlotItemRight.color = transperant;
            return;
        }


        currentQuickSlotItem.sprite = playerInventory.currentQuickSlotItem.itemIcon;

        //if there is only a single quick slot item, hide the left and right ones
        if (playerInventory.quickslotItems.Count==1)
        {
            currentQuickSlotItem.color = nonTransperant;
            quickSlotItemLeft.color = transperant;
            quickSlotItemRight.color = transperant;
            return;
        }
        else
        {
            //if there is more than one, show quick slot images
            currentQuickSlotItem.color = nonTransperant;
            quickSlotItemLeft.color = nonTransperant;
            quickSlotItemRight.color = nonTransperant;
        }

        //Right quick slot
        //if the current quick slot is at the end
        if (currentIndexValue==playerInventory.quickslotItems.Count-1)
            quickSlotItemRight.sprite = playerInventory.quickslotItems[0].itemIcon;
        //else set it to one higher than the current index
        else
            quickSlotItemRight.sprite = playerInventory.quickslotItems[currentIndexValue + 1].itemIcon;

        //Left quick slot
        //if the current quick slot is at the start
        if (currentIndexValue==0)
            quickSlotItemLeft.sprite = playerInventory.quickslotItems[playerInventory.quickslotItems.Count - 1].itemIcon;
        //else set it to one lower than the current index
        else
            quickSlotItemLeft.sprite = playerInventory.quickslotItems[currentIndexValue - 1].itemIcon;
    }
}
