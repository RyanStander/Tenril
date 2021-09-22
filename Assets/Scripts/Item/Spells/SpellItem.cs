using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItem : Item
{
    [Tooltip("The wind up of the spell before actually being cast")]
    public GameObject spellWindUpFX;
    [Tooltip("The actual spell cast when successful")]
    public GameObject spellCastFX;
    public string spellAnimation;

    [Tooltip("Cost of the spell")]
    public int spellCost;

    [Header("Spell Type")]
    [Tooltip("The type of spell being cast")]
    public SpellType spellType;

    [Header("Spell Description")]
    [Tooltip("Description of what the spell do")][TextArea]
    public string spellDescription;

    public virtual void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        Debug.Log("Attempting spell cast!");
    }

    public virtual void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        Debug.Log("Spell cast has succeeded");

        //Check what type of spell it is then casts based on the type
        switch (spellType)
        {
            case SpellType.biomancy:
                characterStats.ConsumeStoredMoonlight(spellCost);
                break;
            case SpellType.pyromancy:
                characterStats.ConsumeStoredSunlight(spellCost);
                break;
        }
    }
    public enum SpellType
    {
        biomancy,
        pyromancy
    }
}
