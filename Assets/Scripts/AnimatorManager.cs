using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [HideInInspector]public Animator animator;
    public bool canRotate;
    internal void PlayTargetAnimation(string targetAnim, bool isInteracting=true,bool canRotate=true)
    {
        //Play a specific animation, if isInteracting is true, no other inputs can be performed during the animation
        animator.SetBool("canRotate", canRotate);
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

    public void EnableIsParrying()
    {
        animator.SetBool("isParrying", true);
    }

    public void DisableIsParrying()
    {
        animator.SetBool("isParrying", false);
    }

    public void EnableCanBeRiposted()
    {
        animator.SetBool("canBeRiposted", true);
    }

    public void DisableCanBeRiposted()
    {
        animator.SetBool("canBeRiposted", false);
    }

    public virtual void TakeFinisherDamageAnimationEvent()
    {

    }
}
