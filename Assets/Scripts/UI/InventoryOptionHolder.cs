using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryOptionHolder : MonoBehaviour
{
    //holds all the possible buttons
    public GameObject equipPrimaryWeaponButton;
    public GameObject equipSecondaryWeaponButton;
    public GameObject useItemButton;
    public GameObject addToQuickSlotButton;
    public GameObject removeQuickSlotButton;
    public GameObject dropItemButton;


    private void OnEnable()
    {
        StartCoroutine(LateSubscribe());
    }

    private void OnDestroyInventoryOptionHolders(EventData eventData)
    {
        EventManager.currentManager.Unsubscribe(EventType.DestroyInventoryOptionHolders, OnDestroyInventoryOptionHolders);

        Destroy(gameObject);
    }

    private IEnumerator LateSubscribe()
    {
        yield return new WaitForSeconds(0.1f);
        EventManager.currentManager.Subscribe(EventType.DestroyInventoryOptionHolders, OnDestroyInventoryOptionHolders);
    }
}
