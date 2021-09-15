using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavigationDebugger : MonoBehaviour
{
    public NavMeshAgent debuggedNavigationAgent; //The agent that will be debugged
    public LineRenderer lineRendererPath; //Line that will be rendered for the path of the agent

    // Start is called before the first frame update
    void Start()
    {
        lineRendererPath = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(debuggedNavigationAgent.hasPath)
        {
            lineRendererPath.positionCount = debuggedNavigationAgent.path.corners.Length; //Set the number of points to debug
            lineRendererPath.SetPositions(debuggedNavigationAgent.path.corners);
            lineRendererPath.enabled = true;
        }
        else
        {
            lineRendererPath.enabled = false;
        }
    }
}
