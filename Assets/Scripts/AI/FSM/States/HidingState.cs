using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HidingState : AbstractStateFSM
{
    //Masks for detection and vision blocking
    public LayerMask detectionBlockLayer = 1 << 9;

    //The layers that are valid for hiding behind
    public LayerMask hidableLayers;

    //Current "anti" target
    public GameObject currentTarget;

    //How sensitive the agent is to hiding, lower means better hiding
    [Range(-1, 1)] public float hideSensitivity = 0;

    //Distance that the target must be within before it will try to relocate
    [Range(1, 10)] public float minimumPlayerDistance = 5f;

    //The minimum obstacle height needed 
    [Range(0, 2.5f)] public float minimumObstacleHeight = 1f;

    //The frequency at which calculations should be made
    [Range(0.01f, 1f)] public float updateFrequency = 0.25f;

    //Courotine for movement logic
    private Coroutine hidingCourotine;

    //Helper bool to prevent excessive running the hiding logic more than once
    private bool isRunningHidingLogic = false;

    //Found hideable colliders, limited in size for original prototype
    private Collider[] foundHiddingColliders = new Collider[25];

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.HIDE;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED HIDE STATE");

            //Update the current target to the agent manager target
            //currentTarget = enemyManager.currentTarget;

            //Reset bool
            isRunningHidingLogic = false;

            //Set the hiding logic to cooldown in case of interruptions and other factors (avoids broken loops)
            enemyManager.currentHidingTime = enemyManager.enemyStats.hidingCooldownTime;
        }

        return enteredState;
    }


    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING HIDE STATE");

            //Run courotine if not already
            if (isRunningHidingLogic == false)
            {
                //Run and close the loop
                hidingCourotine = StartCoroutine(OriginalHidingLogic(currentTarget.transform));
            }

            //If at destination, dont move
            if(!navAgent.pathPending)
            {
                if (navAgent.remainingDistance <= 0.5f)
                {
                    //Move towards destination
                    movementManager.StopMovement(0.25f, Time.deltaTime);
                    
                    //Check if healing is possible
                    if (enemyManager.ShouldTryHealing())
                    {
                        //Attempt to heal
                        finiteStateMachine.EnterState(StateTypeFSM.HEALING);
                    }

                    //Return to evaluation state
                    finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
                }
                else
                {
                    //Move towards destination
                    movementManager.HandleNavMeshTargetMovement(enemyManager.enemyStats.chaseSpeed);
                }
            }
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Stop the logic loop if happening
        StopCoroutine(hidingCourotine);

        //Debug message
        DebugLogString("EXITED HIDE STATE");

        //Return true
        return true;
    }
    
    private IEnumerator OriginalHidingLogic(Transform Target)
    {
        //Set the helper bool
        isRunningHidingLogic = true;

        //Set the hiding cooldown timer
        enemyManager.currentHidingTime = enemyManager.enemyStats.hidingCooldownTime;

        //Declare the wait time
        WaitForSeconds Wait = new WaitForSeconds(updateFrequency);
        while (true)
        {
            //Clears out old information in case of dangling collider information
            for (int i = 0; i < foundHiddingColliders.Length; i++)
            {
                foundHiddingColliders[i] = null;
            }

            //Gets the number of hits detected based on given parameters
            int hits = Physics.OverlapSphereNonAlloc(navAgent.transform.position, enemyManager.enemyStats.obstacleAwarenessRange, foundHiddingColliders, hidableLayers);

            //Prioritize hiding spots that are farther away from the player by setting close by hiding points as null
            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                //If within min player distance oor obstacle height is not sufficient, set to null
                if (Vector3.Distance(foundHiddingColliders[i].transform.position, Target.position) < minimumPlayerDistance || foundHiddingColliders[i].bounds.size.y < minimumObstacleHeight)
                {
                    foundHiddingColliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            //Sort the array of colliders, moving null objects to the end of the array
            System.Array.Sort(foundHiddingColliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                //Sample each collider for potential positions to hide by
                if (NavMesh.SamplePosition(foundHiddingColliders[i].transform.position, out NavMeshHit firstHit, 2f, navAgent.areaMask))
                {
                    //Gets the closest edge that has a normal attached to it, this will be important in determining if the position points away or towards the target
                    if (!NavMesh.FindClosestEdge(firstHit.position, out firstHit, navAgent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {firstHit.position}");
                    }

                    //If the hiding spot is within the hiding sensitivity, set a new destination
                    if (Vector3.Dot(firstHit.normal, (Target.position - firstHit.position).normalized) < hideSensitivity)
                    {
                        navAgent.SetDestination(firstHit.position);
                        break;
                    }
                    //However, if the DOT product was greater than the sensitivity, it means the hiding spot was facing the target
                    else
                    {
                        //As a result, attempt to sample the other side of the object and check for validity
                        if (NavMesh.SamplePosition(foundHiddingColliders[i].transform.position - (Target.position - firstHit.position).normalized * 2, out NavMeshHit secondHit, 2f, navAgent.areaMask))
                        {
                            //Gets the closest edge that has a normal attached to it
                            if (!NavMesh.FindClosestEdge(secondHit.position, out secondHit, navAgent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {secondHit.position} (second attempt)");
                            }

                            //If the direction appears valid and good according to the hide sensitivity, set the destination and break the loop
                            if (Vector3.Dot(secondHit.normal, (Target.position - secondHit.position).normalized) < hideSensitivity)
                            {
                                navAgent.SetDestination(secondHit.position);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //If no positions were found, debug the error as it means the value put into the sample position or scene setup is incorrect
                    Debug.LogError($"Unable to find NavMesh near object {foundHiddingColliders[i].name} at {foundHiddingColliders[i].transform.position}");
                }
            }
            yield return Wait;
        }
    }

    public int ColliderArraySortComparer(Collider colliderA, Collider colliderB)
    {
        //Compares given colliders to eachother and returns a given position for the array based on how it should be sorted
        if (colliderA == null && colliderB != null)
        {
            return 1;
        }
        else if (colliderA != null && colliderB == null)
        {
            return -1;
        }
        else if (colliderA == null && colliderB == null)
        {
            return 0;
        }
        else
        {
            //If using an agent
            if(navAgent != null)
            {
                return ExtensionMethods.GetPathDistance(navAgent, colliderA.transform).CompareTo(ExtensionMethods.GetPathDistance(navAgent, colliderB.transform));
            }
            else
            {
                return Vector3.Distance(navAgent.transform.position, colliderA.transform.position).CompareTo(Vector3.Distance(navAgent.transform.position, colliderB.transform.position));
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(navAgent != null && currentTarget != null)
        {
            //Debug at target
            Gizmos.DrawSphere(navAgent.destination, 0.5f);

            //Obstacle awareness debug
            Gizmos.DrawWireSphere(gameObject.transform.position, enemyManager.enemyStats.obstacleAwarenessRange);

            //Debug around target
            Gizmos.DrawWireSphere(currentTarget.transform.position, minimumPlayerDistance);
        }
    }
}
