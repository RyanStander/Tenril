using System;
using Character;
using UnityEngine;

namespace Player
{
    public class PlayerCombatManager : MonoBehaviour
    {
        public string lastAttack;
    
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private LayerMask riposteLayer;

        private bool startedLoadingBow;
        private Camera mainCamera;

        #region Animator Variables

        private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
        private static readonly int IsHoldingArrow = Animator.StringToHash("isHoldingArrow");
        private static readonly int IsDrawn = Animator.StringToHash("isDrawn");
        private static readonly int IsInteracting = Animator.StringToHash("isInteracting");
        private static readonly int IsBlocking = Animator.StringToHash("isBlocking");
        private static readonly int IsAiming = Animator.StringToHash("isAiming");

        #endregion

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnValidate()
        {
            if (playerManager == null)
                playerManager = GetComponent<PlayerManager>();
        }

        #region Attacking

        internal void HandleAttacks()
        {
            //Player performing weak attack
            if (playerManager.inputHandler.weakAttackInput)
            {
                HandleWeakAttackAction();
            }

            //Player lets go of weak attack button
            if (playerManager.inputHandler.attackLetGoInput)
            {
                if (playerManager.isHoldingArrow)
                {
                    FireArrow();
                }
            }

            //Player performing strong attack
            if (playerManager.inputHandler.strongAttackInput)
            {
                //if finisher successful, do not perform attack
                AttemptFinisher();

                HandleStrongAttackAction();
            }

            //If the player is not holding an arrow, set the attack let go input to false, this feature exists purely for releasing arrows
            if (playerManager.inputHandler.attackLetGoInput && !startedLoadingBow)
                playerManager.inputHandler.attackLetGoInput = false;
        }

        private void HandleWeaponCombo(WeaponItem weapon, bool isWeakAttack)
        {
            if (!playerManager.inputHandler.comboFlag) return;
            playerManager.playerAnimatorManager.animator.SetBool(CanDoCombo, false);

            #region Attacks

            //checks whether its a weak or strong attack
            if (isWeakAttack)
            {
                //Checks the progress through combos, if not the end play the next one
                for (var i = 0; i < weapon.weakAttacks.Count; i++)
                {
                    if (lastAttack != weapon.weakAttacks[i]) continue;
                    //if player has any stamina
                    if (!playerManager.playerStats.HasStamina()) continue;
                    //put the players stamina regen on cooldown
                    playerManager.playerStats.PutStaminaRegenOnCooldown();
                    //Update the last attack
                    lastAttack = weapon.weakAttacks[i + 1];
                    //Sets the damage colliders the weapons damage
                    playerManager.SetDamageColliderDamage(
                        weapon.baseDamage * weapon.weakAttackDamageMultiplier);
                    //Play the following animation
                    playerManager.playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                    break;
                }
            }
            else
            {
                for (var i = 0; i < weapon.strongAttacks.Count - 1; i++)
                {
                    if (lastAttack != weapon.strongAttacks[i]) continue;
                    //if player has any stamina
                    if (!playerManager.playerStats.HasStamina()) continue;
                    //put the players stamina regen on cooldown
                    playerManager.playerStats.PutStaminaRegenOnCooldown();
                    //Update the last attack
                    lastAttack = weapon.strongAttacks[i + 1];
                    //Sets the damage colliders the weapons damage
                    playerManager.SetDamageColliderDamage(
                        weapon.baseDamage * weapon.strongAttackDamageMultiplier);
                    //Play the following animation
                    playerManager.playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                    break;
                }
            }

            #endregion
        }

