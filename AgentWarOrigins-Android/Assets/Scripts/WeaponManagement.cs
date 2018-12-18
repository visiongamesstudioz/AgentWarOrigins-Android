using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EndlessRunner
{
    public class WeaponManagement : MonoBehaviour, IReload
    {
        public Weapon Weapon;

        [Tooltip("Rate of fire")] protected float m_RateOfFire;
        [Tooltip("Range of weapon")] protected float m_WeaponRange;
        [Tooltip("")] protected float m_TotalAmmo;
        [Tooltip("Ammo for one Reload")] protected float m_ClipSize;
        [Tooltip("FirePoint Transform")] [SerializeField] protected Transform m_FirePoint;
        [SerializeField] protected Transform m_ParticlePosition;
        [Tooltip("The random spread of the bullets once they are fired")] protected float
            m_Spread = 0.01f;

        [Tooltip("WeaponManagement type")] protected WeaponType m_WeaponType;

        [Tooltip(
            "The amount of damage done to the object hit. This only applies to weapons that do not have a projectile")] protected float m_HitscanDamageAmount = 10f;

        [Tooltip("Damage Multiplier of the weapon")] protected float m_DamageMultiplier = 1f;

        [Tooltip("Should the weapon reload automatically once it is out of ammo?")] protected bool m_AutoReload;

        [Tooltip("Optionally specify a shell that should be spawned when the weapon is fired")] protected
            GameObject m_Shell;

        [Tooltip("Shell Parent")] [SerializeField] protected Transform m_ShellParent;
        [Tooltip("Loaction of shell spawmed")] [SerializeField] protected Transform m_ShellLocation;
        [SerializeField] protected Vector3 m_ShellForce;
        [Tooltip("If Shell is specified, the force is the amount of torque applied to the shell when it spawns")] [SerializeField] protected Vector3 m_ShellTorque;
        [SerializeField] [Tooltip("Optionally specify a muzzle flash that should appear when the weapon is fired")] protected GameObject m_MuzzleFlash;

        [SerializeField] [Tooltip(
            "If Muzzle Flash is specified, the location is the position and rotation that the muzzle flash spawns at")] protected Transform m_MuzzleFlashLocation;

        [Tooltip("Optionally specify any particles that should play when the weapon is fired")] protected
            ParticleSystem m_Particles;
        protected ParticleSystem m_ImapctParticles;
        [Tooltip("Optionally specify a sound that should randomly play when the weapon is fired")] protected AudioClip[]
            m_FireSound;

        [Tooltip("If Fire Sound is specified, play the sound after the specified delay")] protected float
            m_FireSoundDelay;

        [Tooltip("Optionally specify a sound that should randomly play when the weapon is fired and out of ammo")] protected AudioClip[] m_EmptyFireSound;

        [Tooltip("Optionally specify a sound that should randomly play when the weapon is reloaded")] protected
            AudioClip[] m_ReloadSound;

        [SerializeField] [Tooltip("Layers to Shoot")] protected LayerMask m_LayerMask;
        protected bool m_CanFire;
        protected float m_ShootDelay;
        protected float m_LastShootTime;
        protected float m_NoofShotsFired;
        protected Animator m_Animator;
        protected float m_CurrentAmmoAvailable;
        protected float m_CurrentReloadAmmoAvailable;
        protected bool isReloading;
        protected bool isReloadingStarted;

        protected bool isReloadingEnded = true;
        protected AudioSource m_audioSource;
        protected AudioClip m_ShootaudioClip;
        protected AudioClip m_EmptyShootAudioClip;
        protected AudioClip m_reloadAudioClip;
        protected MissionManager m_MissionManager;
        protected List<Mission> killMissions = new List<Mission>();
        protected List<Mission> deathMissions = new List<Mission>();
        protected List<Mission> destroyDroneMissions = new List<Mission>();
        
        protected bool IsEnemyDead;
        protected GameObject m_player;
        protected MuzzleFlash m_muzzleFlashComp;
        protected Rigidbody m_RigidBody;
        public Transform FirePoint
        {
            get { return m_FirePoint; }
        }


        public WeaponType CurrentWeaponType
        {
            get { return m_WeaponType; }
        }

        protected Transform ParticlePosition
        {
            get { return m_ParticlePosition; }
        }

        private void OnEnable()
        {
            if (isReloading)
            {
                StopReloading();
            }
        }

        public virtual void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_audioSource = GetComponent<AudioSource>();
            m_RigidBody = GetComponent<Rigidbody>();
            m_player=GameObject.FindGameObjectWithTag("Player");
            if (Weapon == null)
                Debug.Log("weapon is null");

            if (PlayerData.PlayerProfile!=null)
            {
                var upgradedWeaponDictionary = PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted;
                //with upgrades
                if (upgradedWeaponDictionary == null)
                    return;
                if (!upgradedWeaponDictionary.ContainsKey(Weapon.WeaponId))
                    upgradedWeaponDictionary.Add(Weapon.WeaponId, 0);
                var noOfUpgradesCompleted = upgradedWeaponDictionary[Weapon.WeaponId];

                m_RateOfFire =
                    Mathf.Ceil(Weapon.RateOfFire +
                               noOfUpgradesCompleted * Weapon.RateOfFire * Weapon.IncreasePercentage / 100);
                m_WeaponRange =
                    Mathf.Ceil(Weapon.WeaponRange +
                               noOfUpgradesCompleted * Weapon.WeaponRange * Weapon.IncreasePercentage / 100);
                m_TotalAmmo =
                    Mathf.Ceil(Weapon.TotalAmmo + noOfUpgradesCompleted * Weapon.TotalAmmo * Weapon.IncreasePercentage / 100);
                m_ClipSize =
                    Mathf.Ceil(Weapon.ClipSize + noOfUpgradesCompleted * Weapon.ClipSize * Weapon.IncreasePercentage / 100);
                m_Spread = Weapon.Spread - noOfUpgradesCompleted * Weapon.Spread * Weapon.IncreasePercentage / 100;
                m_WeaponType = Weapon.WeaponType;
                m_HitscanDamageAmount =
                    Mathf.Ceil(Weapon.HitscanDamageAmount +
                               noOfUpgradesCompleted * Weapon.HitscanDamageAmount * Weapon.IncreasePercentage / 100);
            }
         
            m_DamageMultiplier = Weapon.DamageMultiplier;
            m_AutoReload = Weapon.AutoReload;
            m_Shell = Weapon.Shell;
            m_Particles = Weapon.Particles;
            m_ImapctParticles = Weapon.ImpactParticles;
            m_FireSound = Weapon.FireSound;
            m_FireSoundDelay = Weapon.FireSoundDelay;
            m_EmptyFireSound = Weapon.EmptyFireSound;
            m_ReloadSound = Weapon.ReloadSound;


            m_ShootDelay = 1 / m_RateOfFire;
            m_LastShootTime = 0;
            m_CurrentAmmoAvailable = m_TotalAmmo;
            m_CurrentReloadAmmoAvailable = m_ClipSize;
        }

        public virtual void Start()
        {
            if (m_Particles)
            {
                if (m_ParticlePosition)
                {
                    m_Particles = Instantiate(m_Particles, m_ParticlePosition.position, m_ParticlePosition.rotation) as ParticleSystem;
                    m_Particles.gameObject.transform.parent = transform;
                }

            }
            var upgradedWeaponDictionary = PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted;
            //with upgrades
            if (upgradedWeaponDictionary == null)
                return;
            if (!upgradedWeaponDictionary.ContainsKey(Weapon.WeaponId))
                upgradedWeaponDictionary.Add(Weapon.WeaponId, 0);
            var noOfUpgradesCompleted = upgradedWeaponDictionary[Weapon.WeaponId];

            m_RateOfFire =
                Mathf.Ceil(Weapon.RateOfFire +
                           noOfUpgradesCompleted * Weapon.RateOfFire * Weapon.IncreasePercentage / 100);
            m_WeaponRange =
                Mathf.Ceil(Weapon.WeaponRange +
                           noOfUpgradesCompleted * Weapon.WeaponRange * Weapon.IncreasePercentage / 100);
            m_TotalAmmo =
                Mathf.Ceil(Weapon.TotalAmmo + noOfUpgradesCompleted * Weapon.TotalAmmo * Weapon.IncreasePercentage / 100);
            m_ClipSize =
                Mathf.Ceil(Weapon.ClipSize + noOfUpgradesCompleted * Weapon.ClipSize * Weapon.IncreasePercentage / 100);
            m_Spread = Weapon.Spread - noOfUpgradesCompleted * Weapon.Spread * Weapon.IncreasePercentage / 100;
            m_WeaponType = Weapon.WeaponType;
            m_HitscanDamageAmount =
                Mathf.Ceil(Weapon.HitscanDamageAmount +
                           noOfUpgradesCompleted * Weapon.HitscanDamageAmount * Weapon.IncreasePercentage / 100);
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

            //compenemts
            m_MissionManager = MissionManager.Instance;
            //show initial ammo
            UpdateWeaponAmmoDisplay();
        }

        public virtual void Update()
        {
            if (m_CurrentReloadAmmoAvailable <= 0 && IsAmmoAvailable() && m_AutoReload && !isReloadingStarted)
                StartReloading();

            if (m_CurrentAmmoAvailable < 10)
            {
                //display buyammo button
                UiManager.Instance.ShowRefillAmmoMenu();
            }
            else
            {
                UiManager.Instance.HideRefillAmmoMenu();
            }
        }

        public void StartReloading()
        {
            isReloadingStarted = true;
            //   Debug.Log("reloading started");
            //if (m_Particles)
            //{
            //    if (m_Particles.isPlaying)
            //    {
            //        StopFireParticles();
            //    }
            //}
            StartCoroutine(DisableMuzzleFlash());


            if (m_ReloadSound.Length > 0)
            {
                m_reloadAudioClip = m_ReloadSound[Random.Range(0, m_ReloadSound.Length)];
                AudioManager.Instance.PlaySound(m_audioSource, m_reloadAudioClip);
            }
            //play weapon reload animation
           // PlayWeaponReloadAnimation();
        }

        public bool CanFire()
        {
            //Debug.Log("current time" + Time.time);
            //Debug.Log("last shoot time" + m_LastShootTime);
            //Debug.Log("shoot delay" + m_ShootDelay);
            if (Time.time > m_LastShootTime + m_ShootDelay && m_CurrentReloadAmmoAvailable > 0 )
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual bool Shoot(Vector3 position)
        {
            m_LastShootTime = Time.time;
            m_NoofShotsFired++;
            m_CurrentReloadAmmoAvailable--;
          //  m_CurrentAmmoAvailable--;
            //Debug.Log("no of shots fired" + m_NoofShotsFired);
            //Debug.Log("m_CurrentReloadAmmoAvailable" + m_CurrentReloadAmmoAvailable);
            //Debug.Log("current ammo available" + m_CurrentAmmoAvailable);
            var direction = FireDirection(position);
         //   PlayWeaponRecoilAnimation();
            PlayMuzzleFlash();
            PlayFireParticles();
            if (m_FireSound.Length > 0)
            {               
                    m_ShootaudioClip = m_FireSound[Random.Range(0, m_FireSound.Length)];
                    AudioManager.Instance.PlayOneShotSound(m_audioSource, m_ShootaudioClip);
                
            }
                
       //     RaycastHit hit;
#if UNITY_EDITOR

            //   Debug.Log("direction" + direction);
            Debug.DrawRay(m_FirePoint.position, direction * m_WeaponRange, Color.red, 10f);
#endif

          
            killMissions.Clear();
            deathMissions.Clear();
            destroyDroneMissions.Clear();
            if (m_MissionManager)
            {
                foreach (var mission in m_MissionManager.GetActiveMissions())
                    if (mission.MissionType == MissionType.KillEnemies)
                        killMissions.Add(mission);
                    else if (mission.MissionType == MissionType.Die)
                        deathMissions.Add(mission);
                     else if(mission.MissionType== MissionType.DestroyDrones)
                        {
                        destroyDroneMissions.Add(mission);
                        }
            }
     

            //if (Physics.Raycast(m_FirePoint.position, direction, out hit, m_WeaponRange))
            //{
                
            //}
            RaycastHit[] hits = Physics.RaycastAll(m_FirePoint.position, direction, m_WeaponRange);
            foreach (var hit in hits)
            {
                if (hit.collider)
                {
                    if (m_ImapctParticles)
                    {
                        m_ImapctParticles = Instantiate(m_ImapctParticles, hit.point, new Quaternion(-90, 0, 0, 0));

                    }
                    Health health = null;
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        health = hit.collider.GetComponent<Health>();
                        EnemyAI enemyAi = hit.collider.GetComponent<EnemyAI>();
                        if (!enemyAi.isDieSet)
                        {
                            if (health != null)
                            {
                                float currentHealth = health.GetCurrentHealth();
                                if (currentHealth > 0)
                                {
                                    health.TakeDamage(m_HitscanDamageAmount);
                                }
                                currentHealth = health.GetCurrentHealth();


                                if (currentHealth <= 0)
                                {
                                    health.PlayDeathEffects();

                                    if (Util.IsTutorialComplete())
                                    {

                                        GameManager.noOfEnemiesOnScreen--;
                                        PlayerData.PlayerProfile.PlayerXp += 25; //enemy killed xp
                                        PlayerData.PlayerProfile.CurrentLevelXp += 25;
                                        //current game xp 
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

                                        PlayerData.PlayerProfile.NoofEnemieskilled++;
                                        PlayerData.CurrentGameStats.CurrentKills++;

                                        if (killMissions.Count > 0)
                                            foreach (var mission in killMissions)
                                                if (!mission.IsMissionForOneRun)
                                                {
                                                    if (PlayerData.PlayerProfile.NoofEnemieskilled ==
                                                        mission.AmountOrObjectIdToComplete)
                                                        EventManager.TriggerEvent(mission.MissionTitle);
                                                }
                                                else
                                                {
                                                    //check for current game stats
                                                    if (PlayerData.CurrentGameStats.CurrentKills ==
                                                        mission.AmountOrObjectIdToComplete)
                                                        EventManager.TriggerEvent(mission.MissionTitle);
                                                }
                                        enemyAi.isDieSet = true;


                                        UiManager.Instance.ShowXPText(m_player.transform.position, 25, XPType.KilledEnemy);

                                    }
                                }
                                
                   
                            }
                        }
                        
                       

                    }
                    else if (hit.collider.isTrigger)
                    {
                        health = hit.collider.GetComponent<VehicleHealth>();
                        if (health)
                        {
                            health.TakeDamage(m_HitscanDamageAmount);

                        }
                    }
                    else if (hit.collider.CompareTag("EnemyDrone"))
                    {
                        health = hit.collider.GetComponent<Health>();
                        if (health != null)
                        {
                            float currentHealth = health.GetCurrentHealth();
                            if (currentHealth > 0)
                            {
                                health.TakeDamage(m_HitscanDamageAmount);
                            }
                          
                            currentHealth = health.GetCurrentHealth();
                            if (Util.IsTutorialComplete())
                            {
                                if (currentHealth <= 0)
                                {
                                    PlayerData.PlayerProfile.PlayerXp += 10; //enemy drone destroyed xp
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
                   

                            }

                        }
                    }

                }
            }
          
            return true;
        }

        public void UpdateWeaponAmmoDisplay()
        {
            UiManager.Instance.UpdateWeaponAmmoDisplay((int) m_CurrentReloadAmmoAvailable, (int)Mathf.Max(0, m_CurrentAmmoAvailable));
        }

        public void PlayWeaponRecoilAnimation()
        {
            if (m_Animator)
                m_Animator.SetTrigger("Recoil");
        }

        public void PlayWeaponReloadAnimation()
        {
            if (m_Animator)
                m_Animator.SetTrigger("Reload");
        }

        protected Vector3 FireDirection(Vector3 position)
        {
            var axis = position.normalized;
            if (m_Spread > 0.0)
            {
                var vector3 = Quaternion.AngleAxis(Random.Range(0, 360), axis) * Vector3.up *
                              Random.Range(0.0f, m_Spread);
                axis += vector3;
            }
            return axis;
        }

        public bool IsReloading()
        {
            isReloading = (m_CurrentReloadAmmoAvailable <= 0);
            if (m_CurrentReloadAmmoAvailable <= 0f)
                return true;
            return false;
        }

        public void ChangeWeaponClipParent(GameObject weaponClip, Transform toParent)
        {
            weaponClip.transform.parent = toParent;
        }

        protected bool IsAmmoAvailable()
        {
            return m_CurrentAmmoAvailable > 0;
        }

        public bool IsDoneReloading()
        {
            return isReloadingEnded;
        }

        public void StopReloading()
        {

            var reduceAmmoAmount = (int)(m_ClipSize - m_CurrentReloadAmmoAvailable);

            if (m_CurrentAmmoAvailable - reduceAmmoAmount > 0)
            {
                m_CurrentAmmoAvailable -= reduceAmmoAmount;
                m_CurrentReloadAmmoAvailable = m_ClipSize;
            }
            else
            {
                m_CurrentReloadAmmoAvailable = m_CurrentReloadAmmoAvailable + m_CurrentAmmoAvailable;
                m_CurrentAmmoAvailable = 0;
            }
            isReloadingStarted = false;
            isReloading = false;
        }

        public void ReloadExternally()
        {
            if (!IsAmmoAvailable())
                return;
            StartReloading();
       
            StopReloading();

            UpdateWeaponAmmoDisplay();
        }

        public void BuyAmmo()
        {
            if (m_CurrentAmmoAvailable < 10)
            {
                m_CurrentAmmoAvailable += m_TotalAmmo;
            }
            ReloadExternally();
            UiManager.Instance.UpdateWeaponAmmoDisplay((int) m_CurrentReloadAmmoAvailable,(int) m_CurrentAmmoAvailable);
        }
        protected void PlayFireParticles()
        {
            if (m_Particles)
            {
                if (m_Particles.isPlaying)
                {
                    m_Particles.Stop(true);
                }
                m_Particles.Play(true);
                StartCoroutine(StopFireParticles());

            }

            if (m_Shell)
            {
                var go = Instantiate(m_Shell, m_ShellLocation.position,
                    m_ShellLocation.rotation);
                go.transform.parent = m_ShellParent;
                m_RigidBody.AddRelativeForce(m_ShellForce);
                m_RigidBody.AddRelativeTorque(m_ShellTorque);

                Destroy(go, 5f);
            }
        }

        protected IEnumerator StopFireParticles()
        {
            yield return new WaitForSeconds(0.1f);
            if (m_Particles && m_Particles.isPlaying)
            {
               m_Particles.Stop(true);
               m_Particles.Clear(true);
                
            }
        }

        protected virtual void PlayMuzzleFlash()
        {
            if (m_MuzzleFlash)
            {
                m_MuzzleFlash.SetActive(true);

                StartCoroutine(DisableMuzzleFlash());
            }
        }
 
        protected IEnumerator DisableMuzzleFlash()
        {
            yield return new WaitForSeconds(0.1f);
            m_MuzzleFlash.transform.localScale = new Vector3(5, 5, 5);
            m_MuzzleFlash.SetActive(false);
        }

        public void DisableMuzzleFlash2()
        {
            m_MuzzleFlash.SetActive(false);
        }
        //public bool ReloadComplete(OnReloadComplete onReloadComplete)
        //{
        //}
    }
}

public enum WeaponType
{
    Sr45,
    Mp5,
    SCu,
    Smg,
    Fw97,
    M32
}

