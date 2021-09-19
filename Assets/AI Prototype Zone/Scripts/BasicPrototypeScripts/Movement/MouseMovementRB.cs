using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovementRB : BasicMovementRB
{
    public CharacterAttributes characterAttributes; //The characters attributes for relevant functions
    private Vector3 targetPosition; //Position the object should move to

    private void Start()
    {
        //Set starting position as self
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveOnClick();
    }

    private void MoveOnClick()
    {
        //If clicking, then get target position and move
        if(Input.GetMouseButtonDown(0))
        {
            //Raycast out to get new target position
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                targetPosition = hit.point;
            }
        }

        //Move to position
        MoveTowardsTarget(characterAttributes, targetPosition);
    }
}
