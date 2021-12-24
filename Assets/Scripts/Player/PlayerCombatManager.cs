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

        //Player lets go of weak attack button
        if (inputHandler.weakAttackLetGoInput)
        {
            if (playerManager.isHoldingArrow)
            {
                FireArrow();
            }
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
        if (weapon.strongAttacks.Count==0)
            return;
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
        if (playerInventory.equippedWeapon == null)
            return;

        switch (playerInventory.equippedWeapon.weaponType)
        {
            case WeaponType.TwoHandedSword:
                PerformWeakMeleeAction();
                break;
            case WeaponType.Polearm:
                PerformWeakMeleeAction();
                break;
            case WeaponType.Bow:
                PerformRangedAmmoCheck();
                break;
            case WeaponType.FlyingMage:
                FireMagic();
                break;
        }
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
            ParryAction();
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

    private void PerformRangedAmmoCheck()
    {
        //if player has any stamina
        if (playerStats.HasStamina())
        {
            //put the players stamina regen on cooldown
            playerStats.PutStaminaRegenOnCooldown();

            if (!playerManager.isHoldingArrow)
            {
                //Check if there is ammo
                if (playerInventory.HasAmmo())
                {
                    //Draw arrow
                    PerformBowDraw();
                }
                else
                {
                    Debug.Log("No ammo to fire");
                }


                //Fire arrow when released
                //else indicate no ammo
            }
        }
    }

    private void PerformBowDraw()
    {
        playerAnimatorManager.animator.SetBool("isHoldingArrow", true);
        playerAnimatorManager.PlayTargetAnimation("BowDrawArrow", true);

        Animator bowAnimator = null;

        //Get the bow depending on which hand it is instantiated in
        if (playerInventory.equippedWeapon.rightWeaponModelPrefab != null)
        {
            bowAnimator = weaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
        }
        else if (playerInventory.equippedWeapon.leftWeaponModelPrefab != null)
        {
            bowAnimator = weaponSlotManager.leftHandSlot.GetComponentInChildren<Animator>();
        }
        else
            Debug.LogWarning("No bow prefab was found");

        if (bowAnimator != null)
        {
            //Animate Bow
            //set the bool of is drawn to true
            bowAnimator.SetBool("isDrawn", true);
            //play the draw animation
            bowAnimator.Play("Draw");
        }

        weaponSlotManager.DisplayObjectInHand(playerInventory.equippedAmmo.loadedItemModel, false, false);
        
    }

    private void FireArrow()
    {
        if (playerManager.isInteracting)
            return;

        //If player does not have an arrow in their inventory, do not proceed
        if (!playerInventory.CheckIfItemCanBeConsumed(playerInventory.equippedAmmo))
            return;

        ProjectileInstantiationLocation arrowInstantiationLocation =null;
        Animator bowAnimator = null;

        //Get the bow depending on which hand it is instantiated in
        if (playerInventory.equippedWeapon.rightWeaponModelPrefab != null)
        {
            arrowInstantiationLocation = weaponSlotManager.rightHandSlot.GetComponentInChildren<ProjectileInstantiationLocation>();
            bowAnimator = weaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
        }
        else if (playerInventory.equippedWeapon.leftWeaponModelPrefab != null)
        {
            arrowInstantiationLocation = weaponSlotManager.leftHandSlot.GetComponentInChildren<ProjectileInstantiationLocation>();
            bowAnimator = weaponSlotManager.leftHandSlot.GetComponentInChildren<Animator>();
        }
        else
            Debug.LogWarning("No bow prefab was found");

        //Reset players holding arrow
        playerAnimatorManager.PlayTargetAnimation("Fire",true);
        playerAnimatorManager.animator.SetBool("isHoldingArrow", false);

        if (bowAnimator!=null)
        {
            //set the bool of is drawn to false
            bowAnimator.SetBool("isDrawn", false);
            //play the fire animation
            bowAnimator.Play("Fire");
        }

        //Destroy previous loaded arrow
        weaponSlotManager.HideObjectInHand(false, false);

        //Remove an arrow from inventroy
        playerInventory.RemoveItemFromInventory(playerInventory.equippedAmmo);

        //Create live arrow at specific location
        //TO DO: possibly check to link the rotation up to the camera facing direction
        GameObject liveArrow = Instantiate(playerInventory.equippedAmmo.liveAmmoModel, arrowInstantiationLocation.GetTransform().position, arrowInstantiationLocation.GetTransform().rotation);
        //Get rigidbody and rangedprojectiledmgcollider for modifying
        Rigidbody rigidbody = liveArrow.GetComponentInChildren<Rigidbody>();
        AmmunitionDamageCollider damageCollider = liveArrow.GetComponentInChildren<AmmunitionDamageCollider>();

        if (inputHandler.lockOnFlag) 
        {
            //While locked on we are always facing target, can copy our facing direction to our arrows facing direction
            Quaternion arrowRotation = Quaternion.LookRotation(transform.forward);
            liveArrow.transform.rotation=arrowRotation;
        }
        else
        {
            liveArrow.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, playerManager.lockOnTransform.eulerAngles.y, 0);
        }

        //give ammo rigidbody its values
        rigidbody.AddForce(liveArrow.transform.forward * playerInventory.equippedAmmo.forwardVelocity);
        rigidbody.AddForce(liveArrow.transform.up * playerInventory.equippedAmmo.upwardVelocity);
        rigidbody.useGravity = playerInventory.equippedAmmo.useGravity;
        rigidbody.mass = playerInventory.equippedAmmo.ammoMass;
        liveArrow.transform.parent = null;

        //set live arrow damage
        damageCollider.characterManager = playerManager;
        damageCollider.ammoItem = playerInventory.equippedAmmo;
        damageCollider.currentDamage = playerInventory.equippedAmmo.physicalDamage;
        //animate the bow firing the arrow
    }

    private void FireMagic()
    {
        if (playerManager.isInteracting)
            return;

        //If player does not have an arrow in their inventory, do not proceed
        if (!playerInventory.CheckIfItemCanBeConsumed(playerInventory.equippedAmmo))
            return;

        if (playerInventory.equippedWeapon is SpellcasterWeaponItem spellcasterWeapon)
        {
            switch (spellcasterWeapon.magicWeaponCostType)
            {
                case SpellType.none:
                    break;
                case SpellType.biomancy:
                    if (playerStats.HasEnoughMoonlight(spellcasterWeapon.magicAttackCost))
                    {
                        playerStats.ConsumeStoredMoonlight(spellcasterWeapon.magicAttackCost);
                    }
                    else
                        return;
                    break;
                case SpellType.pyromancy:
                    if (playerStats.HasEnoughSunlight(spellcasterWeapon.magicAttackCost))
                    {
                        playerStats.ConsumeStoredSunlight(spellcasterWeapon.magicAttackCost);
                    }
                    else
                        return;
                    break;
            }

            ProjectileInstantiationLocation spellInstantiationLocation = null;

            //Get the spell weapon depending on which hand it is instantiated in
            if (playerInventory.equippedWeapon.rightWeaponModelPrefab != null)
            {
                spellInstantiationLocation = weaponSlotManager.rightHandSlot.GetComponentInChildren<ProjectileInstantiationLocation>();
            }
            else if (playerInventory.equippedWeapon.leftWeaponModelPrefab != null)
            {
                spellInstantiationLocation = weaponSlotManager.leftHandSlot.GetComponentInChildren<ProjectileInstantiationLocation>();
            }
            else
                Debug.LogWarning("No projectile prefab was found");

            playerAnimatorManager.PlayTargetAnimation("Fire", true);
            
            //Create live arrow at specific location
            //TO DO: possibly check to link the rotation up to the camera facing direction
            GameObject spellParticle = Instantiate(spellcasterWeapon.attackData.liveProjectileModel, spellInstantiationLocation.GetTransform().position, spellInstantiationLocation.GetTransform().rotation);
            //Get rigidbody and rangedprojectiledmgcollider for modifying
            Rigidbody rigidbody = spellParticle.GetComponentInChildren<Rigidbody>();
            ProjectileDamageCollider damageCollider = spellParticle.GetComponentInChildren<ProjectileDamageCollider>();

            if (inputHandler.lockOnFlag)
            {
                //While locked on we are always facing target, can copy our facing direction to our arrows facing direction
                Quaternion arrowRotation = Quaternion.LookRotation(transform.forward);
                spellParticle.transform.rotation = arrowRotation;
            }
            else
            {
                spellParticle.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, playerManager.lockOnTransform.eulerAngles.y, 0);
            }

            //give ammo rigidbody its values
            rigidbody.AddForce(spellParticle.transform.forward * spellcasterWeapon.attackData.forwardVelocity);
            rigidbody.AddForce(spellParticle.transform.up * spellcasterWeapon.attackData.upwardVelocity);
            rigidbody.useGravity = spellcasterWeapon.attackData.useGravity;
            rigidbody.mass = spellcasterWeapon.attackData.projectileMass;
            spellParticle.transform.parent = null;

            //set projectile damage collider
            damageCollider.characterManager = playerManager;
            damageCollider.currentDamage = spellcasterWeapon.baseDamage;
            //animate the bow firing the arrow
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

    #region Special Actions

    /// <summary>
    /// Manages special abilities such as blocking or aiming
    /// </summary>
    internal void HandleWeaponSpecificAbilities()
    {
        HandleParryAction();

        if (playerInventory.equippedWeapon == null)
            return;

        switch (playerInventory.equippedWeapon.weaponType)
        {
            case WeaponType.TwoHandedSword:
                BlockAction();
                break;
            case WeaponType.Polearm:
                BlockAction();
                break;
            case WeaponType.Bow:
                AimAction();
                break;
            case WeaponType.FlyingMage:
                AimAction();
                break;
            default:
                break;
        }

        

    }

    private void ParryAction()
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

    private void AimAction()
    {
        if (inputHandler.blockInput)
        {
            //Prevent aiming if is already performing another action
            if (playerManager.isInteracting)
                return;

            //Prevent aiming if is already aiming
            if (playerManager.isAiming)
                return;

            playerAnimatorManager.animator.SetBool("isAiming", true);

            //disable lock on
            inputHandler.lockOnFlag = false;

            EventManager.currentManager.AddEvent(new SwapToAimCamera());
        }
        else
        {
            //Prevent exiting aiming if not in aim mode
            if (!playerManager.isAiming)
                return;

            playerAnimatorManager.animator.SetBool("isAiming", false);

            EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
        }
    }
    #endregion
}
