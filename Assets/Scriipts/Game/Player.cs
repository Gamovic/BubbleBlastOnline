using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    private float movement = 0f;
    public float speed = 4.0f;
    public Rigidbody2D rigidb;

    void Start()
    {
        
    }

    void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        
    }

    private void Move()
    {
        movement = Input.GetAxisRaw("Horizontal") * speed;

        rigidb.MovePosition(rigidb.position + Vector2.right * movement * Time.fixedDeltaTime);
    }
}
