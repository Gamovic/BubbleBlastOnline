using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SimpleCameraTest : MonoBehaviour
{
    public GameObject cameraPrefab;
    public GameObject backgroundPrefab;
    
    private void Start()
    {
        Debug.Log("SimpleCameraTest started");
        
        // Only run on clients
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server detected, skipping camera test");
            return;
        }
        
        StartCoroutine(TestCameraSpawn());
    }
    
    private IEnumerator TestCameraSpawn()
    {
        Debug.Log("Waiting 3 seconds...");
        yield return new WaitForSeconds(3f);
        
        Debug.Log("Attempting to spawn camera...");
        
        if (cameraPrefab != null)
        {
            GameObject camera = Instantiate(cameraPrefab);
            camera.tag = "Camera";
            Debug.Log("Camera spawned successfully!");
        }
        else
        {
            Debug.LogError("Camera prefab is null!");
        }
        
        if (backgroundPrefab != null)
        {
            GameObject background = Instantiate(backgroundPrefab);
            Debug.Log("Background spawned successfully!");
        }
        else
        {
            Debug.LogError("Background prefab is null!");
        }
    }
}
