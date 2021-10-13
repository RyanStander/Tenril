using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AbstractStateFSM
{
    //The current weapon that is being used
    private WeaponItem currentWeapon;

    //The current weapon attack to be executed
    internal AttackData currentAttack;

    //The previous weapon attack
    internal AttackData previousAttack;

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
            DebugLogString("ENTERED ATTACK STATE");

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

            //Reduce the forward speed
            animatorManager.animator.SetFloat(forwardHash, 0.5f, 0.1f, Time.deltaTime);
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING ATTACK STATE");
        }

        //If no target exists, return to watch state
        if (enemyManager.currentTarget == null)
        {
            //Change to watch state
            finiteStateMachine.EnterState(StateTypeFSM.WATCH);

            //Return early
            return;
        }

        //Return and do not run any more methods if...
        //1. The current action/animation is not completed
        //2. No weapon exists
        //3. The creature is still in recovery time
        //Although these are checked in performing the attack, a pre-emptive check saves on needing to look through valid attacks
        if (enemyManager.isPerformingAction || currentWeapon == null || enemyManager.currentRecoveryTime > 0)
        {
            //Change to evaluate state
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);

            //Return early
            return;
        }

        //Reset the current attack
        ResetCurrentAttack();

        //Set a new intended attack
        SetNewAttack();

        //Attempt to perform the attack
        if(currentAttack != null)
        {
            PerformCurrentAttack();
        }
        else
        {
            DebugLogString("No valid attack could be found/set!");
            //In future this should return to evaulate combat and the character should try to reposition for an attack
        }

        //Return to evaluate state
        finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED ATTACK STATE");

        //Return true
        return true;
    }

    private void PerformCurrentAttack()
    {
        //If the recovery time and allows for an attack and they are not performing an action
        if (enemyManager.currentRecoveryTime <= 0 && !enemyManager.isPerformingAction && currentAttack != null)
        {
            //Debug the attack being performed
            DebugLogString("Attack being performed: " + currentAttack.attackAnimation);

            //Stop locomotion velocity incase any is happening
            animatorManager.animator.SetFloat(forwardHash, 0, 0.1f, Time.deltaTime);
            animatorManager.animator.SetFloat(leftHash, 0, 0.1f, Time.deltaTime);

            //Play the target animation of the attack
            animatorManager.PlayTargetAnimation(currentAttack.attackAnimation, true);

            //Set the manager to believe they are performing an action
            enemyManager.isPerformingAction = true;

            //Set the manager into recovery
            enemyManager.currentRecoveryTime = recoveryTime;
        }
    }

    private void ResetCurrentAttack()
    {
        //Set the current attack to null after saving it as the previous attack
        previousAttack = currentAttack;
        currentAttack = null;
    }

    private void SetNewAttack()
    {
        //Helper bool for if one set of attacks is invalid
        bool isHeavyAttacking = false;

        //Decide on if attempting a strong or weak attack
        //If less than or equal the heavy likeliness, then get a heavy attack
        if(Random.Range(0,1) < enemyManager.enemyStats.heavyAttackLikeliness)
        {
            //Try and get a heavy attack
            currentAttack = GetHeavyAttack();
            isHeavyAttacking = true;
        }
        else
        {
            currentAttack = GetLightAttack();
        }

        //If getting was unsuccesful, try with the other set of attacks
        if(currentAttack == null && isHeavyAttacking)
        {
            //Try getting a light attack
            currentAttack = GetLightAttack();
        }
        else if(currentAttack == null && !isHeavyAttacking)
        {
            //Try getting a heavy attack
            currentAttack = GetHeavyAttack();
        }
    }

    #region Attack Getters
    private AttackData GetLightAttack()
    {
        Debug.Log("Trying to get heavy attack...");
        return GetAttackFromList(currentWeapon.attackSet.lightAttacks);
    }

    private AttackData GetHeavyAttack()
    {
        Debug.Log("Trying to get light attack...");
        return GetAttackFromList(currentWeapon.attackSet.heavyAttacks);
    }

    private AttackData GetAttackFromList(List<AttackData> attackList)
    {
        //List of possible attacks based on range and angle
        List<AttackData> validAttacks = new List<AttackData>();

        //Iterate over each attack and check if it is possible
        foreach (AttackData data in attackList)
        {
            //If the attack is possible based on angle and distance
            if (IsAttackValid(data))
            {
                //Add to the list of valid attacks
                validAttacks.Add(data);
            }
        }

        //If the list of valid attacks has at least one attack, get based on weights
        if (validAttacks.Count > 0)
        {
            //Temporary list to hold each attack weighed
            List<AttackData> weighedAttacks = new List<AttackData>();

            //Add each valid attack to the weighed attack
            foreach (AttackData data in validAttacks)
            {
                //Add the attack a number of times equal to its weight
                for (int i = 0; i < data.attackWeight; i++)
                {
                    //Add to the list
                    weighedAttacks.Add(data);
                }
            }

            //Return a random attack from this weighed list
            return weighedAttacks[Random.Range(0, weighedAttacks.Count)];
        }
        else
        {
            //Return null if no valid attacks are found
            return null;
        }
    }
    #endregion

    #region Valid Attack Checking
    //Bool to check if an attack is possible based on its attack data
    private bool IsAttackValid(AttackData givenData)
    {
        //If close enough to attack and is within the viewable angle
        if (IsWithinAttackRangeData(givenData) && IsWithinAttackViewData(givenData))
        {
            return true;
        }
        else
        {
            return false;
        }
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

    private bool IsWithinAttackRange(float minimumDistance, float maximumDistance)
    {
        //Calculate the direct distance to the target
        float distance = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.root.position);

        //If within the minimum and maximum range
        if (distance >= minimumDistance && distance <= maximumDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsWithinAttackRangeData(AttackData givenData)
    {
        return IsWithinAttackRange(givenData.minimumDistanceNeededToAttack, givenData.maximumDistanceNeededToAttack);
    }

    private bool IsWithinAttackView(float givenAngle)
    {
        //Calculate the current viewable angle
        float viewableAngle = Vector3.Angle(enemyManager.currentTarget.transform.position - enemyManager.transform.position, enemyManager.transform.forward);

        //Check if calculated angle is within the attacks view angle
        if (viewableAngle <= givenAngle && viewableAngle >= -givenAngle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool IsWithinAttackViewData(AttackData givenData)
    {
        return IsWithinAttackView(givenData.attackAngle);
    }
    #endregion
}