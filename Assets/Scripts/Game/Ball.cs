using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector2 startForce;

    public GameObject nextBall;

    public Rigidbody2D rigidb;

    void Start()
    {
        rigidb.AddForce(startForce, ForceMode2D.Impulse);
    }

    public void Split()
    {
        if (nextBall != null)
        {
            /*GameObject ball01 = */Instantiate(nextBall, rigidb.position + Vector2.right / 4f, Quaternion.identity);
            Instantiate(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
