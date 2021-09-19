using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshMovementClick : NavMeshMovement
{
    public CharacterAttributes characterAttributes; //The characters attributes for relevant functions
    private Vector3 targetPosition; //Position the object should move to

    private void Update()
    {
        MoveOnClick();
    }

    private void MoveOnClick()
    {
        //If clicking, then get target position and move
        if (Input.GetMouseButtonDown(0))
        {
            //Raycast out to get new target position
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                targetPosition = hit.point;
                Debug.Log("New position chosen at: "+ targetPosition);
            }

            //Move to target position
            MoveTowardsTarget(characterAttributes, targetPosition);
        }
    }
}
