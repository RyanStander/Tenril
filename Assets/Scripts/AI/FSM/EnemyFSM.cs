using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyFSM : MonoBehaviour
{
    //Reference to the starting and current state being executed
    [SerializeField] private AbstractStateFSM startingState;
    private AbstractStateFSM currentState;

    [SerializeField]
    private List<AbstractStateFSM> validStates;
    private Dictionary<StateTypeFSM, AbstractStateFSM> FSMStates;

    //Bool for if FSM states should debug
    public bool isDebuggingStates = false;

    private void Awake()
    {
        //Set to null for awake
        currentState = null;

        //Set up dictionary
        FSMStates = new Dictionary<StateTypeFSM, AbstractStateFSM>();

        //Get the agents
        EnemyAgentManager agentManager = this.GetComponent<EnemyAgentManager>();
        EnemyAnimatorManager animationManager = this.GetComponent<EnemyAnimatorManager>();
        NavMeshAgent navAgent = this.GetComponent<NavMeshAgent>();

        //Apply relevant information internally to the states
        foreach (AbstractStateFSM state in validStates)
        {
            //Assign variables to the states
            state.SetExecutingFSM(this);
            state.SetNavMeshAgent(navAgent);
            state.SetExecutingManager(agentManager);
            state.SetExecutingAnimationManager(animationManager);
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
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    #region State Management
    //Enter the given state, exits the current one
    public void EnterState(AbstractStateFSM nextState)
    {
        //Null check to avoid errors
        if (nextState != null)
        {
            //Exit the old state
            if (currentState != null)
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
    public void EnterState(StateTypeFSM givenType)
    {
        if (FSMStates.ContainsKey(givenType))
        {
            AbstractStateFSM nextState = FSMStates[givenType];

            //Exit current
            currentState.ExitState();

            //Set & enter new state
            EnterState(nextState);
        }
    }

    //Get a specific state of given type
    public AbstractStateFSM GetState(StateTypeFSM givenType)
    {
        //Search for state and return if existing
        if (FSMStates.ContainsKey(givenType))
        {
            //Return state of type
            return FSMStates[givenType];
        }
        else
        {
            return null;
        }
    }
    #endregion
}