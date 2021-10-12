using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDisplay : MonoBehaviour
{
    public Image itemIcon;
    internal string itemName;

    internal void LoadValues(string itemName, Sprite itemIcon)
    {
        this.itemName = itemName;
        this.itemIcon.sprite = itemIcon;
    }
}
