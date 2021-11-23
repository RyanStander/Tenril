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
    private WeaponSlotManager weaponSlotManager;

    [SerializeField] private LayerMask riposteLayer;


    public string lastAttack;
    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();

        inputHandler = GetComponent<InputHandler>();
        playerStats = GetComponent<PlayerStats>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    #region Attacking
    internal void HandleAttacks()
    {
        //Player performing weak attack
        if (inputHandler.weakAttackInput)
        {
            HandleWeakAttackAction();
        }

        //Player performing strong attack
        if (inputHandler.strongAttackInput)
        {
            //if finisher successful, do not perform attack
            AttemptFinisher();

            HandleStrongAttackAction();
        }
    }

    private void HandleWeaponCombo(WeaponItem weapon, bool isWeakAttack)
    {
        if (inputHandler.comboFlag)
        {
            playerAnimatorManager.animator.SetBool("canDoCombo", false);

            #region Attacks
            //checks whether its a weak or strong attack
            if (isWeakAttack)
            {
                //Checks the progress through combos, if not the end play the next one
                for (int i = 0; i < weapon.weakAttacks.Count; i++)
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
            }
            else
            {


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
                //only attack if there are available
                if (weapon.weakAttacks.Count != 0)
                {
                    //Sets the damage colliders the weapons damage
                    playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.weakAttackDamageMultiplier);
                    //Play animation
                    playerAnimatorManager.PlayTargetAnimation(weapon.weakAttacks[0], true);
                    //Update the last attack
                    lastAttack = weapon.weakAttacks[0];
                }
                else
                    Debug.LogWarning("You are trying to attack with the weapon; " + weapon.name + " that does not have any attacks");
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
            HandleWeaponCombo(playerInventory.equippedWeapon,false);
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

    private void HandleParryAction()
    {
        //FOR FUTURE: check for other types, such as a bow aims instead, staff perhaps smth else
        if (playerInventory.equippedWeapon==null)
            return;

        if (playerInventory.equippedWeapon.canParry)
        {
            //perform parry action
            PerformParryAction();
        }
    }

    #endregion

    #region Combat Actions

    private void PerformWeakMeleeAction()
    {
        //If current attack can perform a combo, proceed with combo
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.equippedWeapon,true);
            inputHandler.comboFlag = false;
        }
        //else, perform starting attack if possible
        else
        {
            if (playerManager.isInteracting)
                return;

            if (playerManager.canDoCombo)
                return;

            HandleWeakAttack(playerInventory.equippedWeapon);
        }
    }

    private void AttemptFinisher()
    {
        if (playerAnimatorManager.animator.GetBool("isInteracting"))
            return;

        if (playerManager.finisherAttackRayCastStartPointTransform == null)
            return;

        RaycastHit hit;

        if (Physics.Raycast(playerManager.finisherAttackRayCastStartPointTransform.position,
            transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer))
        {
            CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
            DamageCollider rightWeaponDamageCollider = weaponSlotManager.rightHandDamageCollider;

            if (enemyCharacterManager != null && enemyCharacterManager.canBeRiposted)
            {
                //Check for team i.d (so cant back stab allies/self)
                //pull into a transform in front of the enemy so the riposte looks clean
                enemyCharacterManager.riposteCollider.LerpToPoint(playerManager.transform);
                //rotate towards that transform
                Vector3 rotationDirection;
                rotationDirection = hit.transform.position - playerManager.transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                playerManager.transform.rotation = targetRotation;

                float criticalDamage = playerInventory.equippedWeapon.finisherDamageMultiplier * rightWeaponDamageCollider.currentDamage;
                enemyCharacterManager.pendingFinisherDamage = criticalDamage;
                //play animation
                playerAnimatorManager.PlayTargetAnimation("Riposte", true);
                //make enemy play animation
                enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Riposted", true);
                //do damage
                return;
            }
        }
    }

    #endregion

    #endregion

    #region Defending

    internal void HandleDefending()
    {
        HandleParryAction();
        BlockAction();
    }

    private void PerformParryAction()
    {
        if (inputHandler.parryInput)
        {
            if (playerManager.isInteracting)
                return;

            playerAnimatorManager.PlayTargetAnimation(playerInventory.equippedWeapon.parry, true);
        }
    }

    private void BlockAction()
    {
        if (inputHandler.blockInput)
        {
            //Prevent blocking if is already performing another action
            if (playerManager.isInteracting)
                return;

            //Prevent blocking if is already blocking
            if (playerManager.isBlocking)
                return;

            //Set blocking to true
            playerAnimatorManager.animator.SetBool("isBlocking", true);

            //Begin blocking
            playerAnimatorManager.animator.Play("BlockStart");

            playerStats.OpenBlockingCollider(playerInventory);
        }
        else
        {
            //Set blocking to false
            playerAnimatorManager.animator.SetBool("isBlocking", false);
        }
    }

    #endregion
}
