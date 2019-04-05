using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public Player[] Players;
    public Weapon[] Weapons;
    public Mission[] Missions;
    public int MaxLevels;
    public Level[] Levels;
    public List<BluePrintItem> BluePrintItems = new List<BluePrintItem>();

    public static DataManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
             DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Mission GetMission(int missionId)
    {
        return Missions[missionId];
    }

    public Level GetLevel(int level)
    {
        return Levels[level - 1];
    }

}
