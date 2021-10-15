using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [HideInInspector]public Animator animator;
    public bool canRotate;
    internal void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        //Play a specific animation, if isInteracting is true, no other inputs can be performed during the animation
        animator.applyRootMotion = isInteracting;
        animator.SetBool("canRotate", false);
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }

    public void CanRotate()
    {
        animator.SetBool("canRotate", true);
    }
    public void StopRotation()
    {
        animator.SetBool("canRotate", false);
    }

    public void EnableInvulnerability()
    {
        animator.SetBool("isInvulnerable", true);
    }

    public void DisableInvulnerability()
    {
        animator.SetBool("isInvulnerable", false);
    }
}
