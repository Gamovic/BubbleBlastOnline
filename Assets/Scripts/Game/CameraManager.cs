using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour
{
    /*[SerializeField]
    private GameObject cameraPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsLocalPlayer)
            SpawnCamera();
    }

    private void SpawnCamera()
    {
        GameObject cameraObject = Instantiate(cameraPrefab, transform.position, Quaternion.identity);
        FollowCamera followCamera = cameraObject.GetComponent<FollowCamera>();

        if (followCamera != null)
            followCamera.SetPlayer(transform);
        else
            Debug.LogError("FollowCamera component not found");
    }*/
}
