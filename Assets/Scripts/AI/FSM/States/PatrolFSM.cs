using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class PatrolFSM : AbstractStateFSM
{
    public Queue<GameObject> patrolPoints = new Queue<GameObject>();
    public GameObject currentTargetPoint;
    public float pointSensitivity = 2f;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.PATROL;
    }

    public override bool EnterState()
    {
        enteredState = false;

        //Run based method 
        if (base.EnterState())
        {
            //Grab patrol points
            //foreach(GameObject point in enemyManager.patrolPoints)
            //{
            //    patrolPoints.Enqueue(point);
            //}

            //Fail if no points given
            if(patrolPoints.Count == 0 || patrolPoints == null)
            {
                Debug.LogError("Failed to grab patrol points from the NPC");
            }

            //Set the closest target point
            SetClosestTargetPoint();

            //Nullcheck for entered state
            if (currentTargetPoint != null)
            {
                enteredState = true;

                //Debug message
                Debug.Log("ENTERED PATROL STATE");
            }
        }

        //Return
        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            Debug.Log("UPDATING PATROL STATE");

            //If remaining distance
            if (navAgent.remainingDistance <= pointSensitivity)
            {
                CycleTargetQueue();

                //finiteStateMachine.EnterState(FSMStateType.PATROL);
            }
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        Debug.Log("EXITED PATROL STATE");

        //Return true
        return true;
    }


    private void SetClosestTargetPoint()
    {
        //Iterate through the queue until the closest target is found
        GameObject closestPatrolPoint = null;
        float closestDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();

        //Find the closest point
        foreach (GameObject point in patrolPoints.ToList())
        {
            //Ignore if null
            if (point == null) { continue; }

            //Calculate distance compared to previous
            if (navAgent.CalculatePath(point.transform.position, path))
            {
                float distance = Vector3.Distance(navAgent.transform.position, path.corners[0]);

                //Loop over remaining corners
                for(int num = 1; num < path.corners.Length; num++)
                {
                    distance += Vector3.Distance(path.corners[num-1], path.corners[num]);
                }

                //Compare distance
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPatrolPoint = point;
                }
            }
        }

        //Reorganize queue to the closest point
        foreach (GameObject point in patrolPoints.ToList())
        {
            //Nullcheck
            if(closestPatrolPoint == null)
            {
                Debug.Log("No closeby patrol point found");
                break;
            }

            //If the point is the closest, stop organizing
            if(point == closestPatrolPoint)
            {
                break;
            }
            else
            {
                //Push to the end of the queue
                GameObject pushedPoint = patrolPoints.Dequeue();
                patrolPoints.Enqueue(pushedPoint);
            }
        }

        //Cycle the queue and target
        CycleTargetQueue();
    }

    private void CycleTargetQueue()
    {
        //Set the target
        currentTargetPoint = patrolPoints.Dequeue();
        patrolPoints.Enqueue(currentTargetPoint);

        //Set the navmesh target
        navAgent.SetDestination(currentTargetPoint.transform.position);
    }
}
