using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
    [SerializeField] private RaycastHit _grapplePoint;

    [SerializeField] private bool _isGrappling = false;

    [SerializeField] private float _maxDistance = 20f;
    [SerializeField] private float _distance;

    [SerializeField] private float _grappleSpeed = 5f;

    private Transform _cameraTransform;
    
    // public Grapple(Transform cameraTrans, float mDistance = 20f, float gSpeed = 5f) {
    //     _maxDistance = mDistance;
    //     
    //     if (_grappleSpeed != 0f)
    //         _grappleSpeed = gSpeed;
    //
    //     _cameraTransform = cameraTrans;
    // }

    // Grapple Update not in mono behaviour as want to call from player controller
    public void GUpdate() {
        // get mouse input
        if (Input.GetMouseButtonDown(1)) {
            // probably change the ground layer mask to something else
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out _grapplePoint, _maxDistance,LayerMask.NameToLayer("Ground"))) {
            }
            Debug.Log("GHe");
        }
        
        DrawRope();
    }

    public void DrawRope() {
        
    }
}