using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HidingMovement : MonoBehaviour
{
    //The object to avoid
    public Transform targetObject;

    //The layers that are valid for hiding behind
    public LayerMask hidableLayers;

    //The sight detection logic
    public HidingLineOfSightDetector lineOfSightDetector;

    //The agent of the 
    public NavMeshAgent navAgent;

    //How sensitive the agent is to hiding, lower means better hiding
    [Range(-1, 1)] public float hideSensitivity = 0;

    //Distance that the target must be within before it matters
    [Range(1, 10)] public float MinPlayerDistance = 5f;

    //The minimum obstacle height needed 
    [Range(0, 5f)] public float MinObstacleHeight = 1.25f;

    //The frequency at which calculations should be made
    [Range(0.01f, 1f)] public float updateFrequency = 0.25f;

    //Courotine for movement logic
    private Coroutine movementCourotine;

    //Colliders, up to given limit (more is performance heavy, but gives more options), that are found are potential hiding targets
    private Collider[] foundColliders = new Collider[10];

    private void Awake()
    {
        //Get the agent
        navAgent = GetComponent<NavMeshAgent>();

        //Subscribe to the events of the line of sight script
        lineOfSightDetector.OnGainSight += HandleGainSight;
        lineOfSightDetector.OnLoseSight += HandleLoseSight;
    }

    private void HandleGainSight(Transform givenTarget)
    {
        //If movement is ongoing, stop it
        if (movementCourotine != null)
        {
            StopCoroutine(movementCourotine);
        }

        //Set the new target to avoid
        targetObject = givenTarget;

        //Begin logic to hide from target
        movementCourotine = StartCoroutine(Hide(givenTarget));
    }

    private void HandleLoseSight(Transform Target)
    {
        //Stop movement
        if (movementCourotine != null)
        {
            StopCoroutine(movementCourotine);
        }

        //Reset the target object
        targetObject = null;
    }

    private IEnumerator Hide(Transform Target)
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
            int hits = Physics.OverlapSphereNonAlloc(navAgent.transform.position, lineOfSightDetector.visionCollider.radius, foundColliders, hidableLayers);

            //Prioritize hiding spots that are farther away from the player by setting close by hiding points as null
            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                //If within min player distance oor obstacle height is not sufficient, set to null
                if (Vector3.Distance(foundColliders[i].transform.position, Target.position) < MinPlayerDistance || foundColliders[i].bounds.size.y < MinObstacleHeight)
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

    public int ColliderArraySortComparer(Collider A, Collider B)
    {
        //Compares given colliders to eachother and returns a given position for the array based on how it should be sorted
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(navAgent.transform.position, A.transform.position).CompareTo(Vector3.Distance(navAgent.transform.position, B.transform.position));
        }
    }
}
