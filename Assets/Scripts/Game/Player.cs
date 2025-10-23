using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Game
{
    public class Player : NetworkBehaviour
    {
        public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();
        public NetworkVariable<Vector2> Velocity = new NetworkVariable<Vector2>();
        public NetworkVariable<bool> IsVisible = new NetworkVariable<bool>(true);

        public float moveSpeed = 5f;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        public override void OnNetworkSpawn()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Subscribe to network variable changes
            Position.OnValueChanged += OnPositionChanged;
            Velocity.OnValueChanged += OnVelocityChanged;
            IsVisible.OnValueChanged += OnVisibilityChanged;
            
            // Set initial position
            if (IsServer)
            {
                Position.Value = transform.position;
            }
        }

        public override void OnNetworkDespawn()
        {
            // Unsubscribe from network variable changes
            Position.OnValueChanged -= OnPositionChanged;
            Velocity.OnValueChanged -= OnVelocityChanged;
            IsVisible.OnValueChanged -= OnVisibilityChanged;
            
            base.OnNetworkDespawn();
        }

        private void OnPositionChanged(Vector2 previousValue, Vector2 newValue)
        {
            if (!IsOwner)
            {
                transform.position = newValue;
            }
        }

        private void OnVelocityChanged(Vector2 previousValue, Vector2 newValue)
        {
            if (!IsOwner)
            {
                rb.velocity = newValue;
            }
        }

        private void OnVisibilityChanged(bool previousValue, bool newValue)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = newValue;
            }
        }

        public void Move(Vector2 moveDirection)
        {
            if (IsOwner)
            {
                // Calculate new velocity
                Vector2 newVelocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
                
                // Apply movement locally for responsiveness
                rb.velocity = newVelocity;
                
                // Update network variables
                if (IsServer)
                {
                    Position.Value = transform.position;
                    Velocity.Value = rb.velocity;
                }
                else
                {
                    // Send movement to server
                    SubmitMovementServerRpc(moveDirection, transform.position);
                }
            }
        }

        [ServerRpc]
        public void SubmitMovementServerRpc(Vector2 moveDirection, Vector2 currentPosition, ServerRpcParams rpcParams = default)
        {
            // Server validates and applies movement
            Vector2 newVelocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            rb.velocity = newVelocity;
            
            // Update network variables
            Position.Value = currentPosition;
            Velocity.Value = rb.velocity;
        }

        void Update()
        {
            if (IsOwner)
            {
                // Handle input for owner
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                Vector2 moveDirection = new Vector2(horizontalInput, 0f).normalized;
                
                if (moveDirection.magnitude > 0)
                {
                    Move(moveDirection);
                }
                
                // Update position on server
                if (IsServer)
                {
                    Position.Value = transform.position;
                    Velocity.Value = rb.velocity;
                }
            }
            else
            {
                // For non-owners, interpolate position for smooth movement
                if (Vector2.Distance(transform.position, Position.Value) > 0.1f)
                {
                    transform.position = Vector2.Lerp(transform.position, Position.Value, Time.deltaTime * 10f);
                }
            }
        }

        // Method to make player visible/invisible (for respawning, etc.)
        [ServerRpc]
        public void SetVisibilityServerRpc(bool visible)
        {
            IsVisible.Value = visible;
        }
    }
}