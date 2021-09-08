using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMouseClick : MonoBehaviour
{
    //Position the object should move to
    private Vector3 targetPosition;

    //Stored rigid body to move
    private Rigidbody rigidBody;

    //Speed at which it moves
    public float movementSpeed = 15f;

    void Start()
    {
        //Starting position
        targetPosition = transform.position;

        //Fetch the Rigidbody from the GameObject with this script attached
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //If mouse click, then get target position
        if (Input.GetMouseButtonDown(0))
        {
            //Raycast out to get new target position
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                targetPosition = hit.point;
            }
        }

        //Move to position
        Vector3 direction = (targetPosition - transform.position).normalized;
        rigidBody.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
    }

}
