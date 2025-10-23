using UnityEngine;
using Unity.Netcode;

public class NetworkErrorSuppressor : MonoBehaviour
{
    private void Start()
    {
        // Suppress the specific timing error
        Application.logMessageReceived += OnLogMessageReceived;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
        // Suppress the specific NetworkTransform timing error
        if (logString.Contains("renderTime was before m_StartTimeConsumed") && 
            logString.Contains("BufferedLinearInterpolator"))
        {
            // Don't log this specific error
            return;
        }
        
        // Log other messages normally
        Debug.unityLogger.Log(type, logString);
    }
}
