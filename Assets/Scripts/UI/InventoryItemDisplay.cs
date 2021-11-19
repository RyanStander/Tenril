using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDisplay : MonoBehaviour
{
    public Image itemIcon;
    internal Item item;
    public string itemType;
    public TMP_Text stackCountText;

    internal void LoadValues(Item item,int stackCount)
    {
        this.item = item;
        itemIcon.sprite = item.itemIcon;

        if (stackCount > 1)
            stackCountText.text = stackCount.ToString();
        else
            stackCountText.enabled = false;
        
        //set the type of item
        if (item is WeaponItem)
            itemType = "weapon";
        if (item is QuickslotItem)
            itemType = "quickslot";
    }

    #region Button On Click variables
    public void EquipPrimaryWeapon()
    {
        if (item is WeaponItem weapon)
            EventManager.currentManager.AddEvent(new EquipWeapon(weapon, true));
        else
            Debug.LogWarning("Item is not a weapon");
    }

    public void EquipSecondaryWeapon()
    {
        if (item is WeaponItem weapon)
            EventManager.currentManager.AddEvent(new EquipWeapon(weapon, false));
        else
            Debug.LogWarning("Item is not a weapon");
    }

    public void AddQuickslotItem()
    {
        if (item is QuickslotItem quickslotItem)
            EventManager.currentManager.AddEvent(new AddQuickslotItem(quickslotItem));
        else
            Debug.LogWarning("Item is not a quickslot item");
    }

    public void RemoveQuickslotItem()
    {
        if (item is QuickslotItem quickslotItem)
            EventManager.currentManager.AddEvent(new RemoveQuickslotItem(quickslotItem));
        else
            Debug.LogWarning("Item is not a quickslot item");
    }

    public void DropItem()
    {
        EventManager.currentManager.AddEvent(new DropItem(item));
    }

    public void UseItem()
    {
        EventManager.currentManager.AddEvent(new UseItem(item));
    }
    #endregion
}
