using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class DiplomacyAbstractStateFSM : ScriptableObject
{
    //The current manager
    protected LeaderManager leaderManager;
    protected DiplomacyFSM finiteStateMachine;

    //The current execution state of the state
    public ExecutionState executionState { get; protected set; }

    //The type of state this is
    public DiplomacyFSMStateType stateType { get; protected set; }

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
        return (leaderManager != null);
    }

    //Update the current state that is active in the state machine
    public abstract void UpdateState();

    //Run when exiting the state
    public virtual bool ExitState()
    {
        executionState = ExecutionState.COMPLETED;
        return true;
    }

    public virtual void SetExecutingManager(LeaderManager givenManager)
    {
        if (givenManager != null)
        {
            leaderManager = givenManager;
        }
    }

    public virtual void SetExecutingFSM(DiplomacyFSM givenFSM)
    {
        if (givenFSM != null)
        {
            finiteStateMachine = givenFSM;
        }
    }

    protected private void DebugLogString(string log)
    {
        //Ask the FSM to debug, which is allowed to reject it
        finiteStateMachine.DebugLogString(log);
    }
}