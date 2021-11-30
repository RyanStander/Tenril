using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshVisualDebugger : MonoBehaviour
{
    //The agent being debugged
    public NavMeshAgent navAgent;

    //The color to set the debugged lines and positions
    public Color pathColor = Color.red;
    public Color nextPositionColor = Color.green;
    public Color offLinkPathColor = Color.cyan;
    public Color offLinkStartColor = Color.magenta;
    public Color offLinkEndColor = Color.blue;
    [Range(0, 0.5f)] public float positionRadius = 0.15f;

    private void Start()
    {
        //Get the navMeshAgent if none was selected
        if(navAgent == null)
        {
            navAgent = gameObject.GetComponentInChildren<NavMeshAgent>();
        }
    }

    private void OnDrawGizmos()
    {
        //Return if no navAgent was provided or if the game is not running
        if (navAgent == null || !Application.isPlaying) return;

        //Temporary holder for the path
        Vector3[] pathCorners = navAgent.path.corners;

        //Iterate over the NavMeshAgent path corners and visually debug it
        for (int i = 0; i < pathCorners.Length - 1; i++)
        {
            Debug.DrawLine(pathCorners[i], pathCorners[i + 1], pathColor);
        }

        //If currently procesing an off link path, display a visual
        if(navAgent.currentOffMeshLinkData.activated)
        {
            //Start of current link
            Gizmos.color = offLinkStartColor;
            Gizmos.DrawSphere(navAgent.currentOffMeshLinkData.startPos, positionRadius);

            //End of current link
            Gizmos.color = offLinkEndColor;
            Gizmos.DrawSphere(navAgent.currentOffMeshLinkData.endPos, positionRadius);

            //Debug a line between these points
            Debug.DrawLine(navAgent.currentOffMeshLinkData.startPos, navAgent.currentOffMeshLinkData.endPos, offLinkPathColor);
        }

        //The next expected position for the agent, slightly scaled up
        Gizmos.color = nextPositionColor;
        Gizmos.DrawWireSphere(pathCorners[0], positionRadius);
    }
}
