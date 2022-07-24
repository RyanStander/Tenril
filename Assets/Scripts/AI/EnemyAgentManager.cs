using AI;
using Character;
using UnityEngine;
using UnityEngine.AI;
using WeaponManagement;

/// <summary>
/// Serves as the uppermost connecting point of all managers and their controlled components
/// The purpose of this is to grant access to other managers through one centralized spot
/// This should ideally duplicate components from being used and makes for quick debugging on missing components 
/// </summary>
public class EnemyAgentManager : CharacterManager
{
    //Enemy stats to manage
    public EnemyStats enemyStats;

    //Navigation agent attached
    public NavMeshAgent navAgent;

    //The targeted animator manager
    public EnemyAnimatorManager animatorManager;

    //The state machine connected to the manager
    public EnemyFSM stateMachine;

    //The inventory of the enemy
    public EnemyInventory inventory;

    //The movement manager for the NPC
    public EnemyMovementManager movementManager;

    //The vision manager for the NPC
    public EnemyVisionManager visionManager;

    //The status manager for the NPC
    public StatusEffectManager statusManager;

    //The consumable manager for the NPC
    public EnemyConsumableManager consumableManager;

    //The attack manager for the NPC
    public EnemyAttackManager attackManager;
    
    //The spellcasting manager for the NPC
    public EnemySpellcastingManager spellcastingManager;
    
    //The effects manager for the NPC
    public EnemyEffectsManager enemyEffectsManager;

    //Helper bool to prevent animations/actions from occuring until an animation is completed
    internal bool isInteracting;

    //Current time in animation recovery
    internal float currentRecoveryTime = 0;

    //Current time in hiding recovery
    internal float currentHidingTime = 0;

    [Header("Collision related colliders")]
    //Collision detection related capsules
    [SerializeField]
    private CapsuleCollider characterCollider;

    [SerializeField] private CapsuleCollider characterCollisionBlocker;

    [Header("Relevant target to act around")]
    //The current target of the agent
    public GameObject currentTarget;

    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");
    private static readonly int CanBeRiposted = Animator.StringToHash("canBeRiposted");

    private void OnValidate()
    {
        SetSerializedFields();
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

        //Set interacting based on the current bool being played
        isInteracting = animatorManager.animator.GetBool(IsInteracting);
        canBeRiposted = animatorManager.animator.GetBool(CanBeRiposted);
    }

    private void HandleRecoveryTimer()
    {
        //If recovery time is not 0, reduce by time
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        //If is not currently performing an animation related action
        if (!isInteracting) return;
        //Check if recovery time is not completed
        if (!(currentRecoveryTime <= 0)) return;

        //If the enemy is not dead yet, reset bool
        if (enemyStats.isDead != true)
        {
            isInteracting = false;
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
        return enemyStats.currentHealth <= (enemyStats.maxHealth * enemyStats.healingThreshold) && enemyStats.canHeal;
    }

    public bool ShouldTryHiding()
    {
        //Return true if health is below threshold
        return enemyStats.currentHealth <= (enemyStats.maxHealth * enemyStats.hidingThreshold) && enemyStats.canHide;
    }

    private void SetSerializedFields()
    {
        //Getters for relevant references
        if (enemyStats == null)
            enemyStats = GetComponent<EnemyStats>();
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();
        if (animatorManager == null)
            animatorManager = GetComponent<EnemyAnimatorManager>();
        if (stateMachine == null)
            stateMachine = GetComponent<EnemyFSM>();
        if (inventory == null)
            inventory = GetComponent<EnemyInventory>();
        if (weaponSlotManager == null)
            weaponSlotManager = GetComponent<WeaponSlotManager>();
        if (movementManager == null)
            movementManager = GetComponent<EnemyMovementManager>();
        if (visionManager == null)
            visionManager = GetComponent<EnemyVisionManager>();
        if (statusManager == null)
            statusManager = GetComponent<StatusEffectManager>();
        if (consumableManager == null)
            consumableManager = GetComponent<EnemyConsumableManager>();
        if (attackManager == null)
            attackManager = GetComponent<EnemyAttackManager>();
        if (spellcastingManager == null)
            spellcastingManager = GetComponent<EnemySpellcastingManager>();
        if (enemyEffectsManager == null)
            enemyEffectsManager = GetComponent<EnemyEffectsManager>();
        
        if (ragdollManager == null)
            ragdollManager = GetComponentInChildren<RagdollManager>();
        if (characterLockOnPoint == null)
            characterLockOnPoint = GetComponentInChildren<CharacterLockOnPoint>();

        //Null check for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyStats == null) throw new MissingComponentException("Missing EnemyStats on " + gameObject + "!");
        if (navAgent == null) throw new MissingComponentException("Missing NavMeshAgent on " + gameObject + "!");
        if (rigidBody == null) throw new MissingComponentException("Missing Rigidbody on " + gameObject + "!");
        if (animatorManager == null)
            throw new MissingComponentException("Missing EnemyAnimatorManager on " + gameObject + "!");
        if (stateMachine == null) throw new MissingComponentException("Missing EnemyFSM on " + gameObject + "!");
        if (inventory == null) throw new MissingComponentException("Missing EnemyInventory on " + gameObject + "!");
        if (weaponSlotManager == null)
            throw new MissingComponentException("Missing WeaponSlotManager on " + gameObject + "!");
        if (movementManager == null)
            throw new MissingComponentException("Missing EnemyMovementManager on " + gameObject + "!");
        if (visionManager == null)
            throw new MissingComponentException("Missing EnemyVisionManager on " + gameObject + "!");
        if (statusManager == null)
            throw new MissingComponentException("Missing StatusEffectManager on " + gameObject + "!");
        if (consumableManager == null)
            throw new MissingComponentException("Missing EnemyConsumableManager on " + gameObject + "!");
        if (attackManager == null)
            throw new MissingComponentException("Missing EnemyAttackManager on " + gameObject + "!");
        if (spellcastingManager == null)
            throw new MissingComponentException("Missing EnemySpellcastingManager on " + gameObject + "!");
        if (enemyEffectsManager == null)
            throw new MissingComponentException("Missing EnemyEffectsManager on " + gameObject + "!");
        if (characterLockOnPoint == null)
            throw new MissingComponentException("Missing CharacterLockOnPoint on " + gameObject + "!");
    }
}