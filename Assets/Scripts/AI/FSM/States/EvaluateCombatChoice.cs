using UnityEngine;

//The purpose of this class is to serve as the framework for future states that may need preconditions
//Currently it only serves as a bridge from chasing and directly attacking
public class EvaluateCombatChoice : AbstractStateFSM
{
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
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING EVALUATE COMBAT STATE");
        }

        //If no target exists, or the target is dead, return to watch state
        if (enemyManager.currentTarget == null || enemyManager.currentTarget.GetComponent<CharacterStats>().isDead)
        {
            //Change to watch state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);

            //Return early
            return;
        }

        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isInteracting)
        {
            movementManager.StopMovement(0.1f, Time.deltaTime);

            //Allow for rotation towards the target for as long as the animator allows rotation
            if(animatorManager.animator.GetBool(animatorManager.canRotateHash))
            {
                movementManager.RotateTowardsTargetPosition(enemyManager.currentTarget.transform.position, enemyManager.enemyStats.attackRotationSpeed);
            }

            //Return early
            return;
        }

        //Directly rotate towards the target
        movementManager.RotateTowardsTargetPosition(enemyManager.currentTarget.transform.position, enemyManager.enemyStats.rotationSpeed);

        //Check if within attack range and has recovered since last animation
        if (enemyManager.currentRecoveryTime <= 0 && IsDirectlyWithinRange(enemyManager.enemyStats.maximumAttackRange))
        {
            //Change to attack state
            finiteStateMachine.EnterState(StateTypeFSM.ATTACK);
        }

        //Check if healing is possible
        else if(enemyManager.ShouldTryHealing() && !IsDirectlyWithinRange(enemyManager.enemyStats.healRange))
        {
            //Attempt to heal
            finiteStateMachine.EnterState(StateTypeFSM.HEALING);
        }

        //If the target is out of attack range
        else if (!IsDirectlyWithinRange(enemyManager.enemyStats.maximumAttackRange))
        {
            //Return to chase state
            finiteStateMachine.EnterState(StateTypeFSM.CHASETARGET);
        }

        //Precisely correct the location of the NavmeshAgent
        movementManager.CorrectAgentLocationPrecise();
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

    private bool IsDirectlyWithinRange(float givenRange)
    {
        //If within attack range based on direct & unpathed distance, return true
        if (Vector3.Distance(enemyManager.currentTarget.transform.position, transform.root.position) <= givenRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
