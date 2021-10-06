using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stand idle until player approaches
public class WatchState : AbstractStateFSM
{
    //Bool to track if alerted
    private bool wasAlerted = false;
    
    //"Blind" range
    public int alertRadius = 10;

    public LayerMask detectionBlockLayer = 1 << 9;
    public LayerMask characterLayer = 1 << 10;

    ////Range at which the the AI can see
    //public int visionRange = 20;

    ////Angle at which the AI can see
    //[Range(0, 180)]
    //public int fieldOfVision = 90;

    //Head of the agent watching
    public Transform agentHead;

    //Performance related rate at which the agent should cast search
    [Range(0,1)]
    public float searchRate = 0.5f;

    //Helped bool for delayed searches
    private bool isWaitingToSearch = true;

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
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING IDLE STATE");

            //If was alerted, enter chase state
            if (wasAlerted == true)
            {
                Debug.Log("Alerted!");
                wasAlerted = false;

                //Change to chase state
                //Currently no state change implemented
                //finiteStateMachine.EnterState();
            }
            else
            {
                CheckForTarget();
            }
            //else if (isWaitingToSearch)
            //{
            //    //Start a delayed courotine for target checking
            //    StartCoroutine(DelayedTargetCheck(searchRate));
            //}
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
        //Cast a sphere wrapping the head and check for characters within range
        Collider[] hitColliders = Physics.OverlapSphere(agentHead.position, alertRadius, characterLayer);
        
        //Declare temporary dictionary to hold targets and their distances
        Dictionary<GameObject, int> targetsByDistance = new Dictionary<GameObject, int>();

        //Check each character for if it is within valid distance and for its faction
        foreach (var hitCollider in hitColliders)
        {
            //Raycast to first check if the target is valid (Performance friendly to raycast first)
            if (Physics.Raycast(agentHead.position, hitCollider.gameObject.transform.position - agentHead.position, out RaycastHit hit))
            {
                //Do nothing if self
                if(hitCollider.gameObject.transform.root == this.gameObject.transform.root) { }
                //If there are no objects blocking the way, move onto faction filtering
                else if (detectionBlockLayer != (detectionBlockLayer | (1 << hit.transform.gameObject.layer)))
                {
                    Debug.Log("Colliding with character object!: " + hit.transform.gameObject);
                    

                    //Debug the line results
                    Debug.DrawRay(agentHead.position, hit.point - agentHead.position, Color.green);

                    //Check if character stats are attached, ignore otherwise
                    if (hitCollider.gameObject.TryGetComponent(out CharacterStats stats))
                    {
                        //Check for enemy faction or no faction
                        //If valid, add to dictionary with unpathed & direct distance
                        if (stats.assignedFaction != enemyManager.enemyStats.assignedFaction && stats.assignedFaction != Faction.NONE)
                        {
                           // Debug.Log("GameObject: " + hitCollider.gameObject + "| Distance: " + (int)Vector3.Distance(agentHead.position, hitCollider.gameObject.transform.position));
                            targetsByDistance.Add(hitCollider.gameObject, (int)Vector3.Distance(agentHead.position, hitCollider.gameObject.transform.position));
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

    private void OnDrawGizmosSelected()
    {
        //Debug the sphere of view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(agentHead.position, alertRadius);
    }
}
