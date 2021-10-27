using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBotAnimationStateController : MonoBehaviour
{
    public Animator animator;
    private int isWalkingHash;
    private int isRunningHash;

    private float agentVelocity = 0;
    public float accelarationFactor = 0.1f;
    public float decelarationFactor = 0.5f;
    private int velocityHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log("Was animator found?:"+ animator);

        //Quick hash for parameters
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        velocityHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        //Get key inputs
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        if(forwardPressed)
        {
            //Add velocity based on time and accelaration factor
            agentVelocity += Time.deltaTime * accelarationFactor;
        }
        else if(!forwardPressed)
        {
            //Add velocity based on time and accelaration factor
            agentVelocity -= Time.deltaTime * decelarationFactor;
        }

        //Clamp between 0 and 1
        agentVelocity = Mathf.Clamp01(agentVelocity);

        //Set the velocity in the animator
        animator.SetFloat(velocityHash, agentVelocity);
    }

    //Will not work with current setup, kept as old reference
    private void KeyPressedAnimation()
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
        else if (isWalking && !forwardPressed)
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
