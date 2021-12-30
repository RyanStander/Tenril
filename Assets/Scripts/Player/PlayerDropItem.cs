using UnityEngine;

public class PlayerDropItem : MonoBehaviour
{
    [SerializeField] private GameObject dropItemPrefab;
    private GameObject createdDropItem;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.PlayerHasDroppedItem, OnDropItem);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.PlayerHasDroppedItem, OnDropItem);
    }

    private void OnDropItem(EventData eventData)
    {
        if (eventData is PlayerHasDroppedItem dropItem)
        {
            createdDropItem = Instantiate(dropItemPrefab,transform.root.transform.position,Quaternion.identity);
            ItemPickup itemPickup = createdDropItem.GetComponent<ItemPickup>();
            itemPickup.item = dropItem.item;
            itemPickup.amountOfItem = dropItem.amountToDrop;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.PlayerHasDroppedItem was received but is not of class PlayerHasDroppedItem.");
        }
    }
}
