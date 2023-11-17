using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private Vector2 playerPos;
    public float minScreenLimitX;
    public float maxScreenLimitX;

    private float horizontalInput;
    //private float verticalInput;

    private bool useSimulatedInput = false;

    private void Start()
    {
        playerPos = transform.position;
    }

    public void UpdateInputs(float horizontal/*, float vertical*/)
    {
        horizontalInput = horizontal * speed * Time.deltaTime;
        //verticalInput = 0f;
    }

    public void SimulateKeyPress(KeyCode key)
    {
        // Simulate a key press for testing
        UpdateInputs((key == KeyCode.RightArrow) ? 1f : -1f/*, 0f*/);
        Update();

        // Reset simulated input
        horizontalInput = 0f;
        //verticalInput = 0f;
    }

    public void UseSimulatedInput(bool useSimulated)
    {
        useSimulatedInput = useSimulated;
    }

    public void Update()
    {
        if (useSimulatedInput)
        {
            // Calculate movement based on simulated input
            //var x = horizontalInput;
            float x = transform.position.x + horizontalInput * speed * Time.deltaTime;

            float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

            //var y = verticalInput;
            // Update player position
            //playerPos += new Vector2(x, y);
            transform.position = new Vector2(clampedPos, transform.position.y);
            //transform.position = new Vector2(playerPos.x, transform.position.y);
        }
        else
        {
            // Use Input for movement during play mode
            horizontalInput = Input.GetAxisRaw("Horizontal");
            float x = transform.position.x + horizontalInput * speed * Time.deltaTime;
            //verticalInput = Input.GetAxisRaw("Vertical");

            // Calculate movement based on actual input
            //var x = horizontalInput * speed * Time.deltaTime;
            //var y = 0f;

            float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

            // Update player position
            //playerPos += new Vector2(x, y);
            transform.position = new Vector2(clampedPos, transform.position.y);
        }
    }
}