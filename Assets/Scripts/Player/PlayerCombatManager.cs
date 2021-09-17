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
        //Player performing weak attack
        if (inputHandler.weakAttackInput)
        {
            inputHandler.weakAttackInput = false;
            HandleWeakAttackAction();
        }

        //Player performing strong attack
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

            //Checks the progress through combos, if not the end play the next one
            #region Attacks
            for (int i = 0; i < weapon.weakAttacks.Count - 1; i++)
            {
                if (lastAttack == weapon.weakAttacks[i])
                {
                    //if player has any stamina
                    if (playerStats.HasStamina())
                    {
                        //put the players stamina regen on cooldown
                        playerStats.PutStaminaRegenOnCooldown();
                        //Update the last attack
                        lastAttack = weapon.weakAttacks[i + 1];
                        //Sets the damage colliders the weapons damage
                        playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.weakAttackDamageMultiplier);
                        //Play the following animation
                        playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                        break;
                    }
                }
            }
            for (int i = 0; i < weapon.strongAttacks.Count - 1; i++)
            {
                if (lastAttack == weapon.strongAttacks[i])
                {
                    //if player has any stamina
                    if (playerStats.HasStamina())
                    {
                        //put the players stamina regen on cooldown
                        playerStats.PutStaminaRegenOnCooldown();
                        //Update the last attack
                        lastAttack = weapon.strongAttacks[i + 1];
                        //Sets the damage colliders the weapons damage
                        playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.strongAttackDamageMultiplier);
                        //Play the following animation
                        playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                        break;
                    }
                }
            }
            #endregion
        }
    }

    private void HandleWeakAttack(WeaponItem weapon)
    {
        //if player has any stamina
        if (playerStats.HasStamina())
        {
            //put the players stamina regen on cooldown
            playerStats.PutStaminaRegenOnCooldown();
            if (weapon != null)
            {
                //Sets the damage colliders the weapons damage
                playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.weakAttackDamageMultiplier);
                //Play animation
                playerAnimatorManager.PlayTargetAnimation(weapon.weakAttacks[0], true);
                //Update the last attack
                lastAttack = weapon.weakAttacks[0];
            }
        }
    }

    private void HandleStrongAttack(WeaponItem weapon)
    {
        //if player has any stamina
        if (playerStats.HasStamina())
        {
            //put the players stamina regen on cooldown
            playerStats.PutStaminaRegenOnCooldown();
            if (weapon != null)
            {
                //Sets the damage colliders the weapons damage
                playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.strongAttackDamageMultiplier);
                //Play animation
                playerAnimatorManager.PlayTargetAnimation(weapon.strongAttacks[0], true);
                //Update the last attack
                lastAttack = weapon.strongAttacks[0];
            }
        }
    }

    #region Input Actions
    private void HandleWeakAttackAction()
    {
        PerformWeakMeleeAction();
    }

    private void HandleStrongAttackAction()
    {
        //if player is able to perform a combo, go to following attack
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.equippedWeapon);
            inputHandler.comboFlag = false;
        }
        //otherwise perform first attack
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
