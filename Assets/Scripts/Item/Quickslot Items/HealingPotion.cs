using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Healing Potion")]
public class HealingPotion : PotionItem
{
    [Tooltip("The amount of health restored")]
    public int healAmount;

    public override void AttemptToUseItem(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.AttemptToUseItem(animatorManager, characterStats);
    }

    public override void SuccessfullyUsedItem(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.SuccessfullyUsedItem(animatorManager, characterStats);

        //Let player regain health
        characterStats.RegainHealth(healAmount);
    }
}
