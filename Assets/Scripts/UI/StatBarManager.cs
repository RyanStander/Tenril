using UnityEngine;

public class StatBarManager : MonoBehaviour
{
    [Header("Resource bars")]
    [SerializeField] private SliderBarDisplayUI healthBar;
    [SerializeField] private SliderBarDisplayUI staminaBar, sunlightBar, moonlightBar;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.UpdatePlayerStats, OnUpdatePlayerStats);
        EventManager.currentManager.Subscribe(EventType.UpdatePlayerHealth, OnUpdatePlayerHealth);
        EventManager.currentManager.Subscribe(EventType.UpdatePlayerStamina, OnUpdatePlayerStamina);
        EventManager.currentManager.Subscribe(EventType.UpdatePlayerMoonlight, OnUpdatePlayerMoonlight);
        EventManager.currentManager.Subscribe(EventType.UpdatePlayerSunlight, OnUpdatePlayerSunglight);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.UpdatePlayerStats, OnUpdatePlayerStats);
        EventManager.currentManager.Unsubscribe(EventType.UpdatePlayerHealth, OnUpdatePlayerHealth);
        EventManager.currentManager.Unsubscribe(EventType.UpdatePlayerStamina, OnUpdatePlayerStamina);
        EventManager.currentManager.Unsubscribe(EventType.UpdatePlayerMoonlight, OnUpdatePlayerMoonlight);
        EventManager.currentManager.Unsubscribe(EventType.UpdatePlayerSunlight, OnUpdatePlayerSunglight);
    }

    private void OnUpdatePlayerStats(EventData eventData)
    {
        if (eventData is UpdatePlayerStats updatePlayerStats)
        {
            healthBar.SetMaxValue(updatePlayerStats.playerMaxHealth);
            healthBar.SetCurrentValue(updatePlayerStats.playerCurrentHealth);

            staminaBar.SetMaxValue(updatePlayerStats.playerMaxStamina);
            staminaBar.SetCurrentValue(updatePlayerStats.playerCurrentStamina);

            sunlightBar.SetMaxValue(updatePlayerStats.playerMaxSunlight);
            sunlightBar.SetCurrentValue(updatePlayerStats.playerCurrentSunlight);

            moonlightBar.SetMaxValue(updatePlayerStats.playerMaxMoonlight);
            moonlightBar.SetCurrentValue(updatePlayerStats.playerCurrentMoonlight);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdatePlayerStats was received but is not of class UpdatePlayerStats.");
        }
    }

    private void OnUpdatePlayerHealth(EventData eventData)
    {
        if (eventData is UpdatePlayerHealth updatePlayerHealth)
        {
            healthBar.SetMaxValue(updatePlayerHealth.playerMaxHealth);
            healthBar.SetCurrentValue(updatePlayerHealth.playerCurrentHealth);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdatePlayerHealth was received but is not of class UpdatePlayerHealth.");
        }
    }

    private void OnUpdatePlayerStamina(EventData eventData)
    {
        if (eventData is UpdatePlayerStamina updatePlayerStamina)
        {
            staminaBar.SetMaxValue(updatePlayerStamina.playerMaxStamina);
            staminaBar.SetCurrentValue(updatePlayerStamina.playerCurrentStamina);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdatePlayerStamina was received but is not of class UpdatePlayerStamina.");
        }
    }

    private void OnUpdatePlayerMoonlight(EventData eventData)
    {
        if (eventData is UpdatePlayerMoonlight updatePlayerMoonlight)
        {
            moonlightBar.SetMaxValue(updatePlayerMoonlight.playerMaxMoonlight);
            moonlightBar.SetCurrentValue(updatePlayerMoonlight.playerCurrentMoonlight);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdatePlayerMoonlight was received but is not of class UpdatePlayerMoonlight.");
        }
    }

    private void OnUpdatePlayerSunglight(EventData eventData)
    {
        if (eventData is UpdatePlayerSunlight updatePlayerSunglight)
        {
            sunlightBar.SetMaxValue(updatePlayerSunglight.playerMaxSunlight);
            sunlightBar.SetCurrentValue(updatePlayerSunglight.playerCurrentSunlight);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdatePlayerSunlight was received but is not of class UpdatePlayerSunlight.");
        }
    }
}
