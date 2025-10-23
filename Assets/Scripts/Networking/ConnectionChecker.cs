using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionChecker : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    private void Awake()
    {
        m_NetworkManager = GetComponentInParent<NetworkManager>();
        if (m_NetworkManager != null)
        {
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            m_NetworkManager.ConnectionApprovalCallback = ApprovalCheck;
            m_NetworkManager.OnClientConnectedCallback += OnClientConnectedCallBack;
            Debug.Log("ConnectionChecker: Connection approval callback set in Awake");
        }
        else
        {
            Debug.LogError("ConnectionChecker: NetworkManager not found!");
        }
    }

    private void OnClientConnectedCallBack(ulong obj)
    {

    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log($"Connection approval request received. Payload length: {request.Payload.Length}");
        
        var clientPass = System.Text.Encoding.ASCII.GetString(request.Payload);
        Debug.Log($"Client password: '{clientPass}'");

        if (clientPass == "room password")
        {
            response.CreatePlayerObject = true;
            response.Approved = true;
            response.Reason = "correct pass";
            Debug.Log("Connection approved! Creating player object for client.");
        }
        else
        {
            response.Approved = false;
            response.Reason = "incorrect pass";
            Debug.Log($"Connection rejected! Expected 'room password', got '{clientPass}'");
        }
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        if (!m_NetworkManager.IsServer && m_NetworkManager.DisconnectReason != string.Empty)
        {
            print("and should be here..");
            Debug.Log($"Approval Declined Reason: {m_NetworkManager.DisconnectReason}");
        }
    }
}
