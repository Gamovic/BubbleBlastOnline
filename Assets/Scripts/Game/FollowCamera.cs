using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float offsetX;

    void LateUpdate()
    {
        float desiredX = player.position.x + offsetX;
        Vector3 desiredPos = new Vector3(desiredX, transform.position.y, transform.position.z);
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        transform.position = smoothedPos;
    }
}
