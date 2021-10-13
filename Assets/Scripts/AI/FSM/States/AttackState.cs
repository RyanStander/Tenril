using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AbstractStateFSM
{
    //The current weapon that is being used
    private WeaponItem currentWeapon;

    //The current weapon attack to be executed
    internal string currentAttack;

    //The previous weapon attack
    internal string previousAttack;

    //The general angle of view at which attacking is valid
    [Range(5,45)]
    public int attackAngle = 35;

    //The  time needed for recovery
    [Range(1, 3)]
    public float recoveryTime = 2;

    //Hashes to allow for quick changes
    private int forwardHash;
    private int leftHash;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.ATTACK;
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

            //Assign the hashes from the animator
            forwardHash = animatorManager.forwardHash;
            leftHash = animatorManager.leftHash;

            //Disable certain navAgent features
            enemyManager.navAgent.isStopped = false; //Prevents agent from using any given speeds by accident
            enemyManager.navAgent.updatePosition = false; //Disable agent forced position
            enemyManager.navAgent.updateRotation = false; //Disable agent forced rotation

            //Get the current weapon
            currentWeapon = enemyManager.inventory.equippedWeapon;
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING EVALUATE COMBAT STATE");
        }

        //If no target exists, return to watch state
        if (enemyManager.currentTarget == null)
        {
            //Change to watch state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);

            //Return early
            return;
        }

        //Return and do not run any more methods until the current action/animation is completed or if no weapon exists
        if (enemyManager.isPerformingAction || currentWeapon == null)
        {
            //Change to evaluate state
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);

            //Return early
            return;
        }

        //Get a random attack, for now we will only test with weak attacks
        SetRandomWeakAttack();

        //If current attack is not null, perform the attack
        if (currentAttack != null)
        {
            PerformCurrentAttack();
        }

        //Return to evaluate state
        finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
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

    private void PerformCurrentAttack()
    {
        float viewableAngle = Vector3.Angle(enemyManager.currentTarget.transform.position - enemyManager.transform.position, enemyManager.transform.forward);

        //If close enough to attack and within the viewable angle, attack
        if(IsDirectlyWithinAttackRange() && viewableAngle <= attackAngle && viewableAngle >= -attackAngle)
        {
            //If the recovery time and allows for an attack and they are not performing an action
            if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
            {
                //Stop locomotion velocity incase any is happening
                animatorManager.animator.SetFloat(forwardHash, 0, 0.1f, Time.deltaTime);
                animatorManager.animator.SetFloat(leftHash, 0, 0.1f, Time.deltaTime);

                //Play the target animation of the attack
                animatorManager.PlayTargetAnimation(currentAttack, true);

                //Set the manager to believe they are performing an action
                enemyManager.isPerformingAction = true;

                //Set the manager into recovery
                enemyManager.currentRecoveryTime = recoveryTime;

                //Reset the current attack
                ResetCurrentAttack();

                //Return to evaluate state
                finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
            }
        }
    }

    private void ResetCurrentAttack()
    {
        //Set the current attack to null after saving it as the previous attack
        previousAttack = currentAttack;
        currentAttack = null;
    }

    private void SetRandomWeakAttack()
    {
        //Null check for if weapon or list is null/empty
        if (currentWeapon == null || currentWeapon.weakAttacks.Count < 1) { currentAttack = null; return; }

        //Return a random attack from the list
        currentAttack = currentWeapon.weakAttacks[Random.Range(0, currentWeapon.weakAttacks.Count)];
    }

    private void SetRandomStrongAttack()
    {
        //Null check for if weapon or list is null/empty
        if (currentWeapon == null || currentWeapon.strongAttacks.Count < 1) { currentAttack = null; return; }

        //Return a random attack from the list
        currentAttack = currentWeapon.strongAttacks[Random.Range(0, currentWeapon.strongAttacks.Count)];
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