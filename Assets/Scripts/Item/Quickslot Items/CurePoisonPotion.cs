using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Cure Poison")]
public class CurePoisonPotion : PotionItem
{
    [SerializeField] private StatusEffect poisonStatusEffect;
    public override void AttemptToUseItem(AnimatorManager animatorManager, WeaponSlotManager weaponManager, ConsumableManager consumableManager, CharacterStats characterStats)
    {
        base.AttemptToUseItem(animatorManager, weaponManager, consumableManager, characterStats);
    }

    public override void SuccessfullyUsedItem(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        base.SuccessfullyUsedItem(animatorManager, characterStats);

        //Remove the creatures poison status
        StatusEffectManager statusEffectManager = characterStats.GetComponent<StatusEffectManager>();
        if (statusEffectManager != null)
            statusEffectManager.RemoveStatusEffect(poisonStatusEffect);
    }
}
