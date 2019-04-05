using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerData
{
    public static PlayerProfile PlayerProfile;
    public static CurrentGameStats CurrentGameStats=new CurrentGameStats();

    public static PlayerProfile RetreivePlayerProfile()
    {
        PlayerProfile = SaveLoadManager.Instance.Load();
        return PlayerProfile;
    }
    public static void SavePlayerData()
    {
        SaveLoadManager.Instance.Save(PlayerProfile);
    }
}
[Serializable]
public class CurrentGameStats
{
    public float CurrentXpEarned;
    public int CurrentKills;
    public int CurrentDeaths;
    public int CurrentObstaclesDodged;
    public int CurrentObstaclesDestroyed;
    public int CurrentEnemyVehiclesDestroyed;
    public int CurrentDronesDestroyed;
    public int TokensCollected;
    public int DiamondsCollected;
    public int NoOfJumps;
    public float CurrentEnergyAmount;
    public int CurrentMoveMultiplier;
    public int CurrentMovementSpeed;
    public float CurrentDistanceTravelled;
    public int CurrentStoreTypeClicked;
}
