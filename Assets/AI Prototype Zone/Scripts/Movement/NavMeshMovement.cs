using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : AbstractMovement
{
    [SerializeField]
    private NavMeshAgent navMeshAgent; //The navigation agent involved in movement

    public override void MoveTowardsTarget(CharacterAttributes attributes, Vector3 targetPosition)
    {
        navMeshAgent.isStopped = false;

        //Set the destination for the agent
        navMeshAgent.SetDestination(targetPosition);
    }

    public override void StopMovement()
    {
        navMeshAgent.isStopped = true;
    }
}
