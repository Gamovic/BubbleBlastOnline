using UnityEngine;
using Unity.Netcode;

public class GameManagerSpawner : NetworkBehaviour
{
    [Header("GameManager Prefab")]
    public GameObject gameManagerPrefab;
    
    private GameObject spawnedGameManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Only spawn on server
        if (IsServer)
        {
            SpawnGameManager();
        }
    }

    private void SpawnGameManager()
    {
        if (gameManagerPrefab != null && spawnedGameManager == null)
        {
            Debug.Log("Spawning GameManager as NetworkObject...");
            
            // Spawn GameManager as NetworkObject
            spawnedGameManager = Instantiate(gameManagerPrefab);
            NetworkObject networkObject = spawnedGameManager.GetComponent<NetworkObject>();
            
            if (networkObject != null)
            {
                networkObject.Spawn();
                Debug.Log("GameManager spawned successfully!");
            }
            else
            {
                Debug.LogError("GameManager prefab does not have a NetworkObject component!");
            }
        }
        else
        {
            Debug.LogError("GameManager prefab is null or already spawned!");
        }
    }

    public override void OnNetworkDespawn()
    {
        // Clean up spawned GameManager
        if (spawnedGameManager != null)
        {
            if (spawnedGameManager.GetComponent<NetworkObject>() != null)
            {
                spawnedGameManager.GetComponent<NetworkObject>().Despawn();
            }
            Destroy(spawnedGameManager);
        }
        
        base.OnNetworkDespawn();
    }
}
