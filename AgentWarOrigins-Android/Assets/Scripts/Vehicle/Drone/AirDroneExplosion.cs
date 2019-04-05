using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class AirDroneExplosion : MonoBehaviour
{
    public ParticleSystem ExplosionParticleSystemhigh;
    public ParticleSystem ExplosionParticleSystemLow;
    public float ExplosionStartTime;
    public float ExplosionRate;
    private GameObject m_Player;
    private List<ParticleCollisionEvent> collisionEvents;
    private ParticleSystem m_ParticleSystem;
    private MissionManager m_MissionManager;
    private List<Mission> deathMissions=new List<Mission>();
    private AudioSource m_AudioSource;
    private AudioSource m_ExplosionAudioSourcehigh;
    private ExplosiveForce m_ExplosiveForceHigh;
    private ExplosiveForce m_ExplosiveForceLow;
    private AudioSource m_ExplosionAudioSourceLow;
    private DroneHealth droneHealth;

    // Use this for initialization
    void Start()
    {
        m_MissionManager = MissionManager.Instance;
        ;
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
        collisionEvents = new List<ParticleCollisionEvent>();
        m_AudioSource = GetComponent<AudioSource>();
        if (ExplosionParticleSystemhigh)
        {
            m_ExplosionAudioSourcehigh = ExplosionParticleSystemhigh.GetComponent<AudioSource>();
            m_ExplosiveForceHigh = ExplosionParticleSystemhigh.GetComponent<ExplosiveForce>();
        }
        if (ExplosionParticleSystemLow)
        {
            m_ExplosionAudioSourceLow = ExplosionParticleSystemLow.GetComponent<AudioSource>();
            m_ExplosiveForceLow = ExplosionParticleSystemLow.GetComponent<ExplosiveForce>();
        }
        InvokeRepeating("PlayParticles", ExplosionStartTime, ExplosionRate);
        droneHealth = GetComponentInParent<DroneHealth>();

    }

    private void Update()
    {
        Vector3 relativePos = m_Player.transform.position - transform.position + new Vector3(5, Random.Range(-1.0f,-2.0f), 2);
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
    }

    private void OnParticleCollision(GameObject other)
    {
        m_ParticleSystem.Stop(true);
        m_ParticleSystem.GetCollisionEvents(other, collisionEvents);

        if (QualitySettings.GetQualityLevel() > 1)
        {
            if (ExplosionParticleSystemhigh)
            {
                ExplosionParticleSystemhigh.gameObject.SetActive(true);

                ExplosionParticleSystemhigh.transform.position = collisionEvents[0].intersection;
                ExplosionParticleSystemhigh.transform.rotation = Quaternion.LookRotation(collisionEvents[0].normal);
                if (!ExplosionParticleSystemhigh.isPlaying)
                {

                    ExplosionParticleSystemhigh.Play(true);

                }
                AudioManager.Instance.PlaySound(m_ExplosionAudioSourcehigh);
                DamagePlayer(collisionEvents[0].intersection, m_ExplosiveForceHigh);

            }

        }

        else
        {
            if (ExplosionParticleSystemLow)
            {
                ExplosionParticleSystemLow.gameObject.SetActive(true);

                ExplosionParticleSystemLow.transform.position = collisionEvents[0].intersection;
                ExplosionParticleSystemLow.transform.rotation = Quaternion.LookRotation(collisionEvents[0].normal);
                if (!ExplosionParticleSystemLow.isPlaying)
                {
                    ExplosionParticleSystemLow.Play(true);

                }
                AudioManager.Instance.PlaySound(m_ExplosionAudioSourceLow);
                DamagePlayer(collisionEvents[0].intersection, m_ExplosiveForceLow);

            }

        }
    }

    public void PlayParticles()
    {
        if (droneHealth)
        {
            if (!droneHealth.IsDestroyed && Vector3.Angle(m_Player.transform.forward, transform.position - m_Player.transform.position) < 65)
            {
                m_ParticleSystem.Play(true);
                if (m_ParticleSystem.isPlaying)
                {
                    if (m_AudioSource)
                    {
                        AudioManager.Instance.PlaySound(m_AudioSource);

                    }
                }
            }
        }
    
    }

    //private void DamagePlayer(Vector3 position)
    //{
    //    deathMissions.Clear();
    //    if (m_MissionManager)
    //    {
    //        foreach (Mission mission in m_MissionManager.GetActiveMissions())
    //        {
    //            if (mission.MissionType == MissionType.Die)
    //            {
    //                deathMissions.Add(mission);
    //            }
    //        }
    //    }
    //    if (explosionForce)
    //    {
    //        List<Rigidbody> explodedPlayer =
    //            explosionForce.Explode(LayerMask.NameToLayer("Player"));
    //        foreach (var player in explodedPlayer)
    //        {
    //            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    //            PlayerControl control = player.GetComponent<PlayerControl>();

    //            if (playerHealth)
    //            {
    //                float proximity = (position - player.transform.position).magnitude;
    //                float effect = 1 - (proximity / explosionForce.ExplosionRadius);
    //                if (playerHealth.GetCurrentHealth() > 0)
    //                {
    //                    Debug.Log("taking damage");
    //                    playerHealth.TakeDamage(explosionForce.DamageAtCenter * effect);
    //                }
    //                float currentHealth = playerHealth.GetCurrentHealth();

    //                if (playerHealth.GetCurrentHealth() <= 0)
    //                {
    //                    if (currentHealth <= 0)
    //                    {
    //                        control.Die = true;
    //                        //add deaths of player
    //                        PlayerData.PlayerProfile.NoofDeaths += 1;
    //                        PlayerData.CurrentGameStats.CurrentDeaths++;
    //                        // Debug.Log("no of death missions " + deathMissions.Count);
    //                        if (deathMissions.Count > 0)
    //                            foreach (var mission in deathMissions)
    //                                if (!mission.IsMissionForOneRun)
    //                                {
    //                                    if (PlayerData.PlayerProfile.NoofDeaths ==
    //                                        mission.AmountOrObjectIdToComplete)
    //                                        EventManager.TriggerEvent(mission.MissionTitle);
    //                                }
    //                                else
    //                                {
    //                                    //check for current game stats
    //                                    if (PlayerData.CurrentGameStats.CurrentDeaths ==
    //                                        mission.AmountOrObjectIdToComplete)
    //                                        EventManager.TriggerEvent(mission.MissionTitle);
    //                                }
    //                    }
    //                }
    //            }
    //        }
    //    }
  


    //}

    private void DamagePlayer(Vector3 position,ExplosiveForce explosionForce)
    {
        deathMissions.Clear();
        if (m_MissionManager)
        {
            foreach (Mission mission in m_MissionManager.GetActiveMissions())
            {
                if (mission.MissionType == MissionType.Die)
                {
                    deathMissions.Add(mission);
                }
            }
        }
        if (explosionForce)
        {
            List<Rigidbody> explodedPlayer =
                explosionForce.Explode(LayerMask.NameToLayer("Player"));
            foreach (var player in explodedPlayer)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

                if (playerHealth)
                {
                    float proximity = (position - player.transform.position).magnitude;
                    float effect = 1 - (proximity / explosionForce.ExplosionRadius);
                    if (playerHealth.GetCurrentHealth() > 0)
                    {
                        playerHealth.TakeDamage(explosionForce.DamageAtCenter * effect);
                    }
                    float currentHealth = playerHealth.GetCurrentHealth();

                    if (playerHealth.GetCurrentHealth() <= 0)
                    {
                        if (currentHealth <= 0 && !playerHealth.IsDead)
                        {
                            playerHealth.IsDead = true;
                            //add deaths of player
                            if (Util.IsTutorialComplete())
                            {
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
        }



    }
}
    


