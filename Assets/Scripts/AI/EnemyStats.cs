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

    private void Start()
    {
        SetupStats();
    }

    public override void TakeDamage(float damageAmount, bool playAnimation = true)
    {
        //Return if already dead
        if (isDead) return;

        //Change current health
        base.TakeDamage(damageAmount);

        //If character health reaches or goes past 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            isDead = true;

            //Handle player death
        }
    }

    public override void RegainHealth(float regainAmount)
    {
        //Return if already dead
        if (isDead) return;

        //change current health
        base.RegainHealth(regainAmount);
    }
}
