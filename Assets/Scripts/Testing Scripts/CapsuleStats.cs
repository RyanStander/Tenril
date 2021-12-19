using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleStats : CharacterStats
{
    [Header("Resource bars")]
    [SerializeField] private SliderBarDisplayUI healthBar;

    private void Start()
    {
        SetupStats();
        healthBar.SetMaxValue(maxHealth);
    }

    public override void TakeDamage(float damageAmount, bool playAnimation = true, string damageAnimtion="Hit")
    {
        base.TakeDamage(damageAmount, playAnimation);

        //update health display on the healthbar
        healthBar.SetCurrentValue(currentHealth);
    }
}
