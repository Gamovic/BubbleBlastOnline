using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos, length;
    [SerializeField]
    private float parallaxEffect;

    private Camera cam;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

        // Get the Camera component from the parent object
        cam = GetComponentInParent<Camera>();
        if (cam == null)
        {
            Debug.LogError("ParallaxBackground script requires a Camera component in the parent object.");
        }
    }

    private void FixedUpdate()
    {
        if (cam != null)
        {
            ParallaxCamera();
        }
        else
        {
            Debug.LogError("Camera reference is not set!");
        }
    }

    private void ParallaxCamera()
    {
        float distanceFromStart = (cam.transform.position.x * parallaxEffect);
        float tempDistance = (cam.transform.position.x * (1 - parallaxEffect));

        transform.position = new Vector3(startPos + distanceFromStart, transform.position.y, transform.position.z);

        if (tempDistance > startPos)
            startPos += length;
        else if (tempDistance < startPos)
            startPos -= length;
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
