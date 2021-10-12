using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorManager : AnimatorManager
{
    //Hashes for quick animator parameter modification
    internal int forwardHash;
    internal int leftHash;

    private Rigidbody rigidBody = null;
    private NavMeshAgent navAgent = null;

    private void Awake()
    {
        //Get the animator & null check
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogWarning("Missing Animator on " + gameObject + "!");

        //Get the rigidbody & null check
        rigidBody = GetComponentInChildren<Rigidbody>();
        if (rigidBody == null) Debug.LogWarning("Missing Rigidbody on " + gameObject + "!");

        //Get the NavMeshAgent & null check
        navAgent = GetComponentInChildren<NavMeshAgent>();
        if (navAgent == null) Debug.LogWarning("Missing NavMeshAgent on " + gameObject + "!");

        //Quick hashes for easy parameter modification
        forwardHash = Animator.StringToHash("Forward");
        leftHash = Animator.StringToHash("Left");
    }

    
    private void OnAnimatorMove()
    {
        //Synchronize the location/speed of the rigidbody with the intended speed of the animation
        SynchronizeRigidbody();

        //Synchronize the height of the transform with the navigation agent
        SynchronizeHeight();
    }

    private void SynchronizeRigidbody()
    {
        //Remove drag from the rigidbody
        rigidBody.drag = 0;

        //Get the delta position of the animator, flattening the Y axis for 2D movement
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;

        //Convert into intended velocity based on position and time
        Vector3 velocity = deltaPosition / Time.deltaTime;

        //Apply proportional velocity to the rigid body
        rigidBody.velocity = velocity;
    }

    private void SynchronizeHeight()
    {
        //Update the transform position to match the Y axis of the navigation agent
        Vector3 position = animator.rootPosition;
        position.y = navAgent.nextPosition.y;
        transform.root.position = position;
    }

    public void EnableCombo()
    {
        animator.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        animator.SetBool("canDoCombo", false);
    }
}
