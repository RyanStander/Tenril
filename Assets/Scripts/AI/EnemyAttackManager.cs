using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose of this class is to track basic information related to an enemies current attack logic
/// Includes tracking desired distance, the current attack and an attack timeout for when they should reconsider an attack
/// </summary>
public class EnemyAttackManager : MonoBehaviour
{
    //Relevant attached manager
    protected EnemyAgentManager enemyManager;

    //Current time to wait before reassesing the available attacks
    [Range(1, 5)] public float timeoutTime = 2f;
    private float timeoutTimer = 0;
    public bool hasTimedOut = true;

    //Bool for if attack can now be succesfully executed
    public bool shouldExecuteAttack = false;

    //Current attack trying to be performed
    public AttackData currentAttack;

    //The previous weapon attack
    internal AttackData previousAttack;

    //The current desired distance
    public float desiredDistance = 0;

    //The range at which attacking should begin, set based on current weapon
    public float currentMaximumAttackRange = 0;

    //Additional offset to prevent exiting combat
    [Range(0,1)] public float attackRangeAdditionalOffset = 0.5f;

    //The range of max and minimum ranges for the attack, used for when ideal distance is not reached, but the attack can still be made
    public Vector2 distanceBoundaries = new Vector2();

    //The offset amount at which the desired distance is considered to have been reached
    [Range(0.05f, 0.5f)] public float repositioningOffset = 0.1f;

    //The current alertness and chasing range to be followed and its targets
    [HideInInspector] public float currentAlertnessRange, currentChasingRange;
    private float targetAlertnessRange, targetChasingRange;

    //Rate at which the radius should return to normal
    [Range(0,5)] public float alertChaseChangeRate = 1;

    private void Awake()
    {
        //Getter for relevant reference
        enemyManager = GetComponentInChildren<EnemyAgentManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyManager == null) throw new MissingComponentException("Missing EnemyAgentManager on " + gameObject + "!");

        //Set the starting alertness and chasing ranges
        currentAlertnessRange = enemyManager.enemyStats.alertRadius;
        currentChasingRange = enemyManager.enemyStats.chaseRange;

        //Set the target ranges
        targetAlertnessRange = enemyManager.enemyStats.alertRadius;
        targetChasingRange = enemyManager.enemyStats.chaseRange;

