using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int healthLevel = 10;
    [SerializeField] protected float maxHealth, currentHealth;
    protected bool isDead = false;

    [Header("Stamina")]
    [SerializeField] protected int staminaLevel = 10;
    [SerializeField] protected float maxStamina, currentStamina;

    [Header("Biomancy")]
    [SerializeField] protected int MoonlightLevel = 10;
    [SerializeField] protected float maxStoredMoonlight, currentStoredMoonlight;

    [Header("Pyromancy")]
    [SerializeField] protected int SunlightLevel = 10;
    [SerializeField] protected float maxStoredSunlight, currentStoredSunlight;

    private void Start()
    {
        SetupStats();
    }

    public virtual void SetupStats()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;

        maxStoredMoonlight = SetMaxStoredMoonlightFromMoonlightLevel();
        currentStoredMoonlight = maxStoredMoonlight;

        maxStoredSunlight = SetMaxStoredSunlightFromSunlightLevel();
        currentStoredSunlight = maxStoredSunlight;
    }

    #region Health
    protected float SetMaxHealthFromHealthLevel()
    {
        //calculates the players health based on health level
        return healthLevel * 10;
    }

    public virtual void TakeDamage(float damageAmount, bool playAnimation = true)
    {
        currentHealth -= damageAmount;
    }

    public virtual void RegainHealth(float regainAmount)
    {
        currentHealth += regainAmount;
    }

    #endregion

    protected float SetMaxStaminaFromStaminaLevel()
    {
        //calculates the players stamina based on stamina level
        return staminaLevel * 10;
    }


    #region Casting Pools
    protected float SetMaxStoredMoonlightFromMoonlightLevel()
    {
        //calculates the players magicka based on magicka level
        return MoonlightLevel * 10;
    }

    protected float SetMaxStoredSunlightFromSunlightLevel()
    {
        //calculates the players magicka based on magicka level
        return SunlightLevel * 10;
    }

    #endregion
}
