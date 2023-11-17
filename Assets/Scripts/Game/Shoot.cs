using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public float minRotation;
    public float maxRotation;

    public GameObject bullet;
    public Transform bulletTransform;
    public bool canShoot;
    public float timeBetweenShot = 0.3f;
    private float cooldownTimer;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        canShoot = true;
    }

    void Update()
    {
        RotateWeapon();

        Shooting();
    }

    private void RotateWeapon()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Limit rotation
        rotZ = Mathf.Clamp(rotZ, minRotation, maxRotation);

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    private void Shooting()
    {
        if (!canShoot)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > timeBetweenShot)
            {
                canShoot = true;
                cooldownTimer = 0f;
            }
        }

        if (Input.GetMouseButton(0) && canShoot)
        {
            float currentRotation = transform.rotation.eulerAngles.z;

            // Check if current rotation is within the allowed range
            if (currentRotation >= minRotation && currentRotation <= maxRotation)
            {
                canShoot = false;
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
            }
        }
    }
}
