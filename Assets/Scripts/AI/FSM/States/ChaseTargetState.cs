using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class ChaseTargetState : AbstractStateFSM
{
    Vector3 worldDeltaPosition;
    Vector2 groundDeltaPosition;
    Vector2 velocity = Vector2.zero;

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

            //Disable parts of navmesh controls in order to allow for animation driven movement
            enemyManager.navAgent.updatePosition = false;
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING CHASE STATE");
        }

        //If no target exists, return to watch state
        if (enemyManager.currentTarget == null)
        {
            //Change to chase state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);
        }
        else
        {
            //Set the destination to the target
            enemyManager.navAgent.SetDestination(enemyManager.currentTarget.transform.position);
        }

        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isPerformingAction)
        {
            //Set to minimal forward movement while still performing action
            animatorManager.animator.SetFloat("Forward", 0, 0.1f, Time.deltaTime);
            return;
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


    private bool IsWithinAttackRange()
    {
        if (enemyManager.navAgent.remainingDistance <= enemyManager.enemyStats.maximumAttackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ClockworksGamesMethodAttempt()
    {
        //Calculate the world delta position using the next and current position
        worldDeltaPosition = enemyManager.navAgent.nextPosition - transform.root.position;

        //Conversion to forward and side motion
        groundDeltaPosition.x = Vector3.Dot(transform.right, worldDeltaPosition);
        groundDeltaPosition.y = Vector3.Dot(transform.forward, worldDeltaPosition);

        //Calculate an intended velocity from these deltas
        velocity = (Time.deltaTime > 1e-5f) ? groundDeltaPosition / Time.deltaTime : velocity = Vector2.zero;

        //Bool for if movement should occur
        bool shouldMove = velocity.magnitude > 0.025f && enemyManager.navAgent.remainingDistance < enemyManager.enemyStats.maximumAttackRange;

        //Based on previous calculations set the animatior parameters
        animatorManager.animator.SetBool("Moving", shouldMove);
        animatorManager.animator.SetFloat("Left", velocity.x);
        animatorManager.animator.SetFloat("Forward", velocity.y);

        Debug.Log(shouldMove);
        Debug.Log(velocity);
    }

    //private void OnAnimatorMove()
    //{
    //    transform.root.position = enemyManager.navAgent.nextPosition;
    //}


    private void RyansMethodAttempt()
    {
        //Calculate target distance
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        //If not yet within range, set the animation speed towards the target
        if (distanceFromTarget > enemyManager.enemyStats.maximumAttackRange)
        {
            animatorManager.animator.SetFloat("Forward", 1, 0.1f, Time.deltaTime);
        }

        //Handle the rotation logic for the AI
        RyansMethodAttemptHandleRotationToTarget();

        //Reset the navigation agent transforms
        enemyManager.navAgent.transform.localPosition = Vector3.zero;
        enemyManager.navAgent.transform.localRotation = Quaternion.identity;

        Debug.Log("Current distance from target: " + distanceFromTarget);

        //If within attack range, swap to the combat stance state
        if (distanceFromTarget <= enemyManager.enemyStats.maximumAttackRange)
        {
            //TODO: Change to combat stance state which still needs implementation
            //finiteStateMachine.EnterState(StateTypeFSM.);
            Debug.Log("Target reaced!");
        }
    }

    private void RyansMethodAttemptHandleRotationToTarget()
    {
        //Circumvents the navigation mesh rotation by directly rotating towards the target
        //Allows enemies to attack a target without worrying about obstructions
        if (enemyManager.isPerformingAction)
        {
            //Calculate the direction and normalize the vector
            Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            direction.y = 0;
            direction.Normalize();

            //Check for a direction of zero
            if (direction == Vector3.zero)
            {
                direction = enemyManager.transform.forward;
            }

            //Force the rotation by slerping towards it
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, Quaternion.LookRotation(direction), enemyManager.enemyStats.rotationSpeed / Time.deltaTime);
        }
        //Rotate with the built in navigation agent
        else
        {
            //Enable the navigation agent
            enemyManager.navAgent.enabled = true;

            //Set the destination to allow for automatic pathing
            enemyManager.navAgent.SetDestination(enemyManager.currentTarget.transform.position);

            //Set a premade velocity, circumvents agent settings
            enemyManager.rigidBody.velocity = enemyManager.rigidBody.velocity;
            
            //Slerp to the intended agent rotation using specified speed
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navAgent.transform.rotation, enemyManager.enemyStats.rotationSpeed / Time.deltaTime);
        }
    }
}
