using System;
using Character;
using UnityEngine;
using WeaponManagement;

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

    #region Serialized Field

    [SerializeField] private CapsuleCollider characterCollider;
    [SerializeField] private CapsuleCollider characterCollisionBlocker;
    [SerializeField]private InputHandler inputHandler;
    [SerializeField]private PlayerLocomotion playerLocomotion;
    [SerializeField]private PlayerAnimatorManager playerAnimatorManager;
    [SerializeField]private PlayerCombatManager playerCombatManager;
    [SerializeField]private PlayerSpellcastingManager playerSpellcastingManager;
    [SerializeField]private PlayerQuickslotManager playerQuickslotManager;
    [SerializeField]private PlayerInventory playerInventory;
    [SerializeField]private PlayerStats playerStats;
    [SerializeField]private PlayerInteraction playerInteraction;

    #endregion
    
    #region Private Fields

    private float timeTillRestart = 3, restartTimeStamp;
    private bool enteredSpellcastingMode = true;
    private PlayerState playerState = PlayerState.Default;
    private bool deathSceneLoading;
    
    #endregion

    #region Public Fields

    public bool canDoCombo, isInteracting, isAiming, isHoldingArrow, isHarvestingResource;
    public Transform aimTransform;
    
    #endregion

    #region Animator Ids

    private static readonly int CanRotate = Animator.StringToHash("canRotate");
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
    private static readonly int IsParrying = Animator.StringToHash("isParrying");
    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");
    private static readonly int IsBlocking = Animator.StringToHash("isBlocking");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");
    private static readonly int IsHoldingArrow = Animator.StringToHash("isHoldingArrow");
    private static readonly int IsHarvestingResource = Animator.StringToHash("isHarvestingResource");

    #endregion


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
        EventManager.currentManager.Subscribe(EventType.LoadPlayerCharacterData, OnLoadPlayerCharacterData);
        EventManager.currentManager.Subscribe(EventType.PlayerLevelUp, OnPlayerLevelUp);
        EventManager.currentManager.Subscribe(EventType.AwardPlayerXP, OnAwardPlayerXP);
        EventManager.currentManager.Subscribe(EventType.PlayerKeybindsUpdates, OnPlayerKeybindsUpdates);
        EventManager.currentManager.Subscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
        EventManager.currentManager.Subscribe(EventType.ChangePlayerState, OnChangePlayerState);
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
        EventManager.currentManager.Unsubscribe(EventType.LoadPlayerCharacterData, OnLoadPlayerCharacterData);
        EventManager.currentManager.Unsubscribe(EventType.PlayerLevelUp, OnPlayerLevelUp);
        EventManager.currentManager.Unsubscribe(EventType.AwardPlayerXP, OnAwardPlayerXP);
        EventManager.currentManager.Unsubscribe(EventType.PlayerKeybindsUpdates, OnPlayerKeybindsUpdates);
        EventManager.currentManager.Unsubscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
        EventManager.currentManager.Unsubscribe(EventType.ChangePlayerState, OnChangePlayerState);
    }

#if (UNITY_EDITOR) 
    private void OnValidate()
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
        rigidBody = GetComponent<Rigidbody>();
        
        ragdollManager = GetComponentInChildren<RagdollManager>();
        characterLockOnPoint = GetComponentInChildren<CharacterLockOnPoint>();
    }
