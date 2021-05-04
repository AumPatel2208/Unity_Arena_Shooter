using UnityEngine;

namespace Player.Weapons {
    public abstract class Gun {
        public  float fireRate;
        protected float _fireTimer;

        protected Gun(float fireRate) {
            this.fireRate = fireRate;
            _fireTimer = 0;
        }

        protected bool depleteTimer() {
            if (_fireTimer > 0) {
                _fireTimer -= Time.deltaTime;
                return false;
            }

            _fireTimer = fireRate;
            return true;
        }

        public abstract void Fire(ref Ray ray);
    }
}