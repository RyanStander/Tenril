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
    [Tooltip("If set to true, it will go straight to the first, otherwise it will go back the same point it was at previously in order to the first")]
    [SerializeField] private bool ifPatrollingReturnGoBackToFirstFirst;
    [SerializeField] private float moveSpeed = 0.5f;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private int currentTransformToMoveTo=0;
    private bool completedPath;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(targetTransformsToMoveTo[currentTransformToMoveTo].position);

        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        navMeshAgent.updatePosition = false;
    }
    private void Update()
    {
        if (!completedPath)
        {
            navMeshAgent.enabled = true;
            //if the agent is within range of the current taret move onto next target
            float remainingDistance = Vector3.Distance(transform.position,targetTransformsToMoveTo[currentTransformToMoveTo].position);
            if (remainingDistance <= navMeshAgent.stoppingDistance+0.5f)
            {
                //increase current val
                currentTransformToMoveTo++;
                //check if its still in the size range
                if (currentTransformToMoveTo >= targetTransformsToMoveTo.Length)
                {
                    //if patrol is enabled repeat rout
                    if (patrol)
                    {
                        currentTransformToMoveTo = 0;
                        navMeshAgent.SetDestination(targetTransformsToMoveTo[currentTransformToMoveTo].position);
                    }
                    else
                    {
                        completedPath = true;
                    }
                }
                
            }
            else
            {
                navMeshAgent.SetDestination(targetTransformsToMoveTo[currentTransformToMoveTo].position);
            }

            float desiredSpeed = navMeshAgent.desiredVelocity.magnitude / navMeshAgent.speed * moveSpeed;
            animator.SetFloat("Forward", desiredSpeed);
        }
        else
        {
            navMeshAgent.enabled = false;
            animator.SetFloat("Forward", 0);
        }
    }

    private void FixedUpdate()
    {
        //Reset the position of the navmesh to match the npc
        navMeshAgent.nextPosition = transform.position;
    }
}