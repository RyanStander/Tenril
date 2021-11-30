using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItem : Item
{
    [Tooltip("The wind up of the spell before actually being cast")]
    public GameObject spellWindUpFX;
    [Tooltip("The actual spell cast when successful")]
    public GameObject spellCastFX;
    [SerializeField] private Vector3 spellCastFXOffset;
    public string spellAnimation;

    [Tooltip("Cost of the spell")]
    public int spellCost;

    [Header("Spell Type")]
    [Tooltip("The type of spell being cast")]
    public SpellType spellType;

    [Header("Spell Description")]
    [Tooltip("Description of what the spell do")][TextArea]
    public string spellDescription;

    protected GameObject instantiatedWarmUpSpellFX, instantiatedSpellFX;

    public virtual void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager=null)
    {
        //Debug.Log("Attempting spell cast!");

        //Start the casting spell effects
        if (spellWindUpFX != null)
        {
            instantiatedWarmUpSpellFX = Instantiate(spellWindUpFX, animatorManager.transform.position, animatorManager.transform.rotation);
        }

        //Play the animation of casting the spell
        animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public virtual void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        //Debug.Log("Spell cast has succeeded");

        //Check what type of spell it is then casts based on the type
        switch (spellType)
        {
            case SpellType.biomancy:
                characterStats.ConsumeStoredMoonlight(spellCost);
                characterStats.PutMoonlightRegenOnCooldown();
                break;
            case SpellType.pyromancy:
                characterStats.ConsumeStoredSunlight(spellCost);
                characterStats.PutSunlightRegenOnCooldown();
                break;
        }

        //Create the successful cast spell effect
        if (spellCastFX != null)
        {
            //Creates a position based on offsets with the original objects location in mind
            Vector3 position = animatorManager.transform.position + 
                animatorManager.transform.forward * spellCastFXOffset.x +
                animatorManager.transform.right * spellCastFXOffset.z+
                animatorManager.transform.up* spellCastFXOffset.y;

            //Create spell effect
             instantiatedSpellFX = Instantiate(spellCastFX, position, animatorManager.transform.rotation);
        }
    }
    public enum SpellType
    {
        biomancy,
        pyromancy
    }
}
