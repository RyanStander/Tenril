using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepositioningUtilityMovementPrototype : MonoBehaviour
{
    //The current object of interest to the agent
    public GameObject objectOfInterest;

    //The speed of the utility agent
    public float agentSpeed = 1;
    public float rotationSpeed = 2;

    //Number of rays that should be used when deciding on a direction
    [Range(3, 100)] public int utilityRayCount = 15;

    //The additional range at which the utility ray should operate within
    public float adittionalRayRange = 1;

    //The current desired distance to achieve, this gets applied to the utility ray range
    public float desiredDistance = 2;

    //Range at which the utility ray should operate within
    private float utilityRayRange;

    //Weights that influence the decision making of encountered objects
    [Range(-1, 1)] public float obstacleWeight = -1;
    //[Range(-1, 1)] public float targetWeight = 1;
    [Range(-1, 1)] public float targetAngleWeight = 0.75f;

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

    //Masks for raycast blocking
    public LayerMask detectionBlockLayer = 1 << 9;

    //Saved distance to the object of interest
    private float distanceToObjectOfInterest;

    //Helper list for debugging hits with obstacles
    private Dictionary<Vector3, Color> hitPoints = new Dictionary<Vector3, Color>();

    //Helper bool for LOS
    private bool hasLOS = false;

    //Agent being used
    public NavMeshAgent navAgent;

    private void Start()
    {
        //Disable certain navAgent features
        navAgent.isStopped = false; //Prevents agent from using any given speeds by accident
        navAgent.updatePosition = false; //Disable agent forced position
        navAgent.updateRotation = false; //Disable agent forced rotation
    }

    #region Target Rotation
    internal void RotateTowardsTargetPosition(Vector3 target, float rotationSpeed)
    {
        //Get the direction
        Vector3 direction = (target - transform.position).normalized;

        //Calculate the look rotation
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Slerp towards what the nav agent wants the target
        transform.root.rotation = Quaternion.Lerp(transform.root.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    internal void RotateTowardsObjectOfInterest()
    {
        //Simulates the agents next steer target
        RotateTowardsTargetPosition(objectOfInterest.transform.position, rotationSpeed);
    }
    #endregion

    public void FixedUpdate()
    {
        //Establish the current distance to the target
        distanceToObjectOfInterest = DistanceToTarget();

        //Establish the range of the rays
        utilityRayRange = adittionalRayRange + desiredDistance;

        //Check for direct obstruction
        hasLOS = IsTargetUnobstructed(objectOfInterest.transform, transform, detectionBlockLayer);

        //Set the destination to calculate from to the object of interest (this creates a reference point)
        navAgent.SetDestination(objectOfInterest.transform.position);

        //Suspend movement logic until the path is completely calcualted
        //Prevents problems with calculating remaining distance between the target and agent
        if (!navAgent.pathPending)
        {
            //Generate rays based on utility and information about target
            GenerateUtilityRays();

            //Get and set the current direction to strive for and follow
            currentDirection = GetAverageUtilityDirection();

            //Rotate towards target (always 'circling')
            RotateTowardsObjectOfInterest();

            //Move towards the target direction
            ApplyDirectionalMovement();

            //Visual of the average direction
            Debug.DrawRay(transform.position, currentDirection * utilityRayRange, Color.magenta);

            //Debug a direct ray to the object of interest
            Vector3 rayOrigin = transform.position; rayOrigin.y *= 0.5f;
            Debug.DrawRay(rayOrigin, (objectOfInterest.transform.position - transform.position), Color.yellow);

            //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
            CorrectAgentLocationPrecise();
        }
    }

    private void ApplyDirectionalMovement()
    {
        //For now we're just making a direct translation
        //In future this should instead be a movement vector for the locomotion blend tree
        //transform.position += currentDirection * agentSpeed * Time.deltaTime;
        Vector3 target = transform.position + (currentDirection * agentSpeed);
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
    }

    private void GenerateUtilityRays()
    {
        //Clear the old utility rays
        utilityRays.Clear();

        //Clear the old hit points
        hitPoints.Clear();

        //Populate the utility ray list based on the number of rays 
        for (int i = 0; i <= utilityRayCount; i++)
        {
            utilityRays.Add(CalculateUtilityRay(i));
        }
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

        //Calculate the utility based on if it hits any obstacles
        if (Physics.Raycast(baseRay, out RaycastHit hit, utilityRayRange, detectionBlockLayer))
        {
            //Calculate a normalized distance between the object and the origin (0 to 1 inverted), the closer to (1) the object the more impactful it is
            float objectDistanceImpact = 1 - (hit.distance / utilityRayRange);

            //Calculate utility based on what is being found at current distance impactfulness
            utility = objectDistanceImpact * obstacleDistanceSensitivity * obstacleWeight;
        }

        //Sample the position at a quarter the ray distance to avoid non-walkable areas for the NavMesh agent
        if (NavMesh.SamplePosition(baseRay.GetPoint(utilityRayRange / 4), out NavMeshHit NavHit, 0.75f, navAgent.areaMask))
        {
            if(!hitPoints.ContainsKey(NavHit.position))
            {
                //Add a visual point
                hitPoints.Add(NavHit.position, Color.cyan);
            }
        }
        else
        {
            //Reduce viability of path
            utility -= 1;
        }

        //Calculate additional utility based on how small the angle is from the ray to the object of interest
        //float angleBetween = Vector3.Angle(objectOfInterest.transform.position - transform.position, rayDirection); //NavMesh separate
        float angleBetween = Vector3.Angle(navAgent.nextPosition - transform.position, rayDirection);

        //Angle impact calculated and inverted so that the smaller angles results in higher favorability
        //By default this value implies moving towards the target as favorable
        float angleImpact = (1 - (angleBetween / 180)) * targetAngleWeight;

        //If intending on moving away from the target
        if (distanceToObjectOfInterest <= desiredDistance)// && hasLOS)
        {
            //Invert the impact
            angleImpact *= -1;
        }

        //Calculate additional utility based on the smallest angles and divide by 2 so that it returns to a 0 to 1 scale
        utility = (utility + angleImpact) / 2;

        //Create color based on given gradient and utility score
        Color utilityColor = utilityGradient.Evaluate(utility);

        //Save the hit point for debugging later (if any detected)
        if (hit.collider != null &&  !hitPoints.ContainsKey(hit.point))
        {
            hitPoints.Add(hit.point, utilityColor);
        }

        //Debug visual of the ray
        if (isDebugging)
        {
            Debug.DrawRay(baseRay.origin, baseRay.direction * utilityRayRange, utilityColor);
        }

        //Return the utility ray calculated
        return new UtilityRay(baseRay, utility, utilityColor);
    }

    #region Rays, Direction and Distance methods
    private Vector3 GetAverageUtilityDirection()
    {
        //Return zero if no rays exist
        if (utilityRays == null || utilityRays.Count == 0) return Vector3.zero;

        //Temporary vector to track the average direction
        Vector3 averageDirection = Vector3.zero;

        //Iterate over each ray and add their direction based on utility
        foreach (UtilityRay utilityRay in utilityRays)
        {
            //Map the ray utility to get a correctly fractioned direction
            averageDirection += utilityRay.baseRay.direction.normalized * Mathf.InverseLerp(-1, 1, utilityRay.rayUtility);
        }

        //Return the average desired direction
        return averageDirection;
    }

    private UtilityRay GetBestUtilityRay()
    {
        //Return null if no rays exist
        if (utilityRays == null || utilityRays.Count == 0) return null;

        //Temporary declaration for best ray
        UtilityRay bestRay = utilityRays[0];

        //Iterate over 
        foreach (UtilityRay utilityRay in utilityRays)
        {
            //If the utility is greater, update the current best ray
            if (utilityRay.rayUtility > bestRay.rayUtility)
            {
                bestRay = utilityRay;
            }
        }

        //Return the best ray
        return bestRay;
    }

    internal float DistanceToTarget()
    {
        //Gets the direct distance to the target
        return Vector3.Distance(transform.position, objectOfInterest.transform.position);
    }
    #endregion

    //Checks for line of sight
    internal bool IsTargetUnobstructed(Transform targetPoint, Transform originPoint, LayerMask detectionBlockLayer)
    {
        //Raycast to first check if the target is valid (Performance friendly to raycast first), checks if any objects are in the way
        if (!Physics.Raycast(originPoint.position, targetPoint.transform.position - originPoint.position, out RaycastHit hit, Vector3.Distance(targetPoint.transform.position, originPoint.position), detectionBlockLayer))
        {
            //Debug the line results
            Debug.DrawRay(originPoint.position, targetPoint.transform.position - originPoint.position, Color.green);

            //Return as unobstructed
            return true;
        }
        else
        {
            //Debug the line results
            Debug.DrawRay(originPoint.position, hit.point - originPoint.position, Color.red);

            //Return as obstructed
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, desiredDistance);

        //Iterate over each point of impact for debugging
        foreach(KeyValuePair<Vector3, Color> point in hitPoints)
        {
            //Debug the points of impact
            Gizmos.color = point.Value;
            Gizmos.DrawSphere(point.Key, 0.1f);
        }
    }


    //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
    internal void CorrectAgentLocationPrecise()
    {
        //Set the navAgents predicted position to be the root transform
        navAgent.nextPosition = transform.root.position;
    }
}
