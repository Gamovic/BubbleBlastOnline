using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace Game
{
    public class PlayerMove : NetworkBehaviour
    {
        // Camera spawning now handled by LocalCameraManager

        [SerializeField]
        private float speed;

        [SerializeField]
        private float minScreenLimitX;

        [SerializeField]
        private float maxScreenLimitX;

        private float horizontalInput;

        private bool useSimulatedInput = false;

        private Vector2 spawnPos = new Vector2(0, 0);

        // Network variable for synchronized position
        public NetworkVariable<Vector2> NetworkPosition = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
            // Subscribe to position changes
            NetworkPosition.OnValueChanged += OnPositionChanged;
            
            if (IsServer && IsLocalPlayer)
            {
                // Move the player (camera spawning handled by LocalCameraManager)
                //Move();
            }
            else if (IsLocalPlayer)
            {
                // Request the server to move the player (camera spawning handled by LocalCameraManager)
                RequestMoveServerRpc(spawnPos);
            }

            randNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            {
                Debug.Log(OwnerClientId + "; " + newValue.someInt + "; " + newValue.someBool + "; " + newValue.someMessage);
            };
        }

        public override void OnNetworkDespawn()
        {
            // Unsubscribe from position changes
            NetworkPosition.OnValueChanged -= OnPositionChanged;
            base.OnNetworkDespawn();
        }

        private void OnPositionChanged(Vector2 previousValue, Vector2 newValue)
        {
            // Update position on all clients
            if (!IsOwner)
            {
                transform.position = newValue;
            }
        }

        [ServerRpc]
        private void RequestMoveServerRpc(Vector3 spawnPosition)
        {
            // Move the player on the server
            Move();

            // Camera spawning is now handled by LocalCameraManager
            // No need to spawn networked cameras anymore
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

            // Send movement input to server for validation
            if (IsLocalPlayer)
            {
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                if (horizontalInput != 0f)
                {
                    SubmitMovementServerRpc(horizontalInput);
                }
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

        [ServerRpc]
        private void SubmitMovementServerRpc(float horizontalInput, ServerRpcParams rpcParams = default)
        {
            // Server validates and applies movement
            if (IsOwner)
            {
                float x = transform.position.x + horizontalInput * speed * Time.deltaTime;
                float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);
                
                Vector2 newPosition = new Vector2(clampedPos, transform.position.y);
                
                // Update position on server
                transform.position = newPosition;
                
                // Update network variable to sync to all clients
                NetworkPosition.Value = newPosition;
                
                Debug.Log($"Server: Player {rpcParams.Receive.SenderClientId} moved to {newPosition}");
            }
        }

        private void Move()
        {
            // This method is now only used for local prediction on the client
            horizontalInput = Input.GetAxisRaw("Horizontal");
            float x = transform.position.x + horizontalInput * speed * Time.deltaTime;

            float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

            // Update player position locally for responsiveness
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
