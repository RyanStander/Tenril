using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stand idle until player approaches
public class WatchState : AbstractStateFSM
{
    //Bool to track if alerted
    private bool wasAlerted = false;

    //Bool for if a search rate should be used
    public bool isUsingSearchRate;

    //Performance related rate at which the agent should cast search
    [Range(0,1)] public float searchRate = 0.5f;

    //Helped bool for delayed searches
    private bool isWaitingToSearch = true;

    //Dictionary to hold potential targets and their distances
    private Dictionary<GameObject, float> targetsByDistance = new Dictionary<GameObject, float>();

    //Current target
    private GameObject currentTarget = null;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.WATCH;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED WATCH STATE");

            //Set alert
            wasAlerted = false;

            //Set helped bool is using delay
            isWaitingToSearch = true;

            //Set current target to null
            currentTarget = null;

            //Reset the NavAgent destination in case of old information
            enemyManager.navAgent.ResetPath();
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING WATCH STATE");
        }

        //If was alerted, enter chase state
        if (wasAlerted == true)
        {
            DebugLogString("Alerted!");

            //Update the current target in the agent manager
            enemyManager.currentTarget = currentTarget;

            //Alert nearby allies of found target
            enemyManager.attackManager.AlertAlliesOfTarget(currentTarget);

            //TODO: Fire off initiate event for Ryan

            //Change to chase state
            finiteStateMachine.EnterState(StateTypeFSM.CHASETARGET);
        }
        else
        {
            if (isWaitingToSearch && isUsingSearchRate)
            {
                //Start a delayed courotine for target checking
                StartCoroutine(DelayedTargetCheck(searchRate));
            }
            else if (!isUsingSearchRate)
            {
                //Default automatic check
                CheckForTarget();
            }

            //Check for the closest target (if any)
            CheckForClosestTarget();
        }

        //Check for if healing should be done
        if (enemyManager.ShouldTryHealing())
        {
            //Attempt to heal
            finiteStateMachine.EnterState(StateTypeFSM.HEALING);
        }

        //Stop enemy movement in case of animation speed carry over
        movementManager.StopMovement();
    }

    public void AllyFoundTarget(GameObject target)
    {
        //Update the current target in the agent manager
        enemyManager.currentTarget = target;

        //Change to chase
        finiteStateMachine.EnterState(StateTypeFSM.CHASETARGET);
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED WATCH STATE");

        //Return true
        return true;
    }

    IEnumerator DelayedTargetCheck(float delayTime)
    {
        //Set helper bool to false
        isWaitingToSearch = false;

        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Set helped bool to true to allow for a new search
        isWaitingToSearch = true;

        //Check for targets, delay was included for performance reasons
        CheckForTarget();
    }

    private void CheckForTarget()
    {
        //Fetches detectable characters and returns a list of enemies
        targetsByDistance = enemyManager.visionManager.GetListOfVisibleExpectedTargets(visionManager.pointOfVision, enemyManager.attackManager.currentAlertnessRange, Faction.NONE, false);
    }

    private void CheckForClosestTarget()
    {
        foreach(KeyValuePair<GameObject, float> valuePair in targetsByDistance)
        {
            DebugLogString(valuePair.Key + " at distance " + valuePair.Value);
        }

        //Null & empty check for the dictionary
        if(targetsByDistance.Count > 0 && targetsByDistance != null)
        {
            //Temporary int for the distance
            float shortestDistance = float.MaxValue;

            //Iterate over the dictionary for the closest target
            foreach (KeyValuePair<GameObject, float> valuePair in targetsByDistance)
            {
                //Check if closer than the current distance
                if(valuePair.Value < shortestDistance)
                {
                    //Set new distance
                    shortestDistance = valuePair.Value;

                    //Set new target
                    currentTarget = valuePair.Key;
                }
            }

            //Debug the chosen target at a specified distance
            targetsByDistance.TryGetValue(currentTarget, out float val);
            DebugLogString("Chosen target: " + currentTarget + " at distance " + val);
            
            //Second null check, if passed declare a found target
            if(currentTarget != null)
            {
                wasAlerted = true;
            }
        }
    }
}