#endif
    private void Awake()
    {
        SetupVariables();
        
        EventManager.currentManager.Subscribe(EventType.CeaseDialogue, OnCeaseDialogue);
    }

    private void Start()
    {
        playerInventory.LoadEquippedWeapons(weaponSlotManager);
    }

    private void Update()
    {
        GetPlayerAnimatorBools();

        HandleStates();
    }

    private void FixedUpdate()
    {
        var delta = Time.deltaTime;
        inputHandler.TickInput();

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
        //Set Player tag if it hasn't been already
        if (gameObject.CompareTag("Untagged"))
            gameObject.tag = "Player";
        //Set layer if it hasn't been already
        if (gameObject.layer == 0)
            gameObject.layer = 10;

        //Get the animator component
        var animator = gameObject.GetComponent<Animator>();
        //Apply root motion of animator
        animator.applyRootMotion = true;
        //Set the update mode
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        //Freeze rotations
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;

        //Set characterCollider 
        characterCollider = gameObject.GetComponent<CapsuleCollider>();

        //Set characterCollisionBlocker
        var collisionBlockerTransform = gameObject.transform.Find("CombatColliders").Find("CharacterCollisionBlocker");
        
        if (collisionBlockerTransform != null)
            characterCollisionBlocker = collisionBlockerTransform.GetComponent<CapsuleCollider>();

        if (characterCollisionBlocker != null)
            Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
        else
            Debug.LogWarning("Could not find a CharacterCollisionBlocker, make sure you created a child in player and added an empty gameObject, name it CharacterCollisionBlocker and add a capsule collider and a rigidbody.");

        //Set finisherAttackRayCastStartPointTransform
        if (finisherAttackRayCastStartPointTransform != null) 
            return;
        
        finisherAttackRayCastStartPointTransform = gameObject.transform.Find("CombatTransforms").Find("FinisherAttackRaycastStartPoint");
        if (finisherAttackRayCastStartPointTransform == null)
            Debug.LogWarning("Could not find a FinisherAttackRaycastStartPoint gameObject on the player, create an empty gameobject as a child of player, give it that name. Position it to be in front of the players chest");

    }

    private void HandleStates()
    {
        switch (playerState)
        {
            case PlayerState.Default:
                break;
            case PlayerState.CombatMode:
                playerLocomotion.HandleDodgeAndJumping();
                playerCombatManager.HandleAttacks();
                playerCombatManager.HandleWeaponSpecificAbilities();
                playerInventory.SwapWeapon(weaponSlotManager);
                playerQuickslotManager.HandleQuickslotInputs();
                break;
            case PlayerState.SpellcastingMode:
                playerSpellcastingManager.HandleSpellcasting();
                break;
            case PlayerState.MenuMode:
                inputHandler.ResetMovementValues();
                break;
            case PlayerState.Dead:
                inputHandler.ResetMovementValues();
                //force player to play death animation if they somehow avoid it
                if (!playerAnimatorManager.animator.GetCurrentAnimatorStateInfo(2).IsName("Death"))
                    playerAnimatorManager.animator.Play("Death");

                if (restartTimeStamp == 0)
                    restartTimeStamp = Time.time + timeTillRestart;

                if (restartTimeStamp <= Time.time && !deathSceneLoading)
                {
                    deathSceneLoading = true;
                    EventManager.currentManager.AddEvent(new LoadData());
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        //Make sure player isn't dead
        if (playerStats.GetIsDead())
        {
            playerState = PlayerState.Dead;
            return;
        }
        //Player is unable to perform certain actions whilst in spellcasting mode
        if (inputHandler.spellcastingModeInput)
        {
            if (!enteredSpellcastingMode)
            {
                playerState = PlayerState.SpellcastingMode;
                enteredSpellcastingMode = true;
                EventManager.currentManager.AddEvent(new PlayerToggleSpellcastingMode(enteredSpellcastingMode));
            }
        }
        else
        {
            if (enteredSpellcastingMode)
            {
                playerState = PlayerState.CombatMode;
                enteredSpellcastingMode = false;
                EventManager.currentManager.AddEvent(new PlayerToggleSpellcastingMode(enteredSpellcastingMode));
            }
        }

        playerInteraction.CheckForInteractableObject();
            
        return;
    }
    
    private void GetPlayerAnimatorBools()
    {
        playerAnimatorManager.canRotate = playerAnimatorManager.animator.GetBool(CanRotate);

        canDoCombo = playerAnimatorManager.animator.GetBool(CanDoCombo);
        isParrying = playerAnimatorManager.animator.GetBool(IsParrying);
        isInteracting = playerAnimatorManager.animator.GetBool(IsInteracting);
        isBlocking = playerAnimatorManager.animator.GetBool(IsBlocking);
        isAiming = playerAnimatorManager.animator.GetBool(IsAiming);
        isHoldingArrow = playerAnimatorManager.animator.GetBool(IsHoldingArrow);
        isHarvestingResource = playerAnimatorManager.animator.GetBool(IsHarvestingResource);
    }

    public override void EnableRagdoll()
    {
        base.EnableRagdoll();
        playerAnimatorManager.animator.enabled = false;
        characterCollider.enabled = false;
        characterCollisionBlocker.enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    #region Tools
    /// <summary>
    /// Displays the pickaxe
    /// </summary>
    internal void DisplayTool(ToolType toolType)
    {
        playerInventory.DisplayTool(weaponSlotManager, toolType);
    }
    #endregion

    internal void HideDisplayObect(bool hideLeftHandObject = true)
    {
        weaponSlotManager.HideObjectsInHand(hideLeftHandObject);
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
            if (oldWeapon != null)
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
            if (useItem.item is ConsumableItem quickslotItem)
            {
                //set item to the current quick slot
                playerInventory.consumableItemInUse = quickslotItem;

                //attempt using item
                quickslotItem.AttemptToUseItem(playerAnimatorManager, playerQuickslotManager, playerStats);
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
            playerInventory.DisplayQuickslotItem(weaponSlotManager, displayQuickslotItem.objectToDisplay);
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
    
    private void OnChangePlayerState(EventData eventData)
    {
        if (eventData is ChangePlayerState changePlayerState)
        {
            playerState = changePlayerState.playerState;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.ChangePlayerState was received but is not of class ChangePlayerState.");
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

    private void OnLoadPlayerCharacterData(EventData eventData)
    {
        if (eventData is LoadPlayerCharacterData loadPlayerCharacterData)
        {
            PlayerData playerData = loadPlayerCharacterData.playerData;

            playerStats.SetPlayerStats(playerData);

            Vector3 position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
            Quaternion rotation = Quaternion.Euler(playerData.rotation[0], playerData.rotation[1], playerData.rotation[2]);

            transform.position = position;
            transform.rotation = rotation;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.LoadPlayerCharacterData was received but is not of class LoadPlayerCharacterData.");
        }
    }

    private void OnAwardPlayerXP(EventData eventData)
    {
        if (eventData is AwardPlayerXp awardPlayerXP)
        {
            //Calculate if the player will recieve any level ups
            LevelSystem.DetermineLevelGain(playerStats.levelData, playerStats.currentXP, playerStats.currentLevel, awardPlayerXP.xpAmount);
            //Add xp gain
            playerStats.IncreaseXP(awardPlayerXP.xpAmount);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.AwardPlayerXP was received but is not of class AwardPlayerXP.");
        }
    }

    private void OnPlayerLevelUp(EventData eventData)
    {
        if (eventData is PlayerLevelUp playerLevelUp)
        {
            playerStats.IncreaseLevel(playerLevelUp.amountOfLevelsGained);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.PlayerLevelUp was received but is not of class PlayerLevelUp.");
        }
    }

    private void OnPlayerKeybindsUpdates(EventData eventData)
    {
        if (eventData is PlayerKeybindsUpdate)
        {
            SpellSlotUIManager spellSlotUIManager = FindObjectOfType<SpellSlotUIManager>();
            if (spellSlotUIManager != null)
            {
                string bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.SpellcastingMode, inputHandler.activeInputDevice);
                //Spellcasting Mode icon
                Sprite spellcastingModeSprite;
                spellcastingModeSprite = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);

                //Spell slots
                Sprite[] spellSlotKeybindSprites = new Sprite[8];

                #region Spells
                //Spell 1
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell1, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[0] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 2
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell2, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[1] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 3
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell3, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[2] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 4
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell4, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[3] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 5
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell5, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[4] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 6
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell6, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[5] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 7
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell7, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[6] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 8
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell8, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[7] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                #endregion

                spellSlotUIManager.UpdateKeybinds(spellcastingModeSprite, spellSlotKeybindSprites);
            }
        }
        else
        {
            Debug.LogWarning("The event of PlayerKeybindsUpdates was not matching of event type PlayerKeybindsUpdates");
        }
    }

    private void OnPlayerChangedInputDevice(EventData eventData)
    {
        if (eventData is PlayerChangedInputDevice)
        {
            SpellSlotUIManager spellSlotUIManager = FindObjectOfType<SpellSlotUIManager>();
            if (spellSlotUIManager != null)
            {
                string bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.SpellcastingMode, inputHandler.activeInputDevice);
                //Spellcasting Mode icon
                Sprite spellcastingModeSprite;
                spellcastingModeSprite = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);

                //Spell slots
                Sprite[] spellSlotKeybindSprites = new Sprite[8];

                #region Spells
                //Spell 1
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell1, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[0] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 2
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell2, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[1] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 3
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell3, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[2] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 4
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell4, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[3] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 5
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell5, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[4] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 6
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell6, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[5] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 7
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell7, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[6] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                //Spell 8
                bindingPath = CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Spell8, inputHandler.activeInputDevice);
                spellSlotKeybindSprites[7] = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
                #endregion

                spellSlotUIManager.UpdateKeybinds(spellcastingModeSprite, spellSlotKeybindSprites);
            }
            else
            {
                Debug.LogWarning("The event of PlayerChangedInputDevice was not matching of event type PlayerChangedInputDevice");
            }
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
