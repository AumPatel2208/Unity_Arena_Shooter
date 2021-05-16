using UnityEngine;

public class Grapple : MonoBehaviour {
    [SerializeField] private RaycastHit _grapplePoint;

    [SerializeField] private bool _isGrappling = false;

    [SerializeField] private float _maxDistance = 200f;
    [SerializeField] private float _distance;

    [SerializeField] private float _grappleSpeed = 5f;


    [SerializeField] private Transform _grappleTip;

    public LayerMask grappleLayer;
    private Transform _cameraTransform;
    public LineRenderer _lr;

    private void Start() {
        _lr = GetComponent<LineRenderer>();
        _cameraTransform = Camera.main.transform;
    }

    // Grapple Update not in mono behaviour as want to call from player controller
    public void GUpdate() {
        // get mouse input
        if (Input.GetMouseButtonDown(1)) {
            if (_isGrappling) {
                _isGrappling = !_isGrappling;
            } else {
                // probably change the ground layer mask to something else
                if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out _grapplePoint, _maxDistance)) {
                    _isGrappling = true;
                }
            }
        }
    }

    private void FixedUpdate(){

    }

    private void LateUpdate() {
        DrawRope();
    }

    // Draw the LineRenderer
    public void DrawRope() {
        if (!_isGrappling) {
            _lr.enabled = false;
            return;
        }

        _lr.enabled = true;
        _lr.SetPosition(0, _grappleTip.position);
        _lr.SetPosition(1, _grapplePoint.point);
    }
}
