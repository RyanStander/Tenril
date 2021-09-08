using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    //Current state of the object attached
    private State currentState; 

    // Update is called once per frame
    void Update()
    {
        RunStateMachine();
    }

    //Runs through the state machine
    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();

        //Switch to the next state if not null
        if(nextState != null)
        {
            SwitchToState(nextState);
        }

    }

    //Switches to the next intended state
    private void SwitchToState(State givenState)
    {
        currentState = givenState;
    }
}
