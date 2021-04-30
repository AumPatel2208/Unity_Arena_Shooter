using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanBullet : MonoBehaviour {
    public const string NAME = "HitScanBullet";

    // lifetime in seconds
    public  float lifetime      = 10f;
    private float lifetimeTimer = 0f;

    void Update() {
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer < 0) {
            PoolingManager.Despawn(NAME, gameObject);
        }
    }

    private void OnEnable() {
        lifetimeTimer = lifetime;
    }
}