using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;

    public WeaponItem attackingWeapon;

    private WeaponHolderSlot leftHandSlot, rightHandSlot, rightSideSlot, leftSideSlot, backSlot;

    public DamageCollider leftHandDamageCollider, rightHandDamageCollider;

    private Animator animator;

    private InputHandler inputHandler;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<InputHandler>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();

        //Get all weapon holder slots on the character
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        //Assign each slot to a respective slot
        foreach (WeaponHolderSlot weaponHolderSlot in weaponHolderSlots)
        {
            switch (weaponHolderSlot.weaponSlot)
            {
                case WeaponHolderSlot.WeaponSlot.rightHandSlot:
                    rightHandSlot = weaponHolderSlot;
                    break;
                case WeaponHolderSlot.WeaponSlot.leftHandSlot:
                    leftHandSlot = weaponHolderSlot;
                    break;
                case WeaponHolderSlot.WeaponSlot.backSlot:
                    backSlot = weaponHolderSlot;
                    break;
                case WeaponHolderSlot.WeaponSlot.leftSideSlot:
                    leftSideSlot = weaponHolderSlot;
                    break;
                case WeaponHolderSlot.WeaponSlot.rightSideSlot:
                    rightSideSlot = weaponHolderSlot;
                    break;
            }
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool hasSecondaryWeapon)
    {
        //Check if there is a secondary weapon, such as dual daggers to equip
        if (hasSecondaryWeapon)
        {
            //set the current weapon in the left hand slot equal to the weapon item
            leftHandSlot.currentWeapon = weaponItem;
            //load the secondary weapon of the weapon item to the left hand slot
            leftHandSlot.LoadWeaponModel(weaponItem,true);
            //if successful, load the damage collider
            if (leftHandSlot != null)
                LoadLeftWeaponDamageCollider();
        }

        #region Weapon Idle Anim           

        //Destroy the weapon on the back slot
        backSlot.UnloadWeaponAndDestroy();

        //
        if (weaponItem != null)
        {
            animator.CrossFade(weaponItem.idleAnimation, 0.2f);
        }
        else
        {
            //No weapon equiped

            //Set animation to unarmed stance 
            //(not animation for it currently, remember to rename to something else)
            //animator.CrossFade("Unarmed", 0.2f);
        }
        #endregion

        //set the current weapon in the right hand slot equal to the weapon item
        rightHandSlot.currentWeapon = weaponItem;
        //load the primary weapon of the weapon item to the left hand slot
        rightHandSlot.LoadWeaponModel(weaponItem,false);
        //if successful, load the damage collider
        if (rightHandSlot != null)
            LoadRightWeaponDamageCollider();

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
        leftHandDamageCollider.currentWeaponDamage = playerInventory.equippedWeapon.baseDamage;
    }

    private void LoadRightWeaponDamageCollider()
    {
        //if there is no right hand slot and there is no currently instantiated
        //right weapon, return
        if (rightHandSlot == null && rightHandSlot.currentWeaponModel == null)
            return;

        //get the value of the damage collider
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        //set the damage of the collider equal to that of the right weapon
        rightHandDamageCollider.currentWeaponDamage = playerInventory.equippedWeapon.baseDamage;
    }

    public void OpenDamageCollider()
    {
        //check if there is a secondary weapon
        //open the damage colliders
            leftHandDamageCollider.EnableDamageCollider();

            rightHandDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        //check if there is a secondary weapon
        //close the damage colliders
            leftHandDamageCollider.DisableDamageCollider();

            rightHandDamageCollider.DisableDamageCollider();
    }

    #endregion
}

//Move current left hand weapon to the back
/*backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
leftHandSlot.UnloadWeaponAndDestroy();

animator.CrossFade(weaponItem.twoHandIdle, 0.2f);*/
