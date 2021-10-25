using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UtilityMovement : MonoBehaviour
{
    //The current desired object the agent should move towards
    public GameObject desiredObject;

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

    //Bool to help with if the rays should be debugged
    public bool isDebugging = true;

    // Update is called once per frame
    void Update()
    {
        GenerateUtilityRays();

        //TODO: Move & Rotation
    }

    private void GenerateUtilityRays()
    {
        //Clear the old utility rays
        utilityRays.Clear();

        //Populate the utility ray list based on the number of rays 
        for (int i = 0; i <= utilityRayCount; i++)
        {
            //utilityRays.Add(CalculateUtilityRay(i));
        }

        RotatableRay();
    }

    private UtilityRay CalculateUtilityRay(int rayNumber)
    {
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

        //Calculate the utility based on what it hits and the favorability of how direct it is to the target
        if (Physics.Raycast(baseRay, out RaycastHit hit, utilityRayRange))
        {
            //Calculate a normalized distance between the object and the origin (0 to 1 inverted), the closer to (1) the object the more impactful it is
            float objectDistanceImpact = 1 - (hit.distance / utilityRayRange);

            //Calculate utility based on what is being found and current distance impactfulness
            if (hit.transform.gameObject == desiredObject)
            {
                utility = objectDistanceImpact * targetDistanceSensitivity * targetWeight;
            }
            else
            {
                utility = objectDistanceImpact * obstacleDistanceSensitivity * obstacleWeight;
            }
        }

        //Direct ray to the object
        Debug.DrawRay(rayOrigin, (desiredObject.transform.position - transform.position), Color.cyan);

        //Calculate additional utility based on how small the angle is from the ray to the target
        float angleBetween = Vector3.Angle(desiredObject.transform.position - transform.position, rayDirection);

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
        return new UtilityRay(baseRay, 0, utilityColor);
    }

    //Initial method used for testing raycasts outwards from an angle, shows a single ray at a time
    //Kept this method in the current commit for documentation purposes, will be removed in the next
    [Range(0, 360)] public float currentAngle = 0;
    private void RotatableRay()
    {
        //Origin of the ray
        Vector3 rayOrigin = transform.position;
        rayOrigin.y *= 0.25f;

        //Calculate the inteded ray direction
        Vector3 rayDirection = Quaternion.AngleAxis(currentAngle, transform.up) * transform.forward;

        Ray baseRay = new Ray(rayOrigin, rayDirection);

        ////Raycast from the origin to direction
        //RaycastHit hit;
        //if (Physics.Raycast(rayOrigin, rayDirection, out hit, utilityRayRange))
        //{
        //    //Debug information
        //    Debug.Log("Hit " + hit.collider.gameObject.name);
        //    Debug.DrawRay(rayOrigin, rayDirection * utilityRayRange, Color.red);
        //}
        //else
        //{
        //    Debug.DrawRay(rayOrigin, rayDirection * utilityRayRange, Color.green);
        //}

        float utility = 0;

        //Calculate the utility based on what it hits and the favorability of how direct it is to the target
        if (Physics.Raycast(baseRay, out RaycastHit hit, utilityRayRange))
        {
            //Calculate a normalized distance between the object and the origin (0 to 1 inverted), the closer to (1) the object the more impactful it is
            float objectDistanceImpact = 1 - (hit.distance / utilityRayRange);

            //Calculate utility based on what is being found and current distance impactfulness
            if (hit.transform.gameObject == desiredObject)
            {
                utility = objectDistanceImpact * targetDistanceSensitivity * targetWeight;
            }
            else
            {
                utility = objectDistanceImpact * obstacleDistanceSensitivity * obstacleWeight;
            }
        }

        Debug.Log("Utility Pre-Targetting: "+ utility);

        //Direct ray to the object
        Debug.DrawRay(rayOrigin, (desiredObject.transform.position - transform.position), Color.cyan);

        //Calculate additional utility based on how small the angle is from the ray to the target
        float angleBetween = Vector3.Angle(desiredObject.transform.position - transform.position, rayDirection);

        //Angle impact calculated and inverted so that the smaller angles results in higher favorability
        float angleImpact = 1 - (angleBetween / 180) * targetAngleWeight;

        //Calculate additional utility based on the smallest anglest and divide by 2 so that it returns to a 0 to 1 scale
        utility = (utility + angleImpact) / 2;

        Debug.Log("Utility Post-Targetting: " + utility);

        //Create color based on given gradient and utility score
        Color utilityColor = utilityGradient.Evaluate(utility);

        //Debug visual of the ray
        if (isDebugging)
        {
            Debug.DrawRay(baseRay.origin, baseRay.direction * utilityRayRange, utilityColor);
        }
    }
}
