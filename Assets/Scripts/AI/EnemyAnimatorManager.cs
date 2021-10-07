using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    private void Awake()
    {
        //Get the animator & null check
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogWarning("Missing Animator on " + gameObject + "!");
    }
    private void OnAnimatorMove()
    {
        //float delta = Time.deltaTime;
        //enemyManager.enemyRigidBody.drag = 0;
        //Vector3 deltaPosition = anim.deltaPosition;
        //deltaPosition.y = 0;
        //Vector3 velocity = deltaPosition / delta;
        //enemyManager.enemyRigidBody.velocity = velocity;
    }
}
