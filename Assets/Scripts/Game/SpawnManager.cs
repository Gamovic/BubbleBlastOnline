using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : MonoBehaviour
{
    /*private bool hostConnected = false;
    private bool clientConnected = false;

    private void Awake()
    {
        if (IsServer)
        {
            SpawnBallServerRpc();
        }
        //NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        //NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void HandleServerStarted()
    {
        hostConnected = true;
        //TrySpawnObjects();
    }

    private void HandleClientConnected(ulong clientId)
    {
        clientConnected = true;
        //TrySpawnObjects();
    }

    private void TrySpawnObjects()
    {
        // Check if both host and client are connected
        if (hostConnected && clientConnected)
        {
            // Spawn your objects here
            //SpawnObjects();
        }
    }

    private void SpawnObjects()
    {
        // Spawn your objects using network object spawning logic
        // Example: Spawn a ball
        //SpawnBallServerRpc();
    }

    [ServerRpc]
    private void SpawnBallServerRpc()
    {
        // Instantiate and spawn the ball on the server
        GameObject ballPrefab = GetBallPrefab(); // Implement this method to get your ball prefab
        if (ballPrefab != null)
        {
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            ball.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            Debug.LogError("Ball prefab not found.");
        }
    }

    private GameObject GetBallPrefab()
    {
        // Replace "BallPrefab" with the name of your ball prefab
        NetworkPrefab ballNetworkPrefab = NetworkManager.Singleton.NetworkConfig.NetworkPrefabs.Find(prefab => prefab.name == "BallPrefab");

        if (ballNetworkPrefab != null)
        {
            return ballNetworkPrefab.Prefab;
        }
        else
        {
            return null;
        }
    }*/
}
