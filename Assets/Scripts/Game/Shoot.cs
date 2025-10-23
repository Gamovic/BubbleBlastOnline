using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Shoot : NetworkBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;

    [SerializeField]
    private float minRotation = -90f;

    [SerializeField]
    private float maxRotation = 90f;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform bulletTransform;

    [SerializeField]
    private Camera playerCamera;
    
    private Transform rotationPoint; // The parent of BulletTransform

    // Use the existing BulletTransform as the reticle - no need for LineRenderer

    [SerializeField]
    private bool canShoot = true;

    [SerializeField]
    private float timeBetweenShot = 0.5f;
    private float cooldownTimer;

    // Network variables for synchronized rotation
    public NetworkVariable<float> syncedRotation = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        Debug.Log($"Shoot: OnNetworkSpawn called - IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}, LocalClientId: {NetworkManager.Singleton.LocalClientId}");
        
        if (playerCamera == null)
        {
            // Try to find the local camera with "Camera" tag first
            GameObject cameraObj = GameObject.FindWithTag("Camera");
            if (cameraObj != null)
            {
                playerCamera = cameraObj.GetComponent<Camera>();
                Debug.Log($"Shoot: Found local camera: {playerCamera.name}");
            }
            else
            {
                playerCamera = Camera.main;
                Debug.LogWarning("Shoot: No local camera found, using Camera.main");
            }
        }

        canShoot = true;
        
        // Find the RotationPoint (parent of BulletTransform)
        if (bulletTransform != null)
        {
            rotationPoint = bulletTransform.parent;
            if (rotationPoint != null)
            {
                Debug.Log("Shoot: Found RotationPoint, using BulletTransform as reticle");
            }
            else
            {
                Debug.LogError("Shoot: BulletTransform has no parent! Make sure it's under RotationPoint.");
            }
        }
        else
        {
            Debug.LogError("Shoot: BulletTransform is null! Make sure it's assigned in the editor.");
        }
        
        // Set initial weapon direction to 0 (facing right)
        if (IsOwner)
        {
            // Player stays at identity rotation, only weapon direction changes
            transform.rotation = Quaternion.identity;
            syncedRotation.Value = 0f;
            UpdateWeaponVisual(0f);
            Debug.Log($"Shoot: Set initial weapon direction to 0 degrees (facing right)");
        }
        
        // Subscribe to rotation changes
        syncedRotation.OnValueChanged += OnRotationChanged;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe from rotation changes
        syncedRotation.OnValueChanged -= OnRotationChanged;
        base.OnNetworkDespawn();
    }

    private void OnRotationChanged(float previousValue, float newValue)
    {
        // DON'T rotate the entire player - only update weapon visual
        UpdateWeaponVisual(newValue);
    }

    void Update()
    {
        if (!IsOwner) 
        {
            Debug.Log($"Shoot: Not owner, skipping input. IsOwner: {IsOwner}, LocalClientId: {NetworkManager.Singleton.LocalClientId}");
            return;
        }

        Debug.Log($"Shoot: Owner detected, calling RotateWeapon and Shooting. LocalClientId: {NetworkManager.Singleton.LocalClientId}");
        RotateWeapon();
        Shooting();
    }
    
    void FixedUpdate()
    {
        // Server handles cooldown logic
        if (NetworkManager.Singleton.IsServer)
        {
            if (!canShoot)
            {
                cooldownTimer += Time.fixedDeltaTime;
                if (cooldownTimer > timeBetweenShot)
                {
                    canShoot = true;
                    cooldownTimer = 0f;
                    Debug.Log("Server: Shooting cooldown finished - canShoot is now true");
                }
            }
        }
    }

    private void RotateWeapon()
    {
        if (playerCamera == null) 
        {
            // Try to find the local camera with "Camera" tag
            GameObject cameraObj = GameObject.FindWithTag("Camera");
            if (cameraObj != null)
            {
                playerCamera = cameraObj.GetComponent<Camera>();
                Debug.Log($"RotateWeapon: Found local camera: {playerCamera.name}");
            }
            else
            {
                Debug.Log("RotateWeapon: playerCamera is null and no local camera found!");
                return;
            }
        }

        // Use mouse position relative to screen center, not world position
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mouseOffset = mouseScreenPos - screenCenter;
        float rotZ = Mathf.Atan2(mouseOffset.y, mouseOffset.x) * Mathf.Rad2Deg;

        Debug.Log($"RotateWeapon: Mouse screen pos: {mouseScreenPos}, Screen center: {screenCenter}");
        Debug.Log($"RotateWeapon: Mouse offset: {mouseOffset}, Raw rotation: {rotZ}°");

        // Center the rotation around 0 degrees
        // When mouse is directly above player, rotZ should be 90°, we want it to be 0°
        // When mouse is to the right, rotZ should be 0°, we want it to be 90°
        // When mouse is to the left, rotZ should be 180°, we want it to be -90°
        float centeredRotation = rotZ - 90f; // Shift by 90° to center it
        
        // Clamp to the desired range
        float clampedRotation = Mathf.Clamp(centeredRotation, minRotation, maxRotation);
        
        Debug.Log($"RotateWeapon: Raw: {rotZ}°, Centered: {centeredRotation}°, Clamped: {clampedRotation}°");
        
        // Update network variable with centered rotation
        syncedRotation.Value = clampedRotation;
        Debug.Log($"Client: Updated synced rotation to {clampedRotation} degrees (centered)");
        
        // Update weapon visual
        UpdateWeaponVisual(clampedRotation);
    }

    private void UpdateWeaponVisual(float rotation)
    {
        // Rotate the RotationPoint to show weapon direction
        if (rotationPoint != null)
        {
            // Rotate the RotationPoint (parent of BulletTransform) to make the reticle rotate around the player
            rotationPoint.rotation = Quaternion.Euler(0, 0, rotation);
            
            Debug.Log($"Weapon visual: Rotated RotationPoint to {rotation}°");
        }
        else
        {
            Debug.LogWarning("RotationPoint is null! Cannot update weapon visual.");
        }
    }

    private void Shooting()
    {
        // Only handle input on the client, send to server
        if (!IsOwner) 
        {
            Debug.Log("Shooting: Not owner, skipping input");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Client: Space pressed, sending shoot request to server. IsOwner: {IsOwner}, IsClient: {IsClient}, IsServer: {IsServer}");
            RequestShootServerRpc();
            Debug.Log("Client: RequestShootServerRpc call completed");
        }
    }

    [ServerRpc]
    private void RequestShootServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server: RequestShootServerRpc called! Sender: {rpcParams.Receive.SenderClientId}, IsServer: {NetworkManager.Singleton.IsServer}");
        
        // Server handles all shooting logic
        if (!canShoot)
        {
            Debug.Log("Server: Cannot shoot - cooldown active");
            return;
        }

        // Calculate direction on server using the BulletTransform's forward direction
        // Use the BulletTransform's right vector (forward direction in 2D)
        Vector2 currentDirection = bulletTransform.right;
        
        Debug.Log($"Server: Using BulletTransform.right as direction: {currentDirection}");
        
        // Verify this matches the BulletTransform direction
        Vector2 bulletTransformDirection = bulletTransform.right;
        Debug.Log($"Server: BulletTransform direction: {bulletTransformDirection}, Should match: {currentDirection}");
        
        // Check if rotation is within allowed range using synced rotation
        // Using original range (-45° to +45°)
        float currentRotation = syncedRotation.Value;
        if (currentRotation >= minRotation && currentRotation <= maxRotation)
        {
            canShoot = false;
            // TEST: Use inverted direction to see if that fixes the issue
            // Calculate bullet rotation based on TOP QUARTER direction
            // The bullet should face the direction it's traveling
            float bulletRotation = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion bulletQuaternion = Quaternion.Euler(0, 0, bulletRotation);
            Debug.Log($"Server: Using TOP QUARTER direction: {currentDirection}");
            Debug.Log($"Server: Bullet rotation: {bulletRotation} degrees, Quaternion: {bulletQuaternion.eulerAngles.z}");
            Debug.Log($"Server: Bullet should face direction: {currentDirection}");
            SpawnBulletServerRpc(bulletTransform.position, bulletQuaternion, currentDirection);
        }
        else
        {
            Debug.Log($"Server: Rotation {currentRotation} not in range [{minRotation}° to {maxRotation}°] - shooting blocked");
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector2 position, Quaternion rotation, Vector2 direction, ServerRpcParams rpcParams = default)
    {
        if (bulletPrefab != null)
        {
            Debug.Log($"Server spawning bullet at {position} with direction {direction}");
            
            // Try to get bullet from object pool first, fallback to instantiate
            NetworkObjectPool networkObjectPool = NetworkObjectPool.Singleton;
            NetworkObject bullet = null;
            
            if (networkObjectPool != null)
            {
                try
                {
                    bullet = networkObjectPool.GetNetworkObject(bulletPrefab, position, rotation);
                    Debug.Log("Server: Got bullet from object pool");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Server: Bullet prefab not in pool, using instantiate fallback. Error: {e.Message}");
                    bullet = null;
                }
            }
            
            if (bullet == null)
            {
                // Fallback to instantiate if pool is not available or bullet not in pool
                Debug.Log("Server: Using instantiate fallback for bullet");
                bullet = Instantiate(bulletPrefab, position, rotation).GetComponent<NetworkObject>();
            }
            
            if (bullet != null)
            {
                // Set shooter ID first
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.shooterClientId = rpcParams.Receive.SenderClientId;
                
                // Spawn the bullet first
                bullet.Spawn(true);
                
                // Set direction after spawning with a small delay to ensure initialization
                StartCoroutine(SetBulletDirectionDelayed(bulletScript, direction));
            }
        }
    }
    
    private System.Collections.IEnumerator SetBulletDirectionDelayed(Bullet bulletScript, Vector2 direction)
    {
        // Wait one frame to ensure the bullet is fully initialized
        yield return null;
        
        // Set the direction
        bulletScript.SetBulletDirection(direction);
    }
}
