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

    public bool canDoCombo, isInteracting, isAiming,isHoldingArrow;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.EquipWeapon, OnEquipWeapon);
        EventManager.currentManager.Subscribe(EventType.UseItem, OnUseItem);
        EventManager.currentManager.Subscribe(EventType.InitiateDialogue, OnInitiateDialogue);
        EventManager.currentManager.Subscribe(EventType.HideWeapon, OnHideWeapon);
        EventManager.currentManager.Subscribe(EventType.ShowWeapon, OnShowWeapon);
        EventManager.currentManager.Subscribe(EventType.DisplayQuickslotItem, OnDisplayQuickslotItem);
        EventManager.currentManager.Subscribe(EventType.HideQuickslotItem, OnHideQuickslotItem);
        EventManager.currentManager.Subscribe(EventType.SendTimeStrength, OnReceiveTimeStrength);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.EquipWeapon, OnEquipWeapon);
        EventManager.currentManager.Unsubscribe(EventType.UseItem, OnUseItem);
        EventManager.currentManager.Unsubscribe(EventType.InitiateDialogue, OnInitiateDialogue);
        EventManager.currentManager.Unsubscribe(EventType.HideWeapon, OnHideWeapon);
        EventManager.currentManager.Unsubscribe(EventType.ShowWeapon, OnShowWeapon);
        EventManager.currentManager.Unsubscribe(EventType.DisplayQuickslotItem, OnDisplayQuickslotItem);
        EventManager.currentManager.Unsubscribe(EventType.HideQuickslotItem, OnHideQuickslotItem);
        EventManager.currentManager.Unsubscribe(EventType.SendTimeStrength, OnReceiveTimeStrength);
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
        ragdollManager = GetComponentInChildren<RagdollManager>();


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
                playerCombatManager.HandleWeaponSpecificAbilities();
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
        playerStats.HandleSunlightPoolRegeneration(timeStrength);
        playerStats.HandleMoonlightPoolRegeneration(-(timeStrength - 1));
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

        //Get the animator component
        Animator animator = gameObject.GetComponent<Animator>();
        //Apply root motion of animator
        animator.applyRootMotion = true;
        //Set the update mode
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        //Freeze rotations
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;

        //Set characterCollider 
        characterCollider = gameObject.GetComponent<CapsuleCollider>();

        //Set characterCollisonBlocker
        Transform collisionBlockerTransform = gameObject.transform.Find("CombatColliders").Find("CharacterCollisionBlocker");
        if (collisionBlockerTransform != null)
            characterCollisionBlocker = collisionBlockerTransform.GetComponent<CapsuleCollider>();

        if (characterCollisionBlocker != null)
            Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
        else
            Debug.LogWarning("Could not find a CharacterCollisionBlocker, make sure you created a child in player and added an empty gameObject, name it CharacterCollisionBlocker and add a capsule collider and a rigidbody.");

        //Set finisherAttackRayCastStartPointTransform
        if (finisherAttackRayCastStartPointTransform == null)
        {
            finisherAttackRayCastStartPointTransform = gameObject.transform.Find("CombatTransforms").Find("FinisherAttackRaycastStartPoint");
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
        isAiming = playerAnimatorManager.animator.GetBool("isAiming");
        isHoldingArrow = playerAnimatorManager.animator.GetBool("isHoldingArrow");
    }

    public override void EnableRagdoll()
    {
        base.EnableRagdoll();
        playerAnimatorManager.animator.enabled = false;
        characterCollider.enabled = false;
        characterCollisionBlocker.enabled = false;
    }

    #region onEvents
    private void OnEquipWeapon(EventData eventData)
    {
        if (eventData is EquipWeapon equipWeapon)
        {
            //Assign the old weapon
            WeaponItem oldWeapon;
            if (equipWeapon.isPrimaryWeapon)
                oldWeapon = playerInventory.primaryWeapon;
            else
                oldWeapon = playerInventory.secondaryWeapon;

            //remove weapon from inventory and and add weapon that is equipped
            playerInventory.RemoveItemFromInventory(equipWeapon.weaponItem);
            
            //if there was previously no weapon in the slot, do not add it to the inventory
            if (oldWeapon!=null)
                playerInventory.AddItemToInventory(oldWeapon);

            //equip the new weapon
            playerInventory.EquipWeapon(weaponSlotManager, equipWeapon.weaponItem, equipWeapon.isPrimaryWeapon);

            EventManager.currentManager.AddEvent(new UpdateInventoryDisplay());
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

    private void OnDisplayQuickslotItem(EventData eventData)
    {
        if (eventData is DisplayQuickslotItem displayQuickslotItem)
        {
            playerInventory.DisplayQuickslotItem(weaponSlotManager,displayQuickslotItem.objectToDisplay);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.DisplayQuickslotItem was received but is not of class DisplayQuickslotItem.");
        }
    }

    private void OnHideQuickslotItem(EventData eventData)
    {
        if (eventData is HideQuickslotItem)
        {
            playerInventory.HideQuickslotItem(weaponSlotManager);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.HideQuickslotItem was received but is not of class HideQuickslotItem.");
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

    private void OnHideWeapon(EventData eventData)
    {
        if (eventData is HideWeapon)
        {
            playerInventory.HideWeapons(weaponSlotManager);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.HideWeapon was received but is not of class HideWeapon.");
        }
    }

    private void OnShowWeapon(EventData eventData)
    {
        if (eventData is ShowWeapon)
        {
            playerInventory.ShowWeapons(weaponSlotManager);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.ShowWeapon was received but is not of class ShowWeapon.");
        }
    }

    private void OnReceiveTimeStrength(EventData eventData)
    {
        if (eventData is SendTimeStrength sendTimeStrength)
        {
            timeStrength = sendTimeStrength.timeStrength;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendTimeStrength was received but is not of class SendTimeStrength.");
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
