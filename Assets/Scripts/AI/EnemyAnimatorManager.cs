using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorManager : AnimatorManager
{
    //Hashes for quick animator parameter modification
    internal int forwardHash;
    internal int leftHash;
    internal int turningHash;
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
        turningHash = Animator.StringToHash("Turning");
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

    public void EnableIsInteracting()
    {
        animator.SetBool("isInteracting", true);
    }

    public void DisableIsInteracting()
    {
        animator.SetBool("isInteracting", false);
    }

    public override void TakeFinisherDamageAnimationEvent()
    {
        base.TakeFinisherDamageAnimationEvent();

        EnemyAgentManager enemyAgentManager = GetComponent<EnemyAgentManager>();

        GetComponent<EnemyStats>().TakeDamage(enemyAgentManager.pendingFinisherDamage, false);
        enemyAgentManager.pendingFinisherDamage = 0;
    }

    public Vector2 getCurrentSpeed()
    {
        return new Vector2(animator.GetFloat(forwardHash), animator.GetFloat(leftHash));
    }
}
