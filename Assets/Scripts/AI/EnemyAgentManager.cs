using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

//Helps manage and link together pieces of an enemy agent
public class EnemyAgentManager : MonoBehaviour
{
    //Enemy stats to manage
    internal EnemyStats enemyStats;

    //Navigation agent attached
    internal NavMeshAgent navAgent;

    //Rigidbody that should be affected physics wise
    internal Rigidbody rigidBody;

    //The targetted animator manager
    internal EnemyAnimatorManager animatorManager;

    //The state machine connected to the manager
    internal EnemyFSM stateMachine;

    //The inventory of the enemy
    internal EnemyInventory inventory;

    //The inventory of the enemy
    internal WeaponSlotManager weaponSlotManager;

    //The current target of the agent
    internal GameObject currentTarget;

    //Helper bool to prevent animations/actions from occuring until an animation is completed
    internal bool isInteracting;

    //Current time in animation recovery
    internal float currentRecoveryTime = 0;

    //Bool to track the current life state of the enemy
    internal bool enemyIsDead = false;

    private void Awake()
    {
        //Getters for relevant references
        enemyStats = GetComponentInChildren<EnemyStats>();
        navAgent = GetComponentInChildren<NavMeshAgent>();
        rigidBody = GetComponentInChildren<Rigidbody>();
        animatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        stateMachine = GetComponentInChildren<EnemyFSM>();
        inventory = GetComponentInChildren<EnemyInventory>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();

        //Nullcheck for missing, debug warning as this does not guarantee it will break the code, but is likely to
        if (enemyStats == null) Debug.LogWarning("Missing EnemyStats on " + gameObject + "!");
        if (navAgent == null) Debug.LogWarning("Missing NavMeshAgent on " + gameObject + "!");
        if (rigidBody == null) Debug.LogWarning("Missing Rigidbody on " + gameObject + "!");
        if (animatorManager == null) Debug.LogWarning("Missing EnemyAnimatorManager on " + gameObject + "!");
        if (stateMachine == null) Debug.LogWarning("Missing EnemyFSM on " + gameObject + "!");
        if (inventory == null) Debug.LogWarning("Missing EnemyInventory on " + gameObject + "!");
        if (weaponSlotManager == null) Debug.LogWarning("Missing WeaponSlotManager on " + gameObject + "!");
    }

    private void Start()
    {
        inventory.LoadEquippedWeapons(weaponSlotManager);
    }

    private void Update()
    {
        //Handle timer for animation delays
        HandleRecoveryTimer();

        //Set interactin based on the current bool being played
        isInteracting = animatorManager.animator.GetBool("isInteracting");
    }

    private void HandleRecoveryTimer()
    {
        //If recovery time is not 0, reduce by time
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        //If currently performing an animation related action
        if (isInteracting)
        {
            //Check if recovery time is completed
            if (currentRecoveryTime <= 0)
            {
                //If the enemy is not dead yet, reset bool
                if (enemyIsDead != true)
                {
                    isInteracting = false;
                }
            }
        }
    }
}
