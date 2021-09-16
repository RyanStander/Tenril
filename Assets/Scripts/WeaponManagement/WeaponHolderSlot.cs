using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    /// <summary>
    /// Place these on areas where the weapons should be attached to, such as hands for blades, back for a back weapon, etc.
    /// Can make a child to override the placement of the weapon.
    /// </summary>
    [Header("Used for overriding where the weapon is loaded onto")]
    public Transform parentOverride;
    [Header("The weapon that is currently loaded")]
    public WeaponItem currentWeapon;

    [Header("The slot to assign the weapon to")]
    public WeaponSlot weaponSlot;

    //The model of the loaded weapon
    public GameObject currentWeaponModel;

    public enum WeaponSlot
    {
        rightHandSlot,
        leftHandSlot,
        backSlot,
        leftSideSlot,
        rightSideSlot
    }

    public void UnloadWeapon()
    {
        //Hide the current weapon if for example sheating sword
        if (currentWeaponModel != null)
        {
            currentWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy()
    {
        //Destroy the current weapon for creation of a new one
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeaponModel(WeaponItem weaponItem,bool isSecondaryWeapon)
    {
        //Destroy the previous weapon model
        UnloadWeaponAndDestroy();

        //if there is no equiped model, hide it and exit out of function
        if (weaponItem == null)
        {
            UnloadWeapon();
            return;
        }

        //Create a new weapon model
        GameObject model;
        if (isSecondaryWeapon)
            model = Instantiate(weaponItem.secondaryWeaponModelPrefab) as GameObject;
        else
            model = Instantiate(weaponItem.primaryWeaponModelPrefab) as GameObject;
        
        if (model != null)
        {
            //Create the weapon at a object based on whether it has override or not
            if (parentOverride != null)
            {
                model.transform.parent = parentOverride;
            }
            else
            {
                model.transform.parent = transform;
            }

            //Reset all its local transforms
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }
        else
        {
            //no model
            Debug.Log("No model was created, check the selected weapon item model prefab");
        }
        currentWeaponModel = model;
    }
}
