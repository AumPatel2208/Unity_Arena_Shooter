using Player.Weapons;
using UnityEngine;

namespace Player {
    public class Weapon : MonoBehaviour {
       private Camera _camera;
           public  Ray    ray;
           public  float  debugRaycastLength;
       
       
           [SerializeField] private Transform gunTip;
       
           private                  WeaponSelector _weaponSelector;
       
           // is 
           private bool  isFiring  = false;

           private Gun[] _guns;
       
           private void Start() {
               _camera = GetComponentInChildren<Camera>();
               _weaponSelector = GetComponent<WeaponSelector>();
               
               // Create guns
               _guns = new Gun[2];
               _guns[(int) global::Weapons.Plasma] =new Gun_Plasma();
               _guns[(int) global::Weapons.Machine] = new Gun_MachineGun();
           }
       
           private void Update() {
               ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
               Debug.DrawRay(ray.origin, ray.direction * debugRaycastLength, Color.red);
       
               if (Input.GetMouseButtonDown(0)) {
                   isFiring = true;
               }
       
               if (Input.GetMouseButtonUp(0)) {
                   isFiring = false;
               }
       
               if (isFiring) {
                   _guns[(int) _weaponSelector.equippedWeapon].Fire(ref ray);
               }
           }

    }
}