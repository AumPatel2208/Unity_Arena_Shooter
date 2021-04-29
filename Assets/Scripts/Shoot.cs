using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
    private Camera _camera;
    public Ray ray;
    public float debugRaycastLength;
    [SerializeField]
    private Transform gunTip;

    public float fireRate = 0.05f;
    private float fireTimer = 0f;
    private bool isFiring = false;

    private void Start() {
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update() {
        ray = _camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * debugRaycastLength, Color.red);

        if (Input.GetMouseButtonDown(0)) {
            isFiring = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            isFiring = false;
            fireTimer = 0f;
        }

        if (isFiring) {
            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime;
            }
            else {
                PoolingManager.Spawn(Bullet.NAME, gunTip.position, Quaternion.LookRotation(ray.direction));
                fireTimer = fireRate;
            }
        }
    }
}