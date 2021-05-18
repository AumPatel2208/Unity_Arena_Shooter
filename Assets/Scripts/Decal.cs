using UnityEngine;

namespace DefaultNamespace {
    public abstract class Decal : MonoBehaviour {
        public  Transform  fakeParent; // https://answers.unity.com/questions/1614882/moving-objects-together-without-parenting.html
        protected Vector2    _positionOffset;
        protected Quaternion _rotationOffset;

        // lifetime in seconds
        public  float lifetime      = 9f;
        private float lifetimeTimer = -1f;

        protected void Update() {
            lifetimeTimer -= Time.deltaTime;
            if (lifetimeTimer < -1) {
                Despawn();
            }

            if (lifetimeTimer > -1) {
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

        protected void OnEnable() {
            lifetimeTimer = lifetime;
        }

        protected abstract void Despawn();


    }
}