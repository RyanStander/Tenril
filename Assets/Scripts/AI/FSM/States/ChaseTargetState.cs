using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class ChaseTargetState : AbstractStateFSM
{
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

        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isPerformingAction)
        {
            //Set to minimal forward movement while still performing action
            animatorManager.animator.SetFloat("Forward", 0, 0.1f, Time.deltaTime);
            return;
        }

        //Calculate target distance
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        //If not yet within range, set the animation speed towards the target
        if (distanceFromTarget > enemyManager.enemyStats.maximumAttackRange)
        {
            animatorManager.animator.SetFloat("Forward", 1, 0.1f, Time.deltaTime);
        }

        //Handle the rotation logic for the AI
        HandleRotationToTarget();

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

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED CHASE STATE");

        //Return true
        return true;
    }

    private void HandleRotationToTarget()
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
