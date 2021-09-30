using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script originally created by Ryan Stander
//Adapted and built into the FSM by Jacques Venter
public class EnemyStats : CharacterStats
{
    //[Header("Behaviour Traits")]
    //TODO: Create traits that affect the behaviour of the AI
    //public List<Traits> enemyTraits = new List<Traits>();

    [Header("Resource bars")]
    [SerializeField] private SliderBarDisplayUI healthBar;

    private void Start()
    {
        SetupStats();
        healthBar.SetMaxValue(maxHealth);
    }

    public override void TakeDamage(float damageAmount, bool playAnimation = true)
    {
        if (isDead)
            return;

        //change current health
        base.TakeDamage(damageAmount);

        //update health display on the healthbar
        healthBar.SetCurrentValue(currentHealth);

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;

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
}
