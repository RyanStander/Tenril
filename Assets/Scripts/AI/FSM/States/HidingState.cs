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

    //Performance related rate at which the agent should cast a check
    [Range(0, 1)] public float checkRate = 0.5f;

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

    //List for colliders that are found as potential hiding targets
    private List<Collider> validObstacles = new List<Collider>();

    //Courotine for movement logic
    private Coroutine hidingCourotine;

    //Dictionary for obstacles by distance, kept in for documentation
    private Dictionary<Collider, float> validObstaclesDistance = new Dictionary<Collider, float>();

    //Helper bool to prevent excessive running the hiding logic more than once
    private bool isRunningHidingLogic = false;

    //Found colliders, limited in size for original prototype
    private Collider[] foundColliders = new Collider[25];

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
                isRunningHidingLogic = true;
            }

            //Move towards destination
            movementManager.HandleNavMeshTargetMovement(enemyManager.enemyStats.chaseSpeed);
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
        WaitForSeconds Wait = new WaitForSeconds(updateFrequency);
        while (true)
        {
            //Clears out old information in case of dangling collider information
            for (int i = 0; i < foundColliders.Length; i++)
            {
                foundColliders[i] = null;
            }

            //Gets the number of hits detected based on given parameters
            int hits = Physics.OverlapSphereNonAlloc(navAgent.transform.position, enemyManager.enemyStats.obstacleAwarenessRange, foundColliders, hidableLayers);

            //Prioritize hiding spots that are farther away from the player by setting close by hiding points as null
            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                //If within min player distance oor obstacle height is not sufficient, set to null
                if (Vector3.Distance(foundColliders[i].transform.position, Target.position) < minimumPlayerDistance || foundColliders[i].bounds.size.y < minimumObstacleHeight)
                {
                    foundColliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            //Sort the array of colliders, moving null objects to the end of the array
            System.Array.Sort(foundColliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                //Sample each collider for potential positions to hide by
                if (NavMesh.SamplePosition(foundColliders[i].transform.position, out NavMeshHit firstHit, 2f, navAgent.areaMask))
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
                        if (NavMesh.SamplePosition(foundColliders[i].transform.position - (Target.position - firstHit.position).normalized * 2, out NavMeshHit secondHit, 2f, navAgent.areaMask))
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
                    Debug.LogError($"Unable to find NavMesh near object {foundColliders[i].name} at {foundColliders[i].transform.position}");
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
            return Vector3.Distance(navAgent.transform.position, colliderA.transform.position).CompareTo(Vector3.Distance(navAgent.transform.position, colliderB.transform.position));
        }
    }

    private void OnDrawGizmos()
    {
        if(navAgent != null)
        {
            //Debug at target
            Gizmos.DrawSphere(navAgent.destination, 0.5f);

            //Obstacle awareness debug
            Gizmos.DrawWireSphere(gameObject.transform.position, enemyManager.enemyStats.obstacleAwarenessRange);

            //Debug around target
            Gizmos.DrawWireSphere(currentTarget.transform.position, minimumPlayerDistance);
        }
    }


    #region Attempted adaptation
    //Kept in for the current commit for documentation, to be removed in future
    private IEnumerator HidingLogic(Transform Target)
    {
        WaitForSeconds waitTime = new WaitForSeconds(updateFrequency);

        while (true)
        {
            //Clear old list of junk data
            validObstacles.Clear();

            //If available, output a navmeshagent for the target
            bool hasAgent = currentTarget.gameObject.TryGetComponent(out NavMeshAgent targetAgent);

            //Get all nearby obstacles for a hide check
            List<Collider> foundObstacles = Physics.OverlapSphere(navAgent.transform.position, enemyManager.enemyStats.obstacleAwarenessRange, hidableLayers).ToList();

            //Prioritize hiding spots that are farther away from the player by setting close by hiding points as null
            foreach (Collider obstacle in foundObstacles)
            {
                //Series of filters
                //Is within height restriction
                if (obstacle.bounds.size.y > minimumObstacleHeight)
                {
                    //Get distance to object from self
                    float selfDistance = ExtensionMethods.GetPathDistance(navAgent, obstacle.transform);

                    //Above minimum player distance, attempt to use NavMesh calculations first, otherwise use distance
                    if (hasAgent)
                    {
                        //Get distance to object from target
                        float targetDistance = ExtensionMethods.GetPathDistance(targetAgent, obstacle.transform);

                        //If not within min player navigation distance, but within self range, add to valid obstacles
                        if (targetDistance > minimumPlayerDistance && selfDistance < enemyManager.enemyStats.obstacleAwarenessRange)
                        {
                            validObstaclesDistance.Add(obstacle, selfDistance);
                        }
                    }
                    else
                    {
                        //If not within min player distance, but within self range, add to valid obstacles
                        if (Vector3.Distance(obstacle.transform.position, Target.position) > minimumPlayerDistance && selfDistance < enemyManager.enemyStats.obstacleAwarenessRange)
                        {
                            validObstaclesDistance.Add(obstacle, selfDistance);
                        }
                    }
                }
            }

            //Sort dictionary by distance
            validObstaclesDistance = validObstaclesDistance.OrderBy(key => key.Value).ToDictionary(t => t.Key, t => t.Value);
            validObstacles.Reverse();

            //Iterate over each obstacle to look for a suitable hiding spot
            //For now it selects the first one found to be valid
            foreach (Collider obstacle in validObstacles)
            {
                Debug.Log(obstacle.name);

                //Sample each collider for potential positions to hide by
                if (NavMesh.SamplePosition(obstacle.transform.position, out NavMeshHit firstHit, 2f, navAgent.areaMask))
                {
                    //Gets the closest edge that has a normal attached to it, this will be important in determining if the position points away or towards the target
                    if (!NavMesh.FindClosestEdge(firstHit.position, out firstHit, navAgent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {firstHit.position}");
                    }

                    //If the hiding spot is within the hiding sensitivity, set destination
                    if (Vector3.Dot(firstHit.normal, (Target.position - firstHit.position).normalized) < hideSensitivity)
                    {
                        navAgent.SetDestination(firstHit.position);
                        break;
                    }
                    //However, if the DOT product was greater than the sensitivity, it means the hiding spot was facing the target
                    else
                    {
                        //As a result, attempt to sample the other side of the object and check for validity
                        if (NavMesh.SamplePosition(obstacle.transform.position - (Target.position - firstHit.position).normalized * 2, out NavMeshHit secondHit, 2f, navAgent.areaMask))
                        {
                            //Gets the closest edge that has a normal attached to it
                            if (!NavMesh.FindClosestEdge(secondHit.position, out secondHit, navAgent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {secondHit.position} (second attempt)");
                            }

                            //If the direction appears valid and good according to the hide sensitivity, set the destination
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
                    Debug.LogError($"Unable to find NavMesh near object {obstacle.name} at {obstacle.transform.position}");
                }
            }

            yield return waitTime;
        }
    }
    #endregion
}
