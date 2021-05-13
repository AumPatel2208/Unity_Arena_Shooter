using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBullet : MonoBehaviour {
    public const             string NAME        = "PlasmaBullet";
    [SerializeField] private float  bulletSpeed = 1f;

    // lifetime in seconds
    public  float lifetime      = 10f;
    private float lifetimeTimer = 0f;

    public bool debug = false;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (!debug) {
            transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
            lifetimeTimer -= Time.deltaTime;
            if (lifetimeTimer < 0) {
                Despawn();
            }
        }
    }

    private void OnEnable() {
        lifetimeTimer = lifetime;
    }

    // on collision
    private void OnTriggerEnter(Collider other) {
        // despawn
        Despawn();
        Debug.Log("ST");
        // apply damage
        
    }
    
    private void OnCollisionEnter(Collision other) {
        // despawn
        Despawn();
        Debug.Log("DT");
        // apply damage
    }


    public void Despawn() {
        PoolingManager.Despawn(NAME, gameObject);
    }
}