using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
    private Camera _camera;
    public  Ray    ray;
    public  float  debugRaycastLength;


    [SerializeField] private Transform gunTip;

    [SerializeField] private ParticleSystem _muzzleFlash;
    private                  WeaponSelector _weaponSelector;

    public  float fireRate  = 0.05f;
    private float fireTimer = 0f;
    private bool  isFiring  = false;


    private void Start() {
        _camera = GetComponentInChildren<Camera>();
        _weaponSelector = GetComponent<WeaponSelector>();
        _muzzleFlash.Stop();
    }

    private void Update() {
        // ray = _camera.ScreenPointToRay(Input.mousePosition);
        ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
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
                Fire();
                fireTimer = fireRate;
            }
        }
    }

    private void Fire() {
        _muzzleFlash.Play();
        switch (_weaponSelector.equippedWeapon) {
            case Weapons.Machine:
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    PoolingManager.Spawn(HitScanBullet.NAME, hit.point, Quaternion.LookRotation(hit.normal));
                break;
            case Weapons.Plasma:
                PoolingManager.Spawn(PlasmaBullet.NAME, ray.origin + (1.5f * ray.direction), Quaternion.LookRotation(ray.direction));
                break;
        }
    }
}