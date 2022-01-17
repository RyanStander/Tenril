using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// State is in charge of handling the repositioning logic of an AI
/// This means an AI will be able to maintain a desired distance from a given target, while continuing to look at them
/// Intended to function with the movement manager by feeding it a given global direction to travel towards
/// </summary>
public class RepositionState : AbstractStateFSM
{
    [Header("Repositioning Utility Movement")]
    //Number of rays that should be used when deciding on a direction
    [Range(3, 100)] public int utilityRayCount = 15;

    //The ratio (out of the number of rays) for how impactful the best ray should be when 
    [Range(0, 1)] public float bestRayImpactRatio = 0.25f;

    //The offset amount at which the object is considered to have reached its target
    [Range(0, 0.5f)] public float repositioningOffset = 0.1f;

    //The additional range at which the utility ray should operate within
    public float adittionalRayRange = 1;

    //The current desired distance to achieve, this gets applied to the utility ray range
    private float desiredDistance = 2;

    //Range at which the utility ray should operate within
    private float utilityRayRange;

    //Obstacles that should be filtered out when detecting an object to weigh
    public ObstacleType filteredObstacles = ObstacleType.stair | ObstacleType.floor | ObstacleType.terrain;

    //Weights that influence the decision making of encountered objects
    [Range(-1, 1)] public float obstacleWeight = -1;

    //Weights that influence the decision making of encountered other characters
    [Range(-1, 1)] public float characterWeight = -0.5f;

    //[Range(-1, 1)] public float targetWeight = 1;
    [Range(-1, 1)] public float targetAngleWeight = 0.75f;

    //Sensitivity affects how important the distance between the agent and other objects is
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
    public bool isDebugging = true; //TO BE RENAMED

    //Masks for raycast blocking
    public LayerMask detectionBlockLayer = 1 << 9;

    //Saved distance to the object of interest
    private float distanceToObjectOfInterest;

    //Helper list for debugging hits with obstacles
    private Dictionary<Vector3, Color> hitPoints = new Dictionary<Vector3, Color>();

