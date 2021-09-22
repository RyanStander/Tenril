using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractStateFSM : ScriptableObject
{
    //The current NavMeshAgent
    protected NavMeshAgent navAgent;
    protected AgentNPC npcAgent;
    protected FiniteStateMachine finiteStateMachine;

    //The current execution state of the state
    public ExecutionState executionState { get; protected set; }

    //The type of state this is
    public FSMStateType stateType { get; protected set; }

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
        bool hasNavAgent = true;
        bool hasNPCAgent = true;

        //Nullcheck
        hasNavAgent = (navAgent != null);
        hasNPCAgent = (npcAgent != null);

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

    public virtual void SetNavMeshAgent(NavMeshAgent givenAgent)
    {
        if(givenAgent != null)
        {
            navAgent = givenAgent;
        }
    }

    public virtual void SetExecutingNPC(AgentNPC givenNPC)
    {
        if(givenNPC != null)
        {
            npcAgent = givenNPC;
        }
    }

    public virtual void SetExecutingFSM(FiniteStateMachine givenFSM)
    {
        if (givenFSM != null)
        {
            finiteStateMachine = givenFSM;
        }
    }

}
