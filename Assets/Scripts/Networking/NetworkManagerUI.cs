using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button serverBtn;
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button clientBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });

        // Subscribe to player spawn event
        //NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        //NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    /*// Called when server is started
    private void HandleServerStarted()
    {
        SpawnPlayer(NetworkManager.Singleton.LocalClientId, true);
    }

    // Called when client is connected to server
    private void HandleClientConnected(ulong clientId)
    {
        // Spawn player for client
        SpawnPlayer(clientId, false);
    }

    // Spawn player and assign camera
    private void SpawnPlayer(ulong clientId, bool isLocalPlayer)
    {
        GameObject player = Instantiate(NetworkManager.Singleton.NetworkConfig.NetworkPrefab[0]).GetComponent<NetworkObject>();

        // Set player as local player if it's the local client
        if (isLocalPlayer)
        {
            player.SpawnAsPlayerObject(clientId);
        }
        else
        {
            player.SpawnWithOwnership(clientId);
        }

        FollowCamera followCamera = player.GetComponentInChildren<FollowCamera>();
        if (followCamera != null)
        {
            followCamera.player = player.transform;
        }
    }*/
}