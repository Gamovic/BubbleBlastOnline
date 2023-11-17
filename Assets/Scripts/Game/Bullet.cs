using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rigidb;
    public float speed;
    public float minRotation;
    public float maxRotation;

    public float destroyDistance = 0.1f;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rigidb = GetComponent<Rigidbody2D>();
        //mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 direction = mousePos - transform.position;
        //Vector3 rotation = transform.position - mousePos;
        //float rotation = Mathf.Clamp(transform.rotation.eulerAngles.z, minRotation, maxRotation);
        //Vector2 direction = Quaternion.Euler(0, 0, rotation) * Vector2.up;
        //rigidb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
        //float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        SetBulletDirection();
    }

    void Update()
    {
        CheckForCollisions();
    }

    private void SetBulletDirection()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = (mousePos - transform.position).normalized;

        // Clamp the direction within the allowed rotation range
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minRotation, maxRotation);
        direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

        // Set the bullet's velocity
        rigidb.velocity = direction * speed;
    }

    private void CheckForCollisions()
    {
        Vector2 currentPosition = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentPosition, destroyDistance);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ball"))
            {
                collider.GetComponent<Ball>().Split();
            }
            else if (collider.CompareTag("Ball") || collider.CompareTag("Wall"))
            {
                Destroy(gameObject);
                break; // Exit loop after bullet is destroyed
            }
        }
    }
}
