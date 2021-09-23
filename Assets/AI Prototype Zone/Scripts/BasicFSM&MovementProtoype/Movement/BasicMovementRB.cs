using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementRB : AbstractMovement
{
    public Rigidbody rigidBody = null; //The rigidbody of the current entity

    private void Start()
    {
        //Attempt to get the rigid body affected if null
        if(rigidBody is null)
        {
            //Get & set the rigidbody
            rigidBody = GetComponent<Rigidbody>();
        }
    }

    public override void MoveTowardsTarget(CharacterAttributes attributes, Vector3 targetPosition)
    {
        //Move the rigid body to the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        rigidBody.MovePosition(transform.position + direction * attributes.movementSpeed * Time.deltaTime);
    }

    public override void StopMovement() { }
}
