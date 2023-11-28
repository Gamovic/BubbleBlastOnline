using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private Vector2 playerPos;
    public float minScreenLimitX;
    public float maxScreenLimitX;

    private float horizontalInput;

    private Camera playerCamera;
    private Vector3 mInput = Vector3.zero; // Mouse

    [SerializeField]
    private float speed = 5f;

    private void Initialize()
    {
        playerCamera = Camera.main;
        //transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
        playerPos = transform.position;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Initialize();
    }

    void Update()
    {
        if (!IsOwner || !Application.isFocused) return; // only move on current editor
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        float x = transform.position.x + horizontalInput * speed * Time.deltaTime;

        float clampedPos = Mathf.Clamp(x, minScreenLimitX, maxScreenLimitX);

        transform.position = new Vector2(clampedPos, transform.position.y);

        /*if (!IsOwner || !Application.isFocused) return; // only move on current editor

        mInput.x = Input.mousePosition.x;
        mInput.y = Input.mousePosition.y;
        mInput.z = playerCamera.nearClipPlane;

        Vector3 mouseWorldCoordinates = playerCamera.ScreenToWorldPoint(mInput);
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, Time.deltaTime * speed);

        if (mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - transform.position;
            targetDirection.z = 0f;
            transform.up = targetDirection;
        }*/
    }
}
