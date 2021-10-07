using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;
using System;

public class StatusEffectManager : MonoBehaviour
{
    [SerializeField]private List<StatusEffect> statusEffects;
    private List<float> timeLeft=new List<float>();

    //Status Effects
    private PoisonEffect poisonEffect;

    //temporary
    [SerializeField]CharacterStats character;

    private void Update()
    {
        //temporary
        CheckThroughStatusEffects(character);
    }

    internal void CheckThroughStatusEffects(CharacterStats characterStats)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            switch (statusEffects[i].statusEffectType)
            {
                case StatusEffectType.none:
                    break;
                case StatusEffectType.poisoned:
                    HandlePoisonStatusEffect(statusEffects[i],characterStats);
                    break;
                case StatusEffectType.rooted:
                    break;
                case StatusEffectType.burning:
                    break;
                case StatusEffectType.frozen:
                    break;
            }
        }
    }

    internal void ReceiveStatusEffect(StatusEffect statusEffect)
    {
        //Check if there is already a status effect of the same type
        foreach (StatusEffect status in statusEffects)
        {
            if (status.statusEffectType==statusEffect.statusEffectType)
            {
                //find the index of the object
                int indexVal = statusEffects.IndexOf(statusEffect);
                //change the duration of the effect to override the current one if the new duration is longer
                if (statusEffect.statusEffectDuration + Time.time> timeLeft[indexVal])
                {
                    timeLeft[indexVal] = statusEffect.statusEffectDuration + Time.time;

                    //IMPROVEMENT: if there is time, change it so that it will base it on which effect is strongest
                }
                //exit out of function
                return;
            }
        }
        //if no status effect of same type was found, add it
        statusEffects.Add(statusEffect);
        timeLeft.Add((statusEffect.statusEffectDuration + Time.time));
    }

    private void RemoveStatusEffect(StatusEffect statusEffect)
    {
        //find the index of the object
        int indexVal = statusEffects.IndexOf(statusEffect);
        //exit if it does not exist
        if (indexVal == -1)
            return;
        //remove status effect
        statusEffects.RemoveAt(indexVal);
        timeLeft.RemoveAt(indexVal);
    }

    private void HandlePoisonStatusEffect(StatusEffect statusEffect,CharacterStats characterStats)
    {
        //find the index of the object
        int indexVal = statusEffects.IndexOf(statusEffect);
        //if the duration of the effect has ended
        if (Time.time > timeLeft[indexVal])
        {
            //remove the status effect
            RemoveStatusEffect(statusEffect);
            poisonEffect = null;
        }
        //otherwise, handle the status effect
        else
        {
            //if a poison effect does not exists, create one
            if (poisonEffect==null)
            {
                poisonEffect = gameObject.AddComponent<PoisonEffect>();
                poisonEffect.SetValues(characterStats, statusEffect);
                poisonEffect.HandlePoisonedEffect();
            }
            //otherwise handle current one
            else
            {
                poisonEffect.HandlePoisonedEffect();
            }
        }
    }
}
