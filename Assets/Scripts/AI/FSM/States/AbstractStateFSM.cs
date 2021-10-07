using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractStateFSM : MonoBehaviour
{
    //Relevant attached managers and agent elements
    protected NavMeshAgent navAgent;
    protected EnemyAgentManager enemyManager;
    protected EnemyFSM finiteStateMachine;
    protected AnimatorManager animatorManager;

    //The current execution state of the state
    public ExecutionState executionState { get; protected set; }

    //The type of state this is
    public StateTypeFSM stateType { get; protected set; }

    //Bool to track if the state is entered
    public bool enteredState { get; protected set; }

    //Initialize state as disabled
    public virtual void OnEnable() { executionState = ExecutionState.NONE; }

    //Run when entering the state
    public virtual bool EnterState()
    {
        //Set the execution state
        executionState = ExecutionState.ACTIVE;

        //Track the success of entering the state
        bool hasNavAgent;
        bool hasNPCAgent;

        //Nullcheck
        hasNavAgent = (navAgent != null);
        hasNPCAgent = (enemyManager != null);

        return hasNavAgent && hasNPCAgent;
    }

    //Update the current state that is active in the state machine
    public abstract void UpdateState();

    //Run when exiting the state
    public virtual bool ExitState()
    { 
        executionState = ExecutionState.COMPLETED;
        return true; 
    }

    #region Setters
    public virtual void SetNavMeshAgent(NavMeshAgent givenAgent)
    {
        if(givenAgent != null)
        {
            navAgent = givenAgent;
        }
    }

    public virtual void SetExecutingManager(EnemyAgentManager givenManager)
    {
        if(givenManager != null)
        {
            enemyManager = givenManager;
        }
    }

    public virtual void SetExecutingAnimationManager(EnemyAnimatorManager givenManager)
    {
        if (givenManager != null)
        {
            animatorManager = givenManager;
        }
    }

    public virtual void SetExecutingFSM(EnemyFSM givenFSM)
    {
        if (givenFSM != null)
        {
            finiteStateMachine = givenFSM;
        }
    }
    #endregion End of Setters

    protected private void DebugLogString(string log)
    {
        //Debug the log if debugging is enabled
        if (finiteStateMachine.isDebuggingStates)
        {
            Debug.Log(log);
        }
    }
}
