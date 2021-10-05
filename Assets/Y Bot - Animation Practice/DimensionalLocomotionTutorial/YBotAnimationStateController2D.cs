using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBotAnimationStateController2D : MonoBehaviour
{
    public Animator animator;
    //Speed at which the agent is moving
    private float agentVelocityX = 0;
    private float agentVelocityZ = 0;

    //Accelaration/Decelaration factors that slow down velocity
    public float accelarationFactor = 2f;
    public float decelarationFactor = 3f;

    //Limits for clamps and movement logic
    public float walkingVelocityLimit = 0.5f;
    public float runningVelocityLimit = 2;
    public float directionalSpeedLimit = 2;

    private int isWalkingHash;
    private int isRunningHash;

    //Hashes for quick animator parameter modification
    private int velocityXHash;
    private int velocityZHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log("Was animator found?:"+ animator);

        //Quick hash for parameters
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        velocityXHash = Animator.StringToHash("VelocityX");
        velocityZHash = Animator.StringToHash("VelocityZ");
    }

    // Update is called once per frame
    void Update()
    {
        //Get key inputs as bools
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);

        //Ternary operator to compare bool statements for limit
        float currentMaximumVelocity = runPressed ? runningVelocityLimit : walkingVelocityLimit;

        ChangeVelocities(forwardPressed, leftPressed, rightPressed, currentMaximumVelocity);
        LockOrResetVelocity(forwardPressed, runPressed, leftPressed, rightPressed, currentMaximumVelocity);

        //Set the velocity in the animator
        animator.SetFloat(velocityXHash, agentVelocityX);
        animator.SetFloat(velocityZHash, agentVelocityZ);
    }

    private void ChangeVelocities(bool forwardPressed, bool leftPressed, bool rightPressed, float currentMaximumVelocity)
    {
        //Add velocity based on time and accelaration factor
        //Forward movement
        if (forwardPressed && agentVelocityZ < currentMaximumVelocity)
        {
            agentVelocityZ += Time.deltaTime * accelarationFactor;
        }

        //Leftward movement
        if (leftPressed && agentVelocityX > -currentMaximumVelocity)
        {
            agentVelocityX -= Time.deltaTime * accelarationFactor;
        }

        //Rightward movement
        if (rightPressed && agentVelocityX < currentMaximumVelocity)
        {
            agentVelocityX += Time.deltaTime * accelarationFactor;
        }

        //Slow down forward movement
        if (!forwardPressed && agentVelocityZ > 0)
        {
            agentVelocityZ -= Time.deltaTime * decelarationFactor;
        }

        //Slow down leftward movement
        if (!leftPressed && agentVelocityX < 0)
        {
            agentVelocityX += Time.deltaTime * decelarationFactor;
        }

        //Slow down leftward movement
        if (!rightPressed && agentVelocityX > 0)
        {
            agentVelocityX -= Time.deltaTime * decelarationFactor;
        }
    }

    private void LockOrResetVelocity(bool forwardPressed, bool runPressed, bool leftPressed, bool rightPressed, float currentMaximumVelocity)
    {
        //Reset velocity
        if (!forwardPressed && agentVelocityZ < 0)
        {
            agentVelocityZ = 0;
        }

        //Reset velocities for idle
        if (!leftPressed && !rightPressed && agentVelocityX != 0 && (agentVelocityX > -0.05f && agentVelocityX < 0.05f))
        {
            agentVelocityX = 0;
        }

        //Forward Clamps
        //---------------
        //Lock forward movement if above maximum
        if (forwardPressed & runPressed && agentVelocityZ > currentMaximumVelocity)
        {
            agentVelocityZ = currentMaximumVelocity;
        }
        //Decelarate to the maximum walk velocity if not running
        else if (forwardPressed && agentVelocityZ > currentMaximumVelocity)
        {
            agentVelocityZ -= Time.deltaTime * decelarationFactor;

            //Clamp to the current maximum if within the offset
            if (agentVelocityZ > currentMaximumVelocity && agentVelocityZ < (currentMaximumVelocity + 0.05f))
            {
                agentVelocityZ = currentMaximumVelocity;
            }
        }
        //Clamp to the current maximum if within the offset
        else if (forwardPressed && agentVelocityZ < currentMaximumVelocity && agentVelocityZ > (currentMaximumVelocity - 0.05f))
        {
            agentVelocityZ = currentMaximumVelocity;
        }

        //Leftward Clamps
        //---------------
        //Lock leftward movement if above maximum
        if (leftPressed & runPressed && agentVelocityX < -currentMaximumVelocity)
        {
            agentVelocityX = -currentMaximumVelocity;
        }
        //Decelarate to the maximum walk velocity if not running
        else if (leftPressed && agentVelocityX < -currentMaximumVelocity)
        {
            agentVelocityX += Time.deltaTime * decelarationFactor;

            //Clamp to the current maximum if within the offset
            if (agentVelocityX < -currentMaximumVelocity && agentVelocityX > (-currentMaximumVelocity - 0.05f))
            {
                agentVelocityX = -currentMaximumVelocity;
            }
        }
        //Clamp to the current maximum if within the offset
        else if (leftPressed && agentVelocityX > -currentMaximumVelocity && agentVelocityX < (-currentMaximumVelocity + 0.05f))
        {
            agentVelocityX = -currentMaximumVelocity;
        }

        //Rightward Clamps
        //---------------
        //Lock rightward movement if above maximum
        if (rightPressed & runPressed && agentVelocityX > currentMaximumVelocity)
        {
            agentVelocityX = currentMaximumVelocity;
        }
        //Decelarate to the maximum walk velocity if not running
        else if (rightPressed && agentVelocityX > currentMaximumVelocity)
        {
            agentVelocityX -= Time.deltaTime * decelarationFactor;

            //Clamp to the current maximum if within the offset
            if (agentVelocityX > currentMaximumVelocity && agentVelocityX < (currentMaximumVelocity + 0.05f))
            {
                agentVelocityX = currentMaximumVelocity;
            }
        }
        //Clamp to the current maximum if within the offset
        else if (rightPressed && agentVelocityX < currentMaximumVelocity && agentVelocityX > (currentMaximumVelocity - 0.05f))
        {
            agentVelocityX = currentMaximumVelocity;
        }
    }
}
