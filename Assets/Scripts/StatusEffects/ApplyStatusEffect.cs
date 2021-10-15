using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ApplyStatusEffect : MonoBehaviour
{
    private StatusEffect statusEffect;

    private void OnTriggerEnter(Collider other)
    {

        //find the attached status effect manager
        StatusEffectManager hitStatusEffectManager = other.transform.GetComponent<StatusEffectManager>();

        //if no status effect manager, ignore
        if (hitStatusEffectManager == null)
            return;

        //chec if theres a poison effect status assigned, if so apply the poison to the status effect manager
        if (statusEffect != null)
        {

            hitStatusEffectManager.ReceiveStatusEffect(statusEffect);
        }
        else
        {
            Debug.LogWarning("The status effect was not given");
        }

    }

    internal void SetStatusEffect(StatusEffect statusEffect)
    {
        this.statusEffect = statusEffect;
    }
}
