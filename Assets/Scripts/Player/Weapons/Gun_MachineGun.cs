using UnityEngine;

namespace Player.Weapons {
    public class Gun_MachineGun : Gun {
        
        public Gun_MachineGun() : base(0.05f) {
        }

        public override void Fire(ref Ray ray) {
            if (depleteTimer()) {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    PoolingManager.Spawn(HitScanBullet.NAME, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}