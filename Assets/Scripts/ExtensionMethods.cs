using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ExtensionMethods
{
    //A more costly version of NavMeshAgents get remaining distance function, but works at any range and avoids the invinity 'bug'
    public static float GetRemainingPathDistance(NavMeshAgent givenAgent)
    {
        //Temporary array to hold path corners
        Vector3[] pathCorners = givenAgent.path.corners;

        //Series of checks to ensure a path can/should have its distance calculated
        if (givenAgent.pathPending || givenAgent.pathStatus == NavMeshPathStatus.PathInvalid || pathCorners.Length == 0)
        {
            //Return negative infinity as this should be seen as invalid
            return Mathf.NegativeInfinity;
        }

        //Temporary float to track accumalated distance
        float remainingDistance = 0;

        //Iterate over each corner and calculate the remaining distance
        for (int i = 0; i < pathCorners.Length - 1; ++i)
        {
            remainingDistance += Vector3.Distance(pathCorners[i], pathCorners[i + 1]);
        }

        //Return the distance
        return remainingDistance;
    }
}
