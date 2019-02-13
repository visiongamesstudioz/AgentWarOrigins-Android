using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using EndlessRunner;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocialManager : MonoBehaviour
{
    public static SocialManager Instance;
    private static bool isGooglePlayServicesShown;
    //keep track of saving or loading during callbacks.
    private bool m_saving;
    private bool m_quitApp;
    //name of file 
    public const string CloudsaveFilename = "game_save_name";
    private PlayerProfile m_PlayerProfile;
    private AsyncOperation _asyncLoad;
    private GameObject _sceneLoader;

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
        Application.backgroundLoadingPriority = ThreadPriority.High;
    }

    // Use this for initialization
    private void Start()
    {
        var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

       StartCoroutine(IsSystemTimeSyncWithNetworkTime());

       SignInGooglePlayServices();

    }

    //check for system time and network time
    private IEnumerator IsSystemTimeSyncWithNetworkTime()
    {
       
        Util.IsSystemTimeSyncingWithNetworkServer = Util.GetNetworkTime().Hour == DateTime.Now.ToLocalTime().Hour;
        yield return null;
    }

    //load save from cloud
    public void LoadDataFromGpgCloud()
    {
#if !UNITY_STANDALONE

        Debug.Log("Loading game progress from the cloud.");

        m_saving = false;
        ((PlayGamesPlatform) Social.Active).SavedGame.OpenWithAutomaticConflictResolution(CloudsaveFilename,
            DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
#endif
    }

    public void SaveDataToGpgCloud(bool quitApp)
    {
        if (IsUserAuthenticatedForPlayServices())
        {
            Debug.Log("Saving progress to the cloud... filename: " + CloudsaveFilename);
            m_saving = true;
            m_quitApp = quitApp;
            //save to named file
            ((PlayGamesPlatform) Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                CloudsaveFilename, //name of file. If save doesn't exist it will be created with this name
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnSavedGameOpened);
        }
    }


    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata gameMetadata)
    {
        if (status == SavedGameRequestStatus.Success)
            if (m_saving)
            {
                //save data to gpg cloud
                var playerProfile = PlayerData.PlayerProfile;
                var playerProfileBinarydata = SerializeObject(playerProfile);
                var builder = new SavedGameMetadataUpdate.Builder();
                var updatedMetadata = builder.Build();
                //saving to cloud
                ((PlayGamesPlatform) Social.Active).SavedGame.CommitUpdate(gameMetadata, updatedMetadata,
                    playerProfileBinarydata, OnsavedGameWrittenToCloud);
            }
            else
            {
                //load data from gpg cloud
                ((PlayGamesPlatform) Social.Active).SavedGame.ReadBinaryData(gameMetadata, OnSavedGameLoaded);
            }
    }

    private void OnsavedGameWrittenToCloud(SavedGameRequestStatus status, ISavedGameMetadata gameData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log(gameData.Description);            
        }
        else
        {
            //cannot write to cloud
            Debug.LogError("cannot update data to cloud");
        }
        if (m_quitApp)
        {
            Application.Quit();
        }
    }

    private void OnSavedGameLoaded(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            bool isCloudDataProcessed = ProcessCloudData(data);
            Debug.Log(isCloudDataProcessed ? "data processed from cloud" : "initialized first time setup");
            if (!isCloudDataProcessed)
            {
                //this is first time ever
                if (Util.IsGamePlayedFirstTimeOnDevice())
                {
                    GameSetUpManager.Instance.InitializeFirstTimeSetup();
                }
            }

            PlayerData.RetreivePlayerProfile();

            if (UiManager.Instance)
            {
                UiManager.Instance.UpdatePlayGamesToggle(true);
                UiManager.Instance.UpdateUi();
            }

            //load next level
            _sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");

            if (_sceneLoader != null && SceneManager.GetActiveScene().buildIndex == 1)
            {
                _sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(2, true);
            }
        }
        else if (status == SavedGameRequestStatus.AuthenticationError)
        {
            Debug.Log("authentication error");
        }
        else if (status == SavedGameRequestStatus.BadInputError)
        {
            Debug.Log("bad input error error");
        }
        else if (status == SavedGameRequestStatus.InternalError)
        {
            Debug.Log("internal error");
        }
        else if (status == SavedGameRequestStatus.TimeoutError)
        {
            Debug.Log("time out error");
        }
        else
        {
            Debug.Log("unknown error");
        }
    }

    private static bool ProcessCloudData(byte[] data)
    {
        Debug.Log("processing cloud data");
        if (data == null || data.Length == 0)
        {
            Debug.Log("No Cloud data to retreive");
            return false;
        }
        var playerProfile = DeserializeObject<PlayerProfile>(data);
        SaveLoadManager.Instance.Save(playerProfile);
        return true;
    }

    public static byte[] SerializeObject<T>(T serializableObject)
    {
        var obj = serializableObject;

        IFormatter formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
    }

    public static T DeserializeObject<T>(byte[] serilizedBytes)
    {
        IFormatter formatter = new BinaryFormatter();
        using (var stream = new MemoryStream(serilizedBytes))
        {
            return (T) formatter.Deserialize(stream);
        }
    }


    public bool IsUserAuthenticatedForPlayServices()
    {
        return Social.localUser.authenticated;
    }


    public void AuthenticateUser()
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                LoadDataFromGpgCloud();
            }
            else
            {
                //unable to authenticate due to some reason
                if (Util.IsGamePlayedFirstTimeOnDevice())
                {
                    // Debug.Log("first time on device");
                    GameSetUpManager.Instance.InitializeFirstTimeSetup();
                }
                //retrieve player profile locally
                PlayerData.RetreivePlayerProfile();

                //load next level
                _sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");

                if (_sceneLoader != null && SceneManager.GetActiveScene().buildIndex == 1)
                {
                    _sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(2, true);
                }
            }
        });
    }

    //private bool IsGamePlayedFirstTimeOnDevice()
    //{
    //    if (!PlayerPrefs.HasKey("IsFirstTime"))
    //        PlayerPrefs.SetInt("IsFirstTime", 1);
    //    if (PlayerPrefs.HasKey("IsFirstTime"))
    //        if (PlayerPrefs.GetInt("IsFirstTime") == 1)
    //            return true;
    //    return false;
    //}

    //private void AuthenticateUser()
    //{
    //    if (CheckNetworkConnection.IsNetworkAvailable())
    //    {
    //        //authenticate user for google play games
    //        Social.localUser.Authenticate(success =>
    //        {

    //            if (success)
    //            {
    //                LoadDataFromGpgCloud();
    //                if (UiManager.Instance)
    //                {
    //                    UiManager.Instance.UpdatePlayGamesToggle(true);
    //                    UiManager.Instance.UpdateUI();
    //                }
    //            }
    //        });
    //    }
    //}

    public void SignInGooglePlayServices()
    {
        if (NetworkConnection.isNetworkAvailable())
        {
            if (!Social.localUser.authenticated)
            {
                if (Util.IsGamePlayedFirstTimeOnDevice())
                {
                    AuthenticateUser();
                }
                else
                {
                    //retrieve player profile locally
                    PlayerData.RetreivePlayerProfile();

                    //load next level
                    _sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");

                    if (_sceneLoader != null && SceneManager.GetActiveScene().buildIndex == 1)
                    {
                        _sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(2, true);
                    }
                }
            }
            else
            {
                LoadDataFromGpgCloud();
            }
        }
        else
        {
            if (Util.IsGamePlayedFirstTimeOnDevice())
                GameSetUpManager.Instance.InitializeFirstTimeSetup();
            //retrieve player profile locally
            PlayerData.RetreivePlayerProfile();

            //load next level
            _sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");

            if (_sceneLoader != null && SceneManager.GetActiveScene().buildIndex == 1)
            {
                _sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(2, true);
            }
        }

    }

    public void SignoutPlayServices()
    {
        UiManager.Instance.UpdatePlayGamesToggle(false);

        PlayGamesPlatform.Instance.SignOut();
    }

    public void PostToLeaderboard(string leaderboardId, float value)
    {
        //post score to leaderboard
        Social.ReportScore((long) value, leaderboardId, success =>
        {
            if (success)
            {
                Debug.Log("successfully posted to leaderboard" + leaderboardId);
            }
        });
    }

    public void ShowLeaderBoardUi()
    {
        // show leaderboard UI
        if (IsUserAuthenticatedForPlayServices())
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            AuthenticateUser();
        }
    }

    public void PlayerStats()
    {
        ((PlayGamesLocalUser) Social.localUser).GetStats((rc, stats) =>
        {
            // -1 means cached stats, 0 is succeess
            // see  CommonStatusCodes for all values.
            if (rc <= 0 && stats.HasDaysSinceLastPlayed())
                Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
        });
    }
}

