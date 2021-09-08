using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public Animator anim;
    private PlayerController inputActions;

    private Vector2 movementInput;

    private bool sprintInput;

    // Start is called before the first frame update
    void Start()
    {
        if (inputActions==null)
        {
            //if no input actions set, create one
            inputActions = new PlayerController();
            //check for key inputs
            inputActions.PlayerMovement.Movement.performed += movementInputActions => movementInput = movementInputActions.ReadValue<Vector2>();

            //inputActions.PlayerMovement.Sprint. += i => sprintInput = true;
        }

        inputActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(sprintInput);
        HandleMovement();

    }

    private void LateUpdate()
    {
        //ResetInputs();
    }

    private void HandleMovement()
    {
        sprintInput = inputActions.PlayerMovement.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        float forwardMovement = movementInput.y;
        float leftMovement = movementInput.x;
        if (sprintInput)
        {
            forwardMovement= movementInput.y*2;
            leftMovement= movementInput.x*2;
        }

        anim.SetFloat("Forward", forwardMovement, 0.1f, Time.deltaTime);
        anim.SetFloat("Left", leftMovement, 0.1f, Time.deltaTime);
    }

    private void ResetInputs()
    {
        sprintInput = false;
    }
}
