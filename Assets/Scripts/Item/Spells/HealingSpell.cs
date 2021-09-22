using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem
{
    [Tooltip("The amount of health restored")]
    public int healAmount;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.AttemptToCastSpell(animatorManager, characterStats);

        //Start the casting spell effects
        if (spellWindUpFX != null)
        {
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWindUpFX, animatorManager.transform.parent);
        }

        //Play the animation of casting the spell
        animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.SuccessfullyCastSpell(animatorManager, characterStats);
        
        //Create the successful cast spell effect
        if (spellCastFX != null)
        {
            GameObject instantiatedSpellFX = Instantiate(spellCastFX, animatorManager.transform);
        }

        //Let player regain health
        characterStats.RegainHealth(healAmount);
    }
}
