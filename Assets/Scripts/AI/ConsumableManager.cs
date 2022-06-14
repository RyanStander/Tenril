using UnityEngine;

public abstract class ConsumableManager : MonoBehaviour
{
    protected CharacterManager characterManager;

    virtual internal void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    public void DisplayItem(GameObject displayObject)
    {
        characterManager.weaponSlotManager.DisplayObjectInHand(displayObject);
    }

    internal void HideItem(WeaponSlotManager weaponSlotManager)
    {
        weaponSlotManager.HideObjectsInHand();
    }

    //Gets called by animations like "Drink" through animation events
    abstract internal void SuccessfulyUsedItem();
}