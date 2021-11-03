using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class ChaseTargetState : AbstractStateFSM
{
    //Hash to allow for quick changes
    private int forwardHash;

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

            //Have root motion be applied
            animatorManager.animator.applyRootMotion = true;

            //Assign the hash from the animator
            forwardHash = animatorManager.forwardHash;

            //Disable certain navAgent features
            enemyManager.navAgent.isStopped = false; //Prevents agent from using any given speeds by accident
            enemyManager.navAgent.updatePosition = false; //Disable agent forced position
            enemyManager.navAgent.updateRotation = false; //Disable agent forced rotation

            //Reset NavMeshAgent location in case of beginning off place
            movementManager.SynchronizeHeight();
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

        //Set movement to zero to prevent accidental chase overlap
        movementManager.SetForwardMovement(0, 0.5f, Time.deltaTime);

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
            movementManager.SetForwardMovement(0, 0.1f, Time.deltaTime);
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
            movementManager.HandleStandardTargetMovement(enemyManager.enemyStats.chaseSpeed);
        }
    }

    #region Range Booleans
    private bool IsWithinGivenRange(float givenRange)
    {
        //If within the given range based on remaining NavMesh distance, return true
        if (enemyManager.navAgent.remainingDistance <= givenRange)
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
        //If within attack range based on remaining NavMesh distance, return true
        return IsWithinGivenRange(enemyManager.enemyStats.maximumAttackRange);
    }

    private bool IsWithinChaseRange()
    {
        //If within attack range based on remaining NavMesh distance, return true
        return IsWithinGivenRange(enemyManager.enemyStats.chaseRange);
    }
    #endregion
}
