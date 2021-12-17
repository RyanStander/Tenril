using JetBrains.Annotations;

[System.Serializable]
public class PlayerData
{

    #region Stats
    public int currentLevel;

    //Health
    public int healthLevel;
    public float currentHealth;

    //Stamina
    public int staminaLevel;
    public float currentStamina;

    //Moonlight
    public int moonlightLevel;
    public float currentMoonlight;

    //Sunlight
    public int sunglightLevel;
    public float currentSunlight;
    #endregion

    #region Unique Data
    //Transform Data
    public float[] position=new float[3];
    public float[] rotation = new float[3];

    //faction
    public int factionID;

    #endregion

    #region Inventory

    //The primary and secondary weapons of the player

    //bool of which weapon is currently in hand

    //current prepared spells

    //current equipped ammo

    //set quickslot items

    //items in inventory

    #endregion

    public PlayerData(PlayerStats playerStats,PlayerInventory playerInventory)
    {
        #region Stats
        
        currentLevel = playerStats.currentLevel;

        healthLevel = playerStats.healthLevel;
        currentHealth = playerStats.currentHealth;

        staminaLevel = playerStats.staminaLevel;
        currentStamina = playerStats.currentStamina;

        moonlightLevel = playerStats.MoonlightLevel;
        currentMoonlight = playerStats.currentStoredMoonlight;

        sunglightLevel = playerStats.sunlightLevel;
        currentSunlight = playerStats.currentStoredSunlight;

        #endregion

        #region Unique Data

        position[0] = playerStats.transform.position.x;
        position[1] = playerStats.transform.position.y;
        position[2] = playerStats.transform.position.z;

        rotation[0] = playerStats.transform.rotation.x;
        rotation[1] = playerStats.transform.rotation.y;
        rotation[2] = playerStats.transform.rotation.z;

        factionID = (int)playerStats.assignedFaction;

        #endregion
    }
}
