using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Delayed Damage Spell")]
public class DelayedDamageSpell : SpellItem
{
    /// <summary>
    /// Functions similar to the normal damage spell but searches for a delayed effect script
    /// </summary>
    [Tooltip("The amount of damage done")]
    public int damageAmount=10;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        base.AttemptToCastSpell(animatorManager, characterStats);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        base.SuccessfullyCastSpell(animatorManager, characterStats);

        //Get the spells damage collider
        DamageCollider damageCollider = instantiatedSpellFX.GetComponent<DamageCollider>();

        //Set the spells damage and open the collider
        if (damageCollider != null)
        {
            damageCollider.currentDamage = damageAmount;

            damageCollider.characterManager= characterManager;

            //search for delayedeffect script
            DelayedEffect delayedEffect = instantiatedSpellFX.GetComponent<DelayedEffect>();
            //if found, wait on opening damage collider
            if (delayedEffect!=null)
            {
                delayedEffect.SetValues(damageCollider);
            }
        }
    }
}
