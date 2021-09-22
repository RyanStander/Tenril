using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellcastingManager : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerStats playerStats;
    private InputHandler inputHandler;
    private SpellItem spellBeingCast;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    public void HandleSpellcasting()
    {
        //iterated through all spells
        for (int i = 0; i < 8; i++)
        {
            //handle spellcast inputs seperately
            HandleSpellCastInput(i);
        }
    }

    private void HandleSpellCastInput(int spellNumber)
    {
        //Simple version, cast spell, if its button has been pressed
        if (inputHandler.castSpell[spellNumber])
        {
            if (playerInventory.preparedSpells[spellNumber] == null)
                return;

            if (playerAnimatorManager.animator.GetBool("isInteracting"))
                return;

            playerInventory.preparedSpells[spellNumber].AttemptToCastSpell(playerAnimatorManager, playerStats);
            spellBeingCast = playerInventory.preparedSpells[spellNumber];
        }
    }

    public void SuccessfulyCastSpell()
    {
        spellBeingCast.SuccessfullyCastSpell(playerAnimatorManager, playerStats);
    }
}
