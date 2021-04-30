using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons {
    Plasma,
    Machine,
}

public class WeaponSelector : MonoBehaviour {
    public Weapons equippedWeapon;

    // Start is called before the first frame update
    void Start() {
        equippedWeapon = Weapons.Machine;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            equippedWeapon = Weapons.Machine;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            equippedWeapon = Weapons.Plasma;
        }
    }
}