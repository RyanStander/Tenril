using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ExtensionMethods
{
    #region NavigationMesh related
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

        //Return the distance
        return GetPathDistance(pathCorners);
    }

    //Functions near identically to GetRemainingPathDistance()
    //However this accounts for when the agent is processing off link "corners" that are not included in the agents path corners
    public static float GetRemainingPathDistanceOffLinkFriendly(NavMeshAgent givenAgent)
    {
        //Begin using the remaining distance
        float remainingDistance = GetRemainingPathDistance(givenAgent);

        //Checks to ensure a path can/should have its distance calculated
        if (remainingDistance == Mathf.NegativeInfinity)
        {
            //Return negative infinity as this should be seen as invalid
            return Mathf.NegativeInfinity;
        }

        //Add on the off mesh position data (distance returns 0 if none currently active)
        remainingDistance += Vector3.Distance(givenAgent.currentOffMeshLinkData.startPos, givenAgent.currentOffMeshLinkData.endPos);

        //Return the distance
        return remainingDistance;
    }

    public static float GetPathDistance(Vector3[] pathCorners)
    {
        //Temporary float to track accumalated distance
        float remainingDistance = 0;

        //Iterate over each corner and calculate the remaining distance
        for (int i = 0; i < pathCorners.Length - 1; ++i)
        {
            remainingDistance += Vector3.Distance(pathCorners[i], pathCorners[i + 1]);
        }

        return remainingDistance;
    }

    public static float GetPathDistance(NavMeshPath givenPath)
    {
        return GetPathDistance(givenPath.corners);
    }

    public static float GetPathDistance(NavMeshAgent givenAgent, Transform destination)
    {
        //Initialize a new path
        NavMeshPath path = new NavMeshPath();

        //Calculate the path
        givenAgent.CalculatePath(destination.position, path);

        //Calculate the distance
        return GetPathDistance(path);
    }
    #endregion

    #region General Helper Methods
    //Find a child gameobject of a given parent based on a desired tag
    public static GameObject FindChildWithTag(GameObject searchedParent, string desiredTag)
    {
        //Iterate over each transform inside the parent until a child of the desired tag is found
        foreach (Transform child in searchedParent.transform)
        {
            //Check if the child tag matches the desired tag
            if (child.CompareTag(desiredTag))
            {
                //Return with the valid child
                return child.gameObject;
            }
        }

        //Return null if none were found
        return null;
    }
    #endregion

    public static float MapToNewRange(float oldMinimum, float oldMaximum, float newMinimum, float newMaximum, float oldValue)
    {
        //Calculate the old range
        float oldRange = (oldMaximum - oldMinimum);

        //Calculate the new range
        float newRange = (newMaximum - newMinimum);

        //Scale the old value to match the new value range
        return (((oldValue - oldMinimum) * newRange) / oldRange) + newMinimum;
    }
}
