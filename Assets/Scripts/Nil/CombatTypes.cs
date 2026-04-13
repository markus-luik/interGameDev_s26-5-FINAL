using UnityEngine;

// Defines the high-level category of attack used to hit a target.
public enum WeaponType
{
    A,
    B
}

// Carries hit context from attacker/projectile to the hit receiver.
public struct HitInfo
{
    public WeaponType weaponType;
    public GameObject source;
}
