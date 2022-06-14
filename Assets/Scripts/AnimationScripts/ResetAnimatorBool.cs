using UnityEngine;

public class ResetAnimatorBool : StateMachineBehaviour
{
    public string targetBool;
    public bool status;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(targetBool, status);
    }
}
