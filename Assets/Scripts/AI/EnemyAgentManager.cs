using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Serves as the uppermost connecting point of all managers and their controlled components
/// The purpose of this is to grant accesss to other managers through one centralized spot
/// This should ideally duplicate components from being used and makes for quick debugging on missing components 
/// </summary>
public class EnemyAgentManager : CharacterManager
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

    //The movement manager for the NPC
    internal EnemyMovementManager movementManager;

    //The vision manager for the NPC
    internal EnemyVisionManager visionManager;

    //The status manager for the NPC
    internal StatusEffectManager statusManager;

    //The consumable manager for the NPC
    internal EnemyConsumableManager consumableManager;

    //The attack manager for the NPC
    internal EnemyAttackManager attackManager;

    //Helper bool to prevent animations/actions from occuring until an animation is completed
    internal bool isInteracting;

    //Current time in animation recovery
    internal float currentRecoveryTime = 0;

    //Current time in hiding recovery
    internal float currentHidingTime = 0;

    [Header("Collision related colliders")]
    //Collision detection related capsules
    [SerializeField] private CapsuleCollider characterCollider;
    [SerializeField] private CapsuleCollider characterCollisionBlocker;

    [Header("Relevant target to act around")]
    //The current target of the agent
    public GameObject currentTarget;

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
        movementManager = GetComponentInChildren<EnemyMovementManager>();
        visionManager = GetComponentInChildren<EnemyVisionManager>();
        statusManager = GetComponentInChildren<StatusEffectManager>();
        consumableManager = GetComponentInChildren<EnemyConsumableManager>();
        attackManager = GetComponentInChildren<EnemyAttackManager>();
        ragdollManager = GetComponentInChildren<RagdollManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyStats == null) throw new MissingComponentException("Missing EnemyStats on " + gameObject + "!");
        if (navAgent == null) throw new MissingComponentException("Missing NavMeshAgent on " + gameObject + "!");
        if (rigidBody == null) throw new MissingComponentException("Missing Rigidbody on " + gameObject + "!");
        if (animatorManager == null) throw new MissingComponentException("Missing EnemyAnimatorManager on " + gameObject + "!");
        if (stateMachine == null) throw new MissingComponentException("Missing EnemyFSM on " + gameObject + "!");
        if (inventory == null) throw new MissingComponentException("Missing EnemyInventory on " + gameObject + "!");
        if (weaponSlotManager == null) throw new MissingComponentException("Missing WeaponSlotManager on " + gameObject + "!");
        if (movementManager == null) throw new MissingComponentException("Missing EnemyMovementManager on " + gameObject + "!");
        if (visionManager == null) throw new MissingComponentException("Missing EnemyVisionManager on " + gameObject + "!");
        if (statusManager == null) throw new MissingComponentException("Missing StatusEffectManager on " + gameObject + "!");
        if (consumableManager == null) throw new MissingComponentException("Missing EnemyConsumableManager on " + gameObject + "!");
        if (attackManager == null) throw new MissingComponentException("Missing EnemyAttackManager on " + gameObject + "!");
    }

    private void Start()
    {
        //Loads the current equipment
        inventory.LoadEquippedWeapons(weaponSlotManager);
        Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
    }

    private void Update()
    {
        //Handle timer for animation delays
        HandleRecoveryTimer();

        //Handle timer for hiding delays
        HandleHidingTimer();

        //Set interactin based on the current bool being played
        isInteracting = animatorManager.animator.GetBool("isInteracting");
        canBeRiposted = animatorManager.animator.GetBool("canBeRiposted");
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
                if (enemyStats.isDead != true)
                {
                    isInteracting = false;
                }
            }
        }
    }

    public override void EnableRagdoll()
    {
        base.EnableRagdoll();
        animatorManager.animator.enabled = false;
        characterCollider.enabled = false;
        characterCollisionBlocker.enabled = false;
    }

    private void HandleHidingTimer()
    {
        //If recovery time is not 0
        if (currentHidingTime > 0)
        {
            //Reduce by time
            currentHidingTime -= Time.deltaTime;

            //Mark as not able to hide yet
            enemyStats.canHide = false;
        }
        else
        {
            //Mark as able to hide
            enemyStats.canHide = true;
        }
    }

    public bool ShouldTryHealing()
    {
        //Return true if health is below threshold
        if (enemyStats.GetCurrentHealth() <= (enemyStats.GetMaximumHealth() * enemyStats.healingThreshold) && enemyStats.canHeal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ShouldTryHiding()
    {
        //Return true if health is below threshold
        if (enemyStats.GetCurrentHealth() <= (enemyStats.GetMaximumHealth() * enemyStats.hidingThreshold) && enemyStats.canHide)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
