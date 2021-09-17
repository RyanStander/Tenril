using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputHandler inputHandler;
    private PlayerLocomotion playerLocomotion;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerCombatManager playerCombatManager;
    private PlayerInventory playerInventory;
    private WeaponSlotManager weaponSlotManager;

    public bool canDoCombo;
    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInventory = GetComponent<PlayerInventory>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    private void Start()
    {
        playerInventory.LoadEquippedWeapons(weaponSlotManager);
    }

    private void Update()
    {
        playerAnimatorManager.canRotate = playerAnimatorManager.animator.GetBool("canRotate");
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        inputHandler.TickInput(delta);
        playerLocomotion.HandleLocomotion(delta);
        playerCombatManager.HandleAttacks(delta);
    }

    public void SetDamageColliderDamage(float damage)
    {
        weaponSlotManager.rightHandDamageCollider.currentWeaponDamage = damage;
    }
}
