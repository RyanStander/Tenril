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
    [SerializeField] protected float staminaRegenAmount=1,staminaRegenRate = 0.1f, staminaRegenCooldownTime = 2;
    protected float staminaCDTimeStamp, staminaRegenTimeStamp;
    protected bool canRegen = true;

    [Header("Biomancy")]
    [SerializeField] protected int MoonlightLevel = 10;
    [SerializeField] protected float maxStoredMoonlight, currentStoredMoonlight;

    [Header("Pyromancy")]
    [SerializeField] protected int SunlightLevel = 10;
    [SerializeField] protected float maxStoredSunlight, currentStoredSunlight;

    [Header("Extras")]
    public Faction assignedFaction = Faction.NONE;

    private void Start()
    {
        SetupStats();
    }

    //Manages values of character's statistics, such as health based on health level.
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
        //Prevent gaining health while at max
        if (currentHealth<maxHealth)
        {
            //regain health
            currentHealth += regainAmount;
            //if current health is higher than max, set to max
            if (currentHealth>maxHealth)
                currentHealth = maxHealth;
        }

    }

    #endregion

    #region Stamina

    public bool HasStamina()
    {
        if (currentStamina > 0)
            return true;
        else
            return false;
    }

    internal void HandleStaminaRegeneration()
    {
        //if the character is able to regenerate stamina and it is not at max already, restore stamina
        //uses cooldowns to manage how fast stamina regenerates
        if (canRegen && currentStamina != maxStamina)
        {
            if (currentStamina < maxStamina && staminaRegenTimeStamp <= Time.time)
            {
                RegenerateStamina();
            }
        }

        if (staminaCDTimeStamp <= Time.time)
            canRegen = true;
    }

    protected float SetMaxStaminaFromStaminaLevel()
    {
        //calculates the players stamina based on stamina level
        return staminaLevel * 10;
    }

    public virtual void DrainStamina(float drain)
    {
        //change current stamina
        currentStamina = currentStamina - drain;
    }

    protected virtual void RegenerateStamina()
    {
        currentStamina+= staminaRegenAmount;
        staminaRegenTimeStamp = Time.time + staminaRegenRate;
    }

    internal void PutStaminaRegenOnCooldown()
    {
        staminaCDTimeStamp = Time.time + staminaRegenCooldownTime;
        canRegen = false;
    }

    public void DrainStaminaWithCooldown(float staminaAmount)
    {
        PutStaminaRegenOnCooldown();
        DrainStamina(staminaAmount);
    }

    #endregion

    #region Casting Pools

    public bool HasEnoughMoonlight(float moonlightCost)
    {
        if (currentStoredMoonlight >= moonlightCost)
            return true;
        else
            return false;
    }

    public bool HasEnoughSunlight(float sunlightCost)
    {
        if (currentStoredSunlight >= sunlightCost)
            return true;
        else
            return false;
    }

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

    public virtual void ConsumeStoredMoonlight(float cost)
    {
        currentStoredMoonlight -= cost;
    }

    public virtual void ConsumeStoredSunlight(float cost)
    {
        currentStoredSunlight -= cost;
    }

    #endregion
}
