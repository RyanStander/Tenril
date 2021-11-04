using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    private DamageCollider givenDamageCollider;//the damage collider to do damage with
    private float damageInterval;//how often the DOT will proc
    private float damageDuration;//how long the DOT will laste

    private float nextDamage;//how long until the next damage interval
    private void FixedUpdate()
    {
        CheckDamage();
    }

    public void SetValues(DamageCollider givenDamageCollider, float damageInterval, float damageDuration)
    {
        this.givenDamageCollider = givenDamageCollider;
        this.damageDuration = damageDuration;
        this.damageInterval = damageInterval;
    }

    private void CheckDamage()
    {
        //check how long is left until next interval
        if (Time.time > nextDamage)
        {
            //if enough time passed, do damage
            nextDamage = Time.time + damageInterval;
            givenDamageCollider.EnableDamageCollider(false);
        }
        else
        {
            givenDamageCollider.DisableDamageCollider();
        }
    }
}
