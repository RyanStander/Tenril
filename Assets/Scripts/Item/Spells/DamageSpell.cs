using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Spells/Damage Spell")]
public class DamageSpell : SpellItem
{
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
            damageCollider.EnableDamageCollider();
            damageCollider.currentDamage = damageAmount;

            damageCollider.characterManager= characterManager;
        }
        //check if there is a fx_sd script, if so, prevent it from hurting the player
        FX_SpawnDirection fX_SpawnDirection = instantiatedSpellFX.GetComponent<FX_SpawnDirection>();
        if (fX_SpawnDirection != null)
        {
            fX_SpawnDirection.damage = damageAmount;
            fX_SpawnDirection.characterManager = characterManager;
        }
    }
}
