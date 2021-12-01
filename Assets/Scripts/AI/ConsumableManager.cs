using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableManager : MonoBehaviour
{
    protected AnimatorManager animatorManager;
    protected WeaponSlotManager weaponManager;

    virtual internal void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        weaponManager = GetComponent<WeaponSlotManager>();
    }

    public void DisplayItem(WeaponSlotManager weaponSlotManager, GameObject displayObject)
    {
        weaponSlotManager.DisplayObjectInHand(displayObject);
    }

    internal void HideItem(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.HideObjectInHand();
    }

    //Gets called by animations like "Drink" through animation events
    abstract internal void SuccessfulyUsedItem();
}