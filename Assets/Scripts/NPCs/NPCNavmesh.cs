using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// handles the movement and navmesh of npcs
/// </summary>
public class NPCNavmesh : MonoBehaviour
{
    [SerializeField] private Transform[] targetTransformsToMoveTo;
    [Tooltip("If set to true, it will repeat the path")]
    [SerializeField] private bool patrol;
    [Tooltip("If set to true, it will go back the same point it was at previously in order to the first, otherwise it will go straight to the first")]
    [SerializeField] private bool pingPongPatrol;
    [SerializeField] private float moveSpeed = 0.5f;
    [Tooltip("This value allows the npc to have an offset to where they try to reach the destination")]
    [SerializeField] private float allowedOffsetForDestination = 1;
    
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private int currentTransformToMoveTo=0;
    private bool completedPath;
    private Vector3 actualTargetPosition;
    private bool decreaseValue;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        SetDestination();

        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        navMeshAgent.updatePosition = false;
    }
    private void Update()
    {
        HandleNPCMovement();
    }

    private void HandleNPCMovement()
    {
        if (!completedPath)
        {
            //if the agent is within range of the current taret move onto next target
            float remainingDistance = Vector3.Distance(transform.position, actualTargetPosition);
            if (remainingDistance <= navMeshAgent.stoppingDistance + 2f)
            {
                if (decreaseValue)
                {
                    //decrease current val
                    currentTransformToMoveTo--;
                }
                else
                {
                    //increase current val
                    currentTransformToMoveTo++;
                }
                //check if its still in the size range
                if (currentTransformToMoveTo >= targetTransformsToMoveTo.Length)
                {
                    //if patrol is enabled repeat rout
                    if (patrol)
                    {
                        //if true, return to the previouse point
                        if (pingPongPatrol)
                        {
                            decreaseValue = true;
                            currentTransformToMoveTo -= 2;
                        }
                        //otherwise it will return to the first position in the list
                        else
                        {
                            currentTransformToMoveTo = 0;
                        }

                    }
                    //If it must not loop, it will no longer continue traversing
                    else
                    {
                        completedPath = true;
                    }
                }
                else if (currentTransformToMoveTo < 0)
                {
                    currentTransformToMoveTo = 1;
                    decreaseValue = false;
                }

                SetDestination();
            }

            float desiredSpeed = navMeshAgent.desiredVelocity.magnitude / navMeshAgent.speed * moveSpeed;
            animator.SetFloat("Forward", desiredSpeed);
        }
        else
        {
            animator.SetFloat("Forward", 0);
        }

        //Reset the position of the navmesh to match the npc
        navMeshAgent.nextPosition = transform.position;
    }

    private void SetDestination()
    {
        if (targetTransformsToMoveTo.Length <= currentTransformToMoveTo)
            return;
        Vector2 randomPosition = Random.insideUnitCircle * allowedOffsetForDestination;
        actualTargetPosition = new Vector3(randomPosition.x,0,randomPosition.y) + targetTransformsToMoveTo[currentTransformToMoveTo].position;
        //Debug.Log("Setting Destination to: " + actualTargetPosition);
        navMeshAgent.SetDestination(actualTargetPosition);
    }
}