using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Root Spell")]
public class RootSpell : SpellItem
{
    [Tooltip("The amount of damage done")]
    public int damageAmount=10;
    [Tooltip("The ticks between damage")]
    public float damageInterval = 1f;

    [Tooltip("How long the root lasts")]
    public float rootDuation;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.AttemptToCastSpell(animatorManager, characterStats);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.SuccessfullyCastSpell(animatorManager, characterStats);

        //Get the spells damage collider
        DamageCollider damageCollider = instantiatedSpellFX.GetComponent<DamageCollider>();

        //Set the spells damage and open the collider
        if (damageCollider != null)
        {
            damageCollider.currentDamage = damageAmount;

            damageCollider.ownCharacterStats=characterStats;

            //Create root effect debuff

            //Create damage over time effect
            DamageOverTime damageOverTime = instantiatedSpellFX.AddComponent<DamageOverTime>();
            damageOverTime.SetValues(damageCollider, damageInterval, rootDuation);
        }
    }
}
