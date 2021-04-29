using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public const string NAME = "Bullet";
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
    }

    private void OnEnable() {
        lifetimeTimer = lifetime;
    }
}