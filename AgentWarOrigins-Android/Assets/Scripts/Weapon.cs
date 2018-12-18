using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons")]
public class Weapon : ScriptableObject
{
    [Tooltip("weapon ID")] public int WeaponId;
    [Tooltip("Weapon Name")] public string WeaponName;
    [Tooltip("Weapon Model")] public GameObject WeaponModel;
    [Tooltip("Rate of fire")] public float RateOfFire;
    [Tooltip("Range of weapon")] public float WeaponRange;
    [Tooltip("Total ammo of weapon")] public float TotalAmmo;
    [Tooltip("Ammo for one Reload")] public float ClipSize;
    [Tooltip("The random spread of the bullets once they are fired")]
    public float
    Spread = 0.01f;
    [Tooltip("WeaponCost")] public int WeaponCost;
    [Tooltip("Lock type")] public LockType LockType;
    [Tooltip("No of Blueprints Required to unlock weapon")]
    public int NoofBluePrints;
    public int NoofUpdrages;
    public int IncreasePercentage;
    public List<LockType> UpgradeLockTypes; //should be ewal to no of upgrades 

    public List<int> UpgradeCosts; //should be equal to no of upgrades


    [Tooltip("Level To Unlock weapon")] public int LevelRequiredToUnlock;
    [Tooltip("WeaponManagement type")] public WeaponType WeaponType;

    [Tooltip(
        "The amount of damage done to the object hit. This only applies to weapons that do not have a projectile")] public float HitscanDamageAmount = 10f;

    [Tooltip("Damage Multiplier of the weapon")] public float DamageMultiplier = 1f;

    [Tooltip("Should the weapon reload automatically once it is out of ammo?")] public bool AutoReload;

    [Tooltip("Optionally specify a shell that should be spawned when the weapon is fired")] public
        GameObject Shell;

    [Tooltip("Optionally specify any particles that should play when the weapon is fired")] public
        ParticleSystem Particles;
    [Tooltip("Optinally specify any bullet impact effects when hit a target")]
    public ParticleSystem ImpactParticles;
    [Tooltip("Optionally specify a sound that should randomly play when the weapon is fired")] public AudioClip[]
        FireSound;

    [Tooltip("If Fire Sound is specified, play the sound after the specified delay")] public float
        FireSoundDelay;

    [Tooltip("Optionally specify a sound that should randomly play when the weapon is fired and out of ammo")] public
        AudioClip[] EmptyFireSound;

    [Tooltip("Optionally specify a sound that should randomly play when the weapon is reloaded")] public AudioClip[]
        ReloadSound;

    [Tooltip("Weapon sprite")] public Sprite WeaponSprite;
}
