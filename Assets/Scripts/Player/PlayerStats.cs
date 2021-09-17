using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerAnimatorManager playerAnimatorManager;

    [Header("Resource bars")]
    [SerializeField] private SliderBarDisplayUI healthBar;
    [SerializeField] private SliderBarDisplayUI staminaBar, sunlightBar, moonlightBar;

    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }
    private void Start()
    {
        SetupStats();
        healthBar.SetMaxValue(maxHealth);
        staminaBar.SetMaxValue(maxStamina);
        sunlightBar.SetMaxValue(maxStoredSunlight);
        moonlightBar.SetMaxValue(maxStoredMoonlight);
    }

    #region Health
    public override void TakeDamage(float damageAmount, bool playAnimation = true)
    {
        if (isDead)
            return;

        //change current health
        base.TakeDamage(damageAmount);

        //update health display on the healthbar
        healthBar.SetCurrentValue(currentHealth);

        //play animation that player has taken damage
        if (playAnimation)
            playerAnimatorManager.PlayTargetAnimation("Hit", true);

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (playAnimation)
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
        healthBar.SetCurrentValue(currentHealth);
    }
    #endregion

    #region Stamina

    public override void DrainStamina(float drain)
    {
        base.DrainStamina(drain);

        //update the current stamina on the stamina bar
        staminaBar.SetCurrentValue(currentStamina);
    }

    protected override void RegenerateStamina()
    {
        base.RegenerateStamina();

        //update the current stamina on the stamina bar
        staminaBar.SetCurrentValue(currentStamina);
    }

    #endregion
}
