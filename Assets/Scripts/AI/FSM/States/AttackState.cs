using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state is in charge of selecting an attack from a list of available attacks and then executing it
/// It does interact with the repositioning state in the event that an attack is not possible from a given distance
/// </summary>
public class AttackState : AbstractStateFSM
{
    //The current weapon that is being used
    private WeaponItem currentWeapon;

    //Helper bool for checking if an attack is ready to be processed
    private bool needsNewAttack = true;

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

            //Get the current weapon
            currentWeapon = enemyManager.inventory.equippedWeapon;
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
        //4. The attack has timed out
        //Although these are checked in performing the attack, a pre-emptive check saves on needing to look through valid attacks
        if (enemyManager.isInteracting || currentWeapon == null || enemyManager.currentRecoveryTime > 0)
        {
            //Change to evaluate state
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);

            //Return early
            return;
        }

        //Get a new attack if not having previously tried to reposition
        if(enemyManager.attackManager.shouldExecuteAttack)
        {
            PerformGivenAttack(enemyManager.attackManager.currentAttack);

            //Request a new attack
            needsNewAttack = true;
        }
        else if(needsNewAttack || enemyManager.attackManager.hasTimedOut)
        {
            //Set a new intended attack
            SetNewAttack();

            //Mark as no longer needing an attack
            needsNewAttack = false;
        }
        else if(enemyManager.attackManager.currentAttack == null)
        {
            DebugLogString("No valid attack could be found/set!");
        }
        else
        {
            finiteStateMachine.EnterState(StateTypeFSM.REPOSITIONING);
        }
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

    private void PerformGivenAttack(AttackData givenAttack)
    {
        //If the recovery time and allows for an attack and they are not performing an action
        if (enemyManager.currentRecoveryTime <= 0 && !enemyManager.isInteracting && givenAttack != null)
        {
            //Debug the attack being performed
            DebugLogString("Attack being performed: " + givenAttack.attackAnimation);
            //Debug.Log("Attack being performed: " + givenAttack.attackAnimation);

            //Stop locomotion velocity incase any is happening
            movementManager.StopMovementImmediate();
            
            //Set the spell to cast along if it exists
            enemyManager.spellcastingManager.spellBeingCast = givenAttack.spellCastWithAttack;

            //Play the target animation of the attack
            animatorManager.PlayTargetAnimation(givenAttack.attackAnimation, true);

            //Set the manager to believe they are performing an action
            enemyManager.isInteracting = true;

            //Set the manager into recovery
            enemyManager.currentRecoveryTime = givenAttack.recoveryTime;

            //Reset the validity of the previous attack being executed
            enemyManager.attackManager.shouldExecuteAttack = false;
        }
    }

    private void SetNewAttack()
    {
        //Temporary value
        AttackData potentialAttack;

        //Temporary helper bool
        var isHeavyAttack = Random.Range(0f, 1) < enemyManager.enemyStats.heavyAttackLikeliness;

        //Decide on if attempting a strong or weak attack
        //If less than or equal the heavy likeliness, then get a heavy attack
        potentialAttack = isHeavyAttack ? GetHeavyAttack() : GetLightAttack();

        //If attack is still unsuccessful, get a random one to reposition to
        if(potentialAttack == null)
        {
            potentialAttack = isHeavyAttack ? GetRandomHeavyAttack() : GetRandomLightAttack();

            //Track the attack
            enemyManager.attackManager.TrackAttackData(potentialAttack);
            //Debug.Log("Tracking " + potentialAttack.attackAnimation);
        }
        else
        {
            //Set current
            enemyManager.attackManager.SaveAttackData(potentialAttack);
            //Debug.Log("Saving " + potentialAttack.attackAnimation);
        }
    }

    #region Attack Getters
    private AttackData GetLightAttack()
    {
        return GetWeighedAttackFromList(currentWeapon.attackSet.lightAttacks);
    }

    private AttackData GetHeavyAttack()
    {
        return GetWeighedAttackFromList(currentWeapon.attackSet.heavyAttacks);
    }

    private AttackData GetRandomLightAttack()
    {
        return GetWeighedRandomAttackFromList(currentWeapon.attackSet.lightAttacks);
    }

    private AttackData GetRandomHeavyAttack()
    {
        return GetWeighedRandomAttackFromList(currentWeapon.attackSet.heavyAttacks);
    }

    private AttackData GetWeighedAttackFromList(List<AttackData> attackList)
    {
        //List of possible attacks based on range and angle
        var validAttacks = new List<AttackData>();

        //Iterate over each attack and check if it is possible
        foreach (var data in attackList)
        {
            //If the attack is possible based on angle and distance
            if (enemyManager.attackManager.IsAttackValid(data))
            {
                //Add to the list of valid attacks
                validAttacks.Add(data);
            }
        }

        //Get a random attack from this list of valid attacks based on likelihood
        var randomAttack = GetWeighedRandomAttackFromList(validAttacks);

        //If the attack is not null
        return randomAttack == null ? randomAttack : null;
    }

    private AttackData GetWeighedRandomAttackFromList(List<AttackData> attackList)
    {
        //Return null if no valid attacks are found
        if (attackList.Count <= 0) return null;
        
        //If the list of valid attacks has at least one attack, get based on weights
        //Temporary list to hold each attack weighed
        var weighedAttacks = new List<AttackData>();

        //Add each attack to the weighed attack
        foreach (var data in attackList)
        {
            //Add the attack a number of times equal to its weight
            for (var i = 0; i < data.attackWeight; i++)
            {
                //Add to the list
                weighedAttacks.Add(data);
            }
        }

        //Return a random attack from this weighed list
        return weighedAttacks[Random.Range(0, weighedAttacks.Count)];
    }
    #endregion
}