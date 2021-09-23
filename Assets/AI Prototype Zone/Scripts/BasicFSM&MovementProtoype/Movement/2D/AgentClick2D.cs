using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentClick2D : Agent2D
{
    private Vector3 targetPosition;

    public override void Start()
    {
        targetPosition = gameObject.transform.position;
        base.Start();
    }

    public override void Update()
    {
        MoveOnClick();
        base.Update();
    }

    public override void SetDestination()
    {
        agent.SetDestination(targetPosition);
    }

    private void MoveOnClick()
    {
        //If clicking, then get target position and move
        if (Input.GetMouseButtonDown(0))
        {
            //Raycast out to get new target position
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                targetPosition = hit.point;
            }
        }
    }
}