using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class EnemyWeaponManagement : WeaponManagement
{
    private EnemyAI m_EnemyAI;
    public override void Start()
    {

        m_MissionManager = MissionManager.Instance;


        m_RateOfFire = Weapon.RateOfFire;

        m_WeaponRange = Weapon.WeaponRange;

        m_TotalAmmo = Weapon.TotalAmmo;
        m_ClipSize = Weapon.ClipSize;
        m_Spread = Weapon.Spread;
        m_WeaponType = Weapon.WeaponType;
        m_HitscanDamageAmount = Weapon.HitscanDamageAmount;

        m_DamageMultiplier = Weapon.DamageMultiplier;
        m_AutoReload = Weapon.AutoReload;
        m_Shell = Weapon.Shell;
        m_Particles = Weapon.Particles;
        m_FireSound = Weapon.FireSound;
        m_FireSoundDelay = Weapon.FireSoundDelay;
        m_EmptyFireSound = Weapon.EmptyFireSound;
        m_ReloadSound = Weapon.ReloadSound;


        m_ShootDelay = 1 / m_RateOfFire;
        m_LastShootTime = 0;
        m_CurrentAmmoAvailable = m_TotalAmmo;
        m_CurrentReloadAmmoAvailable = m_ClipSize;

        m_EnemyAI = GetComponentInParent<EnemyAI>();
        if (m_EnemyAI)
        {
            m_EnemyAI.enabled = true;

        }
        //foreach (var enemy in enemyAi)
        //{
        //    enemy.enabled = true;
        //}
    }

    public override void Update()
    {
        if (m_CurrentReloadAmmoAvailable <= 0 && IsAmmoAvailable() && m_AutoReload && !isReloadingStarted)
            StartReloading();

    }

    protected override void PlayMuzzleFlash()
    {
        if (m_MuzzleFlash && m_EnemyAI.CurrentEnemyState!=EnemyAI.EnemyState.Reload)
        {
            m_MuzzleFlash.SetActive(true);
        //    m_muzzleFlashComp.Show();

            StartCoroutine(DisableMuzzleFlash());
        }
        else if(m_EnemyAI.CurrentEnemyState==EnemyAI.EnemyState.Reload)
        {
            DisableMuzzleFlash2();
        }
    }


    public override bool Shoot(Vector3 position)
    {
        m_LastShootTime = Time.time;
        m_NoofShotsFired++;
        m_CurrentReloadAmmoAvailable--;
        //m_CurrentAmmoAvailable--;
        //Debug.Log("no of shots fired" + m_NoofShotsFired);
        //Debug.Log("m_CurrentReloadAmmoAvailable" + m_CurrentReloadAmmoAvailable);
        //Debug.Log("current ammo available" + m_CurrentAmmoAvailable);
        var direction = FireDirection(position);
    //    PlayWeaponRecoilAnimation();
        PlayMuzzleFlash();
        if (m_FireSound.Length > 0)
        {
            m_ShootaudioClip = m_FireSound[Random.Range(0, m_FireSound.Length)];
            AudioManager.Instance.PlayOneShotSound(m_audioSource, m_ShootaudioClip);

        }

        RaycastHit hit;
//#if UNITY_EDITOR

//        //   Debug.Log("direction" + direction);
//        Debug.DrawRay(m_FirePoint.position, direction * 100, Color.white, 10f);
//#endif
        killMissions.Clear();
        deathMissions.Clear();
        if (m_MissionManager)
        {
            foreach (var mission in m_MissionManager.GetActiveMissions())
                if (mission.MissionType == MissionType.KillEnemies)
                    killMissions.Add(mission);
                else if (mission.MissionType == MissionType.Die)
                    deathMissions.Add(mission);
        }


        if (Physics.Raycast(m_FirePoint.position, direction, out hit, m_WeaponRange))
            if (hit.collider)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();

                    if (health != null)
                    {
                        float currentHealth = 0;
                        health.TakeDamage(m_HitscanDamageAmount);
                        currentHealth = health.GetCurrentHealth();

                        if (currentHealth <= 0 )
                        {
                            health.PlayDeathEffects();
                            health.IsDead = true;
                            if (Util.IsTutorialComplete())
                            {
                                //add deaths of player
                                PlayerData.PlayerProfile.NoofDeaths += 1;
                                PlayerData.CurrentGameStats.CurrentDeaths++;
                                // Debug.Log("no of death missions " + deathMissions.Count);
                                if (deathMissions.Count > 0)
                                    foreach (var mission in deathMissions)
                                        if (!mission.IsMissionForOneRun)
                                        {
                                            if (PlayerData.PlayerProfile.NoofDeaths ==
                                                mission.AmountOrObjectIdToComplete)
                                                EventManager.TriggerEvent(mission.MissionTitle);
                                        }
                                        else
                                        {
                                            //check for current game stats
                                            if (PlayerData.CurrentGameStats.CurrentDeaths ==
                                                mission.AmountOrObjectIdToComplete)
                                                EventManager.TriggerEvent(mission.MissionTitle);
                                        }
                            }
                       
                        }

                    }
                }
             
            }
        return true;
    }
}
