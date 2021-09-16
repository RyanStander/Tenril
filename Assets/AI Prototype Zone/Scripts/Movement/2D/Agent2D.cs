using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent2D : MonoBehaviour
{
    [SerializeField] protected Transform target;
    private protected NavMeshAgent agent;

    // Start is called before the first frame update
    public virtual void Start()
    {
        //Get the attached agent
        agent = GetComponent<NavMeshAgent>();

        //Disable forced rotation
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        SetDestination();
    }

    public virtual void SetDestination()
    {
        agent.SetDestination(target.position);
    }

    public void StopMovement()
    {
        agent.isStopped = true;
    }

    public void StartMovement()
    {
        agent.isStopped = false;
    }
}
