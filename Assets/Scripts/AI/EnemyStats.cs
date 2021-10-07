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

    [Header("Awareness Abilities")]
    public int alertRadius = 10; //"Blind" vision range, accounts for the area around the enemy
    //public int visionRange = 20; //Range at which the the AI can directly see
    //[Range(0, 180)] public int fieldOfVision = 90; //Angle at which the AI can directly see
    public float chaseRange = 20; //Range for creature chasing, should ideally be greater than the alert and vision radius
    public float maximumAttackRange = 1.5f; //The range at which attacking should begin
    public float rotationSpeed = 15; //The rotational speed for the AI

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
