using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplayManager : MonoBehaviour
{
    [SerializeField] private WeapondDisplayDataHolder primaryWeapon;
    [SerializeField] private WeapondDisplayDataHolder secondaryWeapon;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.UpdateWeaponDisplay, OnUpdateWeaponDisplay);
        EventManager.currentManager.AddEvent(new RequestEquippedWeapons());
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.UpdateWeaponDisplay, OnUpdateWeaponDisplay);
    }

    private void OnUpdateWeaponDisplay(EventData eventData)
    {
        if (eventData is UpdateWeaponDisplay updateWeaponDisplay)
        {
            //inputs the new weapon data
            if (updateWeaponDisplay.primaryWeapon!=null)
                primaryWeapon.SetValues(updateWeaponDisplay.primaryWeapon.itemIcon, updateWeaponDisplay.primaryWeapon.name);
            //else
                //Need to change value to something to display no weapon

            if (updateWeaponDisplay.secondaryWeapon!=null)
                secondaryWeapon.SetValues(updateWeaponDisplay.secondaryWeapon.itemIcon, updateWeaponDisplay.secondaryWeapon.name);
            //else
                //Need to change value to something to display no weapon

            //check what weapon is currently equipped
            if (updateWeaponDisplay.isWieldingPrimaryWeapon)
            {
                //show primary as selected
                if (updateWeaponDisplay.primaryWeapon != null)
                    primaryWeapon.SelectWeapon();
                //show secondary as deselected
                if (updateWeaponDisplay.secondaryWeapon != null)
                    secondaryWeapon.DeselectWeapon();
            }
            else
            {
                //show secondary as selected
                if (updateWeaponDisplay.secondaryWeapon != null)
                    secondaryWeapon.SelectWeapon();
                //show priamry as deselected
                if (updateWeaponDisplay.primaryWeapon != null)
                    primaryWeapon.DeselectWeapon();
            }
        }
    }
}
