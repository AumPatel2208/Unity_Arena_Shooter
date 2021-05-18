using UnityEngine;

public class HitScanBullet : MonoBehaviour {
    public const string     NAME = "HitScanBullet";
    public       Transform  fakeParent; // https://answers.unity.com/questions/1614883/moving-objects-together-without-parenting.html
    private      Vector3    _positionOffset;
    private      Quaternion _rotationOffset;
    
    // lifetime in seconds
    public  float lifetime      = 10f;
    private float lifetimeTimer = 0f;

    void Update() {
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer < 0) {
            PoolingManager.Despawn(NAME, gameObject);
        }
        if (lifetimeTimer > 0) {
            if (fakeParent != null) {
                transform.position = fakeParent.position;
                
                // with the offsets
                transform.rotation = fakeParent.rotation * _rotationOffset;
                transform.Translate(_positionOffset);
            }
        }
    }

    public void SetFakeParent(Transform p) {
        //Offset vector
        _positionOffset = transform.InverseTransformPoint(transform.position) - transform.InverseTransformPoint(p.position);
        //Offset rotation
        _rotationOffset = Quaternion.Inverse(p.rotation) * transform.rotation;
        //Our fake parent
        fakeParent = p;
    }
    
    private void OnEnable() {
        lifetimeTimer = lifetime;
    }
}
