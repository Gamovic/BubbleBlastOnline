using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class FollowCamera : NetworkBehaviour
{
    //[SerializeField]
    private Transform player;
    [SerializeField]
    private float smoothSpeed = 0.125f;
    [SerializeField]
    private float offsetX;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float desiredX = player.position.x + offsetX;
            Vector3 desiredPos = new Vector3(desiredX, transform.position.y, transform.position.z);
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

            transform.position = smoothedPos;
        }
    }
}