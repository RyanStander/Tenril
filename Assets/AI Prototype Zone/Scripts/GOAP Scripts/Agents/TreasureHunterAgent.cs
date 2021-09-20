using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureHunterAgent : GAgent
{
    //Priority level of task, higher the number the more important
    public int treasureCollectionPriority = 10;
    public int treasureDeliveryPriority = 8;
    public int stayRestedPriority = 5;

    //Level of energy that the treasure hunter has
    public int maximumEnergy = 100;
    private float currentEnergy;

    //Status text to update
    public TextMeshProUGUI statusText = null;

    //Slider for energy
    public Slider energyBar = null;
    public float visualFillSpeed = 1;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        //Set starting energy
        currentEnergy = maximumEnergy;

        //Set Maximum for bar
        if (energyBar != null) { energyBar.maxValue = maximumEnergy; }
        
        //Create & add goals with priorities, set to false to keep goal after completion
        SubGoal treasureGoal = new SubGoal("CollectedTreasure", 1, false);
        goals.Add(treasureGoal, treasureCollectionPriority);

        SubGoal deliveryGoal = new SubGoal("DeliveredTreasure", 1, false);
        goals.Add(deliveryGoal, treasureDeliveryPriority);

        //Create & add goal with priority, false to keep goal
        SubGoal restingGoal = new SubGoal("IsRested", 1, false);
        goals.Add(restingGoal, stayRestedPriority);

        //Declare agent start beliefs
        beliefs.ModifyState("WellRested", 1); //Agent begins well rested
    }

    public void Update()
    {
        EnergyLogic();
    }

    new void LateUpdate()
    {
        base.LateUpdate();
        UpdateStatusText();
        UpdateEnergyBar();
    }

    //Bool to help manage energy resets
    private bool energyShouldReset = false;
    private void EnergyLogic()
    {
        //Reduce energy over time
        currentEnergy -= Time.deltaTime;

        //If agent is well rested and energy should reset, then reset
        if (beliefs.states.ContainsKey("WellRested") && energyShouldReset)
        {
            //Reset bool
            energyShouldReset = false;

            //Reset energy to maximum
            currentEnergy = maximumEnergy;
        }

        //If energy should reset
        //Happens when agent believes they are rested and current energy is equal or less than 0
        if (beliefs.states.ContainsKey("WellRested") && currentEnergy <= 0)
        {
            //Reset bool
            energyShouldReset = true;

            //Remove well rested belief
            beliefs.states.Remove("WellRested");
        }

        //Clamp between 0 and maximum energy
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maximumEnergy);
    }

    private void UpdateStatusText()
    {
        //Null check, fails silently as a status text might not be wanted for all agents
        if(statusText != null && currentAction !=null)
        {
            //Set text to current action
            statusText.text = currentAction.actionName;
        }
        else
        {
            //Set text to inactive
            statusText.text = "Inactive";
        }
    }

    private void UpdateEnergyBar()
    {
        //Null check, fails silently as a status text might not exist for all agents
        if (energyBar != null)
        {
            //Update energy level
            energyBar.value = currentEnergy;
        }
    }
}