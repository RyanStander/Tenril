using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectUIDisplay : MonoBehaviour
{
    [SerializeField] private GameObject statusEffectDisplay;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.UpdateStatusEffectsDisplay, OnUpdateStatusEffectsDisplay);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.UpdateStatusEffectsDisplay, OnUpdateStatusEffectsDisplay);
    }

    private void OnUpdateStatusEffectsDisplay(EventData eventData)
    {
        if (eventData is UpdateStatusEffectsDisplay updateStatusEffectsDisplay)
        {
            //update display
            UpdateDisplay(updateStatusEffectsDisplay.statusEffectManager);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.UpdateStatusEffectsDisplay was received but is not of class UpdateStatusEffectsDisplay.");
        }
    }

    private void UpdateDisplay(StatusEffectManager statusEffectManager)
    {
        //destroy all children in object
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //create status effects
        for (int i = 0; i < statusEffectManager.statusEffects.Count; i++)
        {
            //create prefab and assign it
            GameObject createdStatusEffectDisplay = Instantiate(statusEffectDisplay, transform);
            //find its status effect icon script
            StatusEffectIcon statusEffectIcon = createdStatusEffectDisplay.GetComponent<StatusEffectIcon>();
            //Pass the values to the script
            statusEffectIcon.SetValues(statusEffectManager.statusEffects[i].itemIcon, statusEffectManager.timeLeft[i]);
        }
    }
}
