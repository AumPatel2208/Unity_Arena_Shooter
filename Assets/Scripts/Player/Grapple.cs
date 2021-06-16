using UnityEngine;

public class Grapple : MonoBehaviour {
    [SerializeField] private RaycastHit _hitPoint;

    [SerializeField] private bool _isPerforming = false;

    [SerializeField] private float _maxDistance = 200f;
    [SerializeField] private float _distance;

    private bool _canPerform;
    private bool _canPull;

    private float _grappleSpeed = 0f;

    [SerializeField] private float _zeroGrappleSpeed = 10f;
    [SerializeField] private float _grappleAcceleration = 0.2f;
    [SerializeField] private float _maxGrappleSpeed = 2f;


    [SerializeField] private Transform _grappleTip;

    public LayerMask grappleLayer;
    private Transform _cameraTransform;
    public LineRenderer _lr;

    private Vector3 _grappleVelocity;

    // for grapple indicator
    public GUIStyle style;

    private class OtherObjectProperties {
        public float weight;
        public Rigidbody rb;

        public OtherObjectProperties(float p_weight) {
            weight = p_weight;
        }
    }

    private OtherObjectProperties _otherObject;

    // fake parent stuff for the grapple point
    private Transform
        _fakeParent; // https://answers.unity.com/questions/1614882/moving-objects-together-without-parenting.html

    private Vector3 _positionOffset;
    // private Quaternion _rotationOffset;

    private void Start() {
        _lr = GetComponent<LineRenderer>();
        _cameraTransform = Camera.main.transform;
        _otherObject = new OtherObjectProperties(0f);
    }

    // Grapple Update not in mono behaviour as want to call from player controller
    public void GUpdate() {
        // set can grapple on raycast hit
        // HACK TODO Layermask a little jank // also ignores ground layer (maybe all other layers not tested)
        //if (!_isPerforming && Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out _hitPoint, _maxDistance, LayerMask.NameToLayer("Player"))) {
        if (!_isPerforming && Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out _hitPoint,
            _maxDistance)) {
            if (_hitPoint.rigidbody) {
                _otherObject.weight = _hitPoint.rigidbody.mass * 9.8f; // get the weight for the other object too
                _otherObject.rb = _hitPoint.rigidbody;
                SetFakeParent(_hitPoint.transform);
                _canPull = true;
            }
            else {
                _canPull = false;
            }

            Debug.Log(_hitPoint.collider.gameObject.name);
            // to avoid grappling onto self
            if (_hitPoint.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                _canPerform = true;
            else
                _canPerform = false;
        }
        else {
            _canPerform = false;
        }

        // get mouse input
        if (_canPerform && Input.GetMouseButtonDown(1)) {
            if (!_isPerforming) {
                _isPerforming = true;
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            _isPerforming = false;
        }

        // if is grappling
        if (_isPerforming) {
            CalculateVelocity();

            if (_grappleSpeed < _maxGrappleSpeed)
                _grappleSpeed += _grappleAcceleration * Time.deltaTime;
            else
                _grappleSpeed = _maxGrappleSpeed;

            // add velocity to the other object to pull it
            if (_canPull) {
                _hitPoint.point = _fakeParent.position + _positionOffset;
                _otherObject.rb.velocity += -_grappleVelocity / _otherObject.weight;
                _grappleVelocity = Vector3.zero; // so the player doesnt move
            }
        }
        else {
            _grappleVelocity = Vector3.zero;
            _grappleSpeed = _zeroGrappleSpeed;
        }
    }

    private void CalculateVelocity() {
        // Add Velocity towards the grapple point
        var dir = (_hitPoint.point - transform.position).normalized;
        if (!_canPull) {
            _grappleVelocity = dir * (_grappleSpeed * Time.deltaTime);
        }
        else {
            _grappleVelocity = (dir * (_grappleSpeed * Time.deltaTime));
        }
    }

    private void LateUpdate() {
        DrawRope();
    }

    // Draw the LineRenderer
    private void DrawRope() {
        if (!_isPerforming) {
            _lr.enabled = false;
            return;
        }

        _lr.enabled = true;
        _lr.SetPosition(0, _grappleTip.position);
        _lr.SetPosition(1, _hitPoint.point);
    }

    // fake parent stuff for grapple point
    public void SetFakeParent(Transform hitTransform) {
        //Offset vector
        _positionOffset = _hitPoint.point - hitTransform.position;
        // //Offset rotation
        // _rotationOffset = Quaternion.Inverse(hitTransform.rotation) * transform.rotation;
        //Our fake parent
        _fakeParent = hitTransform;
    }

    public Vector3 GetGrappleVelocity() {
        return _grappleVelocity;
    }

    public bool IsPerforming() {
        return _isPerforming;
    }

    public bool CanPull() {
        return _canPull;
    }

    private void OnGUI() {
        if (_canPerform)
            GUI.Label(new Rect(Screen.width / 2f - 12.5f, Screen.height / 2f - 12.5f, 25, 25), "", style);
    }
}