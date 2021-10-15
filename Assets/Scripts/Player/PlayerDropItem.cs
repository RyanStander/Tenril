using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropItem : MonoBehaviour
{
    [SerializeField] private GameObject dropItemPrefab;
    private GameObject createdDropItem;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.DropItem, OnDropItem);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.DropItem, OnDropItem);
    }

    private void OnDropItem(EventData eventData)
    {
        if (eventData is DropItem dropItem)
        {
            createdDropItem = Instantiate(dropItemPrefab);
            ItemPickup itemPickup = createdDropItem.GetComponent<ItemPickup>();
            itemPickup.item = dropItem.item;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.EquipWeapon was received but is not of class EquipWeapon.");
        }
    }
}
