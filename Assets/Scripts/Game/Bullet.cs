using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class Bullet : MonoBehaviour
{
    //private Camera playerCamera;
    //private Vector3 mousePos;

    private Rigidbody2D rigidb;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float minRotation;

    [SerializeField]
    private float maxRotation;

    [SerializeField]
    private float destroyDistance = 0.1f;

    private void Initialize()
    {
        rigidb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        Initialize();
    }

    void Update()
    {
        CheckForCollisions();
    }

    public void SetBulletDirection(Vector3 direction)
    {
        // Clamp the direction within the allowed rotation range
        float angle = direction.z;//Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minRotation, maxRotation);
        direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

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

                GameManager.Instance.IncreaseScore(1);
            }
            else if (collider.CompareTag("Ball") || collider.CompareTag("Wall"))
            {
                Destroy(gameObject);
                break; // Exit loop after bullet is destroyed
            }
        }
    }
}
