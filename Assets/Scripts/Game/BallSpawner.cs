using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private GameObject ballPrefab2;

    [SerializeField]
    private GameObject ballPrefab3;

    [SerializeField]
    private GameObject ballPrefab4;

    private const int MaxPrefabCount = 100;

    private bool isSpawning;

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnBallStart;
    }

    private void SpawnBallStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnBallStart;
        Debug.Log("BallSpawner: Server started, initializing ball spawning...");

        if (NetworkObjectPool.Singleton != null)
        {
            isSpawning = true;
            Debug.Log("BallSpawner: NetworkObjectPool found, starting ball spawning");

            NetworkObjectPool.Singleton.OnNetworkSpawn();

            // Reduced number of balls from 7 to 3-4 for better gameplay
            int initialBallCount = Random.Range(3, 5);
            Debug.Log($"BallSpawner: Spawning {initialBallCount} initial balls");
            for (int i = 0; i < initialBallCount; ++i)
            {
                SpawnBall();
            }

            StartCoroutine(SpawnOverTime());
        }
        else
        {
            Debug.LogError("NetworkObjectPool.Singleton is null. Ensure that the pool is properly initialized.");
        }
    }

    private void SpawnBall()
    {
        // Randomly choose which ball prefab to spawn
        GameObject[] ballPrefabs = { ballPrefab, ballPrefab2, ballPrefab3, ballPrefab4 };
        GameObject selectedPrefab = ballPrefabs[Random.Range(0, ballPrefabs.Length)];
        
        Vector3 spawnPosition = GetRandomPositionOnMap();
        Debug.Log($"BallSpawner: Attempting to spawn ball at {spawnPosition}");
        
        NetworkObject poolObj = NetworkObjectPool.Singleton.GetNetworkObject(selectedPrefab, spawnPosition, Quaternion.identity);

        if (poolObj != null)
        {
            Debug.Log("BallSpawner: Got ball from pool, setting up...");
            
            // Set the next ball reference for splitting
            Ball ballScript = poolObj.GetComponent<Ball>();
            if (ballScript != null)
            {
                ballScript.nextBall = selectedPrefab;
            }

            // Only spawn if not already spawned
            if (!poolObj.IsSpawned)
            {
                poolObj.Spawn(true);
                Debug.Log("BallSpawner: Ball spawned successfully");
            }
            else
            {
                Debug.Log("BallSpawner: Ball was already spawned");
            }
        }
        else
        {
            Debug.LogError("BallSpawner: Failed to get ball from pool");
        }
    }

    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-6f, 6f), Random.Range(-3f, 3f), 0f);
    }

    private IEnumerator SpawnOverTime()
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0 && isSpawning)
        {
            // Increased spawn time from 2f to 5f to reduce ball density
            yield return new WaitForSeconds(5f);

            // Only spawn if we have fewer than 6 balls total
            // Note: GetCurrentPrefabCount might not be available, so we'll spawn without this check
            SpawnBall();
        }
    }
}
