using EndlessRunner;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> EnemyList;
    public Transform EnemyParentTransform;
    private readonly Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    private List<WayPoint> m_AssignedWaypoints = new List<WayPoint>();
    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
        for (var i = 0; i < EnemyList.Count; i++)
            CreateEnemyPool(EnemyList[i].EnemyPrefab, EnemyList[i].NoOfInstances);
    }

    public void CreateEnemyPool(GameObject prefab, int noOfInstances)
    {
        int prefabInstanceId = prefab.GetInstanceID();
        if (!poolDictionary.ContainsKey(prefabInstanceId))
        {
            poolDictionary.Add(prefabInstanceId, new Queue<GameObject>());
            for (var i = 0; i < noOfInstances; i++)
            {
                var newGameObject = Instantiate(prefab);
                newGameObject.transform.position = transform.position;
                newGameObject.SetActive(false);
                newGameObject.transform.parent = EnemyParentTransform;
                poolDictionary[prefabInstanceId].Enqueue(newGameObject);
            }
        }
    }

    public void AddEnemyToPool(GameObject obj,int prefabId)
    {
        if (poolDictionary.ContainsKey(prefabId))
        {

            poolDictionary[prefabId].Enqueue(obj);
            //set object parent
            obj.transform.SetParent(EnemyParentTransform);
            obj.SetActive(false);
        }

    }

    public Enemy GetRandomEnemyFromPool()
    {
        var temp = Random.Range(0, EnemyList.Count);
        Enemy enemy;
        var randomEnemy = EnemyList[temp].EnemyPrefab;
        int prefabId = randomEnemy.GetInstanceID();
        if (poolDictionary.ContainsKey(prefabId) && poolDictionary[prefabId].Count > 0)
        {
            //       Debug.Log("remaining enemies" + poolDictionary[prefabId].Count);
            var objectFromPool = poolDictionary[prefabId].Dequeue();
            enemy = new Enemy(objectFromPool, EnemyList[temp].NoofLevels, EnemyList[temp].NoOfWayPoints,EnemyList[temp].HealthIncreasePercentageWithLevel,prefabId);
            return enemy;
        }
        var newGameObject = Instantiate(randomEnemy);
        newGameObject.transform.parent = EnemyParentTransform;
        poolDictionary[prefabId].Enqueue(newGameObject);
        enemy = new Enemy(newGameObject, EnemyList[temp].NoofLevels, EnemyList[temp].NoOfWayPoints,EnemyList[temp].HealthIncreasePercentageWithLevel,prefabId);
        return enemy;
    }

    public Enemy GetRandomEnemyFromPool(int level)
    {
        var temp = Random.Range(0, EnemyList.Count);

        Enemy enemy;
        var randomEnemy = EnemyList[temp].EnemyPrefab;
        var prefabId = randomEnemy.GetInstanceID();
        Debug.Log("prefab id" + prefabId);
        if (poolDictionary.ContainsKey(prefabId) && poolDictionary[prefabId].Count > 0)
        {
            //       Debug.Log("remaining enemies" + poolDictionary[prefabId].Count);
            var objectFromPool = poolDictionary[prefabId].Dequeue();
            enemy = new Enemy(objectFromPool, EnemyList[temp].NoofLevels, EnemyList[temp].NoOfWayPoints,EnemyList[temp].HealthIncreasePercentageWithLevel,prefabId);
            enemy.PrefabID = prefabId;
            return enemy;
        }
        var newGameObject = Instantiate(randomEnemy);
        newGameObject.transform.parent = EnemyParentTransform;
        poolDictionary[prefabId].Enqueue(newGameObject);
        enemy = new Enemy(newGameObject, EnemyList[temp].NoofLevels, EnemyList[temp].NoOfWayPoints,EnemyList[temp].HealthIncreasePercentageWithLevel,prefabId);
        return enemy;
    }

    public void SpawnRandomEnemy(SceneObject sceneObject, int spawnPointIndex)
    {
        SpawnPoint spawnPoint = sceneObject.SpawnPoints[spawnPointIndex];
            //should change according to position should spawn stronger enemies at large distance
            Enemy enemy = GetRandomEnemyFromPool();
            sceneObject.AssignedEnemies.Add(enemy);
            EnemyAI enemyAi = enemy.EnemyPrefab.GetComponent<EnemyAI>();
            Health enemyHealth = enemy.EnemyPrefab.GetComponent<Health>();
            int level = (int)(sceneObject.transform.position.x / 1000);
            //assign enemy health based on distance
            level = Mathf.Clamp(level, 1, enemy.NoofLevels);
            if (level > 1)
            {
                enemyHealth.MaxHealth = enemyHealth.DefaultLevelHealth +
                                        level * enemy.HealthIncreasePercentageWithLevel *
                                        enemyHealth.DefaultLevelHealth / 100;
            }

            enemy.EnemyPrefab.transform.localEulerAngles = new Vector3(0, -90, 0);
            enemy.EnemyPrefab.transform.position = spawnPoint.SpawnPointTransform.position;
            enemy.EnemyPrefab.transform.parent = spawnPoint.IsOnGameObject
                ? spawnPoint.SpawnOverObject.transform
                : sceneObject.transform;
            enemy.IsOnGameObject = spawnPoint.IsOnGameObject;
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
                if (sceneObject.WayPoints[temp].IsOverGameObject)
                {
                    sceneObject.WayPoints[temp].TargetDestination.parent = sceneObject.WayPoints[temp].OverGameObject.transform;
                }
                if (enemyAi != null)
                {
                    enemyAi.AddWayPoint(m_AssignedWaypoints[temp]);
                    m_AssignedWaypoints.Remove(m_AssignedWaypoints[temp]);
                    // m_WayPoints[temp].IsWayPointAssigned = true;
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
}

[Serializable]
public class Enemy
{
    public GameObject EnemyPrefab;
    public int NoofLevels;
    public int HealthIncreasePercentageWithLevel;
    public int NoOfInstances;
    public int NoOfWayPoints;
    public int PrefabID;
    private bool isOnGameObject;

    public Enemy(GameObject gameObject, int noofLevels, int noOfWayPoints,int healthIncreasePercenetageWithLevel,int prefabId)
    {
        EnemyPrefab = gameObject;
        NoofLevels = noofLevels;
        NoOfWayPoints = noOfWayPoints;
        HealthIncreasePercentageWithLevel = healthIncreasePercenetageWithLevel;
        PrefabID = prefabId;
    }
    public bool IsOnGameObject
    {
        get { return isOnGameObject; }
        set { isOnGameObject = value; }
    }

}

[Serializable]
public class WayPoint
{
    public Transform TargetDestination;
    public WaypointTargetAnimationName TargetAnimationName;
    public bool LookTowards;
    public bool IsWayPointAssigned;
    public bool IsOverGameObject;
    public GameObject OverGameObject;
}

public enum WaypointTargetAnimationName
{
    Idle,
    Crouch
}