using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos, length;
    public float parallaxEffect;
    public GameObject cam;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        ParallaxCamera();
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
}
