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
    public GameObject currentSheathModel;

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

    public void UnloadSheath()
    {
        //Hide the current sheath if for example sheating sword
        if (currentSheathModel != null)
        {
            currentSheathModel.SetActive(false);
        }
    }

    public void UnloadSheathAndDestroy()
    {
        //Destroy the current sheat for creation of a new one
        if (currentSheathModel != null)
        {
            Destroy(currentSheathModel);
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
            model = Instantiate(weaponItem.secondaryWeaponModelPrefab);
        else
            model = Instantiate(weaponItem.primaryWeaponModelPrefab);
        
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

    public void LoadUnequippedWeaponModel(WeaponItem weaponItem, bool isSecondaryWeapon)
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
        {
            if (weaponItem.unequippedSecondaryWeaponModelPrefab == null)
                return;
            model = Instantiate(weaponItem.unequippedSecondaryWeaponModelPrefab);
        }
        else
        {
            if (weaponItem.unequippedPrimaryWeaponModelPrefab == null)
                return;
            model = Instantiate(weaponItem.unequippedPrimaryWeaponModelPrefab);
        }

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

    public void LoadWeaponSheath(WeaponItem weaponItem, bool isSecondaryWeapon)
    {
        //Destroy the previous weapon model
        UnloadSheathAndDestroy();

        //if there is no equiped model, hide it and exit out of function
        if (weaponItem == null)
        {
            UnloadSheath();
            return;
        }

        //Create a new sheath model
        GameObject model;
        if (isSecondaryWeapon)
        {
            if (weaponItem.secondarySheathPrefab == null)
                return;
            model = Instantiate(weaponItem.secondarySheathPrefab);
        }
        else
        {
            if (weaponItem.primarySheathPrefab == null)
                return;
            model = Instantiate(weaponItem.primarySheathPrefab);
        }

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
        currentSheathModel = model;
    }
}

public enum WeaponSlot
{
    rightHandSlot,
    leftHandSlot,
    backSlot,
    leftSideSlot,
    rightSideSlot
}
