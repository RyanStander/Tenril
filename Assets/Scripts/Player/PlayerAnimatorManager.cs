using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : AnimatorManager
{
    private void Awake()
    {
        animator = GetComponent<Animator>();
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
}
