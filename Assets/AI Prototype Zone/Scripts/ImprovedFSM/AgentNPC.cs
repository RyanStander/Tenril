using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FiniteStateMachine))]
public class AgentNPC : MonoBehaviour
{
    //Patrol plan
    public List<GameObject> patrolPoints = new List<GameObject>();

    //Agent for movement
    private NavMeshAgent navAgent;
    private FiniteStateMachine stateMachine;

    public void Awake()
    {
        //Get the attached navigation mesh agent
        navAgent = this.GetComponent<NavMeshAgent>();

        //Get the attached state machine
        stateMachine = this.GetComponent<FiniteStateMachine>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
