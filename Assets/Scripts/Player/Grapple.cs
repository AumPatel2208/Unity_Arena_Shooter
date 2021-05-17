using UnityEngine;

public class Grapple : MonoBehaviour {
    [SerializeField] private RaycastHit _grapplePoint;

    [SerializeField] private bool _isGrappling = false;

    [SerializeField] private float _maxDistance = 200f;
    [SerializeField] private float _distance;

    private float _grappleSpeed = 0f;

    [SerializeField] private float _zeroGrappleSpeed = 10f;
    [SerializeField] private float _grappleAcceleration = 0.2f;
    [SerializeField] private float _maxGrappleSpeed = 2f;


    [SerializeField] private Transform _grappleTip;

    public LayerMask grappleLayer;
    private Transform _cameraTransform;
    public LineRenderer _lr;

    private Vector3 _grappleVelocity;

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

        if (_isGrappling) {
            CalculateVelocity();

            if (_grappleSpeed < _maxGrappleSpeed)
                _grappleSpeed += _grappleAcceleration * Time.deltaTime;
            else
                _grappleSpeed = _maxGrappleSpeed;

        } else {
            _grappleVelocity = Vector3.zero;
            _grappleSpeed = _zeroGrappleSpeed;
        }
    }

    private void CalculateVelocity() {
        // Add Velocity towards the grapple point
        var dir = (_grapplePoint.point - transform.position).normalized;

        _grappleVelocity = dir * _grappleSpeed * Time.deltaTime;
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

    public Vector3 GetGrappleVelocity() {
        return _grappleVelocity;
    }
}
