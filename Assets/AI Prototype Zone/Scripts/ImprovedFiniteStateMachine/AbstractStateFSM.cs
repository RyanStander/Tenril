using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateFSM : ScriptableObject
{
    //The current execution state of the state
    public ExecutionState ExecutionState { get; protected set; }

    //Initialize state as disabled
    public virtual void OnEnable()
    {
        ExecutionState = ExecutionState.NONE;
    }

    //Run when entering the state
    public virtual bool EnterState() { ExecutionState = ExecutionState.ACTIVE; return true; }

    //Update the current state that is active in the state machine
    public abstract void UpdateState();

    //Run when exiting the state
    public virtual bool ExitState() { ExecutionState = ExecutionState.COMPLETED; return true; }

}
