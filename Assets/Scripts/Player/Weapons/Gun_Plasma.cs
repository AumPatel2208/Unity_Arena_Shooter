using UnityEngine;

namespace Player.Weapons {
    public class Gun_Plasma : Gun {

        public Gun_Plasma() : base(0.1f) {
        }

        public override void Fire(ref Ray ray) {
            if(depleteTimer())
                PoolingManager.Spawn(PlasmaBullet.NAME, ray.origin + (1.5f * ray.direction), Quaternion.LookRotation(ray.direction));
        }
    }
}