using UnityEngine;
using System.Collections.Generic;
namespace EndlessRunner
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AppearanceProbability))]
    [RequireComponent(typeof(AppearanceRules))]
    public class CollidableObject : InfiniteObject
    {
        public bool canSpawnInLeftSlot;
        public bool canSpawnInCenterSlot;
        public bool canSpawnInRightSlot;
        [SerializeField]
        private bool m_RandomRotation;

        public LayerMask CollidableLayerMask;

        protected MissionManager m_MissionManager;
        private AudioSource m_AudioSource;
        private PlayerControl m_playercontrol;
        private List<Mission> m_collectTokensMissions=new List<Mission>();
        // a helper class which maps the slot position to the bitwise power
        private class SlotPositionPower
        {
            public int position;
            public int power;
            public SlotPositionPower(int pos, int pow)
            {
                position = pos;
                power = pow;
            }
        }

        public bool RandomRotation
        {
            set { m_RandomRotation = value; }
            get { return m_RandomRotation; }
        }
        private List<SlotPositionPower> slotPositions;
        private int slotPositionsMask;


        public override void Init()
        {
            base.Init();

            DetermineSlotPositions();
            m_MissionManager = MissionManager.Instance;

        }

        public override void Awake()
        {
            base.Awake();

            // need to determine the slot positions again because the cloned object doesn't get inited
            DetermineSlotPositions();

            //   m_LayerMask = (1 << 12) | (1 << 13);
            m_AudioSource = GetComponent<AudioSource>();
            GameObjectsInCollisionLayer = Util.GetColliderObjectsInLayer(gameObject, 20);
        }

        public override void Start()
        {
            base.Start();


        }
        private void Update()
        {
            if (GameManager.m_InstantiatedPlayer)
            {
                if (m_playercontrol == null)
                {
                    m_playercontrol = GameManager.m_InstantiatedPlayer.GetComponent<PlayerControl>();
                }

                var targetPosition = m_playercontrol.GetPlayerCurrentPosition();
                if ((transform.position.x - targetPosition.x < 5))
                {
                    Deactivate();
                }
                else
                {
                    Activate();
                }
            }
        }

        private void OnBecameInvisible()
        {
            Deactivate();
        }
        public virtual void OnTriggerEnter(Collider other)
        {
            // m_PlayerProfile = SaveLoadManager.Instance.Load();
            m_collectTokensMissions.Clear();
            if (m_MissionManager)
            {
                foreach (Mission mission in m_MissionManager.GetActiveMissions())
                {
                    if (mission.MissionType == MissionType.CollectTokens)
                    {
                        m_collectTokensMissions.Add(mission);
                    }

                }
            }
           
            if((other.gameObject.layer == LayerMask.NameToLayer("Obstacle")) ||
                other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
                //to check if  obstacle is spawned over other object or the obstac le is spwaned in ascene with enemy
            {
                Deactivate();
            }
            //if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            //{
            //    float height= GameObjectsInCollisionLayer[0].bounds.size.y;

            //}
                if (other.gameObject.CompareTag("Ground"))
            {
                this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
            if (other.gameObject.CompareTag("Player"))
            {
                if (gameObject.tag == "token")
                {
                    //play sound
                    AudioManager.Instance.PlaySound(m_AudioSource);
                    if (Util.IsTutorialComplete())
                    {
                        PlayerData.PlayerProfile.NoofTokensAvailable += 1;

                        PlayerData.CurrentGameStats.TokensCollected++;
                        UiManager.Instance.UpdateTokensCollected(PlayerData.CurrentGameStats.TokensCollected);
                        if (m_collectTokensMissions.Count > 0)
                        {
                            foreach (Mission mission in m_collectTokensMissions)
                            {
                                if (!mission.IsMissionForOneRun)
                                {
                                    if (PlayerData.PlayerProfile.NoofTokensAvailable == mission.AmountOrObjectIdToComplete)
                                    {
                                        EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                                }
                                else
                                {
                                    //check for current game stats
                                    if (PlayerData.CurrentGameStats.TokensCollected == mission.AmountOrObjectIdToComplete)
                                    {
                                        EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                                }

                            }
                        }
                    }

                    //      UiManager.Instance.UpdateCoins(PlayerData.PlayerProfile.NoofCoinsAvailable);

                    Deactivate();

                }
            }
        }

        private void DetermineSlotPositions()
        {
            slotPositions = new List<SlotPositionPower>();
            slotPositionsMask = 0;
            if (canSpawnInLeftSlot)
            {
                slotPositions.Add(new SlotPositionPower(-1, 0));
                slotPositionsMask |= 1;
            }
            if (canSpawnInCenterSlot)
            {
                slotPositions.Add(new SlotPositionPower(0, 1));
                slotPositionsMask |= 2;
            }
            if (canSpawnInRightSlot)
            {
                slotPositions.Add(new SlotPositionPower(1, 2));
                slotPositionsMask |= 4;
            }
        }

        public int GetSlotPositionsMask()
        {
            return slotPositionsMask;
        }

        public Vector3 GetSpawnSlot(Vector3 scenePosition)
        {
            if (slotPositions.Count > 0)
            {
                List<SlotPositionPower> positions = slotPositions;

                int index = Random.Range(0, positions.Count);
                // return the position if the platform can spawn with the given slot
                return scenePosition * positions[index].position;
            }
            return scenePosition;
        }

        public Vector3 GetSpawnSlot(Vector3 scenePosition, int sceneSlots)
        {
            if (slotPositions.Count > 0 && sceneSlots != 0)
            {
                List<SlotPositionPower> positions = slotPositions;
                while (positions.Count > 0)
                {
                    int index = Random.Range(0, positions.Count);
                    int mask = (int)Mathf.Pow(2, positions[index].power);
                    // return the position if the platform can spawn with the given slot
                    if ((sceneSlots & mask) == mask)
                    {
                        return scenePosition * positions[index].position;
                    }
                    // can't spawn yet
                    positions.RemoveAt(index);
                }
            }
            return scenePosition;
        }

        public bool CanSpwanCollidableAtPosition(Vector3 position, float radius)
        {
            Collider[] colliders = Physics.OverlapSphere(position,radius, CollidableLayerMask);
            if (colliders.Length > 0)
            {
                return false;
            }
            return true;
        }

        public bool CanSpwanCollidableAtPosition(Vector3 position)
        {
#if UNITY_EDITOR
            Debug.DrawRay(position,Vector3.forward,Color.red);
#endif
            Ray ray=new Ray(position, Vector3.forward);
            RaycastHit hitInfo;
            if (Physics.SphereCast(ray, 2f, out hitInfo,10,CollidableLayerMask))
            {
                if (hitInfo.collider.gameObject.layer != CollidableLayerMask)
                {
                    return false;
                }
            }

            return true;

        }

        public bool CanSpwanCollidableAtPosition(Vector3 position, bool useSphereCastAll)
        {
#if UNITY_EDITOR
            Debug.DrawRay(position, Vector3.forward, Color.red);
#endif
            Ray ray = new Ray(position, Vector3.forward);
            RaycastHit[] hitInfo =new RaycastHit[5];
           int result= Physics.SphereCastNonAlloc(position, 5f, Vector3.forward,hitInfo, CollidableLayerMask);
            if (hitInfo.Length > 0)
            {
                foreach (var info in hitInfo)
                {
                    if (info.collider)
                    {
                        //    Debug.Log("collided with" + info.collider.name);
                        //    Debug.Log("cannot spawn at location");
                            return false;
                    }              
                }
            }

            //foreach (var info in hitInfo)
            //{
            //    if (hitInfo.Length > 0)
            //    {
            //        if (info.collider.gameObject.layer != LayerMask.NameToLayer("PhysicsSphereCastIgnoreLayer"))
            //        {
            //            return false;
            //        }
            //    }
            //}

            //if (Physics.SphereCast(ray, 10f, out hitInfo))
            //{
            //    if (hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
            //    {
            //        Debug.Log(hitInfo.collider.name);
            //        return false;
            //    }
            //}
            return true;

        }
    } 
}
