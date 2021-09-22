using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FiniteStateMachine : MonoBehaviour
{
    //Reference to the starting and current state being executed
    [SerializeField] private AbstractStateFSM startingState;
    private AbstractStateFSM currentState;

    [SerializeField]
    private List<AbstractStateFSM> validStates;
    private Dictionary<FSMStateType, AbstractStateFSM> FSMStates;

    private void Awake()
    {
        //Set to null for awake
        currentState = null;

        //Set up dictionary
        FSMStates = new Dictionary<FSMStateType, AbstractStateFSM>();

        //Get the agents
        NavMeshAgent navAgent = this.GetComponent<NavMeshAgent>();
        AgentNPC npcAgent = this.GetComponent<AgentNPC>();

        foreach (AbstractStateFSM state in validStates)
        {
            //Assign variables to the states
            state.SetExecutingFSM(this);
            state.SetExecutingNPC(npcAgent);
            state.SetNavMeshAgent(navAgent);
            FSMStates.Add(state.stateType, state);
        }
    }

    private void Start()
    {
        //Null check
        if (startingState != null)
        {
            EnterState(startingState);
        }
    }

    private void Update()
    {
        //Null check
        if(currentState != null)
        {
            currentState.UpdateState();
        }
    }

    #region State Management

    //Enter the given state, exits the current one
    public void EnterState(AbstractStateFSM nextState)
    {
        //Null check to avoid errors
        if(nextState != null)
        {
            //Exit the old state
            if(currentState != null) 
            {
                currentState.ExitState();
            }

            //Set the new state
            currentState = nextState;

            //Enter the new state
            currentState.EnterState();
        }
        else
        {
            Debug.Log("Given state was null!");
        }
    }

    //Enter new state using its type
    public void EnterState(FSMStateType givenType)
    {
        if(FSMStates.ContainsKey(givenType))
        {
            AbstractStateFSM nextState = FSMStates[givenType];

            //Exit current
            currentState.ExitState();

            //Set & enter new state
            EnterState(nextState);
        }
    }

    #endregion
}
