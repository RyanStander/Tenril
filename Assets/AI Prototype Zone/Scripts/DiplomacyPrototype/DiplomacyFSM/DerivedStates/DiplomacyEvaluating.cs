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

            //Calculate relationship level
            RelationshipLevel newRelationshipLevel = CalculateRelationshipLevel();

            //Update civilization relationship level (moves in single increments)
            leaderManager.leaderInfo.currentRelationshipLevel = newRelationshipLevel;

            Debug.Log(newRelationshipLevel);


            //Return to idle (for testing)
            finiteStateMachine.EnterState(DiplomacyFSMStateType.IDLE);
            
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

            Debug.Log(trait.traitName + ": " + traitStanding);
        }
        Debug.Log("RAW: " + currentStanding);

        //Divide the traits by the number of affected traits to more accurately calculate the standing modification
        currentStanding /= Mathf.Clamp(traitNumber, 1, float.MaxValue); //Clamped to never divide less than 1

        //Add in the neutral standing as a middleground
        currentStanding += (int)RelationshipLevel.Neutral;

        Debug.Log("FINAL: " + currentStanding);

        //Clamp the current standing
        currentStanding = Mathf.Clamp(currentStanding, (int)RelationshipLevel.Hostile, (int)RelationshipLevel.Friendly);

        //Return relationship level rounded to the nearest relationship level
        return (RelationshipLevel)currentStanding;
    }
}