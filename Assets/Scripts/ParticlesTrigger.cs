using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesTrigger : MonoBehaviour {
    // HACK will have to change as little jank having it here, as this script is multi-functional
    public const string NAME = "prt_PlasmaHit";
    
    private ParticleSystem _particleSystem;


    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void OnEnable() {
        _particleSystem.Play();
    }
}