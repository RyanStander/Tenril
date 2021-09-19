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

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        //Set starting energy
        currentEnergy = maximumEnergy;

        //Create & add goals with priorities, set to false to keep goal after completion
        SubGoal treasureGoal = new SubGoal("CollectedTreasure", 1, false);
        goals.Add(treasureGoal, treasureCollectionPriority);

        SubGoal deliveryGoal = new SubGoal("DeliveredTreasure", 1, false);
        goals.Add(deliveryGoal, treasureDeliveryPriority);

        ////Create & add goal with priority, false to keep goal
        //SubGoal restingGoal = new SubGoal("IsRested", 1, false);
        //goals.Add(restingGoal, stayRestedPriority);
    }

    public void Update()
    {
        EnergyLogic();
    }

    new void LateUpdate()
    {
        base.LateUpdate();
        UpdateStatusText();
    }

    private void EnergyLogic()
    {
        //If not resting, reduce energy over time
        if (!beliefs.states.ContainsKey("IsResting"))
        {
            currentEnergy -= Time.deltaTime;
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
}