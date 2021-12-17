using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int healthLevel = 10;
    [SerializeField] protected float maxHealth, currentHealth;
    public bool isDead = false;
    
    [Header("Stamina")]
    [SerializeField] protected int staminaLevel = 10;
    [SerializeField] protected float maxStamina, currentStamina;
    [SerializeField] protected float staminaRegenAmount=1,staminaRegenRate = 0.1f, staminaRegenCooldownTime = 2;
    protected float staminaCDTimeStamp, staminaRegenTimeStamp;
    protected bool canRegenStamina = true;

    [Header("Biomancy")]
    [SerializeField] protected int MoonlightLevel = 10;
    [SerializeField] protected float maxStoredMoonlight, currentStoredMoonlight;
    [SerializeField] protected float moonlightRegenAmount = 1, moonlightRegenRate = 0.1f, moonlightRegenCooldownTime = 2;
    protected float moonlightCDTimeStamp, moonlightRegenTimeStamp;
    protected bool canRegenMoonlight = true;

    [Header("Pyromancy")]
    [SerializeField] protected int sunlightLevel = 10;
    [SerializeField] protected float maxStoredSunlight, currentStoredSunlight;
    [SerializeField] protected float sunlightRegenAmount = 1, sunlightRegenRate = 0.1f, sunlightRegenCooldownTime = 2;
    protected float sunlightCDTimeStamp, sunlightRegenTimeStamp;
    protected bool canRegenSunlight = true;

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

    public virtual void TakeDamage(float damageAmount, bool playAnimation = true, string damageAnimation = "Hit")
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
        if (canRegenStamina && currentStamina != maxStamina)
        {
            if (currentStamina < maxStamina && staminaRegenTimeStamp <= Time.time)
            {
                RegenerateStamina();
            }
        }

        if (staminaCDTimeStamp <= Time.time)
            canRegenStamina = true;
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
        canRegenStamina = false;
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

    internal void HandleMoonlightPoolRegeneration(float timeStrength)
    {
        //if the character is able to regenerate moonlight and it is not at max already, restore moonlight
        //uses cooldowns to manage how fast moonlight regenerates
        if (canRegenMoonlight && currentStoredMoonlight != maxStoredMoonlight)
        {
            if (currentStoredMoonlight < maxStoredMoonlight && moonlightRegenTimeStamp <= Time.time)
            {
                RegenerateMoonlight(timeStrength);
            }
        }

        if (moonlightCDTimeStamp <= Time.time)
            canRegenMoonlight = true;
    }

    protected virtual void RegenerateMoonlight(float timeStrength)
    {
        float amountToRegen = moonlightRegenAmount * timeStrength;
        if (amountToRegen<0)
            amountToRegen = 0;
        currentStoredMoonlight += amountToRegen;
        moonlightRegenTimeStamp = Time.time + moonlightRegenRate;
    }

    internal void PutMoonlightRegenOnCooldown()
    {
        moonlightCDTimeStamp = Time.time + moonlightRegenCooldownTime;
        canRegenMoonlight = false;
    }

    protected float SetMaxStoredSunlightFromSunlightLevel()
    {
        //calculates the players magicka based on magicka level
        return sunlightLevel * 10;
    }

    public virtual void ConsumeStoredMoonlight(float cost)
    {
        currentStoredMoonlight -= cost;
    }

    public virtual void ConsumeStoredSunlight(float cost)
    {
        currentStoredSunlight -= cost;
    }

    internal void HandleSunlightPoolRegeneration(float timeStrength)
    {
        //if the character is able to regenerate sunlight and it is not at max already, restore sunlight
        //uses cooldowns to manage how fast sunlight regenerates
        if (canRegenSunlight && currentStoredSunlight != maxStoredSunlight)
        {
            if (currentStoredSunlight < maxStoredSunlight && sunlightRegenTimeStamp <= Time.time)
            {
                RegenerateSunlight(timeStrength);
            }
        }

        if (sunlightCDTimeStamp <= Time.time)
            canRegenSunlight = true;
    }

    protected virtual void RegenerateSunlight(float timeStrength)
    {
        float amountToRegen = sunlightRegenAmount * timeStrength;
        if (amountToRegen < 0)
            amountToRegen = 0;
        currentStoredSunlight += amountToRegen;
        sunlightRegenTimeStamp = Time.time + sunlightRegenRate;
    }

    internal void PutSunlightRegenOnCooldown()
    {
        sunlightCDTimeStamp = Time.time + sunlightRegenCooldownTime;
        canRegenSunlight = false;
    }

    #endregion
}
