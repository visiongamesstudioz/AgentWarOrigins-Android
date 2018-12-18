using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;
using UnityEngine.UI;

public class VungleAds : MonoBehaviour
{
    public static VungleAds Instance;
    public static bool IsVideoForResume;
    public static VideoType VideoType;

    private void Awake()
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

    // Use this for initialization
    private void Start()
    {
        var appID = "";
        var iosAppID = "ios_app_id";
        var androidAppID = "5912f781d0098f654d000f11";
        var windowsAppID = "windows_app_id";
#if UNITY_IPHONE
    appID = iosAppID;
    Dictionary<string, bool> placements = new Dictionary<string, bool>
    {
        { "ios_placement_id_1", false },
        { "ios_placement_id_2", false },
        { "ios_placement_id_3", false }
    };
#elif UNITY_ANDROID
        appID = androidAppID;
        var placements = new Dictionary<string, bool>
        {
            {"EARNDIA54487", false},
            {"NONSKIP64586", false},
            {"RESUMEG40651", false},
            {"SKIPPAB91447", false}
        };
#elif UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
    appID = windowsAppID;
    Dictionary<string, bool> placements = new Dictionary<string, bool>
    {
        { "windows_placement_id_1", false },
        { "windows_placement_id_2", false },
        { "windows_placement_id_3", false }
    };
#endif
        var array = new string[placements.Keys.Count];
        placements.Keys.CopyTo(array, 0);
        Vungle.init(appID);


        //initialize event handlers
        InitializeEventHandlers();
                      
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Vungle.onPause();
        else
            Vungle.onResume();
    }

    public void SetVideoType(int videoType)
    {
        VideoType = (VideoType) videoType;
    }

    public void LoadAd(string placementId)
    {
        string placementID;
#if UNITY_IPHONE
  placementID = "ios_placement_id";
#elif UNITY_ANDROID
        placementID = placementId;
#elif UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
  placementID = "windows_placement_id";
#endif
        Vungle.loadAd(placementID);
    }

    public void ShowVideo(string placementID)
    {
        StopAllCoroutines();
        var options = new Dictionary<string, object>();
        //  options["incentivized"] = isIncentivized;

        Vungle.playAd(options, placementID);
    }

    private void InitializeEventHandlers()
    {
        Vungle.onAdStartedEvent +=
            placementID =>
            {
               
                Debug.Log("Ad " + placementID + " is starting!  Pause your game  animation or sound here.");
            };

        Vungle.onAdFinishedEvent += (placementID, args) =>
        {
            Debug.Log("Ad finished - placementID " + placementID + ", was call to action clicked:" +
                      args.WasCallToActionClicked + ", is completed view:"
                      + args.IsCompletedView);
            if (args.WasCallToActionClicked)
            {
                Debug.Log("user clicked on download button");
            }

            if (args.IsCompletedView)
            {
                switch (VideoType)
                {
                    //double current xp earned;
                    case VideoType.DoubleTokens:
                        PlayerData.PlayerProfile.NoofTokensAvailable += (int)PlayerData.CurrentGameStats.TokensCollected;
                        PlayerData.CurrentGameStats.TokensCollected *= 2;
                
                        GameObject showCurrentStats= GameObject.Find("CurrentGameStats");
                        if (showCurrentStats != null)
                        {
                            GameObject currenttokensCollected = Util.FindGameObjectWithName(showCurrentStats, "tokensCollectedValue");
                            currenttokensCollected.GetComponentInChildren<Text>().text =
                                PlayerData.CurrentGameStats.TokensCollected.ToString();
                        }
                     

                        UiManager.Instance.UpdateUi();
                        break;
                    case VideoType.ResumeVideo:
                        GameManager.Instance.ResumeFromDeath(true);
                        break;
                    case VideoType.AddDiamonds:
                        //award reward tio player
                        PlayerData.PlayerProfile.NoofDiamondsAvailable += 1;
                        if (UiManager.Instance)
                            UiManager.Instance.UpdateDiamonds(PlayerData.PlayerProfile.NoofDiamondsAvailable);
                        break;
                }
            }

            //reset video type to none
            VideoType = VideoType.None;
            //save player data
            PlayerData.SavePlayerData();

            //load ad again
            LoadAd(placementID);
        };

        Vungle.adPlayableEvent +=
            (placementID, adPlayable) =>
            {
                Debug.Log("Ad's playable state has been changed! placementID " + placementID + ". Now: " + adPlayable);
            };

        Vungle.onLogEvent += log => { Debug.Log("Log: " + log); };

        Vungle.onInitializeEvent += () => { Debug.Log("SDK initialized"); };
    }
}

public enum VideoType
{
    None,
    DoubleTokens,
    ResumeVideo,
    AddDiamonds
}