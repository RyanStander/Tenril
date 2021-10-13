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
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING CHASE STATE");
        }

        //Handle rotation and forward movement
        HandleMovement();

        //Change states if within attacking range and if the calculated path is not pending
        if (IsWithinAttackRange() && !enemyManager.navAgent.pathPending)
        {
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED CHASE STATE");

        //Return true
        return true;
    }


    private void HandleMovement()
    {
        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isPerformingAction)
        {
            //Set to minimal forward movement while still performing action
            animatorManager.animator.SetFloat(forwardHash, 0, 0.1f, Time.deltaTime);
            return;
        }

        //If no target exists, return to watch state
        if (enemyManager.currentTarget == null)
        {
            //Change to watch state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);

            //Return early
            return;
        }
        else
        {
            //Set the destination to the target
            enemyManager.navAgent.SetDestination(enemyManager.currentTarget.transform.position);
        }

        //Suspend movement logic until the path is completely calcualted
        //Prevents problems with calculating remaining distance between the target and agent
        if (!enemyManager.navAgent.pathPending)
        {
            //Check if target is too far away, if so then return to watch state
            if(!IsWithinChaseRange())
            {
                //Change to watch state
                finiteStateMachine.EnterState(StateTypeFSM.WATCH);

                //Return early
                return;
            }

            //Rotate towards the next position that is gotten from the agent
            RotateTowardsNextPosition();

            //Handle the forward movement of the agent
            HandleForwardAnimationMovement();

            //Correct the location of the NavmeshAgent with precise or estimated calculations
            //Choice between precise or estimated
            if(hasPreciseAvoidance)
            {
                //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
                CorrectAgentLocationPrecise();
            }
            else
            {
                //Method preserves animation quality (reducing foot sliding) at the cost of obstacle avoidance
                CorrectAgentLocationEstimated();
            }
        }
    }

    private void HandleForwardAnimationMovement()
    {
        //If not yet within attack range, keep moving forward
        if (!IsWithinAttackRange())
        { 
            animatorManager.animator.SetFloat(forwardHash, 1f, 0.1f, Time.deltaTime);
        }
    }

    private void RotateTowardsTargetPosition(Vector3 target, float rotationSpeed)
    {
        //Get the direction
        Vector3 direction = (target - transform.root.position).normalized;

        //Calculate the look rotation
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Slerp towards what the nav agent wants the target
        transform.root.rotation = Quaternion.Slerp(transform.root.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void RotateTowardsNextPosition()
    {
        //Simulates the agents next steer target
        RotateTowardsTargetPosition(enemyManager.navAgent.steeringTarget, enemyManager.enemyStats.rotationSpeed);
    }

    //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
    private void CorrectAgentLocationPrecise()
    {
        //Set the navAgents predicted position to be the root transform
        enemyManager.navAgent.nextPosition = transform.root.position;
    }

    //Method preserves animation quality (reducing foot sliding) at the cost of obstacle avoidance
    private void CorrectAgentLocationEstimated()
    {
        //Get the world delta position in relation to the agent and the intended body to follow
        Vector3 worldDeltaPosition = enemyManager.navAgent.nextPosition - transform.root.position;

        //If not following within the radius, correct it
        if (worldDeltaPosition.magnitude > enemyManager.navAgent.radius)
        {
            //Set the next position
            enemyManager.navAgent.nextPosition = transform.root.position + 0.9f * worldDeltaPosition;
        }
    }

    private bool IsWithinAttackRange()
    {
        //If within attack range based on remaining NavMesh distance, return true
        if (enemyManager.navAgent.remainingDistance <= enemyManager.enemyStats.maximumAttackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsWithinChaseRange()
    {
        //If within attack range based on remaining NavMesh distance, return true
        if (enemyManager.navAgent.remainingDistance <= enemyManager.enemyStats.chaseRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
