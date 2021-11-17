using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerClickMove : MonoBehaviour
{
    private NavMeshAgent navAgent;
    public Camera usedCamera;
    public LayerMask floorLayer;

    private void Start()
    {
        //Get the agent component
        navAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //If pressing, raycast
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            //Check for any collisions from the ray
            if (Physics.Raycast(usedCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, float.MaxValue, floorLayer))
            {
                //Set destination to the collision point
                navAgent.SetDestination(hit.point);
            }
        }
    }
}