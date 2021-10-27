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

    
    [Range(0, 1)] public float heavyAttackLikeliness = 0.25f; //The likeliness that an attack should be heavy

    [Header("Awareness Abilities")]
    public int alertRadius = 10; //"Blind" vision range, accounts for the area around the enemy
    //public int visionRange = 15; //Range at which the the AI can directly see
    //[Range(0, 180)] public int fieldOfVision = 90; //Angle at which the AI can directly see
    public float chaseRange = 15; //Range for creature chasing, should ideally be greater than the alert and vision radius
    public float maximumAttackRange = 1.5f; //The range at which attacking should begin
    [Range(1,10)] public float rotationSpeed = 5; //The rotational speed for the AI
    [Range(1, 10)] public float attackRotationSpeed = 2.5f; //The rotational speed for the AI while attacking, recommended to be lower

    private EnemyAnimatorManager enemyAnimatorManager;

    private void Start()
    {
        SetupStats();

        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
    }

    public override void TakeDamage(float damageAmount, bool playAnimation = true)
    {
        if (enemyAnimatorManager.animator.GetBool("isInvulnerable"))
            return;

        if (isDead)
            return;

        //change current health
        base.TakeDamage(damageAmount);

        //play animation that enemy has taken damage
        if (playAnimation)
            enemyAnimatorManager.PlayTargetAnimation("Hit", true);

        //If enemy health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (playAnimation)
                enemyAnimatorManager.PlayTargetAnimation("Death", true);

            isDead = true;
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
