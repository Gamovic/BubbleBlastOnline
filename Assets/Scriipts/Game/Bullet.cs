using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Bullet : MonoBehaviour
{
    private float speed;
    private Vector2 velocity;
    private bool hasCollided = false;

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    private void Awake()
    {
        tag = "Bullet";
        hasCollided = false;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(velocity * speed * Time.deltaTime);
    }

    private Bullet Clone()
    {
        return (Bullet)this.MemberwiseClone();
    }

    public void Notify(GameEvent )
}