    //Avoidance 
    private ObstacleAvoidanceType defaultAvoidance;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.REPOSITIONING;
    }

    public void Start()
    {
        //Set the default avoidance to match the NavAgent avoidance
        defaultAvoidance = navAgent.obstacleAvoidanceType;
}

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED REPOSITIONING STATE");

            //Disable the obstacle avoidance (improves repositioning movement)
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        return enteredState;
    }


    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING REPOSITIONING STATE");
            
            //Set the destination to calculate from to the object of interest (this creates a reference point)
            navAgent.SetDestination(enemyManager.currentTarget.transform.position);

            //Set the desired distance to the current attack managers desired distance
            desiredDistance = enemyManager.attackManager.desiredDistance;

            //Establish the range of the rays
            utilityRayRange = adittionalRayRange + desiredDistance;

            //Save the distance to the object of interest to prevent repeating distance calculations
            distanceToObjectOfInterest = ExtensionMethods.GetRemainingPathDistance(enemyManager.navAgent);

            //Suspend movement logic until the path is completely calcualted
            //Prevents problems with calculating remaining distance between the target and agent
            if (!navAgent.pathPending)
            {
                //Generate rays based on utility and information about target
                GenerateUtilityRays();

                //Get and set the current direction to strive for and follow
                currentDirection = GetAverageUtilityDirectionWithPreference();

                //Move towards the target direction by passing through information to the movement manager
                enemyManager.movementManager.HandleGlobalDirectionalMovement(currentDirection, enemyManager.enemyStats.repositionSpeed, isWithinDesiredOffset());

                //Debug a direct ray to the object of interest
                Vector3 rayOrigin = transform.root.position; rayOrigin.y += 0.5f;
                Debug.DrawRay(rayOrigin, (enemyManager.currentTarget.transform.position - transform.root.position), Color.yellow);

                //Visual of the average direction
                Debug.DrawRay(rayOrigin, currentDirection * utilityRayRange, Color.magenta);
            }
            else
            {
                enemyManager.movementManager.StopMovement();
            }

            //If the desired position is reached, execute the attack
            if (isWithinDesiredOffset())
            {
                //Mark as completed repositioning and execute the attack
                enemyManager.attackManager.shouldExecuteAttack = true;
                finiteStateMachine.EnterState(StateTypeFSM.ATTACK);
            }

            //If attempts to reach the desired distance for an attack timed out
            if (enemyManager.attackManager.hasTimedOut)
            {
                //Check if the attack is still possible from this position
                if (enemyManager.attackManager.IsCurrentAttackValid())
                {
                    //Mark as completed repositioning and execute the attack
                    enemyManager.attackManager.shouldExecuteAttack = true;

                    //Return to the evaluation state
                    finiteStateMachine.EnterState(StateTypeFSM.ATTACK);
                }
                //Otherwise return to evaluation
                else
                {
                    enemyManager.attackManager.shouldExecuteAttack = false;

                    //Return to the evaluation state
                    finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
                }

            }
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED REPOSITIONING STATE");

        //Disable the obstacle avoidance (improves repositioning movement)
        navAgent.obstacleAvoidanceType = defaultAvoidance;

        hitPoints.Clear();

        //Return true
        return true;
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
        Vector3 rayOrigin = transform.root.position;
        rayOrigin.y += 0.5f;

        //Target angle of the ray based on the current number, converted from radians to angles
        float rayAngle = (2 * Mathf.PI * ((float)rayNumber / (float)utilityRayCount)) * Mathf.Rad2Deg;
        //if(isDebugging) Debug.Log(rayNumber + " | " + rayAngle);

        //Calculate the direction of the ray based on the angle
        Vector3 rayDirection = Quaternion.AngleAxis(rayAngle, transform.root.up) * transform.root.forward;

        //Create a ray of this origin and direction
        Ray baseRay = new Ray(rayOrigin, rayDirection);

        //Declare temporary value to track utility
        float utility = 0;

        //Calculate the utility based on if it hits any obstacles
        if (Physics.Raycast(baseRay, out RaycastHit hit, utilityRayRange, detectionBlockLayer))
        {
            //Declare the weight
            float temporaryWeight = 0;

            //Check if the object should be filtered out because its a character
            if (hit.transform.TryGetComponent(out CharacterStats stats))
            {
                //Apply utility calculation based on 
                temporaryWeight = characterWeight;
            }
            //Check if the object should be filtered out based on obstacle data
            else if (hit.transform.TryGetComponent(out ObstacleData data))
            {
                //If it should not be filtered, apply impact
                if(!filteredObstacles.HasFlag(data.obstacleType))
                {
                    temporaryWeight = obstacleWeight;
                }
            }
            //Otherwise assume it should impact anyways
            else
            {
                temporaryWeight = obstacleWeight;
            }

            //Calculate a normalized distance between the object and the origin (0 to 1 inverted), the closer to (1) the object the more impactful it is
            float objectDistanceImpact = 1 - (hit.distance / utilityRayRange);
            
            //Calculate utility based on what is being found at current distance impactfulness
            utility = objectDistanceImpact * obstacleDistanceSensitivity * temporaryWeight;
        }

        //Point to sample
        Vector3 sampledPosition = baseRay.GetPoint(utilityRayRange / 3);

        //Sample the position at a third the ray distance to avoid non-walkable areas for the NavMesh agent
        if (NavMesh.SamplePosition(sampledPosition, out NavMeshHit NavHit, 1, navAgent.areaMask))
        {
            //Check if the exact point (excluding height) was found
            if(NavHit.position.x != sampledPosition.x || NavHit.position.z != sampledPosition.z)
            {
                //Reduce viability of path
                utility -= 0.5f;

                if (!hitPoints.ContainsKey(NavHit.position))
                {
                    //Add a visual point
                    hitPoints.Add(NavHit.position, Color.red);
                }
            }
            else if (!hitPoints.ContainsKey(NavHit.position))
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
        float angleBetween = Vector3.Angle(navAgent.nextPosition - transform.root.position, rayDirection);

        //Angle impact calculated and inverted so that the smaller angles results in higher favorability
        //By default this value implies moving towards the target as favorable
        float angleImpact = (1 - (angleBetween / 180)) * targetAngleWeight;

        //If intending on moving away from the target
        if (distanceToObjectOfInterest <= desiredDistance)// && hasLOS)
        {
            //Invert the impact scale
            angleImpact = -(angleImpact - 1);
        }

        //Calculate additional utility based on the smallest angles and divide by 2 so that it returns to a 0 to 1 scale
        utility = (utility + angleImpact) / 2;

        //Create color based on given gradient and utility score
        Color utilityColor = utilityGradient.Evaluate(utility);

        //Save the hit point for debugging later (if any detected)
        if (hit.collider != null && !hitPoints.ContainsKey(hit.point))
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

    private Vector3 GetAverageUtilityDirectionWithPreference()
    {
        //Return zero if no rays exist
        if (utilityRays == null || utilityRays.Count == 0) return Vector3.zero;

        //Temporary vector to track the average direction
        Vector3 averageDirection = GetAverageUtilityDirection();

        //Get the best ray
        UtilityRay bestRay = GetBestUtilityRay();

        //Subtract from the average direction
        averageDirection -= bestRay.baseRay.direction;

        //Re-add the best direction with the impact ratio
        averageDirection += bestRay.baseRay.direction.normalized * Mathf.InverseLerp(-1, 1, bestRay.rayUtility) * (bestRayImpactRatio * utilityRayCount);

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

    internal bool isWithinDesiredOffset()
    {
        //Temporarilly store the distance
        float distanceToTarget = ExtensionMethods.GetRemainingPathDistance(enemyManager.navAgent);

        //Check if within upper and lower bounds of the offset (assuming path is calculated)
        if (distanceToTarget > desiredDistance - repositioningOffset && distanceToTarget < desiredDistance + repositioningOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.root.position, desiredDistance);

        //Iterate over each point of impact for debugging
        foreach (KeyValuePair<Vector3, Color> point in hitPoints)
        {
            //Debug the points of impact
            Gizmos.color = point.Value;
            Gizmos.DrawSphere(point.Key, 0.1f);
        }
    }
}
