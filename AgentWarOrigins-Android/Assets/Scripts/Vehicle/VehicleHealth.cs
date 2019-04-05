using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using JetBrains.Annotations;
using UnityEngine;

public class VehicleHealth : Health
{

    [SerializeField]
    [Tooltip("Particle Sounds")]
    protected AudioClip m_ExplosionParticlesSound;


    private AudioSource m_AudioSource;
    private VehiclePathFollower _mVehiclePathFollower;
    private SimpleCarController m_SimpleCarController;
    protected List<Mission> killMissions = new List<Mission>();
    protected List<Mission> explodeVehiclesMissions=new List<Mission>();
    private MissionManager m_MissionManager;
    public LayerMask LayerMaskForExplosion;
    private bool isExploded;
    private ExplosiveForce m_DieParticlesExplosiveForce;
    protected override void Awake()
    {
        base.Awake();
        m_AudioSource = GetComponent<AudioSource>();
        m_SimpleCarController = GetComponent<SimpleCarController>();
        _mVehiclePathFollower=GetComponent<VehiclePathFollower>();
        m_DieParticlesExplosiveForce = m_DieParticles.GetComponent<ExplosiveForce>();
        m_MissionManager=MissionManager.Instance;
    }
    public override void TakeDamage(float damageAmount)
    {
      //  Debug.Log("hitting vehicle");
        TakeDamage(damageAmount, m_DamageMultiplier);
        killMissions.Clear();
        explodeVehiclesMissions.Clear();
        if (m_MissionManager)
        {
            foreach (var mission in m_MissionManager.GetActiveMissions())
            {
                if (mission.MissionType == MissionType.KillEnemies)
                    killMissions.Add(mission);
                if (mission.MissionType == MissionType.ExplodeVehicles)
                {
                    explodeVehiclesMissions.Add(mission);
                }
            }

        }


        if (!IsAlive())
        {
         
            PlayDeathEffects();
            //explode enemis
            _mVehiclePathFollower.enabled = false;
            m_SimpleCarController.enabled = false;
            if (m_DieParticles)
            {
                if (!isExploded)
                {
                    DamageNearByEnemies(m_DieParticlesExplosiveForce.DamageAtCenter);
                    isExploded = true;
                }

            }

            if (Util.IsTutorialComplete())
            {
                PlayerData.PlayerProfile.NoOfEnemyvehiclesDestroyed++;
                PlayerData.CurrentGameStats.CurrentEnemyVehiclesDestroyed++;
                if (explodeVehiclesMissions.Count > 0)
                {
                    foreach (var mission in explodeVehiclesMissions)
                    {
                        if (!mission.IsMissionForOneRun)
                        {
                            if (PlayerData.PlayerProfile.NoOfEnemyvehiclesDestroyed ==
                                mission.AmountOrObjectIdToComplete)
                                EventManager.TriggerEvent(mission.MissionTitle);
                        }
                        else
                        {
                            //check for current game stats
                            if (PlayerData.CurrentGameStats.CurrentEnemyVehiclesDestroyed ==
                                mission.AmountOrObjectIdToComplete)
                                EventManager.TriggerEvent(mission.MissionTitle);
                        }
                    }

                }
            }

        }

    }

    public override void PlayDeathEffects()
    {
        //play any death effects
        if (m_DieParticles)
        {
            m_DieParticles.Play();

        }
        if (m_ExplosionParticlesSound)
        {
            m_AudioSource.clip = m_ExplosionParticlesSound;
            m_AudioSource.Play();
        }
        if (m_DestroyOnDie)
        {
            Destroy(gameObject);
        }
        else
        {           
           //roll the vehicle if moving

        }
    }

    private void DamageNearByEnemies(float damageAmount)
    {

        List<Rigidbody> explodedEnemies =
                 m_DieParticlesExplosiveForce.Explode(LayerMask.NameToLayer("Enemy"));
        foreach (var explodedEnemy in explodedEnemies)
        {
            Health healthCom = explodedEnemy.gameObject.GetComponent<Health>();
            EnemyAI enemyAi = healthCom.gameObject.GetComponent<EnemyAI>();
            if (healthCom)
            {
                float proximity = (transform.position - explodedEnemy.transform.position).magnitude;
                float effect = 1 - (proximity / m_DieParticlesExplosiveForce.ExplosionRadius);
                if (effect >= 0)
                {
                    healthCom.TakeDamage(damageAmount * effect);

                }
                if (healthCom.GetCurrentHealth() <= 0 && !enemyAi.isDieSet)
                {
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
                    GameObject player = GameObject.FindGameObjectWithTag("Player");

                    UiManager.Instance.ShowXPText(player.transform.position, 25, XPType.KilledEnemy);
                    PlayerData.PlayerProfile.NoofEnemieskilled += 1;

                    PlayerData.CurrentGameStats.CurrentKills++;
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
