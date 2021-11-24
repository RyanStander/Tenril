using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Spells/Poison Spray Spell")]
public class PoisonSpraySpell : SpellItem
{
    [Tooltip("the poison applied")]
    public PoisonedStatusEffect poisonedStatusEffect;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        base.AttemptToCastSpell(animatorManager, characterStats);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        base.SuccessfullyCastSpell(animatorManager, characterStats);

        //Get the spells poison effect
        ApplyStatusEffect applyStatusEffect = instantiatedSpellFX.GetComponent<ApplyStatusEffect>();

        //Set the status effect of poison
        if (applyStatusEffect != null)
        {
            applyStatusEffect.SetStatusEffect(poisonedStatusEffect);
        }
    }
}
