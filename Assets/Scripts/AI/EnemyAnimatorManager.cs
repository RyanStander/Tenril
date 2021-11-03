using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorManager : AnimatorManager
{
    //Hashes for quick animator parameter modification
    internal int forwardHash;
    internal int leftHash;
    internal int canRotateHash;

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
        canRotateHash = Animator.StringToHash("canRotate");
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
