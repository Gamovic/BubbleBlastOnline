using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Game
{
    public class Shooting : NetworkBehaviour
    {
        [SerializeField]
        private float minRotation;

        [SerializeField]
        private float maxRotation;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private Transform bulletTransform;

        private Camera playerCamera;
        private Vector3 mousePos;

        [SerializeField]
        private bool canShoot;

        [SerializeField]
        private float timeBetweenShot = 0.5f;
        private float cooldownTimer;


        private void Initialize()
        {
            playerCamera = Camera.main;

            canShoot = true;
        }

        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        void Update()
        {
            if (!IsOwner || !Application.isFocused) return; // only move on current editor

            RotateWeapon();
            Shoot();
        }

        private void RotateWeapon()
        {
            mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 rotation = mousePos - transform.position;

            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            // Limit rotation
            rotZ = Mathf.Clamp(rotZ, minRotation, maxRotation);

            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        private void Shoot()
        {
            if (!IsOwner) return;

            if (!canShoot)
            {
                cooldownTimer += Time.deltaTime;

                if (cooldownTimer > timeBetweenShot)
                {
                    canShoot = true;
                    cooldownTimer = 0f;
                }
            }

            if (IsOwner && Input.GetMouseButton(0) && canShoot)
            {
                float currentRotation = transform.rotation.eulerAngles.z;

                Vector3 currentRot = new Vector3(0f, 0f, currentRotation);

                //SpawnBulletServerRpc(currentDirection);

                canShoot = false;


                //Vector2 currentDirection = Quaternion.Euler(0, 0, currentRotation) * Vector2.right;

                SpawnBulletServerRpc(currentRot);

                /*// Check if current rotation is within the allowed range
                if (currentRotation >= minRotation && currentRotation <= maxRotation)
                {
                    canShoot = false;
                    
                    SpawnBulletServerRpc(currentDirection);
                }*/
            }
        }

        [ServerRpc]
        private void SpawnBulletServerRpc(Vector3 direction)
        {

            // Instantiate the bullet on the server
            GameObject bullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);

            // Set the bullet's direction
            bullet.GetComponent<Bullet>().SetBulletDirection(direction);

            bullet.GetComponent<NetworkObject>().Spawn();

            // Spawn the bullet on the clients using the NetworkSpawnManager
            /*NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Spawn the bullet on the clients using the NetworkSpawnManager
                networkObject.Spawn();
            }*/
        }
    }
}