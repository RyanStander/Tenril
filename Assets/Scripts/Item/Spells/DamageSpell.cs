using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Damage Spell")]
public class DamageSpell : SpellItem
{
    [Tooltip("The amount of damage done")]
    public int damageAmount=10;

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
            damageCollider.EnableDamageCollider();
            damageCollider.currentDamage = damageAmount;

            damageCollider.ownCharacterStats=characterStats;
        }
    }
}
