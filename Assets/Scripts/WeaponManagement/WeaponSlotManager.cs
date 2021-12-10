using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    private CharacterStats chracterStats;
    private CharacterInventory characterInventory;

    [HideInInspector]public WeaponHolderSlot leftHandSlot, rightHandSlot, rightSideSlot, leftSideSlot, backSlot;

    public DamageCollider leftHandDamageCollider, rightHandDamageCollider;

    private GameObject leftDisplayObject, rightDisplayObject;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        chracterStats = GetComponent<CharacterStats>();
        characterInventory = GetComponent<CharacterInventory>();

        //Get all weapon holder slots on the character
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        //Assign each slot to a respective slot
        foreach (WeaponHolderSlot weaponHolderSlot in weaponHolderSlots)
        {
            switch (weaponHolderSlot.weaponSlot)
            {
                case WeaponSlot.rightHandSlot:
                    rightHandSlot = weaponHolderSlot;
                    break;
                case WeaponSlot.leftHandSlot:
                    leftHandSlot = weaponHolderSlot;
                    break;
                case WeaponSlot.backSlot:
                    backSlot = weaponHolderSlot;
                    break;
                case WeaponSlot.leftSideSlot:
                    leftSideSlot = weaponHolderSlot;
                    break;
                case WeaponSlot.rightSideSlot:
                    rightSideSlot = weaponHolderSlot;
                    break;
            }
        }
    }

    public void HideWeapons()
    {
        if(leftHandSlot.currentWeaponModel != null)
            leftHandSlot.currentWeaponModel.SetActive(false);
        if (rightHandSlot.currentWeaponModel != null)
            rightHandSlot.currentWeaponModel.SetActive(false);
    }

    public void ShowWeapons()
    {
        if (leftHandSlot.currentWeaponModel != null)
            leftHandSlot.currentWeaponModel.SetActive(true);
        if (rightHandSlot.currentWeaponModel != null)
            rightHandSlot.currentWeaponModel.SetActive(true);
    }

    public void DisplayObjectInHand(GameObject objectToDisplay, bool isInLeftHandSlot = true, bool hideWeapon = true)
    {
        if (isInLeftHandSlot)
        {
            if (hideWeapon)
            {
                if (leftHandSlot.currentWeaponModel != null)
                    leftHandSlot.currentWeaponModel.SetActive(false);
            }
            if (objectToDisplay != null)
            {
                if (leftHandSlot.parentOverride != null)
                    leftDisplayObject = Instantiate(objectToDisplay, leftHandSlot.parentOverride);
                else
                    leftDisplayObject = Instantiate(objectToDisplay, leftHandSlot.transform);
            }
        }
        else
        {
            if (hideWeapon)
            {
                if (rightHandSlot.currentWeaponModel != null)
                    rightHandSlot.currentWeaponModel.SetActive(false);
            }
            if (objectToDisplay != null)
            {
                if (rightHandSlot.parentOverride != null)
                    rightDisplayObject = Instantiate(objectToDisplay, rightHandSlot.parentOverride);
                else
                    rightDisplayObject = Instantiate(objectToDisplay, rightHandSlot.transform);
            }
        }
    }
    public void HideObjectInHand(bool hideLeftHandObject = true,bool showWeaponIfHidden=true)
    {
        if (hideLeftHandObject)
        {
            if (leftDisplayObject != null)
                Destroy(leftDisplayObject);
            if(showWeaponIfHidden)
                if (leftHandSlot.currentWeaponModel != null)
                    leftHandSlot.currentWeaponModel.SetActive(true);
        }
        else
        {
            if (rightDisplayObject != null)
                Destroy(rightDisplayObject);
            if (showWeaponIfHidden)
                if (rightHandSlot.currentWeaponModel != null)
                    rightHandSlot.currentWeaponModel.SetActive(true);
        }

    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, WeaponItem unequippedWeapon = null)
    {
        //remove all previous weapons and sheaths displayed
        if (leftHandSlot != null)
        {
            leftHandSlot.UnloadWeaponAndDestroy();
        }
        if (rightHandSlot != null)
        {
            rightHandSlot.UnloadWeaponAndDestroy();
        }
        if (backSlot != null)
        {
            backSlot.UnloadWeaponAndDestroy();
            backSlot.UnloadSheathAndDestroy();
        }
        if (leftSideSlot != null)
        {
            leftSideSlot.UnloadWeaponAndDestroy();
            leftSideSlot.UnloadSheathAndDestroy();
        }
        if (rightSideSlot != null)
        {
            rightSideSlot.UnloadWeaponAndDestroy();
            rightSideSlot.UnloadSheathAndDestroy();
        }

        if (weaponItem == null)
            return;

        //Check if there is a left weapon, such as dual daggers to equip
        if (weaponItem.leftWeaponModelPrefab != null)
        {
            //set the current weapon in the left hand slot equal to the weapon item
            leftHandSlot.currentWeapon = weaponItem;
            //load the secondary weapon of the weapon item to the left hand slot
            leftHandSlot.LoadWeaponModel(weaponItem, true);
            //if successful, load the damage collider
            if (leftHandSlot != null)
                LoadLeftWeaponDamageCollider();
        }

        if (weaponItem.rightWeaponModelPrefab != null)
        {
            //set the current weapon in the right hand slot equal to the weapon item
            rightHandSlot.currentWeapon = weaponItem;
            //load the primary weapon of the weapon item to the left hand slot
            rightHandSlot.LoadWeaponModel(weaponItem, false);
            //if successful, load the damage collider
            if (rightHandSlot != null)
                LoadRightWeaponDamageCollider();
        }

        #region Weapon Idle Anim           

        if (weaponItem != null)
        {
            animator.CrossFade(weaponItem.idleAnimation, 0.1f);
        }
        else
        {
            //No weapon equiped

            //Set animation to unarmed stance 
            //(no animation for it currently, remember to rename to something else)
            animator.CrossFade("UnarmedIdle", 0.2f);

            return;
        }
        #endregion

        //loads the sheaths of the weapon currently wielded
        LoadWeaponSheath(weaponItem);

        //loads the weapons taht are not being wielded
        LoadUnequippedWeapons(unequippedWeapon);

    }

    #region Damage Colliders
    //Loads the damage of the item on the damage collider of the weapon
    private void LoadLeftWeaponDamageCollider()
    {
        //if there is no left hand slot and there is no currently instantiated
        //left weapon, return
        if (leftHandSlot == null && leftHandSlot.currentWeaponModel == null)
            return;

        //get the value of the damage collider
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        //set the damage of the collider equal to that of the left weapon
        leftHandDamageCollider.currentDamage = characterInventory.equippedWeapon.baseDamage;
        //set the damage collider's character manager
        leftHandDamageCollider.characterManager = GetComponent<CharacterManager>();
    }

    private void LoadRightWeaponDamageCollider()
    {
        //if there is no right hand slot or there is no currently instantiated
        //right weapon, return
        if (rightHandSlot == null || rightHandSlot.currentWeaponModel == null)
            return;

        //get the value of the damage collider
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();

        //set the damage of the collider equal to that of the right weapon
        rightHandDamageCollider.currentDamage = characterInventory.equippedWeapon.baseDamage;
        //set the damage collider's character manager
        rightHandDamageCollider.characterManager = GetComponent<CharacterManager>();
    }

    public void OpenDamageCollider()
    {
        //check if there is a secondary weapon
        //open the damage colliders
        if (leftHandDamageCollider != null)
            leftHandDamageCollider.EnableDamageCollider();

        if (rightHandDamageCollider != null)
            rightHandDamageCollider.EnableDamageCollider();
        else
            Debug.Log("Damage collider not set");
    }

    public void CloseDamageCollider()
    {
        //check if there is a secondary weapon
        //close the damage colliders
        if (leftHandDamageCollider != null)
            leftHandDamageCollider.DisableDamageCollider();

        if (rightHandDamageCollider != null)
            rightHandDamageCollider.DisableDamageCollider();
        else
            Debug.Log("Damage collider not set");
    }

    #endregion

    private void LoadWeaponSheath(WeaponItem weaponItem)
    {
        switch (weaponItem.weaponSlotWhenNotWielded)
        {
            case WeaponSlot.rightHandSlot:
                Debug.LogWarning("You set the weaponSlotWhenNotWielded of " + weaponItem.name + " to be that of a hand, this cannot be done");
                break;
            case WeaponSlot.leftHandSlot:
                Debug.LogWarning("You set the weaponSlotWhenNotWielded of " + weaponItem.name + " to be that of a hand, this cannot be done");
                break;
            case WeaponSlot.backSlot:
                if (backSlot != null)
                    backSlot.LoadWeaponSheath(weaponItem, false);
                break;
            case WeaponSlot.leftSideSlot:
                if (leftSideSlot != null)
                {
                    leftSideSlot.LoadWeaponSheath(weaponItem, false);
                    if (weaponItem.displaySecondaryWeaponWhenUnequipped)
                        rightHandSlot.LoadWeaponSheath(weaponItem, true);
                }
                break;
            case WeaponSlot.rightSideSlot:
                if (rightSideSlot != null)
                {
                    rightSideSlot.LoadWeaponSheath(weaponItem, false);
                    if (weaponItem.displaySecondaryWeaponWhenUnequipped)
                        leftSideSlot.LoadWeaponSheath(weaponItem, true);
                }
                break;
        }
    }

    private void LoadUnequippedWeapons(WeaponItem unequippedWeapon)
    {
        //Displays the weapon slot if an unequipped weapon was given
        if (unequippedWeapon != null)
            switch (unequippedWeapon.weaponSlotWhenNotWielded)
            {
                case WeaponSlot.rightHandSlot:
                    Debug.LogWarning("You set the unequipped weapon, " + unequippedWeapon.name + " to be that of a hand, this cannot be done");
                    break;
                case WeaponSlot.leftHandSlot:
                    Debug.LogWarning("You set the unequipped weapon, " + unequippedWeapon.name + " to be that of a hand, this cannot be done");
                    break;
                case WeaponSlot.backSlot:
                    if (backSlot != null)
                        backSlot.LoadUnequippedWeaponModel(unequippedWeapon, false);
                    break;
                case WeaponSlot.leftSideSlot:
                    if (leftSideSlot != null)
                    {
                        leftSideSlot.LoadUnequippedWeaponModel(unequippedWeapon, false);
                        if (unequippedWeapon.displaySecondaryWeaponWhenUnequipped)
                            rightHandSlot.LoadUnequippedWeaponModel(unequippedWeapon, true);
                    }
                    break;
                case WeaponSlot.rightSideSlot:
                    if (rightSideSlot != null)
                    {
                        rightSideSlot.LoadUnequippedWeaponModel(unequippedWeapon, false);
                        if (unequippedWeapon.displaySecondaryWeaponWhenUnequipped)
                            leftSideSlot.LoadUnequippedWeaponModel(unequippedWeapon, true);
                    }
                    break;
            }
    }

    public void DrainWeakStaminaAttack()
    {
        //Drains stamina based on what attack type the player is using
        chracterStats.DrainStamina(characterInventory.equippedWeapon.baseStaminaCost * characterInventory.equippedWeapon.weakAttackCostMultiplier);
    }

    public void DrainStrongStaminaAttack()
    {
        //Drains stamina based on what attack type the player is using
        chracterStats.DrainStamina(characterInventory.equippedWeapon.baseStaminaCost * characterInventory.equippedWeapon.strongAttackCostMultiplier);
    }
}