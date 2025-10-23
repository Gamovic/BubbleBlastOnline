using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    //private Camera playerCamera;
    //private Vector3 mousePos;

    private Rigidbody2D rigidb;

    [SerializeField]
    private float speed = 10f; // Default speed if not set in inspector

    [SerializeField]
    private float minRotation;

    [SerializeField]
    private float maxRotation;

    [SerializeField]
    private float destroyDistance = 0.1f;
    
    [SerializeField]
    private float lifetime = 5f; // Bullet lifetime in seconds
    
    private float lifeTimer = 0f;
    
    // Track which player shot this bullet
    public ulong shooterClientId = 0;

    private void Initialize()
    {
        Debug.Log("Bullet Initialize called");
        
        rigidb = GetComponent<Rigidbody2D>();
        
        // Ensure the bullet has proper physics setup
        if (rigidb == null)
        {
            Debug.LogError("Bullet: No Rigidbody2D component found! Adding one...");
            rigidb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Set up physics properties
        rigidb.gravityScale = 0f; // No gravity for bullets
        rigidb.drag = 0f; // No air resistance
        rigidb.angularDrag = 0f; // No angular drag
        rigidb.isKinematic = false; // Make sure it's not kinematic
        rigidb.bodyType = RigidbodyType2D.Dynamic; // Make sure it's dynamic
        
        Debug.Log($"Bullet physics setup - IsKinematic: {rigidb.isKinematic}, BodyType: {rigidb.bodyType}, GravityScale: {rigidb.gravityScale}");
        
        // Ensure we have a collider
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Bullet: No Collider2D found! Adding CircleCollider2D...");
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true; // Make it a trigger for collision detection
            collider.radius = 0.1f; // Set a small radius
        }
        else
        {
            Debug.Log("Bullet already has a collider");
        }
    }

    public override void OnNetworkSpawn()
    {
        Initialize();
        Debug.Log("Bullet spawned on network");
    }
    
    private void Awake()
    {
        Initialize();
    }

    void Update()
    {
        // Debug bullet movement every 0.5 seconds to avoid spam
        if (Time.time - lastDebugTime > 0.5f)
        {
            if (rigidb != null)
            {
                Debug.Log($"Bullet Update - Position: {transform.position}, Velocity: {rigidb.velocity}, Speed: {speed}, IsKinematic: {rigidb.isKinematic}");
            }
            else
            {
                Debug.LogError("Bullet Update - No Rigidbody2D found!");
            }
            lastDebugTime = Time.time;
        }
        
        // Check bullet lifetime
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                DestroyBullet();
            }
        }
    }
    
    private float lastDebugTime = 0f;
    
    // Use Unity's built-in collision detection
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only handle collision on server
        if (!NetworkManager.Singleton.IsServer) return;
        
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Bullet hit ball!");
            
            // Get the ball component
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                // Split the ball
                ball.Split();
            }
            
            // Destroy the bullet
            DestroyBullet();
        }
        else if (other.CompareTag("Wall"))
        {
            Debug.Log("Bullet hit wall!");
            
            // Destroy bullet when hitting wall
            DestroyBullet();
        }
    }

    public void SetBulletDirection(Vector2 direction)
    {
        Debug.Log($"SetBulletDirection called with direction: {direction}");
        
        // Ensure we have a rigidbody
        if (rigidb == null)
        {
            Debug.Log("Rigidbody2D is null, trying to get component...");
            rigidb = GetComponent<Rigidbody2D>();
        }
        
        if (rigidb == null)
        {
            Debug.LogError("Bullet: No Rigidbody2D found!");
            return;
        }
        
        Debug.Log($"Rigidbody2D found - IsKinematic: {rigidb.isKinematic}, GravityScale: {rigidb.gravityScale}");
        
        // Normalize the direction vector
        direction = direction.normalized;
        Debug.Log($"Normalized direction: {direction}");
        
        // Set the velocity directly
        Vector2 newVelocity = direction * speed;
        rigidb.velocity = newVelocity;
        
        Debug.Log($"Bullet direction set - Direction: {direction}, Speed: {speed}, New Velocity: {newVelocity}, Actual Velocity: {rigidb.velocity}");
        
        // Force the rigidbody to not be kinematic
        rigidb.isKinematic = false;
        Debug.Log($"Set isKinematic to false, current velocity: {rigidb.velocity}");
    }


    private void DestroyBullet()
    {
        // Only server can despawn network objects
        if (NetworkManager.Singleton.IsServer)
        {
            // Check if this is a network object
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                if (networkObject.IsSpawned)
                {
                    // Simply despawn the network object - the pool handler will handle returning it
                    networkObject.Despawn();
                }
            }
            else
            {
                // Regular destroy for non-network bullets
                Destroy(gameObject);
            }
        }
    }
}
