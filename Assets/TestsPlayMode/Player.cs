using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private Vector2 playerPos;

    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        playerPos = transform.position;
    }

    public void UpdateInputs(float horizontal, float vertical)
    {
        horizontalInput = horizontal * speed * Time.deltaTime;
        verticalInput = vertical * speed * Time.deltaTime;
    }

    public void Update()
    {
        // Calculate movement
        var x = horizontalInput * speed * Time.deltaTime;
        var y = verticalInput * speed * Time.deltaTime;

        // Update player position
        playerPos += new Vector2(x, y);
        transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }
}