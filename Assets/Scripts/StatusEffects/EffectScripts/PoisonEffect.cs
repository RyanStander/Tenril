using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private CharacterStats characterStats;
    private PoisonedStatusEffect poisonedStatusEffect;
    private float timeLeft;
    public void HandlePoisonedEffect()
    {
        //if enough time has passed
        if (Time.time > timeLeft)
        {
            timeLeft = Time.time + poisonedStatusEffect.damageInterval;
            characterStats.TakeDamage(poisonedStatusEffect.damageAmount, false);
        }
    }

    public void SetValues(CharacterStats characterStats, StatusEffect statusEffect)
    {
        this.characterStats = characterStats;
        if (statusEffect is PoisonedStatusEffect poisonedStatusEffect)
        {
            this.poisonedStatusEffect = poisonedStatusEffect;
            timeLeft = Time.time + poisonedStatusEffect.damageInterval;
        }
        else
        {
            Debug.LogWarning("Received the wrong status effect for a poison effect, please ensure that the correct status effect is given");
        }
    }
}
