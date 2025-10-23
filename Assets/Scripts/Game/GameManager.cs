using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [Header("Score UI Elements")]
    public TMP_Text Player1ScoreText;
    public TMP_Text Player2ScoreText;
    public TMP_Text Player1ScoreValue;  // Player 1 score text element
    public TMP_Text Player2ScoreValue;  // Player 2 score text element
    
    public static GameManager Instance;
    
    // Legacy field for backward compatibility
    public TMP_Text scoreText;

    // Network variables for synchronized scores
    public NetworkVariable<int> player1Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> player2Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameManager Instance set in Awake");
        }
        else
        {
            Debug.Log("GameManager Instance already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Ensure this GameManager has a NetworkObject component
        if (GetComponent<NetworkObject>() == null)
        {
            Debug.LogError("GameManager needs a NetworkObject component to work properly!");
            return;
        }
        else
        {
            Debug.Log("GameManager has NetworkObject component");
        }
        
        // Start coroutine to spawn when network is ready
        StartCoroutine(SpawnWhenNetworkReady());
    }
    
    private System.Collections.IEnumerator SpawnWhenNetworkReady()
    {
        // Wait for network to be ready
        while (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Spawn as NetworkObject if not already spawned
        if (GetComponent<NetworkObject>() != null && !GetComponent<NetworkObject>().IsSpawned)
        {
            Debug.Log("Spawning GameManager as NetworkObject...");
            GetComponent<NetworkObject>().Spawn();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        Debug.Log($"GameManager OnNetworkSpawn - IsServer: {IsServer}, IsClient: {IsClient}");
        
        // Subscribe to score changes
        player1Score.OnValueChanged += OnPlayer1ScoreChanged;
        player2Score.OnValueChanged += OnPlayer2ScoreChanged;
        
        // Subscribe to client connection changes
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        
        // Update UI with current values
        UpdateScoreUI();
        UpdateScoreVisibility();
        
        // Force UI update after a short delay to ensure everything is initialized
        Invoke(nameof(ForceUIUpdate), 0.5f);
    }
    
    private void ForceUIUpdate()
    {
        Debug.Log("ForceUIUpdate called");
        UpdateScoreUI();
        UpdateScoreVisibility();
    }
    

    public override void OnNetworkDespawn()
    {
        // Unsubscribe from score changes
        player1Score.OnValueChanged -= OnPlayer1ScoreChanged;
        player2Score.OnValueChanged -= OnPlayer2ScoreChanged;
        
        // Unsubscribe from client connection changes
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        
        base.OnNetworkDespawn();
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected. Updating score visibility.");
        UpdateScoreVisibility();
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected. Updating score visibility.");
        UpdateScoreVisibility();
    }
    
    private void UpdateScoreVisibility()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        
        int connectedClients = NetworkManager.Singleton.ConnectedClients.Count;
        Debug.Log($"Connected clients: {connectedClients}");
        
        // Show Player 1 score when at least 1 client (host)
        if (Player1ScoreValue != null)
        {
            Player1ScoreValue.gameObject.SetActive(connectedClients >= 1);

            Player1ScoreText.gameObject.SetActive(connectedClients >= 1);
        }
        
        // Show Player 2 score when at least 2 clients (host + 1 client)
        if (Player2ScoreValue != null)
        {
            Player2ScoreValue.gameObject.SetActive(connectedClients >= 2);

            Player2ScoreText.gameObject.SetActive(connectedClients >= 2);
        }
        
        Debug.Log($"Score visibility - Player 1: {(connectedClients >= 1 ? "Visible" : "Hidden")}, Player 2: {(connectedClients >= 2 ? "Visible" : "Hidden")}");
    }

    public void IncreasePlayer1Score(int amount)
    {
        if (!IsSpawned) return;
        
        if (IsServer)
        {
            player1Score.Value += amount;
        }
        else
        {
            IncreasePlayer1ScoreServerRpc(amount);
        }
    }

    public void IncreasePlayer2Score(int amount)
    {
        if (!IsSpawned) return;
        
        if (IsServer)
        {
            player2Score.Value += amount;
        }
        else
        {
            IncreasePlayer2ScoreServerRpc(amount);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncreasePlayer1ScoreServerRpc(int amount)
    {
        if (IsServer)
        {
            player1Score.Value += amount;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncreasePlayer2ScoreServerRpc(int amount)
    {
        if (IsServer)
        {
            player2Score.Value += amount;
        }
    }

    private void OnPlayer1ScoreChanged(int previousValue, int newValue)
    {
        UpdateScoreUI();
    }

    private void OnPlayer2ScoreChanged(int previousValue, int newValue)
    {
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        Debug.Log($"UpdateScoreUI called - Player1ScoreValue: {(Player1ScoreValue != null ? "Connected" : "NULL")}, Player2ScoreValue: {(Player2ScoreValue != null ? "Connected" : "NULL")}");
        
        // Try to find UI elements if not assigned
        if (Player1ScoreValue == null)
        {
            Player1ScoreValue = GameObject.Find("Player1ScoreValue")?.GetComponent<TMP_Text>();
        }
        if (Player2ScoreValue == null)
        {
            Player2ScoreValue = GameObject.Find("Player2ScoreValue")?.GetComponent<TMP_Text>();
        }
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        }
        
        // Update the score values using the connected UI elements
        if (Player1ScoreValue != null)
        {
            Player1ScoreValue.text = player1Score.Value.ToString();
            Debug.Log($"Updated Player1ScoreValue to: {player1Score.Value}");
        }
        else if (scoreText != null)
        {
            scoreText.text = player1Score.Value.ToString();
            Debug.Log($"Updated scoreText to: {player1Score.Value}");
        }
        else
        {
            Debug.LogWarning("No UI text elements found for Player 1 score! Make sure UI elements have correct names.");
        }
            
        if (Player2ScoreValue != null)
        {
            Player2ScoreValue.text = player2Score.Value.ToString();
            Debug.Log($"Updated Player2ScoreValue to: {player2Score.Value}");
        }
        else
        {
            Debug.LogWarning("No UI text element found for Player 2 score! Make sure UI elements have correct names.");
        }
            
        Debug.Log($"Score Update - Player 1: {player1Score.Value}, Player 2: {player2Score.Value}");
    }

    // Method to increase score for a specific player
    public void IncreaseScore(int amount, bool isPlayer1)
    {
        Debug.Log($"IncreaseScore called - Amount: {amount}, IsPlayer1: {isPlayer1}");
        
        // Check if we're in a networked game and GameManager is spawned
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient && GetComponent<NetworkObject>() != null && GetComponent<NetworkObject>().IsSpawned)
        {
            Debug.Log("Network is connected and GameManager is spawned, increasing score...");
            if (isPlayer1)
            {
                Debug.Log("Increasing Player 1 score");
                IncreasePlayer1Score(amount);
            }
            else
            {
                Debug.Log("Increasing Player 2 score");
                IncreasePlayer2Score(amount);
            }
        }
        else
        {
            // Fallback: Update score locally for now
            Debug.Log("GameManager not networked, updating score locally...");
            if (isPlayer1)
            {
                player1Score.Value += amount;
                Debug.Log($"Local Player 1 score: {player1Score.Value}");
            }
            else
            {
                player2Score.Value += amount;
                Debug.Log($"Local Player 2 score: {player2Score.Value}");
            }
            UpdateScoreUI();
        }
    }

    // Legacy method for backward compatibility
    public void IncreaseScore(int amount)
    {
        // Default to Player 1 for backward compatibility
        IncreaseScore(amount, true);
    }
    
    // Test method for debugging
    [ContextMenu("Test Score Increase")]
    public void TestScoreIncrease()
    {
        Debug.Log("TestScoreIncrease called");
        IncreaseScore(10, true);
    }
}
