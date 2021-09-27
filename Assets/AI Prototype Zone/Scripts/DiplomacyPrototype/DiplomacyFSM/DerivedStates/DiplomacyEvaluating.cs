using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DiplomacyEvaluating", menuName = "Diplomacy/States/Evaluating", order = 2)]
public class DiplomacyEvaluating : DiplomacyAbstractStateFSM
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = DiplomacyFSMStateType.EVALUATING;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED EVALUATING STATE");
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            //Run based method
            DebugLogString("UPDATING EVALUATING STATE");

            //Update the relationship between the AI leader and player
            UpdateRelationships();
        }
    }

    private void UpdateRelationships()
    {
        //Calculate relationship level
        RelationshipLevel newRelationshipLevel = CalculateRelationshipLevel();

        //Update civilization relationship level (moves in single increments)
        leaderManager.leaderInfo.currentRelationshipLevel = newRelationshipLevel;

        //Update the visuals
        leaderManager.UpdateVisuals();

        //Make a decision based on the current relationship
        MakeDecisionBasedOnRelationship();
    }

    private void MakeDecisionBasedOnRelationship()
    {
        //Switch statement for quick work
        switch(leaderManager.leaderInfo.currentRelationshipLevel)
        {
            //Compliment
            case RelationshipLevel.Friendly:
                finiteStateMachine.EnterState(DiplomacyFSMStateType.COMPLIMENT);
                break;

            //Neutral statement
            case RelationshipLevel.Receptive:
            case RelationshipLevel.Neutral:
                finiteStateMachine.EnterState(DiplomacyFSMStateType.NEUTRAL);
                break;

            //Insult
            case RelationshipLevel.Weary:
            case RelationshipLevel.Hostile:
                finiteStateMachine.EnterState(DiplomacyFSMStateType.INSULT);
                break;

            default:
                DebugLogString("Invalid relationship to swap to! Returning to Idle...");
                finiteStateMachine.EnterState(DiplomacyFSMStateType.IDLE);
                break;
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED EVALUATING STATE");

        //Return true
        return true;
    }

    public RelationshipLevel CalculateRelationshipLevel()
    {
        //Current standing, will eventually have neutral standing added on
        float currentStanding = 0;

        //The number of traits iterated through, used in calculating the standing
        int traitNumber = 0; 

        //Get the production types that are affected by traits
        foreach(LeaderProductionTrait trait in leaderManager.leaderInfo.leaderTraits)
        {
            //Get the weighed standing modifier using the relevant player production
            float traitStanding = trait.CalculateLikability(
                leaderManager.playerProduction.playerProductions[trait.targettedProduction].value,
                leaderManager.playerProduction.maximumProductionValue);

            DebugLogString(trait.traitName + ": " + traitStanding);

            //Add to the current standing
            currentStanding += traitStanding;
            traitNumber++;
        }

        //Divide the traits by the number of affected traits to more accurately calculate the standing modification
        currentStanding /= Mathf.Clamp(traitNumber, 1, float.MaxValue); //Clamped to never divide less than 1

        //Add in the neutral standing as a middleground
        currentStanding += (int)RelationshipLevel.Neutral;

        //Debug the standing value
        DebugLogString("Final Standing RAW Value: " + currentStanding);

        //Clamp the current standing after rounding
        currentStanding = Mathf.Clamp(Mathf.RoundToInt(currentStanding), (int)RelationshipLevel.Hostile, (int)RelationshipLevel.Friendly);

        //Return relationship level rounded to the nearest relationship level
        return (RelationshipLevel)currentStanding;
    }
}