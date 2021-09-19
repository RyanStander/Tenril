using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMovement2D : MonoBehaviour
{
    private Rigidbody2D rigidBody; //Body of the unit to move
    private float horizontalInput; 
    private float verticalInput;
    public float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInputs();
        CorrectSpriteOrientation();
    }

    private void GetInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void CorrectSpriteOrientation()
    {
        //Change based on input
        if (horizontalInput > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (horizontalInput < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
    }
}
