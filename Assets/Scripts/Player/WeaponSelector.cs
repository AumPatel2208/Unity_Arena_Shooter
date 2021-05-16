using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons {
    Plasma,
    Machine,
}

public class WeaponSelector : MonoBehaviour {
    public Dictionary<Weapons, GameObject> gunModels;

    public  Weapons equippedWeapon;
    private Weapons _prevEquippedWeapons;

    void Start() {
        equippedWeapon = Weapons.Machine;
        _prevEquippedWeapons = Weapons.Plasma;

        var plasma = GameObject.Find("plasma_gun").gameObject;
        var pistol = GameObject.Find("pistol").gameObject;

        gunModels = new Dictionary<Weapons, GameObject>();
        gunModels.Add(Weapons.Plasma, plasma);
        gunModels.Add(Weapons.Machine, pistol);

        foreach (var gun in gunModels.Values) {
            gun.SetActive(false);
        }
        EquipWeapon(Weapons.Machine);
    }
    void Update() {
        // key inputs to change gun
        
        // change equipped weapon and call Equip weapon when they change it
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            equippedWeapon = Weapons.Machine;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            equippedWeapon = Weapons.Plasma;
        }

        if (_prevEquippedWeapons != equippedWeapon) {
            EquipWeapon(equippedWeapon);
        }
    }

    private void EquipWeapon() {
        // play animation of some shit
        gunModels[_prevEquippedWeapons].SetActive(false);
        gunModels[equippedWeapon].SetActive(true);
        _prevEquippedWeapons = equippedWeapon;
    }
    private void EquipWeapon(Weapons gun) {
        equippedWeapon = gun;
        // play animation of some shit
        gunModels[_prevEquippedWeapons].SetActive(false);
        gunModels[equippedWeapon].SetActive(true);
        _prevEquippedWeapons = equippedWeapon;
    }
}