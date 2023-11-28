using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Game
{
    public class Player : NetworkBehaviour
    {
        public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();

        public float moveSpeed = 5f;
        private Rigidbody2D rb;

        // Start coroutine to listen for key inputs on the client
        public override void OnNetworkSpawn()
        {
            rb = GetComponent<Rigidbody2D>();
            /*if (IsOwner)
            {
                StartCoroutine(MoveOnClient());
            }*/
        }

        /*IEnumerator MoveOnClient()
        {
            while (true)
            {
                yield return null;
            }
        }*/

        public void Move(Vector2 moveDirection)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                // If the server, move based on input
                Vector2 velocity = moveDirection * moveSpeed/* * Time.deltaTime*/;
                rb.velocity = new Vector2(velocity.x, rb.velocity.y);
                Position.Value = transform.position;

                //Vector2 newPosition = Position.Value + moveDirection * moveSpeed * Time.deltaTime;
                //Position.Value = newPosition;


                // Log the server move
                Debug.Log("Server move: " + Position.Value);
            }
            else
            {
                // If client, request the server to move
                SubmitPositionRequestServerRpc(moveDirection);
            }
            //Debug.Log("Move method called on " + (NetworkManager.Singleton.IsServer ? "Server" : "Client"));
            //if (NetworkManager.Singleton.IsServer)
            //{
                // If the server, move based on input
                //Vector2 newPosition = Position.Value + moveDirection * moveSpeed * Time.deltaTime;
                //Position.Value = newPosition;

                /*// Server logic to move horizontally in 2D
                var randomX = Random.Range(-3f, 3f);
                var newPosition = new Vector3(randomX, 0f);
                transform.position = newPosition;
                Position.Value = newPosition;*/
            //}
            //else
            //{
                // If client, request the server to move
                //SubmitPositionRequestServerRpc(moveDirection);
            //}
        }

        [ServerRpc]
        public void SubmitPositionRequestServerRpc(Vector2 moveDirection, ServerRpcParams rpcParams = default)
        {
            // Server calculates new position based on input
            Vector2 newPosition = Position.Value + moveDirection * moveSpeed;
            Position.Value = newPosition;
            //Vector2 velocity = moveDirection * moveSpeed/* * Time.deltaTime*/;
            //rb.velocity = new Vector2(velocity.x, rb.velocity.y);
            //Position.Value = transform.position;


            // Server generates a new random position and updates it
            //Position.Value = new Vector3(Random.Range(-3f, 3f), 0f);

            Debug.Log("SubmitPositionRequestServerRpc called on Server");

        }

        void Update()
        {
            // Update the local position based on the network variable
            //transform.position = new Vector3(Position.Value.x, Position.Value.y, transform.position.z);
            rb.position = Position.Value;

            /*if (IsOwner)
            {
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                Vector2 moveDirection = new Vector2(horizontalInput, 0f).normalized;

                // Call the Move method based on input
                Move(moveDirection);
            }*/
            // Update the local position based on the network variable
            //transform.position = Position.Value;
        }
    }
}