using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages other player scripts, most updates happen here.
/// </summary>

public class PlayerManager : MonoBehaviour
{
    private InputHandler inputHandler;
    private PlayerLocomotion playerLocomotion;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerCombatManager playerCombatManager;
    private PlayerSpellcastingManager playerSpellcastingManager;
    private PlayerQuickslotManager playerQuickslotManager;
    private PlayerInventory playerInventory;
    private PlayerStats playerStats;
    private PlayerInteraction playerInteraction;
    private WeaponSlotManager weaponSlotManager;

    public bool canDoCombo;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.EquipWeapon, OnEquipWeapon);
        EventManager.currentManager.Subscribe(EventType.UseItem, OnUseItem);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.EquipWeapon, OnEquipWeapon);
        EventManager.currentManager.Unsubscribe(EventType.UseItem, OnUseItem);
    }

    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerSpellcastingManager = GetComponent<PlayerSpellcastingManager>();
        playerQuickslotManager = GetComponent<PlayerQuickslotManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        playerInteraction = GetComponent<PlayerInteraction>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    private void Start()
    {
        playerInventory.LoadEquippedWeapons(weaponSlotManager);
    }

    private void Update()
    {
        playerAnimatorManager.canRotate = playerAnimatorManager.animator.GetBool("canRotate");

        //Player is unable to perform certain actions whilst in spellcasting mode
        if (inputHandler.spellcastingModeInput)
        {
            playerSpellcastingManager.HandleSpellcasting();
        }
        else
        {
            playerLocomotion.HandleDodgeAndJumping();
            playerCombatManager.HandleAttacks();
            playerCombatManager.HandleDefending();
            playerInventory.SwapWeapon(weaponSlotManager);
            playerQuickslotManager.HandleQuickslotInputs();
        }

        playerInteraction.CheckForInteractableObject();
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        inputHandler.TickInput(delta);
        
        playerLocomotion.HandleLocomotion(delta);

        playerStats.HandleStaminaRegeneration();
    }

    private void LateUpdate()
    {
        inputHandler.ResetInputs();
    }

    #region onEvents
    private void OnEquipWeapon(EventData eventData)
    {
        if (eventData is EquipWeapon equipWeapon)
        {
            playerInventory.EquipWeapon(weaponSlotManager, equipWeapon.weaponItem, equipWeapon.isPrimaryWeapon);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.EquipWeapon was received but is not of class EquipWeapon.");
        }
    }

    private void OnUseItem(EventData eventData)
    {
        if (eventData is UseItem useItem)
        {
            if (useItem.item is QuickslotItem quickslotItem)
            {
                //set item to the current quick slot
                playerInventory.quickslotItemInUse = quickslotItem;
                //attempt using item
                quickslotItem.AttemptToUseItem(playerAnimatorManager, playerStats);
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UseItem was received but is not of class UseItem.");
        }
    }
    #endregion

    #region Getters & Setters
    internal void SetDamageColliderDamage(float damage)
    {
        weaponSlotManager.rightHandDamageCollider.currentDamage = damage;
    }

    internal PlayerStats GetPlayerStats()
    {
        return playerStats;
    }

    #endregion
}
