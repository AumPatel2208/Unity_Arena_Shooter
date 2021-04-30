using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBullet : MonoBehaviour {
    public const string NAME = "PlasmaBullet";
    [SerializeField] private float bulletSpeed = 1f;

    // lifetime in seconds
    public float lifetime = 10f;
    private float lifetimeTimer = 0f;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer < 0) {
            PoolingManager.Despawn(NAME, gameObject);
        }
    }

    private void OnEnable() {
        lifetimeTimer = lifetime;
    }
}