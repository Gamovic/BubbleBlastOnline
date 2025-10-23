using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraSetup : MonoBehaviour
{
    [Header("Camera Setup")]
    public GameObject cameraPrefab;
    public GameObject backgroundPrefab;
    
    private void Start()
    {
        Debug.Log("CameraSetup started");
        
        // Only run on clients, not on server
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Running on server, skipping camera setup");
            return;
        }

        Debug.Log("Starting camera setup coroutine");
        // Wait a moment for players to spawn, then setup camera
        StartCoroutine(SetupCameraAfterDelay());
    }

    private IEnumerator SetupCameraAfterDelay()
    {
        Debug.Log("Waiting for players to spawn...");
        // Wait for players to spawn
        yield return new WaitForSeconds(2f);
        
        Debug.Log("Creating LocalCameraManager...");
        // Create LocalCameraManager
        GameObject cameraManagerObj = new GameObject("LocalCameraManager");
        LocalCameraManager cameraManager = cameraManagerObj.AddComponent<LocalCameraManager>();
        
        // Set the prefabs
        cameraManager.cameraPrefab = cameraPrefab;
        cameraManager.backgroundPrefab = backgroundPrefab;
        
        Debug.Log($"Local camera system setup complete. Camera prefab: {cameraPrefab != null}, Background prefab: {backgroundPrefab != null}");
    }
}
