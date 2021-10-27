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
    }

    private void OnUpdateWeaponDisplay(EventData eventData)
    {
        if (eventData is UpdateWeaponDisplay updateWeaponDisplay)
        {
            //inputs the new weapon data
            primaryWeapon.SetValues(updateWeaponDisplay.primaryWeapon.itemIcon, updateWeaponDisplay.primaryWeapon.name);
            secondaryWeapon.SetValues(updateWeaponDisplay.secondaryWeapon.itemIcon, updateWeaponDisplay.secondaryWeapon.name);
            //check what weapon is currently equipped
            if (updateWeaponDisplay.isWieldingPrimaryWeapon)
            {
                //show primary as selected
                primaryWeapon.SelectWeapon();
                //show secondary as deselected
                secondaryWeapon.DeselectWeapon();
            }
            else
            {
                //show secondary as selected
                secondaryWeapon.SelectWeapon();
                //show priamry as deselected
                primaryWeapon.DeselectWeapon();
            }
        }
    }
}
