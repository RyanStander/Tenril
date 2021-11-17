using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages other player scripts, most updates happen here.
/// </summary> 
#region RequiredComponents
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(PlayerAnimatorManager))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerLocomotion))]
[RequireComponent(typeof(PlayerCombatManager))]
[RequireComponent(typeof(WeaponSlotManager))]
[RequireComponent(typeof(PlayerSpellcastingManager))]
[RequireComponent(typeof(PlayerQuickslotManager))]
[RequireComponent(typeof(StatusEffectManager))]
[RequireComponent(typeof(PlayerInteraction))]
#endregion
public class PlayerManager : CharacterManager
{
    private InputHandler inputHandler;
    private PlayerLocomotion playerLocomotion;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerCombatManager playerCombatManager;
    private PlayerSpellcastingManager playerSpellcastingManager;
    private PlayerQuickslotManager playerQuickslotManager;
    private PlayerInventory playerInventory;
    private PlayerStats playerStats;
    private PlayerInteraction playerInteraction;

    [SerializeField] private CapsuleCollider characterCollider;
    [SerializeField] private CapsuleCollider characterCollisionBlocker;

    public bool canDoCombo, isInteracting;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.EquipWeapon, OnEquipWeapon);
        EventManager.currentManager.Subscribe(EventType.UseItem, OnUseItem);
        EventManager.currentManager.Subscribe(EventType.InitiateDialogue, OnInitiateDialogue);

    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.EquipWeapon, OnEquipWeapon);
        EventManager.currentManager.Unsubscribe(EventType.UseItem, OnUseItem);
        EventManager.currentManager.Unsubscribe(EventType.InitiateDialogue, OnInitiateDialogue);

        //EventManager.currentManager.Subscribe(EventType.InitiateDialogue, OnCeaseDialogue);
    }

    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerSpellcastingManager = GetComponent<PlayerSpellcastingManager>();
        playerQuickslotManager = GetComponent<PlayerQuickslotManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        playerInteraction = GetComponent<PlayerInteraction>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();


        EventManager.currentManager.Subscribe(EventType.CeaseDialogue, OnCeaseDialogue);

        SetupVariables();
    }

    private void Start()
    {
        playerInventory.LoadEquippedWeapons(weaponSlotManager);
    }

    private void Update()
    {

        GetPlayerAnimatorBools();

        //Make sure player isnt dead
        if (!playerStats.GetIsDead())
        {
            //Player is unable to perform certain actions whilst in spellcasting mode
            if (inputHandler.spellcastingModeInput)
            {
                playerSpellcastingManager.HandleSpellcasting();
            }
            else
            {
                playerLocomotion.HandleDodgeAndJumping();
                playerCombatManager.HandleAttacks();
                playerCombatManager.HandleDefending();
                playerInventory.SwapWeapon(weaponSlotManager);
                playerQuickslotManager.HandleQuickslotInputs();
            }

            playerInteraction.CheckForInteractableObject();
        }
        else
        {
            //force player to play death animation if they somehow avoid it
            if (!playerAnimatorManager.animator.GetCurrentAnimatorStateInfo(2).IsName("Death"))
            {
                playerAnimatorManager.animator.Play("Death");
            }
        }
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        inputHandler.TickInput(delta);
        
        playerLocomotion.HandleLocomotion(delta);

        playerStats.HandleStaminaRegeneration();
    }

    private void LateUpdate()
    {
        inputHandler.ResetInputs();
    }

    private void SetupVariables()
    {
        //Set Player tag if it hasnt been already
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Player";
        //Set layer if it hasnt been already
        if (gameObject.layer == 0)
            gameObject.layer = 10;

        //Set characterCollider 
        characterCollider = gameObject.GetComponent<CapsuleCollider>();

        //Set characterCollisonBlocker
        Transform collisionBlockerTransform = gameObject.transform.Find("CharacterCollisionBlocker");
        if (collisionBlockerTransform != null)
            characterCollisionBlocker = collisionBlockerTransform.GetComponent<CapsuleCollider>();

        if (characterCollisionBlocker != null)
            Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
        else
            Debug.LogWarning("Could not find a CharacterCollisionBlocker, make sure you created a child in player and added an empty gameObject, name it CharacterCollisionBlocker and add a capsule collider and a rigidbody.");

        //Set finisherAttackRayCastStartPointTransform
        if (finisherAttackRayCastStartPointTransform == null)
        {
            finisherAttackRayCastStartPointTransform = gameObject.transform.Find("FinisherAttackRaycastStartPoint");
            if (finisherAttackRayCastStartPointTransform == null)
                Debug.LogWarning("Could not find a FinisherAttackRaycastStartPoint gameObject on the player, create an empty gameobject as a child of player, give it that name. Position it to be in front of the players chest");
        }

    }

    private void GetPlayerAnimatorBools()
    {
        playerAnimatorManager.canRotate = playerAnimatorManager.animator.GetBool("canRotate");

        canDoCombo = playerAnimatorManager.animator.GetBool("canDoCombo");
        isParrying = playerAnimatorManager.animator.GetBool("isParrying");
        isInteracting = playerAnimatorManager.animator.GetBool("isInteracting");
        isBlocking = playerAnimatorManager.animator.GetBool("isBlocking");
    }

    #region onEvents
    private void OnEquipWeapon(EventData eventData)
    {
        if (eventData is EquipWeapon equipWeapon)
        {
            playerInventory.EquipWeapon(weaponSlotManager, equipWeapon.weaponItem, equipWeapon.isPrimaryWeapon);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.EquipWeapon was received but is not of class EquipWeapon.");
        }
    }

    private void OnUseItem(EventData eventData)
    {
        if (eventData is UseItem useItem)
        {
            if (useItem.item is QuickslotItem quickslotItem)
            {
                //set item to the current quick slot
                playerInventory.quickslotItemInUse = quickslotItem;
                //attempt using item
                quickslotItem.AttemptToUseItem(playerAnimatorManager, playerStats);
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UseItem was received but is not of class UseItem.");
        }
    }

    private void OnInitiateDialogue(EventData eventData)
    {
        if (eventData is InitiateDialogue)
        {
            //Hide model
            gameObject.SetActive(false);
            //Disable Character Controls
            inputHandler.GetInputActions().CharacterControls.Disable();
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.InitiateDialogue was received but is not of class InitiateDialogue.");
        }
    }

    private void OnCeaseDialogue(EventData eventData)
    {
        if (eventData is CeaseDialogue)
        {
            //Show model
            gameObject.SetActive(true);
            //Disable Character Controls
            inputHandler.GetInputActions().CharacterControls.Enable();
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.CeaseDialogue was received but is not of class CeaseDialogue.");
        }
    }
    #endregion

    #region Getters & Setters
    internal void SetDamageColliderDamage(float damage)
    {
        weaponSlotManager.rightHandDamageCollider.currentDamage = damage;
    }

    internal PlayerStats GetPlayerStats()
    {
        return playerStats;
    }

    #endregion
}
