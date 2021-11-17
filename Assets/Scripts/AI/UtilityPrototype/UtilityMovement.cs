using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;

public class UtilityMovement : MonoBehaviour
{
    //The current object of interest to the agent
    public GameObject objectOfInterest;

    //Agent being used
    public NavMeshAgent navAgent;

    //The speed of the utility agent
    public float agentSpeed = 1;

    //Number of rays that should be used when deciding on a direction
    [Range(3,100)] public int utilityRayCount;

    //Range at which the utility ray should operate within
    public int utilityRayRange;

    //Weights that influence the decision making of encountered objects
    [Range(-1, 1)] public float obstacleWeight = -1;
    [Range(-1, 1)] public float targetWeight = 1;
    [Range(-1, 1)] public float targetAngleWeight = 1;

    //Sensitivity affects navigation through the distance between the agent and other objects
    [Range(0, 1)] public float obstacleDistanceSensitivity = 1;
    [Range(0, 1)] public float targetDistanceSensitivity = 1;

    //Modifies from where the raycast is done, it starts at the center of the agent as an origin point
    [Range(0, 2)] public float heightMultiplier = 0.5f;

    //Color gradient used for ray desirability measurement
    public Gradient utilityGradient;

    //List of utility rays in the current frame or update cycle
    private List<UtilityRay> utilityRays = new List<UtilityRay>();

    //The current direction being followed
    private Vector3 currentDirection = Vector3.zero;

    //Bool to help with if the rays should be debugged
    public bool isDebugging = true;

    private void Start()
    {
        //Disable certain navAgent features
        navAgent.isStopped = false; //Prevents agent from using any given speeds by accident
        navAgent.updatePosition = false; //Disable agent forced position
        navAgent.updateRotation = false; //Disable agent forced rotation

        //Set the destination to the target
        navAgent.SetDestination(objectOfInterest.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Generate rays based on utility and information about target
        GenerateUtilityRays();

        //Suspend movement logic until the path is completely calcualted
        //Prevents problems with calculating remaining distance between the target and agent
        if (!IsPathPending())
        {
            //Set the current direction being followed
            currentDirection = GetAverageDirectionExponential().normalized;

            //Movement
            Movement(currentDirection);

            //Visual of the average direction
            Debug.DrawRay(transform.position, currentDirection * utilityRayRange, Color.white);

            //Debug the looking direction
            Debug.DrawRay(transform.position, transform.forward * utilityRayRange, Color.magenta);

            //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
            CorrectAgentLocationPrecise();
        }
    }

    private void Movement(Vector3 currentDirection)
    {
        //Calculate the look rotation
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(currentDirection.x, 0, currentDirection.z));

        //Slerp towards what the nav agent wants the target
        transform.root.rotation = Quaternion.Lerp(transform.root.rotation, lookRotation, Time.deltaTime * 5);

        //Move target in average ray direction
        transform.position += transform.forward * agentSpeed * Time.deltaTime;
    }

    private void GenerateUtilityRays()
    {
        //Clear the old utility rays
        utilityRays.Clear();

        //Populate the utility ray list based on the number of rays 
        for (int i = 0; i <= utilityRayCount; i++)
        {
            utilityRays.Add(CalculateUtilityRay(i));
        }
    }

    private UtilityRay GetBestUtilityRay()
    {
        //Return null if no rays exist
        if (utilityRays == null || utilityRays.Count == 0) return null;

        //Temporary declaration for best ray
        UtilityRay bestRay = utilityRays[0];

        //Iterate over 
        foreach(UtilityRay utilityRay in utilityRays)
        {
            //If the utility is greater, update the current best ray
            if(utilityRay.rayUtility > bestRay.rayUtility)
            {
                bestRay = utilityRay;
            }
        }

        //Return the best ray
        return bestRay;
    }

    private UtilityRay CalculateUtilityRay(int rayNumber)
    {
        //Get the current point that is being used by the NavMeshAgent
        Vector3 simulatedPoint = navAgent.nextPosition;

        //Origin of the ray
        Vector3 rayOrigin = transform.position;
        rayOrigin.y *= 0.5f;

        //Target angle of the ray based on the current number, converted from radians to angles
        float rayAngle = (2 * Mathf.PI * ((float)rayNumber / (float)utilityRayCount)) * Mathf.Rad2Deg;
        //if(isDebugging) Debug.Log(rayNumber + " | " + rayAngle);

        //Calculate the direction of the ray based on the angle
        Vector3 rayDirection = Quaternion.AngleAxis(rayAngle, transform.up) * transform.forward;

        //Create a ray of this origin and direction
        Ray baseRay = new Ray(rayOrigin, rayDirection);

        //Declare temporary value to track utility
        float utility = 0;

        //Calculate the utility based on what it hits and the favorability of how direct it is to the target/steeringPoint
        if (Physics.Raycast(baseRay, out RaycastHit hit, utilityRayRange))
        {
            //Calculate a normalized distance between the object and the origin (0 to 1 inverted), the closer to (1) the object the more impactful it is
            float objectDistanceImpact = 1 - (hit.distance / utilityRayRange);

            //Calculate utility based on what is being found and current distance impactfulness
            if (hit.transform.gameObject == objectOfInterest)
            {
                utility = objectDistanceImpact * targetDistanceSensitivity * targetWeight;
            }
            else
            {
                utility = objectDistanceImpact * obstacleDistanceSensitivity * obstacleWeight;
            }
        }

        //Direct ray to the object
        Debug.DrawRay(rayOrigin, (objectOfInterest.transform.position - transform.position), Color.cyan);

        //Direct ray to the current simulated agent point
        Debug.DrawRay(rayOrigin, (simulatedPoint - transform.position), Color.blue);

        //Calculate additional utility based on how small the angle is from the ray to the simulated agent point
        float angleBetween = Vector3.Angle(simulatedPoint - transform.position, rayDirection);

        //Angle impact calculated and inverted so that the smaller angles results in higher favorability
        float angleImpact = (1 - (angleBetween / 180)) * targetAngleWeight;

        //Calculate additional utility based on the smallest anglest and divide by 2 so that it returns to a 0 to 1 scale
        utility = (utility + angleImpact) / 2;

        //Create color based on given gradient and utility score
        Color utilityColor = utilityGradient.Evaluate(utility);

        //Debug visual of the ray
        if (isDebugging)
        {
            Debug.DrawRay(baseRay.origin, baseRay.direction * utilityRayRange, utilityColor);
        }

        //Return the utility ray calculated
        return new UtilityRay(baseRay, utility, utilityColor);
    }

    private Vector3 GetAverageDirectionExponential()
    {
        //Return zero if no rays exist
        if (utilityRays == null || utilityRays.Count == 0) return Vector3.zero;

        //Temporary vector to track the average direction
        Vector3 averageDirection = Vector3.zero;

        //Iterate over each ray and add their direction based on utility
        foreach (UtilityRay utilityRay in utilityRays)
        {
            averageDirection += utilityRay.baseRay.direction.normalized * Mathf.Pow(utilityRay.rayUtility, 5);
        }

        //Return the average desired direction
        return averageDirection;
    }

    internal bool IsPathPending()
    {
        //Quick bool return for if the agent is still calculating a path
        return navAgent.pathPending;
    }

    //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
    internal void CorrectAgentLocationPrecise()
    {
        //Set the navAgents predicted position to be the root transform
        navAgent.nextPosition = transform.root.position;
    }
}
