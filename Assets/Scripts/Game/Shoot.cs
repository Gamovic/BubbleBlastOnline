using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Shoot : NetworkBehaviour
{
    /*private Camera mainCam;
    private Vector3 mousePos;

    [SerializeField]
    private float minRotation;

    [SerializeField]
    private float maxRotation;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform bulletTransform;

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private bool canShoot;

    [SerializeField]
    private float timeBetweenShot = 0.5f;
    private float cooldownTimer;

    //private float syncedRotation;

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector2 position, Quaternion rotation, Vector2 direction)
    {
        NetworkObject bullet = Instantiate(bulletPrefab, position, rotation).GetComponent<NetworkObject>();
        bullet.GetComponent<Bullet>().SetBulletDirection(direction);
        bullet.Spawn(true);
    }*/

    /*[ClientRpc]
    private void SyncRotationClientRpc(float rotation)
    {
        // Set the rotation on the server
        syncedRotation = rotation;

        // Call the ServerRpc to synchronize rotation to other clients
        SyncRotationServerRpc(syncedRotation);
    }*/

    /*public override void OnNetworkSpawn()
    {
        playerCamera.GetComponent<Camera>();

        //mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        canShoot = true;
        // Set the initial syncedRotation when the object is spawned
        //syncedRotation = transform.rotation.eulerAngles.z;
    }

    void Start()
    {
        //mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //canShoot = true;
    }

    void Update()
    {*/
        /*if (!IsOwner)
        {
            // Update rotation for non-owners
            transform.rotation = Quaternion.Euler(0, 0, syncedRotation);
            return;
        }*/
        /*if (!IsOwner) return;

        RotateWeapon();
        Shooting();*/

        // Update the syncedRotation value
        //syncedRotation = transform.rotation.eulerAngles.z;

        /*if (IsServer)
        {
            // Call the ServerRpc to synchronize rotation to other clients
            SyncRotationServerRpc(syncedRotation);
        }
        else
        {
            // Call the ClientRpc to send rotation information to the server
            SyncRotationClientRpc(syncedRotation);
        }*/
    /*}

    private void RotateWeapon()
    {
        mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Limit rotation
        rotZ = Mathf.Clamp(rotZ, minRotation, maxRotation);

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    private void Shooting()
    {
        if (!canShoot)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > timeBetweenShot)
            {
                canShoot = true;
                cooldownTimer = 0f;
            }
        }

        if (Input.GetKeyDown("space")*//*Input.GetMouseButton(0)*/ /*&& canShoot)
        {
            float currentRotation = transform.rotation.eulerAngles.z;
            Vector2 currentDirection = Quaternion.Euler(0, 0, currentRotation) * Vector2.right;

            // Check if current rotation is within the allowed range
            if (currentRotation >= minRotation && currentRotation <= maxRotation)
            {
                canShoot = false;
                //NetworkObject bulletPrefab = Instantiate(bullet, bulletTransform.position, Quaternion.identity).GetComponent<NetworkObject>();
                //bulletPrefab.Spawn(true);

                // Pass rotation of weapon to the server
                //Vector2 bulletPosition = bulletTransform.position;
                //Quaternion bulletRotation = bulletTransform.rotation;

                SpawnBulletServerRpc(bulletTransform.position, bulletTransform.rotation, currentDirection);

                //Instantiate(bullet, bulletTransform.position, Quaternion.identity);
                //bullet.GetComponent<NetworkObject>().Spawn(true);
            }
        }
    }*/
}
