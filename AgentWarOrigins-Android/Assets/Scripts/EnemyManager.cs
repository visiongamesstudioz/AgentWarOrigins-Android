using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> EnemyList;
    public Transform EnemyParentTransform;
    private readonly Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    public static EnemyManager Instance;

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

        //do
        //{
        //    temp = Random.Range(0, EnemyList.Count);

        //} while (EnemyList[temp].Level!=level);


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