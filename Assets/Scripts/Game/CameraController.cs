using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraController : NetworkBehaviour
{
    [Header("Camera Settings")]
    public float followSpeed = 2f;
    public float cameraOffset = 0f;
    public float minX = -10f;
    public float maxX = 10f;
    
    private Transform target;
    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        cam = GetComponent<Camera>();
        
        // Find the local player to follow
        if (IsOwner)
        {
            FindLocalPlayer();
        }
    }

    private void FindLocalPlayer()
    {
        // Find the local player object
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                target = player.transform;
                break;
            }
        }
    }

    void LateUpdate()
    {
        if (target != null && IsOwner)
        {
            // Calculate target position
            Vector3 targetPosition = new Vector3(
                Mathf.Clamp(target.position.x + cameraOffset, minX, maxX),
                transform.position.y,
                transform.position.z
            );

            // Smoothly move camera to target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
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
}
