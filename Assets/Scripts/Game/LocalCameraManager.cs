using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LocalCameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject cameraPrefab;
    public GameObject backgroundPrefab; // Prefab with parallax background layers
    public float followSpeed = 2f;
    public float cameraOffset = 0f;
    public float minX = -50f;
    public float maxX = 50f;
    
    private Camera localCamera;
    private Transform target;
    private Vector3 velocity = Vector3.zero;
    private GameObject backgroundInstance;

    private void Start()
    {
        // Only run on clients, not on server
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            return;
        }

        // Start coroutine to wait for player and setup camera
        StartCoroutine(SetupCameraWhenPlayerReady());
    }

    private IEnumerator SetupCameraWhenPlayerReady()
    {
        // Wait for the local player to spawn
        while (target == null)
        {
            FindLocalPlayer();
            yield return new WaitForSeconds(0.1f);
        }

        // Spawn local camera and background
        SpawnLocalCamera();
        SpawnLocalBackground();
        
        Debug.Log("Local camera setup complete for player: " + target.name);
    }

    private void FindLocalPlayer()
    {
        // Find the local player object
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"Found {players.Length} players with 'Player' tag");
        
        foreach (GameObject player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                Debug.Log($"Player: {player.name}, IsOwner: {networkObject.IsOwner}, ClientId: {networkObject.OwnerClientId}");
                if (networkObject.IsOwner)
                {
                    target = player.transform;
                    Debug.Log($"Found local player: {player.name}");
                    break;
                }
            }
        }
        
        if (target == null)
        {
            Debug.LogWarning("No local player found yet");
        }
    }

    private void SpawnLocalCamera()
    {
        if (cameraPrefab != null)
        {
            Debug.Log("Spawning local camera...");
            // Spawn camera locally (not as NetworkObject)
            GameObject cameraObj = Instantiate(cameraPrefab);
            localCamera = cameraObj.GetComponent<Camera>();
            
            // Set camera tag so it can be found by other scripts
            cameraObj.tag = "Camera";
            
            // Position camera
            if (target != null)
            {
                cameraObj.transform.position = new Vector3(target.position.x + cameraOffset, 0, -10);
                Debug.Log($"Camera positioned at: {cameraObj.transform.position}");
            }

            // Setup FollowCamera script if it exists
            FollowCamera followCamera = cameraObj.GetComponent<FollowCamera>();
            if (followCamera != null)
            {
                followCamera.SetPlayer(target);
                Debug.Log("FollowCamera script configured");
            }
            else
            {
                Debug.Log("No FollowCamera script found on camera prefab");
            }
        }
        else
        {
            Debug.LogError("Camera prefab is null!");
        }
    }

    private void SpawnLocalBackground()
    {
        if (backgroundPrefab != null)
        {
            Debug.Log("Spawning local background...");
            // Spawn background locally (not as NetworkObject)
            backgroundInstance = Instantiate(backgroundPrefab);
            
            // Make sure parallax backgrounds reference the local camera
            ParallaxBackground[] parallaxLayers = backgroundInstance.GetComponentsInChildren<ParallaxBackground>();
            Debug.Log($"Found {parallaxLayers.Length} parallax layers");
            foreach (ParallaxBackground parallax in parallaxLayers)
            {
                if (parallax != null)
                {
                    parallax.SetLocalCamera(localCamera);
                    Debug.Log($"Set camera for parallax layer: {parallax.gameObject.name}");
                }
            }
            
            // Also try to find parallax layers in the scene that might already exist
            ParallaxBackground[] existingLayers = FindObjectsOfType<ParallaxBackground>();
            Debug.Log($"Found {existingLayers.Length} existing parallax layers in scene");
            foreach (ParallaxBackground parallax in existingLayers)
            {
                if (parallax != null && parallax.gameObject != backgroundInstance)
                {
                    parallax.SetLocalCamera(localCamera);
                    Debug.Log($"Set camera for existing parallax layer: {parallax.gameObject.name}");
                }
            }
        }
        else
        {
            Debug.LogError("Background prefab is null!");
        }
    }

    void LateUpdate()
    {
        if (target != null && localCamera != null)
        {
            // Calculate target position
            float desiredX = target.position.x + cameraOffset;
            float clampedX = Mathf.Clamp(desiredX, minX, maxX);
            
            Vector3 targetPosition = new Vector3(
                clampedX,
                localCamera.transform.position.y,
                localCamera.transform.position.z
            );

            // Check if camera is being clamped
            if (Mathf.Abs(desiredX - clampedX) > 0.1f)
            {
                Debug.Log($"Camera clamped! Desired: {desiredX:F2}, Clamped: {clampedX:F2}, Bounds: [{minX}, {maxX}]");
            }

            // Smoothly move camera to target position
            localCamera.transform.position = Vector3.SmoothDamp(
                localCamera.transform.position, 
                targetPosition, 
                ref velocity, 
                1f / followSpeed
            );
        }
    }

    // Method to set a new target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Method to update camera bounds
    public void SetCameraBounds(float min, float max)
    {
        minX = min;
        maxX = max;
    }

    // Method to get the local camera
    public Camera GetLocalCamera()
    {
        return localCamera;
    }
}
