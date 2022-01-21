using TMPro;
using UnityEngine;

/// <summary>
/// Holds the data of the player stats sheet
/// </summary>
public class PlayerStatsHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text skillPointsRemainingText;
    [SerializeField] private TMP_Text healthLevelText, staminaLevelText, moonlightLevelText, sunlightLevelText;
    [SerializeField] private TMP_Text maxHealthText, maxStaminaText, maxMoonlightText, maxSunlightTest;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.UpdatePlayerStats, OnUpdatePlayerStats);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.UpdatePlayerStats, OnUpdatePlayerStats);
    }

    private void OnUpdatePlayerStats(EventData eventData)
    {
        if (eventData is UpdatePlayerStats updatePlayerStats)
        {
            skillPointsRemainingText.text = "Skill points : "+updatePlayerStats.skillPoints;

            maxHealthText.text= "Health: "+updatePlayerStats.playerMaxHealth;
            maxStaminaText.text = "Stamina: " + updatePlayerStats.playerMaxStamina;
            moonlightLevelText.text = "Moonlight: " + updatePlayerStats.playerMaxSunlight;
            sunlightLevelText.text = "Sunlight: " + updatePlayerStats.playerMaxMoonlight;

            healthLevelText.text = updatePlayerStats.playerHealthLevel.ToString();
            staminaLevelText.text = updatePlayerStats.playerStaminaLevel.ToString();
            moonlightLevelText.text = updatePlayerStats.playerMoonlightLevel.ToString();
            sunlightLevelText.text = updatePlayerStats.playerSunlightLevel.ToString();
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdatePlayerStats was received but is not of class UpdatePlayerStats.");
        }
    }
}
