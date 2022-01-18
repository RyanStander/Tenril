using UnityEngine;

public class ChaseTargetState : AbstractStateFSM
{
    //Bool to toggle between higher quality animations or better obstacle avoidance
    public bool hasPreciseAvoidance = true;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.CHASETARGET;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED CHASE STATE");

            //Reset NavMeshAgent location in case of beginning off place
            movementManager.SynchronizeTransformToAnimation();
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING CHASE STATE");
        }

        //Handle rotation and forward movement and state swapping
        HandleChaseLogic();
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Reset NavMeshAgent location in case of beginning off place
        movementManager.SynchronizeTransformToAnimation();

        //Debug message
        DebugLogString("EXITED CHASE STATE");

        //Return true
        return true;
    }

    private void HandleChaseLogic()
    {
        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isInteracting)
        {
            //Set to minimal forward movement while the action is being performed
            movementManager.StopMovement(0.1f, Time.deltaTime);

            //Return early
            return;
        }

        //If no target exists, return to the watch state
        if (enemyManager.currentTarget == null)
        {
            //Change to watch state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);

            //Return early
            return;
        }

        //Set the destination of the navigation agent to the target
        navAgent.SetDestination(enemyManager.currentTarget.transform.position);

        //Check if healing is possible
        if (enemyManager.ShouldTryHealing())
        {
            //Attempt to heal
            finiteStateMachine.EnterState(StateTypeFSM.HEALING);
        }

        //If the calculated path is not pending, conduct range checks
        if(!navAgent.pathPending)
        {
            //Change states if within attacking range
            if (IsWithinAttackRange())
            {
                //Change to evaluation state
                finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);

                //Return early
                return;
            }
            //Check if target is too far away
            else if (!IsWithinChaseRange())
            {
                //Change to watch state
                finiteStateMachine.EnterState(StateTypeFSM.WATCH);

                //Return early
                return;
            }

            //Execute standard movement & rotation towards the target
            movementManager.HandleNavMeshTargetMovement(enemyManager.enemyStats.chaseSpeed);
        }
    }

    #region Range Booleans
    private bool IsWithinGivenRange(float givenRange)
    {
        //Temporary flaot to track distance
        float remainingDistance = enemyManager.navAgent.remainingDistance;

        //Check for the infinity 'bug', if detected then manually calculate the distance
        if (remainingDistance == Mathf.Infinity)
        {
            remainingDistance = ExtensionMethods.GetRemainingPathDistanceOffLinkFriendly(navAgent);
        }

        //If within the given range based on remaining NavMesh distance, return true
        if (remainingDistance <= givenRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsWithinGivenHeight(float givenHeight)
    {
        //Temporary flaot to track difference
        float heightDifference = transform.position.y - navAgent.destination.y;

        //If within the given range based on differing height, return true
        if (heightDifference <= givenHeight && heightDifference >= -givenHeight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsWithinGivenRangeAndHeight(float givenRange, float givenHeight)
    {
        //If within the given ranges, return true
        if (IsWithinGivenRange(givenRange) && IsWithinGivenHeight(givenHeight))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsWithinAttackRange()
    {
        //If within attack range based on remaining NavMesh distance and height, return true
        //The height check should only temporarilly use this enemy stats value as ranged weapons will need to use a different system
        return IsWithinGivenRangeAndHeight(enemyManager.enemyStats.maximumAttackRange, enemyManager.enemyStats.maximumAttackHeight);
    }

    private bool IsWithinChaseRange()
    {
        //If within attack range based on remaining NavMesh distance, return true
        return IsWithinGivenRange(enemyManager.attackManager.currentChasingRange);
    }
    #endregion
}
