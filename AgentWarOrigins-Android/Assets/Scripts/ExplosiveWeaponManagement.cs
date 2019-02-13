using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class ExplosiveWeaponManagement : WeaponManagement
{
    public ParticleSystem ExplosionParticles;

    public override void Awake()
    {
        base.Awake();
        GameObject explosion = GameObject.Find("GroundExplosion");
        if (explosion)
        {
            ExplosionParticles = explosion.GetComponent<ParticleSystem>();

        }

    }


    // public LayerMask ExplosionLayerMask;
    public override bool Shoot(Vector3 position)
    {

        m_LastShootTime = Time.time;
        m_NoofShotsFired++;
        m_CurrentReloadAmmoAvailable--;
     //   m_CurrentAmmoAvailable--;
        //Debug.Log("no of shots fired" + m_NoofShotsFired);
        //Debug.Log("m_CurrentReloadAmmoAvailable" + m_CurrentReloadAmmoAvailable);
        //Debug.Log("current ammo available" + m_CurrentAmmoAvailable);
        var direction = FireDirectionSpread(position);
        //PlayWeaponRecoilAnimation();
        PlayMuzzleFlash();
        PlayFireParticles();
        if (m_FireSound.Length > 0)
        {
            m_ShootaudioClip = m_FireSound[Random.Range(0, m_FireSound.Length)];
            m_audioSource.pitch = Random.Range(0.5f, 1.5f);
            AudioManager.Instance.PlaySound(m_audioSource, m_ShootaudioClip, m_FireSoundDelay);
        }
#if UNITY_EDITOR

        //   Debug.Log("direction" + direction);
        Debug.DrawRay(m_FirePoint.position, direction, Color.white, 10f);
#endif
        killMissions.Clear();
        deathMissions.Clear();
        foreach (Mission mission in m_MissionManager.GetActiveMissions())
        {
            if (mission.MissionType == MissionType.KillEnemies)
            {
                killMissions.Add(mission);
            }
            else if (mission.MissionType == MissionType.Die)
            {
                deathMissions.Add(mission);
            }
        }

        RaycastHit[] raycastHits=null;
        raycastHits=Physics.RaycastAll(m_FirePoint.position, direction, m_WeaponRange, m_LayerMask);
       
            //Debug.Log(hit.collider.name + "from weapon script");

        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider)
            {
                Debug.Log(hit.collider.name);
              
                    //play explosion 
                    if (ExplosionParticles)
                    {
                        ExplosionParticles.gameObject.transform.position = hit.point;
                        ExplosionParticles.Play(true);

                        if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Ground"))
                        {

                        //  ParticleSystem explosion = Instantiate(ExplosionParticles, hit.point, Quaternion.identity) as ParticleSystem;
                        ExplosiveForce explosionForce = ExplosionParticles.GetComponent<ExplosiveForce>();
                        List<Rigidbody> explodedEnemies =
                    explosionForce.Explode(LayerMask.NameToLayer("Enemy"));
                        foreach (var explodedEnemy in explodedEnemies)
                        {

                            Health healthCom = explodedEnemy.gameObject.GetComponent<Health>();
                            EnemyAI enemyAi = healthCom.gameObject.GetComponent<EnemyAI>();
                            if (healthCom && !enemyAi.isDieSet)
                            {
                                float proximity = (hit.point - explodedEnemy.transform.position).magnitude;
                                float effect = 1 - (proximity / explosionForce.ExplosionRadius);
                                if (healthCom.GetCurrentHealth() > 0)
                                {
                                    healthCom.TakeDamage(m_HitscanDamageAmount * effect);

                                }
                                if (healthCom.GetCurrentHealth() <= 0)
                                {
                                    healthCom.PlayDeathEffects();
                                    GameManager.noOfEnemiesOnScreen--;
                                    PlayerData.PlayerProfile.PlayerXp += 25; //xp for kill
                                    PlayerData.PlayerProfile.CurrentLevelXp += 25;
                                    PlayerData.CurrentGameStats.CurrentXpEarned += 25;

                                    if (PlayerData.PlayerProfile.CurrentLevelXp >=
                                        DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel)
                                            .XpRequiredToReachNextLevel)
                                    {
                                        PlayerData.PlayerProfile.CurrentLevelXp -=
                                            DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel)
                                                .XpRequiredToReachNextLevel;
                                        PlayerData.PlayerProfile.CurrentLevel++;
                                    }
                                    //    GameObject player = GameObject.FindGameObjectWithTag("Player");

                                    UiManager.Instance.ShowXPText(m_player.transform.position, 25, XPType.KilledEnemy);
                                    PlayerData.PlayerProfile.NoofEnemieskilled += 1;

                                    PlayerData.CurrentGameStats.CurrentKills++;
                                    UiManager.Instance.UpdateCurrentKills(PlayerData.CurrentGameStats.CurrentKills);
                                    if (killMissions.Count > 0)
                                    {

                                        foreach (Mission mission in killMissions)
                                        {
                                            if (!mission.IsMissionForOneRun)
                                            {
                                                if (PlayerData
                                                    .PlayerProfile.NoofEnemieskilled == mission.AmountOrObjectIdToComplete)
                                                {

                                                    EventManager.TriggerEvent(mission.MissionTitle);
                                                }
                                            }
                                            else
                                            {
                                                //check for current game stats
                                                if (PlayerData.CurrentGameStats.CurrentKills == mission.AmountOrObjectIdToComplete)
                                                {
                                                    EventManager.TriggerEvent(mission.MissionTitle);
                                                }

                                            }

                                        }
                                    }
                                    enemyAi.isDieSet = true;
                                }

                            }

                        }
                    }


                }
                if (hit.collider.CompareTag("EnemyDrone"))
                {
                    Health health = hit.collider.GetComponent<Health>();
                    if (health != null)
                    {
                        float currentHealth = health.GetCurrentHealth();
                        if (currentHealth > 0)
                        {
                            health.TakeDamage(m_HitscanDamageAmount);
                        }
                        currentHealth = health.GetCurrentHealth();
                        if (currentHealth < 0)
                        {
                            PlayerData.PlayerProfile.PlayerXp += 10; //enemy dronekilled xp
                            PlayerData.PlayerProfile.CurrentLevelXp += 10;
                            //current game xp 
                            PlayerData.CurrentGameStats.CurrentXpEarned += 10;
                            if (PlayerData.PlayerProfile.CurrentLevelXp >=
                                DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel)
                                    .XpRequiredToReachNextLevel)
                            {
                                PlayerData.PlayerProfile.CurrentLevelXp -=
                                    DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel)
                                        .XpRequiredToReachNextLevel;
                                PlayerData.PlayerProfile.CurrentLevel++;
                            }
                            PlayerData.PlayerProfile.NoOfEnemyDronesDestroyed++;
                            PlayerData.CurrentGameStats.CurrentDronesDestroyed++;
                            UiManager.Instance.ShowXPText(m_player.transform.position, 10, XPType.DestroyedDrone);

                            if (destroyDroneMissions.Count > 0)
                                foreach (var mission in destroyDroneMissions)
                                    if (!mission.IsMissionForOneRun)
                                    {
                                        if (PlayerData.PlayerProfile.NoOfEnemyDronesDestroyed ==
                                            mission.AmountOrObjectIdToComplete)
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                                    else
                                    {
                                        //check for current game stats
                                        if (PlayerData.CurrentGameStats.CurrentDronesDestroyed ==
                                            mission.AmountOrObjectIdToComplete)
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                        }

                        //drone misions
                        //   if()
                    }
                }
                if (hit.collider.isTrigger)
                {
                    VehicleHealth health = hit.collider.GetComponent<VehicleHealth>();
                    if (health)
                    {
                        health.TakeDamage(m_HitscanDamageAmount);

                    }
                }

            }
        }
         
        
        return true;
    }
    
}
