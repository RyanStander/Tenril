using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private InputHandler inputHandler;
    private PlayerStats playerStats;

    public string lastAttack;

    //private WeaponSlotManager weaponSlotManager
    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();

        inputHandler = GetComponent<InputHandler>();
        playerStats = GetComponent<PlayerStats>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    public void HandleAttacks(float delta)
    {
        if (inputHandler.weakAttackInput)
        {
            inputHandler.weakAttackInput = false;
            HandleWeakAttackAction();
        }

        if (inputHandler.strongAttackInput)
        {
            inputHandler.strongAttackInput = false;
            HandleStrongAttackAction();
        }
    }

    private void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            playerAnimatorManager.animator.SetBool("canDoCombo", false);

            #region Attacks
            for (int i = 0; i < weapon.weakAttacks.Count - 1; i++)
            {
                if (lastAttack == weapon.weakAttacks[i])
                {
                    lastAttack = weapon.weakAttacks[i + 1];
                    playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.weakAttackDamageMultiplier);
                    playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                    break;
                }
            }
            for (int i = 0; i < weapon.strongAttacks.Count - 1; i++)
            {
                if (lastAttack == weapon.strongAttacks[i])
                {
                    lastAttack = weapon.strongAttacks[i + 1];
                    playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.strongAttackDamageMultiplier);
                    playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                    break;
                }
            }
            #endregion
        }
    }

    private void HandleWeakAttack(WeaponItem weapon)
    {
        //weaponSlotManager.attackingWeapon = weapon;
        if (weapon != null)
        {
            playerAnimatorManager.PlayTargetAnimation(weapon.weakAttacks[0], true);
            playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.weakAttackDamageMultiplier);
            lastAttack = weapon.weakAttacks[0];
        }
    }

    private void HandleStrongAttack(WeaponItem weapon)
    {
        //weaponSlotManager.attackingWeapon = weapon;
        if (weapon != null)
        {
            playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.strongAttackDamageMultiplier);
            playerAnimatorManager.PlayTargetAnimation(weapon.strongAttacks[0], true);
            lastAttack = weapon.strongAttacks[0];
        }
    }

    #region Input Actions
    private void HandleWeakAttackAction()
    {
        PerformWeakMeleeAction();
    }

    private void HandleStrongAttackAction()
    {
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.equippedWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            if (playerManager.canDoCombo)
                return;

            HandleStrongAttack(playerInventory.equippedWeapon);
        }
    }

    #endregion

    #region Combat Actions

    private void PerformWeakMeleeAction()
    {
        //If current attack can perform a combo, proceed with combo
        if (playerAnimatorManager.animator.GetBool("canDoCombo"))
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.equippedWeapon);
            inputHandler.comboFlag = false;
        }
        //else, perform starting attack if possible
        else
        {
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            if (playerManager.canDoCombo)
                return;

            HandleWeakAttack(playerInventory.equippedWeapon);
        }
    }

    #endregion
}
