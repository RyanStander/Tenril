using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerAnimatorManager playerAnimatorManager;
    private BlockingCollider blockingCollider;

    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        blockingCollider = GetComponentInChildren<BlockingCollider>();
    }
    private void Start()
    {
        SetupStats();

        EventManager.currentManager.AddEvent(new UpdatePlayerStats(maxHealth, currentHealth, maxStamina,currentStamina,maxStoredMoonlight,currentStoredMoonlight,maxStoredSunlight,currentStoredSunlight));
    }

    public void OpenBlockingCollider(PlayerInventory playerInventory)
    {
        if (playerInventory==null)
        {
            Debug.LogWarning("Player inventory was not found, make sure the script calling this function has an inventory reference");
            return;
        }
        if (playerInventory.equippedWeapon==null)
        {
            Debug.LogWarning("Player inventory weapon was not found, weapon is required to block");
            return;
        }
        blockingCollider.SetColliderDamageAbsorption(playerInventory.equippedWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider()
    {
        blockingCollider.DisableBlockingCollider();
    }

    #region Health
    public override void TakeDamage(float damageAmount, bool playAnimation = true, string damageAnimation = "Hit")
    {
        if (playerAnimatorManager.animator.GetBool("isInvulnerable"))
            return;

        if (isDead)
            return;

        //change current health
        base.TakeDamage(damageAmount);

        //update health display on the healthbar
        EventManager.currentManager.AddEvent(new UpdatePlayerHealth(maxHealth, currentHealth));

        //play animation that player has taken damage
        if (playAnimation)
            playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            
            playerAnimatorManager.PlayTargetAnimation("Death", true);

            isDead = true;

            //Handle player death
        }
    }

    public override void RegainHealth(float regainAmount)
    {
        if (isDead)
            return;

        //change current health
        base.RegainHealth(regainAmount);

        //update health display on the healthbar
        EventManager.currentManager.AddEvent(new UpdatePlayerHealth(maxHealth, currentHealth));
    }
    #endregion

    #region Stamina

    public override void DrainStamina(float drain)
    {
        base.DrainStamina(drain);

        //update the current stamina on the stamina bar
        EventManager.currentManager.AddEvent(new UpdatePlayerStamina(maxStamina, currentStamina));
    }

    protected override void RegenerateStamina()
    {
        base.RegenerateStamina();

        //update the current stamina on the stamina bar
        EventManager.currentManager.AddEvent(new UpdatePlayerStamina(maxStamina, currentStamina));
    }

    #endregion

    #region Spellcasting

    public override void ConsumeStoredMoonlight(float cost)
    {
        base.ConsumeStoredMoonlight(cost);

        //update the current moonlight on the moonlight bar
        EventManager.currentManager.AddEvent(new UpdatePlayerMoonlight(maxStoredMoonlight, currentStoredMoonlight));
    }

    public override void ConsumeStoredSunlight(float cost)
    {
        base.ConsumeStoredSunlight(cost);

        //update the current sunlight on the sunlight bar
        EventManager.currentManager.AddEvent(new UpdatePlayerHealth(maxStoredSunlight, currentStoredSunlight));
    }

    #endregion

    internal bool GetIsDead()
    {
        return isDead;
    }
}
