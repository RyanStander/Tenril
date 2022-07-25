using Player;
using UnityEngine;

public class PlayerSpellcastingManager : CharacterSpellcastingManager
{
    private PlayerInventory playerInventory;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerStats playerStats;
    private PlayerManager playerManager;
    private InputHandler inputHandler;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    internal void HandleSpellcasting()
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
            if (playerInventory.preparedSpells[spellNumber]==null)
            {
                Debug.Log("There is no spell prepared on this slot");
                return;
            }
            //Check the type of spell being cast
            //if the player does not have enough magic to cast the spell, they wont.
            switch (playerInventory.preparedSpells[spellNumber].spellType)
            {
                case SpellType.biomancy:
                    if (!playerStats.HasEnoughMoonlight(playerInventory.preparedSpells[spellNumber].spellCost))
                    {
                        //Play failed cast animation

                        //Return
                        return;
                    }
                    break;
                case SpellType.pyromancy:
                    if (!playerStats.HasEnoughSunlight(playerInventory.preparedSpells[spellNumber].spellCost))
                    {
                        //Play failed cast animation

                        //Return
                        return;
                    }
                    break;
            }
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
        if (playerManager == null)
            Debug.Log("could not find player manager");
        spellBeingCast.SuccessfullyCastSpell(playerAnimatorManager, playerStats, playerManager);
    }
}
