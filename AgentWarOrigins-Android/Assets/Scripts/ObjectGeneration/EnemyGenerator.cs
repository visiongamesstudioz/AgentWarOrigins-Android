using EndlessRunner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour {

    private EnemyManager m_EnemyManager;
    private List<Enemy> assignedEnemies = new List<Enemy>();
    public List<WayPoint> WayPoints;

    private List<WayPoint> m_AssignedWaypoints = new List<WayPoint>();

    public List<SpawnPoint> SpawnPoints;
    // Use this for initialization
    void Start () {
	    m_EnemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }
	
    public void SpawnRandomEnemy(SpawnPoint spawnPoint)
    {
        if (m_EnemyManager != null)
        {
                //should change according to position should spawn stronger enemies at large distance
                Enemy enemy = m_EnemyManager.GetRandomEnemyFromPool();
                assignedEnemies.Add(enemy);
                EnemyAI enemyAi = enemy.EnemyPrefab.GetComponent<EnemyAI>();
                Health enemyHealth = enemy.EnemyPrefab.GetComponent<Health>();
                int level = (int)(transform.position.x / 1000);
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
                    : transform;
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
