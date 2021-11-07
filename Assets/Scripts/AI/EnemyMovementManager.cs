using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

//Meant to help drive movement in an enemy
public class EnemyMovementManager : MonoBehaviour
{
    //Relevant attached manager
    protected EnemyAgentManager enemyManager;

    [Header("General Movement")]

    //Bool to toggle between higher quality animations or better obstacle avoidance
    public bool hasPreciseAvoidance = true;

    //The time it takes for movement to dampen when swapping states
    public float movementDampeningTime = 0.1f;

    [Header("Ground & Air Detection")]
    public float fallDurationToPerformLand = 0.5f; //The fall time needed to perform a landing
    public LayerMask EnvironmentLayer; //The layer being checked
    public Vector3 raycastOffset; //The offset 
    public float groundCheckRadius = 0.25f; //The radius of the sphere check

    private float fallDuration = 0; //Duration of the current fall

    private void Awake()
    {
        //Getter for relevant reference
        enemyManager = GetComponentInChildren<EnemyAgentManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyManager == null) throw new MissingComponentException("Missing EnemyAgentManager on " + gameObject + "!");

        //Automatically disable NavMeshAgent control
        DisableNavMeshAgent();
    }

    private void Start()
    {
        //Have root motion be applied
        enemyManager.animatorManager.animator.applyRootMotion = true;
    }

    internal void DisableNavMeshAgent()
    {
        //Disable certain navAgent features
        enemyManager.navAgent.isStopped = false; //Prevents agent from using any given speeds by accident
        enemyManager.navAgent.updatePosition = false; //Disable agent forced position
        enemyManager.navAgent.updateRotation = false; //Disable agent forced rotation
    }

    private void Update()
    {
        //Check for falling and handle it
        HandleFalling();
    }

    //Standard cycle of using the NavMeshAgent for navigation while trying to follow a given target
    internal void HandleNavMeshTargetMovement(float targetSpeed)
    {
        //Return and do not run any more methods until the current action/animation is completed
        if (enemyManager.isInteracting)
        {
            //Set to minimal forward movement while still performing action
            SetForwardMovement(0, 0.5f, Time.deltaTime);
            return;
        }

        //Suspend movement logic until the path is completely calcualted
        //Prevents problems with calculating remaining distance between the target and agent
        if (!IsPathPending())
        {
            //Rotate towards the next position that is gotten from the agent
            RotateTowardsNextPosition();

            //Set movement to 0 if rooted, otherwise continue
            if(enemyManager.statusManager.GetIsRooted())
            {
                StopMovementImmediate();
            }
            else
            {
                //Handle the forward movement of the agent
                SetForwardMovement(targetSpeed, 0.5f, Time.deltaTime);
            }
            
            //Correct the location of the NavmeshAgent with precise or estimated calculations
            //Choice between precise or estimated
            if (hasPreciseAvoidance)
            {
                //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
                CorrectAgentLocationPrecise();
            }
            else
            {
                //Method preserves animation quality (reducing foot sliding) at the cost of obstacle avoidance
                CorrectAgentLocationEstimated();
            }
        }
    }

    private void OnAnimatorMove()
    {
        //If not falling
        if(IsGrounded())
        {
            //Synchronize the transform to the root position of the animation
            SynchronizeTransformToAnimation();
        }
    }

    internal void SynchronizeTransformToAnimation()
    {
        //Update the transform position in addition to matching the Y axis of the navigation agent
        Vector3 position = enemyManager.animatorManager.animator.rootPosition;
        //position.y = enemyManager.navAgent.nextPosition.y;
        transform.position = position;
    }

    //Method sacrifices animation quality (increasing foot sliding) at the improvement of obstacle avoidance
    internal void CorrectAgentLocationPrecise()
    {
        //Set the navAgents predicted position to be the root transform
        enemyManager.navAgent.nextPosition = transform.root.position;
    }

    //Method preserves animation quality (reducing foot sliding) at the cost of obstacle avoidance
    internal void CorrectAgentLocationEstimated()
    {
        //Get the world delta position in relation to the agent and the intended body to follow
        Vector3 worldDeltaPosition = enemyManager.navAgent.nextPosition - transform.root.position;

        //If not following within the radius, correct it
        if (worldDeltaPosition.magnitude > enemyManager.navAgent.radius)
        {
            //Set the next position
            enemyManager.navAgent.nextPosition = transform.root.position + 0.9f * worldDeltaPosition;
        }
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

    internal void RotateTowardsNextPosition()
    {
        //Simulates the agents next steer target
        RotateTowardsTargetPosition(enemyManager.navAgent.steeringTarget, enemyManager.enemyStats.rotationSpeed);
    }
    #endregion

    internal bool IsPathPending()
    {
        //Quick bool return for if the agent is still calculating a path
        return enemyManager.navAgent.pathPending;
    }

    internal void SetForwardMovement(float givenValue, float givenDampTime, float givenTime)
    {
        enemyManager.animatorManager.animator.SetFloat(enemyManager.animatorManager.forwardHash, givenValue, givenDampTime, givenTime);
    }

    internal IEnumerator StopMovementCourotine()
    {
        //Timer for stop movement to complete dampening
        float timeRemaining = movementDampeningTime;
        while (timeRemaining > 0)
        {
            //Lower the remaining time
            timeRemaining -= Time.deltaTime;

            //Run stopping movement
            StopMovement(movementDampeningTime, Time.deltaTime);

            //Yield return
            yield return null;
        }
    }

    public void StopMovement(float givenDampeningTime, float givenTime)
    {
        //Stop the animations movement of forward and leftward based on given parameters
        enemyManager.animatorManager.animator.SetFloat(enemyManager.animatorManager.forwardHash, 0, givenDampeningTime, givenTime);
        enemyManager.animatorManager.animator.SetFloat(enemyManager.animatorManager.leftHash, 0, givenDampeningTime, givenTime);
    }

    public void StopMovementImmediate()
    {
        //Stop the animations movement of forward and leftward immediately
        enemyManager.animatorManager.animator.SetFloat(enemyManager.animatorManager.forwardHash, 0);
        enemyManager.animatorManager.animator.SetFloat(enemyManager.animatorManager.leftHash, 0);
    }

    private void HandleFalling()
    {
        //If not currently grounded, track the fall
        if (!IsGrounded())
        {
            //Track the current duration of the fall
            fallDuration += Time.deltaTime;

            //Play the falling animation
            enemyManager.animatorManager.PlayTargetAnimation("Falling", true);
            //enemyManager.animatorManager.PlayTargetAnimation("Falling Exclusive", true);
        }
        else
        {
            //If the creature is falling for enough time, perform a land animation
            if (fallDuration > fallDurationToPerformLand)
            {
                //Play the landing animation
                enemyManager.animatorManager.PlayTargetAnimation("Land", true);
            }
            else if (fallDuration > 0)
            {
                //Return to empty state
                enemyManager.animatorManager.PlayTargetAnimation("Empty", true);
            }

            //Reset the fall timer
            fallDuration = 0;
        }
    }

    private bool IsGrounded()
    {
        //Check with a sphere if the character is on the ground, based on outcome, will either be set to being grounded or not
        if (Physics.CheckSphere(transform.position - raycastOffset, groundCheckRadius, EnvironmentLayer))
        {
            enemyManager.animatorManager.animator.SetBool("isGrounded", true);
            return true;
        }
        else
        {
            enemyManager.animatorManager.animator.SetBool("isGrounded", false);
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        //Draw the IsGrounded check
        Gizmos.DrawWireSphere(transform.position - raycastOffset, groundCheckRadius);
    }
}