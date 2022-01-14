using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Spells/Healing Spell")]
public class HealingSpell : SpellItem
{
    [Tooltip("The amount of health restored")]
    public int healAmount;

    public override void AttemptToCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        base.AttemptToCastSpell(animatorManager, characterStats);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorManager, CharacterStats characterStats, CharacterManager characterManager = null)
    {
        base.SuccessfullyCastSpell(animatorManager, characterStats);

        //Let player regain health
        characterStats.RegainHealth(healAmount);
    }
}
