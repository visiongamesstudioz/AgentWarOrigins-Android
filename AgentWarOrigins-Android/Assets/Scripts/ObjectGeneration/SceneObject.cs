using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace EndlessRunner
{
    /*
       * The scene surrounds the platforms
       */

    [RequireComponent(typeof(AppearanceProbability))]
    [RequireComponent(typeof(SceneAppearanceRules))]
    public class SceneObject : InfiniteObject
    {

        // Set this offset if the platform object's center doesn't match up with the true center (such as with just left or right corners)
        public Vector3 centerOffset;

        // If section transition is true, this list contains the sections that it can transition from (used with the toSection list)
        [HideInInspector] public List<int> fromSection;
        // If section transition is true, this list contains the sections that it can transition to (used with the fromSection list)
        [HideInInspector] public List<int> toSection;

        //// Direction of platform. At least one option must be true. Straight is the most common so is the default
        //public bool straight = true;
        //public bool leftTurn;
        //public bool rightTurn;

        // force different collidable object types to spawn on top of the platform, such as obstacle and coin
        // (assuming the propabilities allow the object to spawn)
        public bool forceDifferentCollidableTypes;

        // the number of collidable objects that can fit on one platform. The objects are spaced along the local x position of the platform
        public int CollidablePositions;
        public bool CollidableLeftSpawn;
        public bool CollidableCenterSpawn;
        public bool CollidableRightSpawn;
        public int NoOfTokenBunches;
        //// boolean to determine what horizontal location objects can spawn. If collidablePositions is greater than 0 then at least one
        //// of these booleans must be true 

        public bool CanSpawnEnemies;
        public int m_NoOfEnemies;
        public bool hasDrones;
        public List<SpawnPoint> SpawnPoints;
        public List<WayPoint> WayPoints;
        //// the list of control points if the platform is a curve
        //[HideInInspector]
        //public List<Vector3> controlPoints;
        // a mapping of the distance of the curve to the starting control point

        private int slotPositionsAvailable;
        private int collidableSpawnPosition;

        // true if a scene object has linked to this platform. No other scene objects will be able to spawn near this object.
        private bool requireLinkedSceneObject;
        private EnemyManager m_EnemyManager;
        private BoxCollider[] m_BoxColliders;
        private List<WayPoint> m_AssignedWaypoints = new List<WayPoint>();
        private List<Enemy> assignedEnemies = new List<Enemy>();
        private GameObject player;
        private int sceneLocalIndex = -1;
        private List<AirDroneController> enemyDrones=new List<AirDroneController>();
        public int SceneLocalIndex
        {
            get { return sceneLocalIndex; }
            set { sceneLocalIndex = value; }
        }

        public List<Enemy> AssignedEnemies
        {
            get
            {
                return assignedEnemies;
            }

            set
            {
                assignedEnemies = value;
            }
        }

        public override void Init()
        {
            base.Init();
            objectType = ObjectType.Scene;

        }

        public override void Awake()
        {
            base.Awake();

            //assign ioclod
            childIoClod = GetComponentsInChildren<IOClod>();

            foreach (var childLod in childIoClod)
            {
                Util.ioClodsDictionary.Add(childLod.gameObject, childLod);
            }
            //get all drones in scene
            if (hasDrones)
            {
                enemyDrones = Util.GetComponentsInLayer<AirDroneController>(gameObject,
                    LayerMask.NameToLayer("EnemyDrones"));
                foreach (var enemydrone in enemyDrones)
                {
                    enemydrone.gameObject.SetActive(false);
                    enemydrone.transform.localEulerAngles=Vector3.zero;
                }
            }

            //get current infinite object size size
            if (renderers != null)
            {

                currentObjectSize = renderers.bounds.size;
                currentObjectLength = currentObjectSize.x;
                //Debug.Log("current object length" + currentObjectLength);
            }

            else
            {
                List<Collider> list = Util.GetColliderObjectsInLayer(gameObject, LayerMask.NameToLayer("Ground"));
                for (int i = 0; i < list.Count; i++)
                {
                    currentObjectSize += list[i].bounds.size;
                }
                currentObjectLength = currentObjectSize.x;

            }

            m_EnemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

            m_BoxColliders = GetComponents<BoxCollider>();

            collidableSpawnPosition = 0;
            requireLinkedSceneObject = false;

            slotPositionsAvailable = 0;
            if (CollidableLeftSpawn)
            {
                slotPositionsAvailable |= 1;
            }
            if (CollidableCenterSpawn)
            {
                slotPositionsAvailable |= 2;
            }
            if (CollidableRightSpawn)
            {
                slotPositionsAvailable |= 4;
            }

            CollidableObject[] collidableObjects = GetComponentsInChildren<CollidableObject>();
            for (int i = 0; i < collidableObjects.Length; ++i)
            {
                collidableObjects[i].SetStartParent(collidableObjects[i].transform.parent);
            }

            //   SpawnEnemies();

        }

        private void SpawnEnemies()
        {
            m_AssignedWaypoints = WayPoints;

            //assign random target animation to waypoints

            AssignRandomAnimationForWayPoints();
            ////spawn enemies
            if (CanSpawnEnemies)
            {
                if (SpawnPoints.Count != m_NoOfEnemies)
                {
                    return;
                }
                if (m_EnemyManager != null)
                {
                    for (int i = 0; i < m_NoOfEnemies; i++)
                    {
                        //should change according to position should spawn stronger enemies at large distance
                        Enemy enemy = m_EnemyManager.GetRandomEnemyFromPool();
                        assignedEnemies.Add(enemy);
                        EnemyAI enemyAi = enemy.EnemyPrefab.GetComponent<EnemyAI>();
                        Health enemyHealth = enemy.EnemyPrefab.GetComponent<Health>();
                        int level = (int) (transform.position.x / 1000);
                        //assign enemy health based on distance
                        level = Mathf.Clamp(level, 1, enemy.NoofLevels);
                        if (level > 1)
                        {
                            enemyHealth.MaxHealth = enemyHealth.DefaultLevelHealth +
                                                    level * enemy.HealthIncreasePercentageWithLevel *
                                                    enemyHealth.DefaultLevelHealth / 100;
                        }

                        enemy.EnemyPrefab.transform.localEulerAngles = new Vector3(0, -90, 0);
                        enemy.EnemyPrefab.transform.position = SpawnPoints[i].SpawnPointTransform.position;
                        enemy.EnemyPrefab.transform.parent = SpawnPoints[i].IsOnGameObject
                            ? SpawnPoints[i].SpawnOverObject.transform
                            : transform;
                        enemy.IsOnGameObject = SpawnPoints[i].IsOnGameObject;
                        enemy.EnemyPrefab.SetActive(true);

                        //assign waypoints to enemies
                        for (int j = 0; j < enemy.NoOfWayPoints; j++)
                        {
                            int temp = FindClosestWayPoint(enemy);
                            //need to add way points to enemies 
                            if (temp == -1)
                            {
                                continue;
                            }
                            //check if returned waypoint is in front of enemy
                            if (!Util.IsObjectInFront(enemy.EnemyPrefab,
                                m_AssignedWaypoints[temp].TargetDestination.gameObject))
                            {
                                continue;
                            }

                            //if (IsWayPointAssigned(m_AssignedWaypoints[temp]))
                            //{
                            //    continue;
                            //}
                            //set waypoint parent depending on waypoint position
                            if (WayPoints[temp].IsOverGameObject)
                            {
                                WayPoints[temp].TargetDestination.parent = WayPoints[temp].OverGameObject.transform;
                            }
                            if (enemyAi != null)
                            {
                                enemyAi.AddWayPoint(m_AssignedWaypoints[temp]);
                                m_AssignedWaypoints.Remove(m_AssignedWaypoints[temp]);
                                // m_WayPoints[temp].IsWayPointAssigned = true;
                            }


                        }



                    }
                }
            }
        }

        private void AssignRandomAnimationForWayPoints()
        {
            foreach (var waypoint in WayPoints)
            {

                waypoint.TargetAnimationName = (WaypointTargetAnimationName) Random.Range(0, 2);
            }
        }


        private void Update()
        {
            if (GameManager.m_InstantiatedPlayer)
            {
                var targetPosition =
                    GameManager.m_InstantiatedPlayer.GetComponent<PlayerControl>().GetPlayerCurrentPosition();
                //if (Camera.main)
                //{

                //    if ((transform.position - targetPosition).x <
                //        Camera.main.farClipPlane && (transform.position - targetPosition).x + GetCurrentSceneLength() > -10f)
                //    {
                //        //check if first child is not active
                //        if (!childRenderers[0].enabled)
                //        {
                //            Activate();

                //        }

                //    }
                //    else
                //    {
                //        //check if first child is not active

                //        if (childRenderers[0].enabled)
                //            Deactivate();

                //    }
                //}

                if (Camera.main)
                {
                    if (!((transform.position - targetPosition).x <
                          Camera.main.farClipPlane &&
                          (transform.position - targetPosition).x + GetCurrentSceneLength() > -10f))
                    {

                    //    Deactivate();
                        //check for occlusion culling here
                        foreach (var childLod in childIoClod)
                        {
                            childLod.HideAll();
                        }

                    }
                }
                //check for occlusion culling here
                ////foreach (var childLod in childIoClod)
                ////{
                ////    childLod.ShowLOD();
                ////}



            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {

                if (hasDrones)
                {
                    foreach (var enemyDrone in enemyDrones)
                    {
                        enemyDrone.gameObject.SetActive(true);
                    }
                }
            }


        }

        private void OnTriggerExit(Collider other)
        {

            if (other.gameObject.CompareTag("Player"))
            {
                if (hasDrones)
                {
                    foreach (var enemyDrone in enemyDrones)
                    {
                        enemyDrone.gameObject.SetActive(false);
                    }
                }
            }
        }




        private int FindClosestWayPoint(Enemy enemy)
        {
            if (m_AssignedWaypoints.Count == 0)
            {
                return -1;
            }
            int closest = 0;
            float lastDistance = Vector3.Distance(enemy.EnemyPrefab.transform.position,
                m_AssignedWaypoints[0].TargetDestination.position);
            for (int i = 1; i < m_AssignedWaypoints.Count; i++)
            {
                float thisDistance = Vector3.Distance(enemy.EnemyPrefab.transform.position,
                    m_AssignedWaypoints[i].TargetDestination.position);

                if (lastDistance > thisDistance && i != closest)
                {
                    closest = i;
                }
            }
            return closest;
        }

        public void EnableLinkedSceneObjectRequired()
        {
            requireLinkedSceneObject = true;
        }

        public bool LinkedSceneObjectRequired()
        {
            return requireLinkedSceneObject;
        }

        public int GetSlotsAvailable()
        {
            return slotPositionsAvailable;
        }

        // Determine if an object is already spawned in the same position. Do this using bitwise and/or.
        // For example, the following situation might occur:
        // Spawn pos 3:
        // 0 |= (2 ^ 3), result of 0000 1000 (decimal 8)
        // Spawn pos 1:
        // 8 |= (2 ^ 1), result of 0000 1010 (decimal 10)
        // Check pos 3:
        // 10 & (2 ^ 3), result of 0000 1000 (decimal 8), position is not free
        // Check pos 2:
        // 10 & (2 ^ 2), result of 0000 0000 (decimal 0), space is free
        // Spawn pos 0:
        // 10 |= (2 ^ 0), result of 0000 1011 (decimal 11)
        public bool CanSpawnCollidable(int pos)
        {
            return (collidableSpawnPosition & (int) Mathf.Pow(2, pos)) == 0;
        }

        public bool CanSpawnCollidable()
        {
            return CollidablePositions != 0 && collidableSpawnPosition != (int) Mathf.Pow(2, CollidablePositions) - 1;
        }

        public void CollidableSpawned(int pos)
        {
            collidableSpawnPosition |= (int) Mathf.Pow(2, pos);
        }

        //public override void Orient(Vector3 position, Quaternion rotation)
        //{
        //    base.Orient(position, rotation);

        //    // reset the number of collidables that have been spawned on top of the platform
        //    collidableSpawnPosition = 0;
        //}
        public override void Activate()
        {

            base.Activate();
            SpawnEnemies();

        }

        public override void Deactivate()
        {

            base.Deactivate();

            for (int i = 0; i < assignedEnemies.Count; i++)
            {
                //add enemies back to pool

                m_EnemyManager.AddEnemyToPool(assignedEnemies[i].EnemyPrefab, assignedEnemies[i].PrefabID);
                //if (assignedEnemies[i].EnemyPrefab != null)
                //{
                //    assignedEnemies[i].EnemyPrefab.transform.position = SpawnPoints[i].SpawnPointTransform.transform.position;
                //}
            }

            if (hasDrones)
            {
                foreach (var enemyDrone in enemyDrones)
                {
                    enemyDrone.transform.position = enemyDrone.StartPointTransform.position;
                    enemyDrone.ResetCurrentPathFollowerId();
                }
            }
        }

        private bool IsWayPointAssigned(WayPoint wayPoint)
        {
            return wayPoint.IsWayPointAssigned;
        }

        private void OnDisable()
        {
            for (var i = 0; i < assignedEnemies.Count; i++)
            {
                try
                {
                    if (assignedEnemies[i].EnemyPrefab != null)
                    {
                        assignedEnemies[i].EnemyPrefab.transform.position =
                            SpawnPoints[i].SpawnPointTransform.transform.position;
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                assignedEnemies.Clear();
            }

        }
    }
}

[Serializable]
public class SpawnPoint
{
    public Transform SpawnPointTransform;
    public bool IsOnGameObject;
    public GameObject SpawnOverObject;
    public WaypointTargetAnimationName TargetAnimationName;

}