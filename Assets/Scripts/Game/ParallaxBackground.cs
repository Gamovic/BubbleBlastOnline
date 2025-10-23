using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos, length;
    [SerializeField]
    private float parallaxEffect = 0.5f;

    private Camera cam;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position.x;
        length = spriteRenderer.bounds.size.x;

        Debug.Log($"ParallaxBackground started on {gameObject.name}");
        
        // Try to find camera in parent first, then by tag, then Camera.main
        cam = GetComponentInParent<Camera>();
        if (cam == null)
        {
            GameObject cameraObj = GameObject.FindWithTag("Camera");
            if (cameraObj != null)
            {
                cam = cameraObj.GetComponent<Camera>();
            }
        }
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        if (cam == null)
        {
            Debug.LogWarning($"ParallaxBackground on {gameObject.name} - Camera not found, will retry...");
            // Start coroutine to keep looking for camera
            StartCoroutine(FindCameraCoroutine());
        }
        else
        {
            Debug.Log($"ParallaxBackground on {gameObject.name} - Camera found: {cam.name}");
        }
    }

    private System.Collections.IEnumerator FindCameraCoroutine()
    {
        while (cam == null)
        {
            // Try to find camera by tag
            GameObject cameraObj = GameObject.FindWithTag("Camera");
            if (cameraObj != null)
            {
                cam = cameraObj.GetComponent<Camera>();
                if (cam != null)
                {
                    Debug.Log($"ParallaxBackground on {gameObject.name} - Camera found via tag: {cam.name}");
                    break;
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FixedUpdate()
    {
        if (cam != null)
        {
            UpdateParallaxPosition(cam.transform.position.x);
        }
        else
        {
            // Try to find camera again if we still don't have one
            if (cam == null)
            {
                GameObject cameraObj = GameObject.FindWithTag("Camera");
                if (cameraObj != null)
                {
                    cam = cameraObj.GetComponent<Camera>();
                    if (cam != null)
                    {
                        Debug.Log($"ParallaxBackground on {gameObject.name} - Camera found in FixedUpdate: {cam.name}");
                    }
                }
            }
        }
    }

    private void UpdateParallaxPosition(float cameraXPosition)
    {
        float distanceFromStart = (cameraXPosition * parallaxEffect);
        float tempDistance = (cameraXPosition * (1 - parallaxEffect));

        Vector3 newPosition = new Vector3(startPos + distanceFromStart, transform.position.y, transform.position.z);
        transform.position = newPosition;

        // Improved background looping to prevent shaking
        if (tempDistance > startPos + length)
        {
            startPos += length;
            Debug.Log($"Parallax {gameObject.name}: Looping right, new startPos: {startPos}");
        }
        else if (tempDistance < startPos - length)
        {
            startPos -= length;
            Debug.Log($"Parallax {gameObject.name}: Looping left, new startPos: {startPos}");
        }
            
        // Debug log every 60 frames to avoid spam
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Parallax on {gameObject.name}: CameraX={cameraXPosition:F2}, NewPos={newPosition.x:F2}, Effect={parallaxEffect}, StartPos={startPos:F2}");
        }
    }

    // Method to set a specific camera (for local camera system)
    public void SetLocalCamera(Camera localCamera)
    {
        cam = localCamera;
    }

    // Method to update parallax effect strength
    public void SetParallaxEffect(float effect)
    {
        parallaxEffect = effect;
    }

    // Method to reset parallax position
    public void ResetParallaxPosition()
    {
        startPos = transform.position.x;
    }

    /*private float startPos, length;
    [SerializeField]
    private float parallaxEffect;
    [SerializeField]
    private Camera cam;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

        // Find the camera within the hierarchy
        FindCameraInHierarchy();

        // Check if 'cam' is assigned properly
        if (cam == null)
        {
            Debug.LogError("Camera reference is not set!");
            return;
        }

        // Check if this script is already attached to the camera
        if (cam.GetComponent<ParallaxBackground>() == null)
        {
            // If not, attach it to the camera
            cam.gameObject.AddComponent<ParallaxBackground>();
        }
    }

    void FixedUpdate()
    {
        ParallaxCamera();
    }

    private void FindCameraInHierarchy()
    {
        // Find the camera by tag or name in the hierarchy
        GameObject foundCamera = GameObject.FindWithTag("MainCamera"); // Change "MainCamera" to the tag or name of your camera

        // Check if a camera was found
        if (foundCamera != null)
        {
            cam = foundCamera.GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("Camera not found in hierarchy!");
        }
    }

    private void ParallaxCamera()
    {
        // Check if 'cam' is assigned properly
        if (cam == null)
        {
            Debug.LogError("Camera reference is not set!");
            return;
        }

        float distanceFromStart = (cam.transform.position.x * parallaxEffect);
        float tempDistance = (cam.transform.position.x * (1 - parallaxEffect));

        transform.position = new Vector3(startPos + distanceFromStart, transform.position.y, transform.position.z);

        if (tempDistance > startPos)
            startPos += length;
        else if (tempDistance < startPos)
            startPos -= length;
    }*/
}
