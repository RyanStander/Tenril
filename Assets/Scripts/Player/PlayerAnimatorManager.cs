using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : AnimatorManager
{
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
