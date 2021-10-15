using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPoison : MonoBehaviour
{
    private PoisonedStatusEffect poisonedStatusEffect;
    [SerializeField]private float lifeTime=5;
    private float timeLeft;

    private void Start()
    {
        timeLeft = Time.time + lifeTime;
    }
    private void Update()
    {
        //if duration expires, destroy the particle
        if (Time.time > timeLeft)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        //find the attached status effect manager
        StatusEffectManager hitStatusEffectManager = other.transform.GetComponent<StatusEffectManager>();

        //if no status effect manager, ignore
        if (hitStatusEffectManager==null) 
            return;

        //chec if theres a poison effect status assigned, if so apply the poison to the status effect manager
        if (poisonedStatusEffect!=null)
        {

            hitStatusEffectManager.ReceiveStatusEffect(poisonedStatusEffect);
        }
        else
        {
            Debug.LogWarning("The poisoned status effect was not set for the sepll");
        }

    }

    internal void SetPoisonedStatusEffect(PoisonedStatusEffect poisonedStatusEffect)
    {
        this.poisonedStatusEffect = poisonedStatusEffect;
    }
}
