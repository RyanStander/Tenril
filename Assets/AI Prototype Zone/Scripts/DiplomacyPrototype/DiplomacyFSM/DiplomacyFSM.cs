using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiplomacyFSM : MonoBehaviour
{
    //Reference to the starting and current state being executed
    [SerializeField] private DiplomacyAbstractStateFSM startingState;
    private DiplomacyAbstractStateFSM currentState;

    [SerializeField]
    private List<DiplomacyAbstractStateFSM> validStates;
    private Dictionary<DiplomacyFSMStateType, DiplomacyAbstractStateFSM> FSMStates;

    private void Awake()
    {
        //Set to null for awake
        currentState = null;

        //Set up dictionary
        FSMStates = new Dictionary<DiplomacyFSMStateType, DiplomacyAbstractStateFSM>();

        //Get the agents
        LeaderManager leaderManager = this.GetComponent<LeaderManager>();

        foreach (DiplomacyAbstractStateFSM state in validStates)
        {
            //Assign variables to the states
            state.SetExecutingFSM(this);
            state.SetExecutingManager(leaderManager);
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
    public void EnterState(DiplomacyAbstractStateFSM nextState)
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
    public void EnterState(DiplomacyFSMStateType givenType)
    {
        if (FSMStates.ContainsKey(givenType))
        {
            DiplomacyAbstractStateFSM nextState = FSMStates[givenType];

            //Exit current
            currentState.ExitState();

            //Set & enter new state
            EnterState(nextState);
        }
    }

    #endregion
}
