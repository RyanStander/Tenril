using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stand idle until player approaches
public class WatchState : AbstractStateFSM
{
    //Bool to track if alerted
    private bool wasAlerted = false;

    //Masks for detection and vision blocking
    public LayerMask detectionBlockLayer = 1 << 9;
    public LayerMask characterLayer = 1 << 10;

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
            DebugLogString("ENTERED IDLE STATE");

            //Set alert
            wasAlerted = false;

            //Set helped bool is using delay
            isWaitingToSearch = true;

            //Set current target to null
            currentTarget = null;
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING IDLE STATE");
        }

        //If was alerted, enter chase state
        if (wasAlerted == true)
        {
            DebugLogString("Alerted!");

            //Update the current target in the agent manager
            enemyManager.currentTarget = currentTarget;

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

        //Stop enemy movement in case of animation speed carry over
        movementManager.StopMovement();
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED IDLE STATE");

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
        targetsByDistance = enemyManager.visionManager.GetListOfVisibleEnemyTargets(visionManager.pointOfVision, enemyManager.enemyStats.alertRadius, characterLayer, detectionBlockLayer);
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

    private void OnDrawGizmosSelected()
    {
        //Return if needed items are not available yet
        if (enemyManager == null || enemyManager.enemyStats == null || enemyManager.visionManager == null) return;

        //Debug the sphere of view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(visionManager.pointOfVision.position, enemyManager.enemyStats.alertRadius);

        //Debug the sphere of chasing
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(visionManager.pointOfVision.position, enemyManager.enemyStats.chaseRange);
    }
}