        private void HandleWeakAttack(WeaponItem weapon)
        {
            //if player has any stamina
            if (!playerManager.playerStats.HasStamina()) return;
            //put the players stamina regen on cooldown
            playerManager.playerStats.PutStaminaRegenOnCooldown();
            if (weapon == null) return;
            //only attack if there are available
            if (weapon.weakAttacks.Count != 0)
            {
                //Sets the damage colliders the weapons damage
                playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.weakAttackDamageMultiplier);
                //Play animation
                playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.weakAttacks[0], true);
                //Update the last attack
                lastAttack = weapon.weakAttacks[0];
            }
            else
                Debug.LogWarning("You are trying to attack with the weapon; " + weapon.name +
                                 " that does not have any attacks");
        }

        private void HandleStrongAttack(WeaponItem weapon)
        {
            if (weapon.strongAttacks.Count == 0)
                return;
            //if player has any stamina
            if (!playerManager.playerStats.HasStamina()) return;
            //put the players stamina regen on cooldown
            playerManager.playerStats.PutStaminaRegenOnCooldown();
            if (weapon == null) return;
            //Sets the damage colliders the weapons damage
            playerManager.SetDamageColliderDamage(weapon.baseDamage * weapon.strongAttackDamageMultiplier);
            //Play animation
            playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.strongAttacks[0], true);
            //Update the last attack
            lastAttack = weapon.strongAttacks[0];
        }

        #region Input Actions

        private void HandleWeakAttackAction()
        {
            if (playerManager.playerInventory.equippedWeapon == null)
                return;

            switch (playerManager.playerInventory.equippedWeapon.weaponType)
            {
                case WeaponType.TwoHandedSword:
                    PerformMeleeAction(true);
                    break;
                case WeaponType.Polearm:
                    PerformMeleeAction(true);
                    break;
                case WeaponType.Bow:
                    PerformRangedAmmoCheck();
                    break;
                case WeaponType.FlyingMage:
                    FireMagic();
                    break;
                case WeaponType.DualBlades:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleStrongAttackAction()
        {
            if (playerManager.playerInventory.equippedWeapon == null)
                return;

            switch (playerManager.playerInventory.equippedWeapon.weaponType)
            {
                case WeaponType.TwoHandedSword:
                    PerformMeleeAction(false);
                    break;
                case WeaponType.Polearm:
                    PerformMeleeAction(false);
                    break;
                case WeaponType.Bow:
                    PerformRangedAmmoCheck();
                    break;
                case WeaponType.FlyingMage:
                    break;
                case WeaponType.DualBlades:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleParryAction()
        {
            //FOR FUTURE: check for other types, such as a bow aims instead, staff perhaps smth else
            if (playerManager.playerInventory.equippedWeapon == null)
                return;

            if (playerManager.playerInventory.equippedWeapon.canParry)
            {
                //perform parry action
                ParryAction();
            }
        }

        #endregion

        #region Combat Actions

        private void PerformMeleeAction(bool isWeakAttack)
        {
            //if player is able to perform a combo, go to following attack
            if (playerManager.canDoCombo)
            {
                playerManager.inputHandler.comboFlag = true;
                HandleWeaponCombo(playerManager.playerInventory.equippedWeapon, isWeakAttack);
                playerManager.inputHandler.comboFlag = false;
            }
            //else, perform starting attack if possible
            else
            {
                if (playerManager.isInteracting)
                    return;

                if (playerManager.canDoCombo)
                    return;

                if (isWeakAttack)
                    HandleWeakAttack(playerManager.playerInventory.equippedWeapon);
                else
                    HandleStrongAttack(playerManager.playerInventory.equippedWeapon);
            }
        }

        private void PerformRangedAmmoCheck()
        {
            //if player has any stamina
            if (!playerManager.playerStats.HasStamina()) return;
            //put the players stamina regen on cooldown
            playerManager.playerStats.PutStaminaRegenOnCooldown();

            if (playerManager.isHoldingArrow) return;
            //Check if there is ammo
            if (playerManager.playerInventory.HasAmmo())
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

        private void PerformBowDraw()
        {
            //If player does not have an arrow in their inventory, do not proceed
            if (!playerManager.playerInventory.CheckIfItemCanBeConsumed(playerManager.playerInventory.equippedAmmo))
                return;

            startedLoadingBow = true;
            playerManager.playerAnimatorManager.animator.SetBool(IsHoldingArrow, true);
            playerManager.playerAnimatorManager.PlayTargetAnimation("BowDrawArrow", true);

            Animator bowAnimator = null;

            //Get the bow depending on which hand it is instantiated in
            if (playerManager.playerInventory.equippedWeapon.rightWeaponModelPrefab != null)
            {
                bowAnimator = playerManager.weaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            }
            else if (playerManager.playerInventory.equippedWeapon.leftWeaponModelPrefab != null)
            {
                bowAnimator = playerManager.weaponSlotManager.leftHandSlot.GetComponentInChildren<Animator>();
            }
            else
                Debug.LogWarning("No bow prefab was found");

            if (bowAnimator != null)
            {
                //Animate Bow
                //set the bool of is drawn to true
                bowAnimator.SetBool(IsDrawn, true);
                //play the draw animation
                bowAnimator.Play("Draw");
            }

            playerManager.weaponSlotManager.DisplayObjectInHand(playerManager.playerInventory.equippedAmmo.loadedItemModel,
                false, false);
        }

        private void FireArrow()
        {
            if (playerManager.isInteracting)
                return;

            //If player does not have an arrow in their inventory, do not proceed
            if (!playerManager.playerInventory.CheckIfItemCanBeConsumed(playerManager.playerInventory.equippedAmmo))
                return;

            ProjectileInstantiationLocation arrowInstantiationLocation = null;
            Animator bowAnimator = null;

            //Get the bow depending on which hand it is instantiated in
            if (playerManager.playerInventory.equippedWeapon.rightWeaponModelPrefab != null)
            {
                arrowInstantiationLocation =
                    playerManager.weaponSlotManager.rightHandSlot.GetComponentInChildren<ProjectileInstantiationLocation>();
                bowAnimator = playerManager.weaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            }
            else if (playerManager.playerInventory.equippedWeapon.leftWeaponModelPrefab != null)
            {
                arrowInstantiationLocation =
                    playerManager.weaponSlotManager.leftHandSlot.GetComponentInChildren<ProjectileInstantiationLocation>();
                bowAnimator = playerManager.weaponSlotManager.leftHandSlot.GetComponentInChildren<Animator>();
            }
            else
                Debug.LogWarning("No bow prefab was found");

            //Reset players holding arrow
            playerManager.playerAnimatorManager.PlayTargetAnimation("Fire", true);
            playerManager.playerAnimatorManager.animator.SetBool(IsHoldingArrow, false);
            playerManager.inputHandler.attackLetGoInput = false;
            startedLoadingBow = false;

            if (bowAnimator != null)
            {
                //set the bool of is drawn to false
                bowAnimator.SetBool(IsDrawn, false);
                //play the fire animation
                bowAnimator.Play("Fire");
            }

            //Destroy previous loaded arrow
            playerManager.weaponSlotManager.HideObjectsInHand(false);

            //Remove an arrow from inventory
            playerManager.playerInventory.RemoveItemFromInventory(playerManager.playerInventory.equippedAmmo);

            //Create live arrow at specific location
            //TO DO: possibly check to link the rotation up to the camera facing direction
            if (arrowInstantiationLocation == null) return;
            var liveArrow = Instantiate(playerManager.playerInventory.equippedAmmo.liveAmmoModel,
                arrowInstantiationLocation.GetTransform().position, arrowInstantiationLocation.GetTransform().rotation);
            //Get rigidbody and ranged projectile dmg collider for modifying
            var rigidbody = liveArrow.GetComponentInChildren<Rigidbody>();
            var damageCollider = liveArrow.GetComponentInChildren<AmmunitionDamageCollider>();

            var arrowRotation = Quaternion.LookRotation(transform.forward);
            if (playerManager.inputHandler.lockOnFlag)
            {
                //While locked on we are always facing target, can copy our facing direction to our arrows facing direction
                liveArrow.transform.rotation = arrowRotation;
            }
            else
            {
                if (mainCamera != null)
                    liveArrow.transform.rotation = Quaternion.Euler(mainCamera.transform.eulerAngles.x,
                        playerManager.aimTransform.transform.eulerAngles.y, 0);
            }

            //give ammo rigidbody its values
            rigidbody.AddForce(liveArrow.transform.forward * playerManager.playerInventory.equippedAmmo.forwardVelocity);
            rigidbody.AddForce(liveArrow.transform.up * playerManager.playerInventory.equippedAmmo.upwardVelocity);
            rigidbody.useGravity = playerManager.playerInventory.equippedAmmo.useGravity;
            rigidbody.mass = playerManager.playerInventory.equippedAmmo.ammoMass;
            liveArrow.transform.parent = null;

            //set live arrow damage
            damageCollider.characterManager = playerManager;
            damageCollider.ammoItem = playerManager.playerInventory.equippedAmmo;
            damageCollider.currentDamage = playerManager.playerInventory.equippedAmmo.physicalDamage;

            //animate the bow firing the arrow
        }

        private void FireMagic()
        {
            if (playerManager.isInteracting)
                return;

            //If player does not have an arrow in their inventory, do not proceed
            if (!playerManager.playerInventory.CheckIfItemCanBeConsumed(playerManager.playerInventory.equippedAmmo))
                return;

            if (!(playerManager.playerInventory.equippedWeapon is SpellcasterWeaponItem spellcasterWeapon)) return;
            switch (spellcasterWeapon.magicWeaponCostType)
            {
                case SpellType.none:
                    break;
                case SpellType.biomancy when playerManager.playerStats.HasEnoughMoonlight(spellcasterWeapon.magicAttackCost):
                    playerManager.playerStats.ConsumeStoredMoonlight(spellcasterWeapon.magicAttackCost);
                    break;
                case SpellType.biomancy:
                    return;
                case SpellType.pyromancy when playerManager.playerStats.HasEnoughSunlight(spellcasterWeapon.magicAttackCost):
                    playerManager.playerStats.ConsumeStoredSunlight(spellcasterWeapon.magicAttackCost);
                    break;
                case SpellType.pyromancy:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ProjectileInstantiationLocation spellInstantiationLocation = null;

            //Get the spell weapon depending on which hand it is instantiated in
            if (playerManager.playerInventory.equippedWeapon.rightWeaponModelPrefab != null)
            {
                spellInstantiationLocation = playerManager.weaponSlotManager.rightHandSlot
                    .GetComponentInChildren<ProjectileInstantiationLocation>();
            }
            else if (playerManager.playerInventory.equippedWeapon.leftWeaponModelPrefab != null)
            {
                spellInstantiationLocation = playerManager.weaponSlotManager.leftHandSlot
                    .GetComponentInChildren<ProjectileInstantiationLocation>();
            }
            else
                Debug.LogWarning("No projectile prefab was found");

            playerManager.playerAnimatorManager.PlayTargetAnimation("Fire", true);

            //Create live arrow at specific location
            //TO DO: possibly check to link the rotation up to the camera facing direction
            var spellParticle = Instantiate(spellcasterWeapon.attackData.liveProjectileModel,
                spellInstantiationLocation.GetTransform().position, spellInstantiationLocation.GetTransform().rotation);
            //Get rigidbody and ranged projectile dmg collider for modifying
            var rigidbody = spellParticle.GetComponentInChildren<Rigidbody>();
            var damageCollider = spellParticle.GetComponentInChildren<ProjectileDamageCollider>();

            var arrowRotation = Quaternion.LookRotation(transform.forward);
            if (playerManager.inputHandler.lockOnFlag)
            {
                //While locked on we are always facing target, can copy our facing direction to our arrows facing direction
                spellParticle.transform.rotation = arrowRotation;
            }
            else
            {
                if (mainCamera != null)
                    spellParticle.transform.rotation = Quaternion.Euler(mainCamera.transform.eulerAngles.x,
                        playerManager.characterLockOnPoint.transform.eulerAngles.y, 0);
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

        private void AttemptFinisher()
        {
            if (playerManager.playerAnimatorManager.animator.GetBool(IsInteracting))
                return;

            if (playerManager.finisherAttackRayCastStartPointTransform == null)
                return;

            if (!Physics.Raycast(playerManager.finisherAttackRayCastStartPointTransform.position,
                transform.TransformDirection(Vector3.forward), out var hit, 0.7f, riposteLayer)) return;
            var enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
            var rightWeaponDamageCollider = playerManager.weaponSlotManager.rightHandDamageCollider;

            if (enemyCharacterManager == null || !enemyCharacterManager.canBeRiposted) return;
            //Check for team i.d (so cant back stab allies/self)
            //pull into a transform in front of the enemy so the riposte looks clean
            enemyCharacterManager.riposteCollider.LerpToPoint(playerManager.transform);
            //rotate towards that transform
            var rotationDirection = hit.transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            var tr = Quaternion.LookRotation(rotationDirection);
            var targetRotation =
                Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            var criticalDamage = playerManager.playerInventory.equippedWeapon.finisherDamageMultiplier *
                                 rightWeaponDamageCollider.currentDamage;
            enemyCharacterManager.pendingFinisherDamage = criticalDamage;
            //play animation
            playerManager.playerAnimatorManager.PlayTargetAnimation("Riposte", true);
            //make enemy play animation
            enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Riposted", true);
            //do damage
            return;
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

            if (playerManager.playerInventory.equippedWeapon == null)
                return;

            switch (playerManager.playerInventory.equippedWeapon.weaponType)
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
                case WeaponType.DualBlades:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ParryAction()
        {
            if (!playerManager.inputHandler.parryInput) return;
            if (playerManager.isInteracting)
                return;

            playerManager.playerAnimatorManager.PlayTargetAnimation(playerManager.playerInventory.equippedWeapon.parry,
                true);
        }

        private void BlockAction()
        {
            if (playerManager.inputHandler.blockInput)
            {
                //Prevent blocking if is already performing another action
                if (playerManager.isInteracting)
                    return;

                //Prevent blocking if is already blocking
                if (playerManager.isBlocking)
                    return;

                //Set blocking to true
                playerManager.playerAnimatorManager.animator.SetBool(IsBlocking, true);

                //Begin blocking
                playerManager.playerAnimatorManager.animator.Play("BlockStart");

                playerManager.playerStats.OpenBlockingCollider(playerManager.playerInventory);
            }
            else
            {
                //Set blocking to false
                playerManager.playerAnimatorManager.animator.SetBool(IsBlocking, false);
            }
        }

        private void AimAction()
        {
            if (playerManager.inputHandler.blockInput)
            {
                //Prevent aiming if is already performing another action
                if (playerManager.isInteracting)
                    return;

                //Prevent aiming if is already aiming
                if (playerManager.isAiming)
                    return;

                playerManager.playerAnimatorManager.animator.SetBool(IsAiming, true);

                //disable lock on
                playerManager.inputHandler.lockOnFlag = false;

                EventManager.currentManager.AddEvent(new SwapToAimCamera());
            }
            else
            {
                //Prevent exiting aiming if not in aim mode
                if (!playerManager.isAiming)
                    return;

                playerManager.playerAnimatorManager.animator.SetBool(IsAiming, false);

                EventManager.currentManager.AddEvent(new SwapToExplorationCamera());
            }
        }

        #endregion
    }
}