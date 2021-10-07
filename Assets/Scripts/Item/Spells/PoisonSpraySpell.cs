using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Poison Spray Spell")]
public class PoisonSpraySpell : SpellItem
{
    [Tooltip("the poison applied")]
    public PoisonedStatusEffect poisonedStatusEffect;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.AttemptToCastSpell(animatorManager, characterStats);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.SuccessfullyCastSpell(animatorManager, characterStats);

        //Get the spells damage collider
        ApplyPoison applyPoison = instantiatedSpellFX.GetComponent<ApplyPoison>();

        //Set the spells damage and open the collider
        if (applyPoison != null)
        {
            applyPoison.SetPoisonedStatusEffect(poisonedStatusEffect);
        }
    }
}
