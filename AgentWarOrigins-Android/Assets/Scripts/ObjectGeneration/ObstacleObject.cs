using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AppearanceProbability))]
    [RequireComponent(typeof(CollidableAppearanceRules))]
    public class ObstacleObject : CollidableObject
    {
        // Used for the player death animation. On a jump the player will flip over, while a duck the player will fall backwards
        [SerializeField] [Tooltip("is Jump Obstacle")] private bool m_IsJump;

        [SerializeField] [Tooltip("Can ShootRun on top of obstacle")]
        // True if the player is allowed to run on top of the obstacle. If the player hits the obstacle head on it will still hurt him.
        private bool m_CanRunOnTop;

        [SerializeField] [Tooltip("DamageAmount")] private float[] m_DamageAmount = {0, 50, 100};
        // True if the object can be destroyed through an attack. The method obstacleAttacked will be called to play the handle the destruction
        [SerializeField] [Tooltip("Is Obstacle destructible")] private bool m_IsDestructible;
        //type of obstacle
        [SerializeField] [Tooltip("Type of obstacle to calculate damage to player")] private ObstacleType MObstacleType;

        [SerializeField] [Tooltip("Destructive particles")] private ParticleSystem destructiveParticles;
        [SerializeField] [Tooltip("Destructive Animation")] private UnityEngine.Animation destructiveAnimation;
        private bool collideWithPlayer;
        protected int playerLayer;
        private int startLayer;
        private int groundlayer;
        private WaitForSeconds destructionDelay;
        //      private GameManager gameManager;
        //    private PlayerAnimation playerAnimation;
        private readonly List<Mission> dodgeMissions = new List<Mission>();
        private readonly List<Mission> deathMissions = new List<Mission>(3);
        private List<Mission> destroyObstacleMissions=new List<Mission>();
        protected Rigidbody m_RigidBody;
        protected CollidableAppearanceRules m_CollidableAppearanceRules;
        private bool isGroundHit;

        public bool IsTriggerEntered { get; set; }

        public bool IsNormalColliderEntered { get; set; }


        public override void Init()
        {
            base.Init();
            objectType = ObjectType.Obstacle;
            
        }

        public override void Awake()
        {
            base.Awake();
            playerLayer = LayerMask.NameToLayer("Player");
            groundlayer = LayerMask.NameToLayer("Ground");


            GameObjectsInCollisionLayer = Util.GetColliderObjectsInLayer(gameObject, 20);
            GameObjectsInTriggerLayer = Util.GetColliderObjectsInLayer(gameObject, 19);
            m_RigidBody = GetComponent<Rigidbody>();
            //run collidale appearance here
            m_CollidableAppearanceRules = GetComponent<CollidableAppearanceRules>();
        }

        public override void Start()
        {
            //    gameManager = GameManager.Instance;
            //     playerAnimation = PlayerController.Instance.GetComponent<PlayerAnimation>();
            startLayer = gameObject.layer;

            collideWithPlayer = true;
            m_MissionManager = MissionManager.Instance;

        }

        public virtual void Update()
        {
            if (!isGroundHit)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 4f))
                    if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        m_RigidBody.useGravity = true;
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            //     m_PlayerProfile = SaveLoadManager.Instance.Load();
            dodgeMissions.Clear();

            if (m_MissionManager != null)
                foreach (var mission in m_MissionManager.GetActiveMissions())
                {
                    if (mission.MissionType == MissionType.DodgeObstacle)
                        dodgeMissions.Add(mission);
                }
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isGroundHit = true;
                m_RigidBody.useGravity = false;
                m_RigidBody.isKinematic = true;
                //get parent scene object
                var sceneObject = other.gameObject.GetComponentInParent<SceneObject>();
                //check for scene spawn
                if (sceneObject)
                    for (var i = 0; i < m_CollidableAppearanceRules.avoidScenes.Count; i++)
                        if (!m_CollidableAppearanceRules.avoidScenes[i].CanSpawnObject(sceneObject.SceneLocalIndex))
                            Deactivate();
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
                //to check if  obstacle is spawned over other object or the obstac le is spwaned in ascene with enemy
                Deactivate();

            if (other.gameObject.CompareTag("Player"))
            {
                if (Util.IsTutorialComplete())
                {
                    if (m_CanRunOnTop)
                    {
                        if (Vector3.Dot(Vector3.up, other.transform.position - thisTransform.position) > 0.5f)
                        {
                            IsTriggerEntered = true;
                            thisGameObject.layer = groundlayer;

                            PlayerData.PlayerProfile.NofOfObstaclesDodged++;
                            PlayerData.CurrentGameStats.CurrentObstaclesDodged++;
                            //show 
                            UiManager.Instance.ShowXPText(other.transform.position, 10, XPType.ObstacleDodged);

                            //add xp to current xp
                            //add to total player xp
                            PlayerData.PlayerProfile.PlayerXp += 10; //should chande later
                            PlayerData.CurrentGameStats.CurrentXpEarned += 10;
                            PlayerData.PlayerProfile.CurrentLevelXp += 10;
                            var requiredXptoComplete =
                                DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel)
                                    .XpRequiredToReachNextLevel;

                            if (PlayerData.PlayerProfile.CurrentLevelXp >= requiredXptoComplete)
                            {
                                PlayerData.PlayerProfile.CurrentLevelXp -= requiredXptoComplete;
                                PlayerData.PlayerProfile.CurrentLevel++;
                            }
                            //UiManager.Instance.UpdateCurrentLevel(PlayerData.PlayerProfile.CurrentLevel);
                            //UiManager.Instance.UpdateCurrentLevelXp(PlayerData.PlayerProfile.CurrentLevelXp, requiredXptoComplete);
                            //UiManager.Instance.UpdateCurrentLevelXpSlider(PlayerData.PlayerProfile.CurrentLevelXp, requiredXptoComplete);
                            //  Debug.Log("no of dodge missions " + dodgeMissions.Count);
                            //   Debug.Log("no of obstacles dodged  " + m_PlayerProfile.NofOfObstaclesDodged);
                            //foreach (Collider collider in m_Colliders)
                            //{
                            //    collider.isTrigger = false;
                            //}
                            foreach (var childCollider in GameObjectsInTriggerLayer)
                                childCollider.isTrigger = false;
                            if (dodgeMissions.Count > 0)
                                foreach (var mission in dodgeMissions)
                                    if (!mission.IsMissionForOneRun)
                                    {
                                        if (PlayerData.PlayerProfile.NofOfObstaclesDodged ==
                                            mission.AmountOrObjectIdToComplete)
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                                    else
                                    {
                                        //check for current game stats
                                        if (PlayerData.CurrentGameStats.CurrentObstaclesDodged ==
                                            mission.AmountOrObjectIdToComplete)
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                        }
                    }
                        

                }

                //if (m_isTriggerEntered)
                //{


                //if (m_isCollisionEntered)
                //{
                //    playerControl.MoveSpeedMultiplier = 0;
                //    foreach (var collisionTriggerGo in GameObjectsInCollisionLayer)
                //    {
                //        if (collisionTriggerGo)
                //        {
                //            collisionTriggerGo.isTrigger = false;

                //        }
                //    }

                //    // childColliders[1].isTrigger = false;

                //    if (playerHealth)
                //    {
                //        switch (MObstacleType)
                //        {
                //            case ObstacleType.DeathObstacle:

                //                playerHealth.TakeDamage(100);

                //                break;
                //            case ObstacleType.SemiDeathObstacle:

                //                playerHealth.TakeDamage(50);

                //                break;
                //            case ObstacleType.NohealthChange:
                //                playerHealth.TakeDamage(0);
                //                playerAnimation.Stumble(); //stumbles if any non death obstacles hit

                //                break;


                //        }
                //        //get player health
                //        float playerCurrentHealth = playerHealth.GetCurrentHealth();
                //        if (playerCurrentHealth <= 0 && !playerControl.Die)
                //        {
                //            playerControl.Die = true;
                //            //if (m_IsJump)
                //            //{
                //            //    playerAnimation.Death(DeathType.DyingBackwards);

                //            //}
                //            //else
                //            //{
                //            //    //  playerAnimation.Death(DeathType.Flyingdeath);   //should change later
                //            //}
                //            //play death sound
                //            if (other.GetComponent<Player>() != null)
                //            {
                //                AudioManager.Instance.PlaySound(other.gameObject.GetComponent<AudioSource>(), other.GetComponent<Player>().DieAudioClips[Random.Range(0, other.GetComponent<Player>().DieAudioClips.Length)]);
                //            }
                //            PlayerData.PlayerProfile.NoofDeaths += 1;
                //            PlayerData.CurrentGameStats.CurrentDeaths++;
                //            if (deathMissions.Count > 0)
                //            {
                //                foreach (Mission mission in deathMissions)
                //                {
                //                    if (!mission.IsMissionForOneRun)
                //                    {
                //                        if (PlayerData.PlayerProfile.NoofDeaths == mission.AmountOrObjectIdToComplete)
                //                        {
                //                            //List<Mission> completedMissions = m_PlayerProfile.CompletedMissions;
                //                            //completedMissions.Add(mission);
                //                            EventManager.TriggerEvent(mission.MissionTitle);
                //                        }
                //                    }
                //                    else
                //                    {
                //                        //check for current game stats
                //                        if (PlayerData.CurrentGameStats.CurrentDeaths == mission.AmountOrObjectIdToComplete)
                //                        {
                //                            EventManager.TriggerEvent(mission.MissionTitle);
                //                        }

                //                    }

                //                }
                //            }
                //          //  GameManager.Instance.OnDeath();
                //        }

                //        if (m_IsDestructible)
                //        {
                //            ObstacleAttacked();
                //        }

                //        //Debug.Log("no of death missions " + deathMissions.Count);
                //        //Debug.Log("no of dies  " + m_PlayerProfile.NoofDeaths);


                //   // Deactivate();

                //    }

                //    //  gameManager.ObstacleCollision(this, other.ClosestPointOnBounds(thisTransform.position));
                //}
            }
            //         PlayerData.SavePlayerData();
        }


        //private void OnTriggerStay(Collider other)
        //{
        //    if (other.gameObject.CompareTag("Player"))
        //    {
        //        PlayerControl playerControl = other.GetComponent<PlayerControl>();
        //        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        //        PlayerAnimation playerAnimation = other.GetComponent<PlayerAnimation>();
        //        bool collide = true;
        //        if (m_CanRunOnTop)
        //        {
        //            if (Vector3.Dot(Vector3.up, (other.transform.position - thisTransform.position)) > 0)
        //            {

        //                collide = false;
        //                thisGameObject.layer = groundlayer;
        //                foreach (Collider collider in m_Colliders)
        //                {
        //                    collider.isTrigger = false;
        //                }
        //            }
        //        }

        //    }
        //}


        public override void Activate()
        {
            base.Activate();
            if (optimizeDeactivation)
                foreach (var child in GameObjectsInTriggerLayer)
                {
                    if (!child.isTrigger)
                        child.isTrigger = true;
                    child.enabled = true;
                }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (optimizeDeactivation)
            {
                foreach (var child in GameObjectsInTriggerLayer)
                {
                    if (!child.isTrigger)
                        child.isTrigger = true;
                    child.enabled = false;
                }

            }

            IsTriggerEntered = false;
        }

        private void OnBecameInvisible()
        {
            Deactivate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            deathMissions.Clear();
            destroyObstacleMissions.Clear();
            if (m_MissionManager != null)
            {
                foreach (var mission in m_MissionManager.GetActiveMissions())
                {
                    if (mission.MissionType == MissionType.Die)
                        deathMissions.Add(mission);
                    if (mission.MissionType == MissionType.DestroyObstacle)
                        destroyObstacleMissions.Add(mission);
                }
            }
            if (collision.gameObject.CompareTag("Player")) //to add other conditions
            {
                var playerControl = collision.gameObject.GetComponent<PlayerControl>();
                var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (!IsTriggerEntered)
                {
                    if (playerHealth && !playerHealth.IsDead)
                    if (playerControl.IsResumeFromDeath)
                            {
                                foreach (var child in GameObjectsInCollisionLayer)
                                    if (child)
                                        if (!child.isTrigger)
                                            child.isTrigger = true;
                            }
                            else
                            {
                                //handle on player hit
                                OnPlayerHitObstacle(playerHealth);
                            }

                        

                    if (m_IsDestructible)
                    {
                        if (destroyObstacleMissions.Count > 0)
                        {
                            foreach (var mission in destroyObstacleMissions)
                            {
                                if (!mission.IsMissionForOneRun)
                                {
                                    if (PlayerData.PlayerProfile.NoOfObstaclesDestroyed ==
                                        mission.AmountOrObjectIdToComplete)
                                        EventManager.TriggerEvent(mission.MissionTitle);
                                }
                                else
                                {
                                    //check for current game stats
                                    if (PlayerData.CurrentGameStats.CurrentObstaclesDestroyed ==
                                        mission.AmountOrObjectIdToComplete)
                                        EventManager.TriggerEvent(mission.MissionTitle);
                                }
                            }
                            

                        }

                        //     ObstacleAttacked();
                        PlayerData.PlayerProfile.NoOfObstaclesDestroyed++;
                        PlayerData.CurrentGameStats.CurrentObstaclesDestroyed++;
                    }
                        
                }
            }
        }

        public void OnPlayerHitObstacle(PlayerHealth playerHealth)
        {
            //get player health
            switch (MObstacleType)
            {
                case ObstacleType.DeathObstacle:

                    playerHealth.TakeDamage(playerHealth.GetMaxHealth(), 1);

                    break;
                case ObstacleType.SemiDeathObstacle:

                    playerHealth.TakeDamage(playerHealth.GetMaxHealth() / 2, 1);

                    break;
                case ObstacleType.NohealthChange:
                    playerHealth.TakeDamage(0);
                    break;
            }
           
                var playerCurrentHealth = playerHealth.GetCurrentHealth();
                if (playerCurrentHealth <= 0 && !playerHealth.IsDead)
                {
                    playerHealth.IsDead = true;
                //check if tutorial is complete to complete missions
                if (Util.IsTutorialComplete())
                {
                    int totalnoOfdeaths = PlayerData.PlayerProfile.NoofDeaths;
                    totalnoOfdeaths += 1;
                    PlayerData.PlayerProfile.NoofDeaths = totalnoOfdeaths;
                    PlayerData.CurrentGameStats.CurrentDeaths++;
                    if (deathMissions.Count > 0)
                        foreach (var mission in deathMissions)
                            if (!mission.IsMissionForOneRun)
                            {
                                if (totalnoOfdeaths ==
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
                    
                    //  GameManager.Instance.OnDeath();
                }
            
    
        }
    }
}

public enum ObstacleType
{
    DeathObstacle,
    SemiDeathObstacle,
    NohealthChange,
    None
}

