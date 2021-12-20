using UnityEngine;

/// <summary>
/// Holds the stats for the player, performs functions to do with their stats as well as leveling and xp. A few combat functions as well.
/// </summary>
public class PlayerStats : CharacterStats
{
    [Header("Main Level")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public LevelData levelData;
    public int skillPoints = 0;

    private PlayerAnimatorManager playerAnimatorManager;
    private BlockingCollider blockingCollider;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.PlayerGainSkill, OnSpendSkillPoint);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.PlayerGainSkill, OnSpendSkillPoint);
    }

    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        blockingCollider = GetComponentInChildren<BlockingCollider>();
    }
    private void Start()
    {
        SetupStats();

        EventManager.currentManager.AddEvent(new UpdatePlayerStats(maxHealth, currentHealth, maxStamina,currentStamina,maxStoredMoonlight,currentStoredMoonlight,maxStoredSunlight,currentStoredSunlight));
    }

    public void SetPlayerStats(PlayerData playerData)
    {
        currentLevel = playerData.currentLevel;

        healthLevel = playerData.healthLevel;
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = playerData.currentHealth;

        staminaLevel = playerData.staminaLevel;
        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = playerData.currentStamina;

        MoonlightLevel = playerData.moonlightLevel;
        maxStoredMoonlight = SetMaxStoredMoonlightFromMoonlightLevel();
        currentStoredMoonlight = playerData.currentMoonlight;

        sunlightLevel = playerData.sunglightLevel;
        maxStoredSunlight = SetMaxStoredSunlightFromSunlightLevel();
        currentStoredSunlight = playerData.currentMoonlight;

        assignedFaction = (Faction)playerData.factionID;

        EventManager.currentManager.AddEvent(new UpdatePlayerStats(maxHealth, currentHealth, maxStamina,currentStamina,maxStoredMoonlight,currentStoredMoonlight,maxStoredSunlight,currentStoredSunlight));
    }

    public void OpenBlockingCollider(PlayerInventory playerInventory)
    {
        if (playerInventory==null)
        {
            Debug.LogWarning("Player inventory was not found, make sure the script calling this function has an inventory reference");
            return;
        }
        if (playerInventory.equippedWeapon==null)
        {
            return;
        }
        blockingCollider.SetColliderDamageAbsorption(playerInventory.equippedWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider()
    {
        blockingCollider.DisableBlockingCollider();
    }

    #region Health
    public override void TakeDamage(float damageAmount, bool playAnimation = true, string damageAnimation = "Hit")
    {
        if (playerAnimatorManager.animator.GetBool("isInvulnerable"))
            return;

        if (isDead)
            return;

        //change current health
        base.TakeDamage(damageAmount);

        //update health display on the healthbar
        EventManager.currentManager.AddEvent(new UpdatePlayerHealth(maxHealth, currentHealth));

        //play animation that player has taken damage
        if (playAnimation)
            playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            
            playerAnimatorManager.PlayTargetAnimation("Death", true);

            isDead = true;

            //Handle player death
        }
    }

    public override void RegainHealth(float regainAmount)
    {
        if (isDead)
            return;

        //change current health
        base.RegainHealth(regainAmount);

        //update health display on the healthbar
        EventManager.currentManager.AddEvent(new UpdatePlayerHealth(maxHealth, currentHealth));
    }
    #endregion

    #region Stamina

    public override void DrainStamina(float drain)
    {
        base.DrainStamina(drain);

        //update the current stamina on the stamina bar
        EventManager.currentManager.AddEvent(new UpdatePlayerStamina(maxStamina, currentStamina));
    }

    protected override void RegenerateStamina()
    {
        base.RegenerateStamina();

        //update the current stamina on the stamina bar
        EventManager.currentManager.AddEvent(new UpdatePlayerStamina(maxStamina, currentStamina));
    }

    protected override void RegenerateMoonlight(float timeStrength)
    {
        base.RegenerateMoonlight(timeStrength);

        //update the current moonlight on the moonlight bar
        EventManager.currentManager.AddEvent(new UpdatePlayerMoonlight(maxStoredMoonlight, currentStoredMoonlight));
    }

    protected override void RegenerateSunlight(float timeStrength)
    {
        base.RegenerateSunlight(timeStrength);

        //update the current sunlight on the sunlight bar
        EventManager.currentManager.AddEvent(new UpdatePlayerSunlight(maxStoredSunlight, currentStoredSunlight));
    }

    #endregion

    #region Spellcasting

    public override void ConsumeStoredMoonlight(float cost)
    {
        base.ConsumeStoredMoonlight(cost);

        //update the current moonlight on the moonlight bar
        EventManager.currentManager.AddEvent(new UpdatePlayerMoonlight(maxStoredMoonlight, currentStoredMoonlight));
    }

    public override void ConsumeStoredSunlight(float cost)
    {
        base.ConsumeStoredSunlight(cost);

        //update the current sunlight on the sunlight bar
        EventManager.currentManager.AddEvent(new UpdatePlayerSunlight(maxStoredSunlight, currentStoredSunlight));
    }

    #endregion

    #region Leveling

    internal void IncreaseLevel(int levelsGained)
    {
        currentLevel += levelsGained;
        skillPoints += levelsGained;
    }

    internal void IncreaseXP(int xpGained)
    {
        currentXP += xpGained;
    }

    #endregion

    #region Skill Spending

    private void SpendSkillPoint(Skill skill)
    {
        switch (skill)
        {
            case Skill.Health:
                //Increase stat level
                healthLevel++;
                //Get previous max
                float previousMaxHP = maxHealth;
                //Set max to new level
                maxHealth = SetMaxHealthFromHealthLevel();
                //get the precentage increase of state
                float healthPercentileIncrease = 1-(previousMaxHP / maxHealth);
                //get the amount of current stat to increase by
                float currentHealthToIncrease = maxHealth * healthPercentileIncrease;
                //increase current stat
                currentHealth += currentHealthToIncrease;
                break;
            case Skill.Stamina:
                //Increase stat level
                staminaLevel++;
                //Get previous max
                float previousMaxStamina = maxStamina;
                //Set max to new level
                maxStamina=SetMaxStaminaFromStaminaLevel();
                //get the precentage increase of state
                float staminaPercentileIncrease = 1 - (previousMaxStamina / maxStamina);
                //get the amount of current stat to increase by
                float currentStaminaToIncrease = maxStamina * staminaPercentileIncrease;
                //increase current stat
                currentStamina += currentStaminaToIncrease;
                break;
            case Skill.Moonlight:
                //Increase stat level
                MoonlightLevel++;
                //Get previous max
                float previousMaxMoonlight = maxStoredMoonlight;
                //Set max to new level
                maxStoredMoonlight=SetMaxStoredMoonlightFromMoonlightLevel();
                //get the precentage increase of state
                float moonlightPercentileIncrease = 1 - (previousMaxMoonlight / maxStoredMoonlight);
                //get the amount of current stat to increase by
                float currentMoonlightToIncrease = maxStoredMoonlight * moonlightPercentileIncrease;
                //increase current stat
                currentStoredMoonlight += currentMoonlightToIncrease;
                break;
            case Skill.Sunlight:
                //Increase stat level
                sunlightLevel++;
                //Get previous max
                float previousMaxSunlight = maxStoredSunlight;
                //Set max to new level
                maxStoredSunlight=SetMaxStoredSunlightFromSunlightLevel();
                //get the precentage increase of state
                float sunlightPercentileIncrease = 1 - (previousMaxSunlight / maxStoredSunlight);
                //get the amount of current stat to increase by
                float currentSunlightToIncrease = maxStoredSunlight * sunlightPercentileIncrease;
                //increase current stat
                currentStoredSunlight += currentSunlightToIncrease;
                break;
        }
    }

    #endregion

    #region On Events

    private void OnSpendSkillPoint(EventData eventData)
    {
        if (eventData is PlayerGainSkill gainSkill)
        {
            //Check if skill points are to be deducted
            if (gainSkill.consumeSkillPoint)
            {
                //if there are no skill points, exit
                if (skillPoints==0)
                {
                    Debug.Log("Attempting to increase skill, but there was no skill points to spend");
                    return;
                }
                //else deduct skill points
                else
                {
                    skillPoints--;
                }
            }

            SpendSkillPoint(gainSkill.skillToGain);
        }

    }

    #endregion

    internal bool GetIsDead()
    {
        return isDead;
    }
}
