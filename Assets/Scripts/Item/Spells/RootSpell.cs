using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponManagement;

[CreateAssetMenu(menuName = "Items/Spells/Root Spell")]
public class RootSpell : SpellItem
{
    [Tooltip("The amount of damage done")]
    public int damageAmount=10;

    [Tooltip("The ticks between damage")]
    public float damageInterval = 1f;

    [Tooltip("the root applied")]
    public StatusEffect rootedStatusEffect;

    [Tooltip("How long the root lasts")]
    public float rootDuation;

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

            //Create root effect debuff
            ApplyStatusEffect applyStatusEffect = instantiatedSpellFX.GetComponent<ApplyStatusEffect>();
            if (applyStatusEffect != null)
                applyStatusEffect.SetStatusEffect(rootedStatusEffect);
            else
                Debug.LogWarning("No ApplyStatusEffect component was found on the root spell effect, please make sure its applied");

            //Create damage over time effect
            DamageOverTime damageOverTime = instantiatedSpellFX.AddComponent<DamageOverTime>();
            damageOverTime.SetValues(damageCollider, damageInterval, rootDuation);
        }
    }
}
