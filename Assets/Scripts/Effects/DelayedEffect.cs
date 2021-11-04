using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedEffect : MonoBehaviour
{
    private DamageCollider damageCollider;

    [SerializeField]private ParticleSystem particleEffect;
    [SerializeField] private float lifetime=3;
    private float startTime;
    private bool damageColliderEnabled = false;

    private float durationUntilDestroy;
    public void SetValues(DamageCollider damageCollider)
    {
        this.damageCollider = damageCollider;
    }

    private void Start()
    {
        float startDelay = particleEffect.main.startDelay.constant;
        startTime = startDelay + Time.time;

        durationUntilDestroy = lifetime + Time.time;
    }

    private void Update()
    {
        //checks if enough time has passed and the collider is enabled
        if (Time.time > startTime && !damageColliderEnabled)
        {
            damageCollider.EnableDamageCollider();
            damageColliderEnabled = true;
        }
        //checks if enough time has passeds
        if (Time.time > durationUntilDestroy)
        {
            Destroy(gameObject);
        }
    }
}
