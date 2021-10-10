using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    private Rigidbody rigidBody = null;

    private void Awake()
    {
        //Get the animator & null check
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogWarning("Missing Animator on " + gameObject + "!");

        //Get the rigidbody & null check
        rigidBody = GetComponentInChildren<Rigidbody>();
        if (rigidBody == null) Debug.LogWarning("Missing Rigidbody on " + gameObject + "!");
    }

    
    private void OnAnimatorMove()
    {
        //Synchronize the location/speed of the rigidbody with the intended speed of the animation
        SynchronizeRigidbody();
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
}
