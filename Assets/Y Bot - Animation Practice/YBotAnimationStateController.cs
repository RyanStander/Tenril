using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBotAnimationStateController : MonoBehaviour
{
    public Animator animator;
    private int isWalkingHash;
    private int isRunningHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log("Was animator found?:"+ animator);

        //Quick hash for parameters
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        //Bool to check for key press
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = animator.GetBool(isWalkingHash);

        //If not walking but pressing forward
        if (!isWalking && forwardPressed)
        {
            //Modify parameter
            isWalking = true;
        }
        //If walking but not pressing forward
        else if(isWalking && !forwardPressed)
        {
            //Modify parameter
            isWalking = false;
        }

        //If currently moving but wanting to run
        if (!isRunning && forwardPressed && runPressed)
        {
            //Modify parameter
            isRunning = true;
        }
        //If not moving or running
        else if (isRunning && (!forwardPressed || !runPressed))
        {
            //Modify parameter
            isRunning = false;
        }

        //Set parameter
        animator.SetBool(isWalkingHash, isWalking);
        animator.SetBool(isRunningHash, isRunning);
    }
}
