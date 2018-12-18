using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class StaticObstacle : InfiniteObject
{
    public bool IsJump;
    public ObstacleType ObstacleType;
    public bool CanRunOnTop;

    private readonly List<Mission> deathMissions = new List<Mission>();

    public bool IsTriggerEntered { get; set; }

    public bool IsCollisionEntered { get; set; }

    // Use this for initialization
    public override void Start()
    {
        childColliders = GetComponentsInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        deathMissions.Clear();
        if (MissionManager.Instance)
        {
            foreach (var mission in MissionManager.Instance.GetActiveMissions())
                if (mission.MissionType == MissionType.Die)
                    deathMissions.Add(mission);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
            other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
        {
            ObstacleObject obstacleObject = other.GetComponent<ObstacleObject>();
            if (obstacleObject)
            {
                //to check if  obstacle is spawned over other object or the obstac le is spwaned in ascene with enemy
                obstacleObject.Deactivate();
            }
           
        }

        if (other.gameObject.CompareTag("Player"))
        {
                if (CanRunOnTop)
            {
                if (Vector3.Dot(Vector3.up, other.transform.position - transform.position) > 0.5)
                {
                    IsTriggerEntered = true;
                    childColliders[0].isTrigger = false;
                }
                    
            }
                
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
            other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
        {
            ObstacleObject obstacleObject = other.gameObject.GetComponent<ObstacleObject>();
            if (obstacleObject)
            {
                //to check if  obstacle is spawned over other object or the obstac le is spwaned in ascene with enemy
                obstacleObject.Deactivate();
            }

        }
        if (other.gameObject.CompareTag("Player"))
        {
            var playerControl = other.gameObject.GetComponent<PlayerControl>();
            var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (!IsTriggerEntered)
            {

                if (playerHealth && !playerHealth.IsDead)
                {
                    switch (ObstacleType)
                    {
                        case ObstacleType.DeathObstacle:
                            playerHealth.TakeDamage(playerHealth.GetMaxHealth(), 1);


                            break;
                        case ObstacleType.SemiDeathObstacle:

                            playerHealth.TakeDamage(playerHealth.GetMaxHealth() / 2, 1);

                            break;
                        case ObstacleType.NohealthChange:
                            playerHealth.TakeDamage(0,1);

                            break;
                    }
                    //get player health
                    var playerCurrentHealth = playerHealth.GetCurrentHealth();
                    if (playerCurrentHealth <= 0 && !playerHealth.IsDead)
                    {
                        playerHealth.IsDead = true;
                        if (Util.IsTutorialComplete())
                        {
                            PlayerData.PlayerProfile.NoofDeaths += 1;
                            PlayerData.CurrentGameStats.CurrentDeaths++;
                            if (deathMissions.Count > 0)
                                foreach (var mission in deathMissions)
                                    if (!mission.IsMissionForOneRun)
                                    {
                                        if (PlayerData.PlayerProfile.NoofDeaths == mission.AmountOrObjectIdToComplete)
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                                    else
                                    {
                                        //check for current game stats
                                        if (PlayerData.CurrentGameStats.CurrentDeaths == mission.AmountOrObjectIdToComplete)
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                    }
                        }

                        //    GameManager.Instance.OnDeath();
                    }

                }
            }
        }


    }
}

