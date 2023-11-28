using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace Game
{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField]
        private Camera playerCamera;

        //private GameObject cameraPrefab;
        //private Camera playerCamera;

        [SerializeField]
        private float speed;

        [SerializeField]
        private float minScreenLimitX;

        [SerializeField]
        private float maxScreenLimitX;

        private float horizontalInput;

        private bool useSimulatedInput = false;

        private Vector2 spawnPos = new Vector2(0, 0);

        private NetworkVariable<MyCustomData> randNumber = new NetworkVariable<MyCustomData>(
            new MyCustomData
            {
                someInt = 56,
                someBool = true,
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public struct MyCustomData : INetworkSerializable
        {
            public int someInt;
            public bool someBool;
            public FixedString128Bytes someMessage;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref someInt);
                serializer.SerializeValue(ref someBool);
                serializer.SerializeValue(ref someMessage);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer && IsLocalPlayer)
            {
                // Move the player and spawn the camera on the host
                //Move();
                SpawnCamera();
            }
            else if (IsLocalPlayer)
            {
                // Request the server to move the player and spawn the camera
                RequestMoveAndSpawnCameraServerRpc(spawnPos);
            }


            randNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            {
                Debug.Log(OwnerClientId + "; " + newValue.someInt + "; " + newValue.someBool + "; " + newValue.someMessage);
            };
        }

        [ServerRpc]
        private void RequestMoveAndSpawnCameraServerRpc(Vector3 spawnPosition)
        {
            // Move the player on the server
            Move();

            // Spawn the camera on the server and synchronize it to clients
            OnSpawnCameraServerRpc(spawnPosition);
        }

        [ServerRpc]
        private void OnSpawnCameraServerRpc(Vector3 spawnPosition)
        {
            // Spawn the camera prefab on the server
            if (playerCamera == null)
            {
                GameObject cameraObj = Instantiate(playerCamera.gameObject, spawnPosition, Quaternion.identity);
                NetworkObject networkObject = cameraObj.GetComponent<NetworkObject>();
                networkObject.Spawn();

                // Set the spawned camera as a child of the player
                Transform playerTransform = transform;
                cameraObj.transform.SetParent(playerTransform);
                cameraObj.transform.localPosition = new Vector3(0f, 0f, -10f);

                // Assign the player to the camera script
                FollowCamera followCamera = cameraObj.GetComponent<FollowCamera>();
                followCamera.SetPlayer(playerTransform);

                // Assign the camera to the playerCamera variable
                playerCamera = cameraObj.GetComponent<Camera>();
            }
        }

        [ClientRpc]
        private void OnSpawnCameraClientRpc(Vector3 spawnPosition)
        {

        }

        private void SpawnCamera()
        {
            // Spawn the camera prefab on the host
            OnSpawnCameraServerRpc(spawnPos);
        }

        public void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKeyDown(KeyCode.T))
            {
                //TestServerRpc(new ServerRpcParams());

                TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } });

                /*randNumber.Value = new MyCustomData
                {
                    someInt = 10,
                    someBool = false,
                    someMessage = "Hallo",
                };*/
            }

            if (IsServer && IsLocalPlayer)
            {
                // Move the player and spawn the camera on the host
                Move();
                //SpawnCamera();
            }
            else if (IsLocalPlayer)
            {
                // Request the server to move the player and spawn the camera
                RequestMoveAndSpawnCameraServerRpc(spawnPos);
            }

            /*if (useSimulatedInput)
            {
                // Calculate movement based on simulated input
                float x = transform.position.x + horizontalInput * speed * Time.deltaTime;

                float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

                transform.position = new Vector2(clampedPos, transform.position.y);
            }
            else
            {
                // Use Input for movement during play mode
                horizontalInput = Input.GetAxisRaw("Horizontal");
                float x = transform.position.x + horizontalInput * speed * Time.deltaTime;

                float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

                // Update player position
                transform.position = new Vector2(clampedPos, transform.position.y);
            }*/
        }

        private void Move()
        {
            // Use Input for movement during play mode
            horizontalInput = Input.GetAxisRaw("Horizontal");
            float x = transform.position.x + horizontalInput * speed * Time.deltaTime;

            float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

            // Update player position
            transform.position = new Vector2(clampedPos, transform.position.y);
        }

        public void UpdateInputs(float horizontal)
        {
            horizontalInput = horizontal * speed * Time.deltaTime;
        }

        public void SimulateKeyPress(KeyCode key)
        {
            // Simulate a key press for testing
            UpdateInputs((key == KeyCode.RightArrow) ? 1f : -1f);
            Update();

            // Reset simulated input
            horizontalInput = 0f;
        }

        public void UseSimulatedInput(bool useSimulated)
        {
            useSimulatedInput = useSimulated;
        }

        [ServerRpc]
        private void TestServerRpc(ServerRpcParams serverRpcParams)
        {
            Debug.Log("TestServerRpc " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
        }

        [ClientRpc]
        private void TestClientRpc(ClientRpcParams clientRpcParams)
        {
            Debug.Log("Test Client Rpc");
        }
    }
}
