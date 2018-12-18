using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowStats : MonoBehaviour
{

    public Text NoofGamesPlayed;
    public Text TotalDistanceTravelled;
    public Text NoofEnemiesKilled;
    public Text NoofDeaths;
    public Text TotalObstaclesDodged;
    public Text TotalXpEarned;
    public Text LongestDistanceInOneRun;
    public Text TotalVehiclesDestoyed;
    public Text TotalDronesDestoyed;
    //public Text TotalJumps;
    //public Text TotalSlides;

    private void OnEnable()
    {
        NoofGamesPlayed.text = PlayerData.PlayerProfile.NoofGamesPlayed.ToString();
        TotalDistanceTravelled.text = PlayerData.PlayerProfile.TotalDistanceTravelled.ToString() + " M";
        NoofEnemiesKilled.text = PlayerData.PlayerProfile.NoofEnemieskilled.ToString();
        NoofDeaths.text = PlayerData.PlayerProfile.NoofDeaths.ToString();
        TotalObstaclesDodged.text = PlayerData.PlayerProfile.NofOfObstaclesDodged.ToString();
        TotalXpEarned.text = PlayerData.PlayerProfile.PlayerXp.ToString();
        TotalDronesDestoyed.text = PlayerData.PlayerProfile.NoOfEnemyDronesDestroyed.ToString();
        TotalVehiclesDestoyed.text = PlayerData.PlayerProfile.NoOfEnemyvehiclesDestroyed.ToString();
        LongestDistanceInOneRun.text = PlayerData.PlayerProfile.LongestDistanceTravelledInOneRun.ToString();
        //TotalJumps.text = PlayerData.PlayerProfile.NofJumps.ToString();
        //TotalSlides.text = PlayerData.PlayerProfile.NoofSlides.ToString();
    }


}
