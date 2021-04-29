using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour {
    private Rigidbody _rigidbody;
    private Collider _boxCollider;

    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Gey");
        
        _rigidbody.constraints = RigidbodyConstraints.None;
        _boxCollider.isTrigger = false;
    }
}