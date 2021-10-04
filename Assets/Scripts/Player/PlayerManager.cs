using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages other player scripts, most updates happen here.
/// </summary>

public class PlayerManager : MonoBehaviour
{
    private InputHandler inputHandler;
    private PlayerLocomotion playerLocomotion;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerCombatManager playerCombatManager;
    private PlayerSpellcastingManager playerSpellcastingManager;
    private PlayerQuickslotManager playerQuickslotManager;
    private PlayerInventory playerInventory;
    private PlayerStats playerStats;
    private WeaponSlotManager weaponSlotManager;

    private InteractableUI interactableUI;
    public GameObject interactableUIGameObject;
    public GameObject itemInteractableGameObject;

    public bool canDoCombo;
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
        weaponSlotManager = GetComponent<WeaponSlotManager>();

        interactableUI = FindObjectOfType<InteractableUI>();
    }

    private void Start()
    {
        playerInventory.LoadEquippedWeapons(weaponSlotManager);
    }

    private void Update()
    {
        playerAnimatorManager.canRotate = playerAnimatorManager.animator.GetBool("canRotate");

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

        CheckForInteractableObject();
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

    internal void CheckForInteractableObject()
    {
        RaycastHit hit;

        //Check in a sphere cast for any interactable objects
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f))
        {
            if (hit.collider.tag == "Interactable")
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                //if there is an interactable object
                if (interactableObject != null)
                {
                    //Assign text to the interactable object
                    string interactableText = interactableObject.interactableText;
                    interactableUI.interactableText.text = interactableText;
                    
                    //Display it
                    interactableUIGameObject.SetActive(true);

                    //if interact button is pressed while the option is available
                    if (inputHandler.interactInput)
                    {
                        //call the interaction
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
        else
        {
            //otherwise hide the objects if moving away
            if (interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }

            if (itemInteractableGameObject != null && inputHandler.interactInput)
            {
                itemInteractableGameObject.SetActive(false);
            }
        }
    }

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
