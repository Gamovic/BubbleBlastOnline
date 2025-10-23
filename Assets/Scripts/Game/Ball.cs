using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    public Vector2 startForce;

    public GameObject ball;
    public GameObject ball2;
    public GameObject ball3;
    public GameObject ball4;

    public GameObject nextBall;

    public Rigidbody2D rigidb;
    
    // Ball splitting properties
    public int splitLevel = 0; // How many times this ball has been split
    public int maxSplitLevel = 5; // Maximum number of splits allowed
    public float minSize = 0.3f; // Minimum size before ball stops splitting
    public int pointsValue = 10; // Points awarded when this ball is destroyed

    private void Initialize()
    {
        // Add random horizontal component to make balls bounce left or right
        Vector2 randomForce = startForce;
        randomForce.x += Random.Range(-2f, 2f); // Add random horizontal force
        rigidb.AddForce(randomForce, ForceMode2D.Impulse);
    }

    public override void OnNetworkSpawn()
    {
        Initialize();

        // Check if the object is spawned on the server
        /*if (IsServer)
        {
            SpawnBallOnClientsServerRpc();
        }*/
    }

    /*[ServerRpc]
    private void SpawnBallOnClientsServerRpc()
    {
        // Spawn the ball on all clients
        SpawnBallClientRpc();
    }*/

    /*[ClientRpc]
    private void SpawnBallClientRpc()
    {
        // Instantiate the ball on the clients
        GameObject ball = Instantiate(gameObject, rigidb.position, Quaternion.identity);

        // Ensure that the spawned ball is a network object
        ball.GetComponent<NetworkObject>().Spawn();
    }*/

    public void Split()
    {
        // Only server can split balls
        if (!NetworkManager.Singleton.IsServer) return;
        
        Debug.Log($"Ball splitting - Level: {splitLevel}, Scale: {transform.localScale.x}, MinSize: {minSize}");
        
        // Check if ball can still be split
        if (splitLevel >= maxSplitLevel || transform.localScale.x <= minSize)
        {
            // Ball is too small or has been split too many times, just destroy it
            Debug.Log("Ball too small or max splits reached, destroying");
            DestroyBall();
            return;
        }

        if (nextBall != null)
        {
            // Get the ball from the pool
            NetworkObjectPool networkObjectPool = NetworkObjectPool.Singleton;
            if (networkObjectPool != null)
            {
                NetworkObject originalBall = NetworkObject;

                // Create two smaller balls
                GameObject ballGo1 = networkObjectPool.GetNetworkObject(nextBall, rigidb.position + Vector2.right / 4f, Quaternion.identity).gameObject;
                GameObject ballGo2 = networkObjectPool.GetNetworkObject(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity).gameObject;

                ballGo1.name = nextBall.name;
                ballGo2.name = nextBall.name;

                // Configure the new balls
                Ball ball1Script = ballGo1.GetComponent<Ball>();
                Ball ball2Script = ballGo2.GetComponent<Ball>();

                // Set split level (one more than current)
                ball1Script.splitLevel = splitLevel + 1;
                ball2Script.splitLevel = splitLevel + 1;

                // Set smaller size
                float newScale = Mathf.Max(transform.localScale.x * 0.7f, minSize);
                ballGo1.transform.localScale = new Vector3(newScale, newScale, 1f);
                ballGo2.transform.localScale = new Vector3(newScale, newScale, 1f);

                // Set different forces for variety
                ball1Script.startForce = new Vector2(Random.Range(2f, 4f), Random.Range(3f, 6f));
                ball2Script.startForce = new Vector2(Random.Range(-4f, -2f), Random.Range(3f, 6f));

                // Set points value (higher for smaller balls)
                ball1Script.pointsValue = pointsValue * 2;
                ball2Script.pointsValue = pointsValue * 2;

                // Spawn the new balls only if they're not already spawned
                NetworkObject ball1NetworkObject = ballGo1.GetComponent<NetworkObject>();
                NetworkObject ball2NetworkObject = ballGo2.GetComponent<NetworkObject>();
                
                if (!ball1NetworkObject.IsSpawned)
                    ball1NetworkObject.Spawn(true);
                if (!ball2NetworkObject.IsSpawned)
                    ball2NetworkObject.Spawn(true);

                // Return the original ball to the pool
                networkObjectPool.ReturnNetworkObject(originalBall, nextBall);
            }
            else
            {
                Debug.LogError("NetworkObjectPool.Singleton is null. Ensure that the pool is properly initialized.");
            }
        }
    }

    private void DestroyBall()
    {
        // Simply despawn - the pool handler will handle returning it
        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
    
    // Method to destroy ball and award points to specific player
    public void DestroyBallWithScore(ulong shooterClientId)
    {
        Debug.Log($"DestroyBallWithScore called - Shooter ID: {shooterClientId}, Points: {pointsValue}");
        
        // Try to find GameManager if Instance is null
        if (GameManager.Instance == null)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                Debug.Log("Found GameManager via FindObjectOfType");
                GameManager.Instance = gameManager;
            }
        }
        
        // Award points to the player who shot this ball
        if (GameManager.Instance != null)
        {
            // Determine if this is player 1 or player 2 based on client ID
            // Player 1 is typically the host (client ID 0), Player 2 is the first client (client ID 1)
            bool isPlayer1 = (shooterClientId == 0); // Host is usually client ID 0
            Debug.Log($"Awarding {pointsValue} points to {(isPlayer1 ? "Player 1" : "Player 2")} (Client ID: {shooterClientId})");
            GameManager.Instance.IncreaseScore(pointsValue, isPlayer1);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null! Cannot award points. Make sure GameManager is in the scene.");
        }

        // Simply despawn - the pool handler will handle returning it
        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }

    // Method to handle collision with bullets
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only handle collisions on server
        if (!NetworkManager.Singleton.IsServer) return;
        
        if (other.CompareTag("Bullet"))
        {
            Debug.Log($"Ball hit by bullet! Ball scale: {transform.localScale.x}, Split level: {splitLevel}");
            
            // Get the bullet component for shooter ID
            Bullet bullet = other.GetComponent<Bullet>();
            ulong shooterId = bullet != null ? bullet.shooterClientId : 0;
            
            // Check if this is the final split
            bool isFinalSplit = (splitLevel >= maxSplitLevel || transform.localScale.x <= minSize);
            Debug.Log($"Is final split: {isFinalSplit}");
            
            // Split the ball
            Split();
            
            // If this is the final split, award points
            if (isFinalSplit)
            {
                Debug.Log($"Awarding {pointsValue} points to shooter {shooterId}");
                DestroyBallWithScore(shooterId);
            }
        }
    }
}