        //Update the maximum attack range
        UpdateAttackRange();
    }

    public void UpdateAttackRange()
    {
        //Get the maximum attack range based on the current weapon equipt
        currentMaximumAttackRange = enemyManager.inventory.equippedWeapon.attackSet.GetMaximumAttackRange();
    }


    //Called by states to reset the attack management process
    public void TrackAttackData(AttackData givenAttack)
    {
        //Reset the previously tracked attack
        ResetCurrentAttack();

        //Save the current attack being tracked
        currentAttack = givenAttack;

        //Calculate the current desired distance based on the average 
        desiredDistance = givenAttack.mostDesirableDistance;

        //Update the distance boundaries
        distanceBoundaries = new Vector2(givenAttack.minimumDistanceNeededToAttack, givenAttack.maximumDistanceNeededToAttack);

        //Reset the timeout timer and boolean
        timeoutTimer = timeoutTime;
        hasTimedOut = false;

        //Reset the validity of the current attack
        shouldExecuteAttack = false;
    }

    //Called by states to reset the attack management process
    public void SaveAttackData(AttackData givenAttack)
    {
        //Reset the previously tracked attack
        ResetCurrentAttack();

        //Save the current attack being tracked
        currentAttack = givenAttack;

        //Calculate the current desired distance based on the average 
        desiredDistance = givenAttack.mostDesirableDistance;

        //Update the distance boundaries
        distanceBoundaries = new Vector2(givenAttack.minimumDistanceNeededToAttack, givenAttack.maximumDistanceNeededToAttack);

        //Reset the timeout timer and boolean
        timeoutTimer = timeoutTime;
        hasTimedOut = false;

        //Reset the validity of the current attack
        shouldExecuteAttack = true;
    }

    public void ResetCurrentAttack()
    {
        //Set the current attack to null after saving it as the previous attack
        previousAttack = currentAttack;
        currentAttack = null;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        AttackTimeoutClock();
        EaseTowardsTargetRadiuses();
    }

    private void AttackTimeoutClock()
    {
        //Countdown the attack timeout
        if(timeoutTimer > 0)
        {
            //Decrease by time
            timeoutTimer -= Time.deltaTime;

            //Mark as still running
            hasTimedOut = false;
        }
        else
        {
            //Mark as timed out
            hasTimedOut = true;
        }
    }

    #region Chasing and alertness radius related
    private void EaseTowardsTargetRadiuses()
    {
        //Reduce by time multiplied by rate of change
        currentAlertnessRange -= alertChaseChangeRate * Time.deltaTime;
        currentChasingRange -= alertChaseChangeRate * Time.deltaTime;

        //Clamp so it doesnt go beyond its minimum
        currentAlertnessRange = Mathf.Max(currentAlertnessRange, targetAlertnessRange);
        currentChasingRange = Mathf.Max(currentChasingRange, targetChasingRange);
    }

    public void HightenAlertChaseRadiuses()
    {
        //Set radiuses to maximum
        currentAlertnessRange = enemyManager.enemyStats.maximumAlertRadius;
        currentChasingRange = enemyManager.enemyStats.maximumChaseRange;
    }

    public void AlertAlliesOfAttack()
    {
        //Get nearby allies within range
        Dictionary<GameObject, float> allies = enemyManager.visionManager.GetListOfVisibleExpectedTargets(gameObject.transform, enemyManager.enemyStats.informAlliesRadius, enemyManager.enemyStats.assignedFaction, true);

        //Iterate over all allies in the list
        foreach(KeyValuePair<GameObject, float> ally in allies)
        {
            //Cast them (If possible) to enemy managers
            if (ally.Key.TryGetComponent(out EnemyAgentManager manager))
            {
                //Highten their radiuses
                manager.attackManager.HightenAlertChaseRadiuses();
            }
        }
    }

    public void AlertAlliesOfTarget(GameObject foundTarget)
    {
        //Get nearby allies within range
        Dictionary<GameObject, float> allies = enemyManager.visionManager.GetListOfVisibleExpectedTargets(gameObject.transform, enemyManager.enemyStats.informAlliesRadius, enemyManager.enemyStats.assignedFaction, true);

        //Iterate over all allies in the list
        foreach (KeyValuePair<GameObject, float> ally in allies)
        {
            //Cast them (If possible) to enemy managers
            if (ally.Key.TryGetComponent(out EnemyAgentManager manager))
            {
                //Get the current state and if they are waiting for a target, give them the target
                if(manager.stateMachine.GetCurrentState() is WatchState watchState)
                {
                    watchState.AllyFoundTarget(foundTarget);
                }
            }
        }
    }
    #endregion

    public bool IsWithinDesiredOffset(float currentDistance)
    { 
        //Check if within upper and lower bounds of the offset
        if (currentDistance > desiredDistance - repositioningOffset && currentDistance < desiredDistance + repositioningOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWithinBoundaries(float currentDistance)
    {
        //Check if within upper and lower bounds of the available range
        if (currentDistance >= distanceBoundaries.x && currentDistance <= distanceBoundaries.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Valid Attack Checking
    //Bool to check if an attack is possible based on its attack data
    public bool IsAttackValid(AttackData givenData)
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

    //Bool to check if the current attack is valid
    public bool IsCurrentAttackValid()
    {
        //If close enough to attack and is within the viewable angle
        if (IsWithinAttackRangeData(currentAttack) && IsWithinAttackViewData(currentAttack))
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

    private void OnDrawGizmosSelected()
    {
        //Return if needed items are not available yet
        if (enemyManager == null || enemyManager.enemyStats == null || enemyManager.visionManager == null) return;

        //Debug the sphere of view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyManager.visionManager.pointOfVision.position, currentAlertnessRange);

        //Debug the sphere of chasing
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemyManager.visionManager.pointOfVision.position, currentChasingRange);

        //Debug the sphere of alerting allies
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(enemyManager.visionManager.pointOfVision.position, enemyManager.enemyStats.informAlliesRadius);
    }
}
