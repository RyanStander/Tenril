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

    //Head of the agent watching
    public Transform agentHead;

    //Bool for if a searchrate should be used
    public bool isUsingSearchRate;

    //Performance related rate at which the agent should cast search
    [Range(0,1)]
    public float searchRate = 0.5f;

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
        targetsByDistance = enemyManager.visionManager.GetListOfTargets(agentHead, enemyManager.enemyStats.alertRadius, characterLayer, detectionBlockLayer);
        return;

        //Cast a sphere wrapping the head and check for characters within range
        Collider[] hitColliders = Physics.OverlapSphere(agentHead.position, enemyManager.enemyStats.alertRadius, characterLayer);

        //Clear the dictionary to free up space for new detected targets
        targetsByDistance.Clear();

        //Check each character for if it is within valid distance and for its faction
        foreach (var hitCollider in hitColliders)
        {
            //If existing, get the vision point of the object, otherwise use hte hid collider
            GameObject visionPoint = ExtensionMethods.FindChildWithTag(hitCollider.gameObject, "VisionTargetPoint");

            //If no vision point was found, default to the hit collider
            if(visionPoint == null)
            {
                visionPoint = hitCollider.gameObject;
            }

            //Raycast to first check if the target is valid (Performance friendly to raycast first)
            if (Physics.Raycast(agentHead.position, visionPoint.transform.position - agentHead.position, out RaycastHit hit))
            {
                //Continue if the found object was not self
                if(hitCollider.gameObject.transform.root != gameObject.transform.root)
                {
                    //If there are no objects blocking the way, move onto faction filtering
                    if (detectionBlockLayer != (detectionBlockLayer | (1 << hit.transform.gameObject.layer)))
                    {
                        //DebugLogString("Colliding with character object!: " + hit.transform.gameObject);

                        //Debug the line results
                        Debug.DrawRay(agentHead.position, hit.point - agentHead.position, Color.green);

                        //Check if character stats are attached, ignore otherwise
                        if (hitCollider.gameObject.TryGetComponent(out CharacterStats stats))
                        {
                            //Check for enemy faction or no faction as well as for if the target is still alive
                            //If valid, add to dictionary with unpathed & direct distance
                            if (!stats.isDead && stats.assignedFaction != enemyManager.enemyStats.assignedFaction && stats.assignedFaction != Faction.NONE)
                            {
                                DebugLogString("Target Character Found: " + hitCollider.gameObject + "| Distance: " + Vector3.Distance(agentHead.position, hitCollider.gameObject.transform.position));
                                targetsByDistance.Add(hitCollider.gameObject, Vector3.Distance(agentHead.position, hitCollider.gameObject.transform.position));
                            }
                        }
                    }
                    else
                    {
                        //Debug the line results
                        Debug.DrawRay(agentHead.position, hit.point - agentHead.position, Color.red);
                    }
                }
            }
        }
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
        if (enemyManager == null || enemyManager.enemyStats == null) return;

        //Debug the sphere of view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(agentHead.position, enemyManager.enemyStats.alertRadius);

        //Debug the sphere of chasing
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(agentHead.position, enemyManager.enemyStats.chaseRange);
    }
}
