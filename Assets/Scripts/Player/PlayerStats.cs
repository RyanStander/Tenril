using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerAnimatorManager playerAnimatorManager;

    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }
    private void Start()
    {
        SetupStats();
    }

    public override void TakeDamage(float damageAmount, bool playAnimation = true)
    {
        if (isDead)
            return;

        //change current health
        base.TakeDamage(damageAmount);

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
}
