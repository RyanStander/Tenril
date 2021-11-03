using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this class is to serve as the framework for future states that may need preconditions
//Currently it only serves as a bridge from chasing and directly attacking
public class EvaluateCombatChoice : AbstractStateFSM
{
    //Hash to allow for quick changes
    private int forwardHash;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.EVALUATECOMBAT;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED EVALUATE COMBAT STATE");

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
            DebugLogString("UPDATING EVALUATE COMBAT STATE");
        }

        //Run based method
        base.UpdateState();

        //If no target exists, return to watch state
        if (enemyManager.currentTarget == null)
        {
            //Change to watch state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);

            //Return early
            return;
        }

        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isInteracting)
        {
            //Set to minimal forward movement while still performing action
            animatorManager.animator.SetFloat(forwardHash, 0, 0.1f, Time.deltaTime);

            //Allow for rotation towards the target for as long as the animator allows rotation
            if(animatorManager.animator.GetBool(animatorManager.canRotateHash))
            {
                RotateTowardsTargetPosition(enemyManager.currentTarget.transform.position, enemyManager.enemyStats.attackRotationSpeed);
            }

            //Return early
            return;
        }

        //Directly rotate towards the target
        RotateTowardsTargetPosition(enemyManager.currentTarget.transform.position, enemyManager.enemyStats.rotationSpeed);

        //Check if within attack range and has recovered since last animation
        if (enemyManager.currentRecoveryTime <= 0 && IsDirectlyWithinAttackRange())
        {
            //Change to attack state
            finiteStateMachine.EnterState(StateTypeFSM.ATTACK);
        }
        //If the target is out of attack range, return to chasing
        else if (!IsDirectlyWithinAttackRange())
        {
            //Change to chase state
            finiteStateMachine.EnterState(StateTypeFSM.CHASETARGET);
        }

        //Precisely correct the location of the NavmeshAgent
        CorrectAgentLocationPrecise();
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED EVALUATE COMBAT STATE");

        //Return true
        return true;
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

    //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
    private void CorrectAgentLocationPrecise()
    {
        //Set the navAgents predicted position to be the root transform
        enemyManager.navAgent.nextPosition = transform.root.position;
    }

    private bool IsDirectlyWithinAttackRange()
    {
        //If within attack range based on direct & unpathed distance, return true
        if (Vector3.Distance(enemyManager.currentTarget.transform.position, transform.root.position) <= enemyManager.enemyStats.maximumAttackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
