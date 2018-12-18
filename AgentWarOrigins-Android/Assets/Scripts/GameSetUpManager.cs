using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetUpManager : MonoBehaviour
{

    public static GameSetUpManager Instance;
    private bool m_IsFirstGame=true;

    void Awake()
    {
        if (Instance == null)
        {
            //if not, set Instance to this

            Instance = this;

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        //If Instance already exists and it's not this:
        else if (Instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one Instance of a GameSetUpManager.
            Destroy(gameObject);

        }

    
     //   PlayerPrefs.DeleteAll();
    }

    public void InitializeFirstTimeSetup()
    {
        m_IsFirstGame = false;
        var playerProfile = new PlayerProfile
        {
            NoofCoinsAvailable = 1000,
            NoofDiamondsAvailable = 100,
            CurrentLevel = 1,
            CurrentLevelXp = 0,
        };



        //initialize level of player
        //unlock first character
        var unlockPlayerList = new List<int> { 1 };
        //1 is default PlayerID
        playerProfile.UnlockedPlayerList = unlockPlayerList;
        //set current selected player to default
        playerProfile.CurrentSelectedPlayer = 1;
        //unlock default out for each character
        var unlockOutfitListPerPlayerDict = new Dictionary<int, List<int>>();
        for (var i = 1; i <= DataManager.Instance.Players.Length; i++)
        {
            List<int> defualtOutfitPerPlayer = new List<int> {1};
            //add default outfit to each player
            unlockOutfitListPerPlayerDict.Add(i, defualtOutfitPerPlayer);
        }  // 1 is default outfitID
        playerProfile.UnlockedOutfitsPerPlayer = unlockOutfitListPerPlayerDict;
        //add default weapon tp player profile
        List<int> defaultWeapon = new List<int> {1};
        playerProfile.UnlockedWeaponsList = defaultWeapon;
        SaveLoadManager.Instance.Save(playerProfile);
       
        PlayerPrefs.SetInt("IsFirstTime", m_IsFirstGame ? 1 : 0);

        PlayerPrefs.Save();
    }
   
}
