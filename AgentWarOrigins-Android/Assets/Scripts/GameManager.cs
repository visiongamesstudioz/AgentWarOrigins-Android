using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace EndlessRunner
{
    /*
     * The game manager is a singleton which manages the game state. It coordinates with all of the other classes to tell them
     * when to start different game states such as pausing the game or ending the game.*/

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static bool IsFromAnotherScene;
        public delegate void EventHandler();
        public float ReviveHealthValue;
        public AudioClip MenuAudioClip;
        public AudioClip GamePlayAudioClip;
        //    public event EventHandler OnDeath;
        //     public event EventHandler OnPause;
        private InfiniteObjectGenerator m_InfiniteObjectGenerator;
        private InputControl m_InputControl;
        private PlayerControl m_PlayerControl;
        private  Rigidbody m_PlayerRigidbody;
        private static bool m_IsGameActive;
        public static GameObject m_InstantiatedPlayer;
        public static int LevelStarted;
        private float m_LastSpeedIncreasedTime;

        public float IncreaseSpeedAmount;
        public float SpeedIncreaseTimeInterval;
        private string m_SceneName;
        //    private PlayerProfile m_PlayerProfile;
        private float m_MovementSpeed;
        public static int noOfEnemiesOnScreen;
        private float remainingRevives;
        private float ReviveEnergyAmountFactor;
        public Camera m_MainCamera;
        private AudioSource m_audioSource;


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
            //PlayerPrefs.DeleteAll();
            //if (PlayerPrefs.HasKey("IsFirstTime"))
            //{
            //    int value = PlayerPrefs.GetInt("IsFirstTime");

            //    m_IsFirstGame = value != 0;
            //}
        }

        private void OnEnable()
        {
            OnLevelFinishedLoading(SceneManager.GetActiveScene(),LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;


        }

        private void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private void Start()
        {
            //enable infinite objects

            m_InfiniteObjectGenerator = InfiniteObjectGenerator.instance;

            m_audioSource = GetComponent<AudioSource>();
            //if (m_IsFirstGame)   //check if player playing first time on device
            //{
            //    Debug.Log("first time on device");
            // //  GameSetUpManager.Instance.InitializeFirstTimeSetup();
            //   SocialManager.Instance.LoadDataFromGpgCloud();

            //   // InitializeSelectedCharacter();
            //}
            //      m_PlayerProfile = SaveLoadManager.Instance.Load();
            ////initialize scene canvas
            if (UiManager.Instance)
            {
                UiManager.Instance.InitializeSceneCanvas();
                UiManager.Instance.InitializeSettingsCanvas();
                //  InitializeSelectedCharacter();
                // get player profile
                //    get current level
                if (PlayerData.PlayerProfile != null)
                {
                    UiManager.Instance.UpdateUi();
                    LevelStarted = PlayerData.PlayerProfile.CurrentLevel;
                }                    
            }



            //get current levelk
            m_LastSpeedIncreasedTime = 0;
            
        }

        private void Update()
        {
            ////increase game play speed
            if (Time.time > m_LastSpeedIncreasedTime + SpeedIncreaseTimeInterval)
            {
                m_LastSpeedIncreasedTime = Time.time;
                // m_InputControl.movementSpeed += IncreaseSpeedAmount;
                if (m_InstantiatedPlayer && m_IsGameActive)
                {
                    if (!m_PlayerControl.IsinTrigger)
                        m_MovementSpeed =
                            m_PlayerControl.MoveSpeedMultiplier +=
                                IncreaseSpeedAmount;

                    if (m_InfiniteObjectGenerator)
                    {
                        m_InfiniteObjectGenerator.DistanceBetweenCollidables.x =
                            Mathf.Clamp(m_InfiniteObjectGenerator.DistanceBetweenCollidables.x -= 5, 30, 75);
                    }
                }
            }
            //if (SceneManager.GetActiveScene().buildIndex == 1 && m_IsGameActive)
            //{
            //    UiManager.Instance.DisableBuyDiamondsButton();
            //    UiManager.Instance.DisableBuyCoinsButton();
            //}
            //else
            //{
            //    UiManager.Instance.EnableBuyDiamondsButton();
            //    UiManager.Instance.EnableBuyCoinsButton();
            //}
        }

        public bool IsGameActive()
        {
            return m_IsGameActive;
        }

        public void StartGame()
        {
            m_MainCamera=Camera.main;

            //increase no of games played by 1
            PlayerData.PlayerProfile.NoofGamesPlayed += 1;
            m_IsGameActive = true;

            m_MainCamera.GetComponent<CameraController>().enabled = true;
            m_MainCamera.fieldOfView = 50f;
            m_MainCamera.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
            if (m_InstantiatedPlayer)
                m_PlayerRigidbody.isKinematic = false;
            //show pause button
            //    m_InfiniteObjectGenerator.SpawnObjectRun(true);
            UiManager.Instance.ShowPauseButton();
            UiManager.Instance.ShowCurrentStatsInPauseCanvas();

            UiManager.Instance.DisableSceneCanavas();
            //start game music

            AudioManager.Instance.PlaySound(m_audioSource,
                m_IsGameActive ? GamePlayAudioClip : MenuAudioClip);

            //PlayerData.SavePlayerData();
           
        }

        public void ShowHome()
        {
            //should chagne later
            //StartGame();
        }

        //public void LoadLevel(int level)
        //{
        //   // StopAllCoroutines();
        //    m_IsGameActive = false;
        //    UiManager.Instance.ShowSceneCanvas();
        //    // The Application loads the Scene in the background at the same time as the current Scene.
        //    //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        //    StartCoroutine(SocialManager.Instance.LoadLevel(level));
        //}
        //void InitializeDefaultMission()
        //{
        //   PlayerProfile playerProfile= new PlayerProfile();
        //    playerProfile.CompletedMissions.Add(DataManager.Instance.Missions[0]);
        //    SaveLoadManager.Instance.Save(playerProfile);
        //}
        //public void InitializeFirstTimeSetup()
        //{
        //    m_IsFirstGame = false;
        //    Debug.Log("fi9rst time");
        //    var playerProfile = new PlayerProfile();

        //    playerProfile.NoofCoinsAvailable = 250;
        //    playerProfile.NoofDiamondsAvailable = 100;

        //    //initialize level of player
        //    playerProfile.CurrentLevel = 1;
        //    playerProfile.CurrentLevelXp = 0;
        //    //unlock first character
        //    var unlockPlayerList = new List<int> { 1 };
        //    //1 is default PlayerID
        //    playerProfile.UnlockedPlayerList = unlockPlayerList;
        //    //set current selected player to default
        //    playerProfile.CurrentSelectedPlayer = 1;
        //    //unlock default out for each character
        //    var unlockOutfitListPerPlayerDict = new Dictionary<int, List<int>>();
        //    var defualtOutfitPerPlayer = new List<int>();
        //    defualtOutfitPerPlayer.Add(1);
        //    for (var i = 1; i <= DataManager.Instance.Players.Length; i++) //noofplayers
        //        unlockOutfitListPerPlayerDict.Add(i, defualtOutfitPerPlayer); // 1 is default outfitID
        //    playerProfile.UnlockedOutfitsPerPlayer = unlockOutfitListPerPlayerDict;
        //    SaveLoadManager.Instance.Save(playerProfile);

        //    PlayerPrefs.SetInt("IsFirstTime", true ? 1 : 0);
        //    PlayerPrefs.Save();
        //}

        public GameObject InitializeSelectedCharacter()
        {
            //var playerProfile = SaveLoadManager.Instance.Load();
            int selectedCharacterId = PlayerData.PlayerProfile.CurrentSelectedPlayer;
            if (selectedCharacterId == 0)
                selectedCharacterId = 1;
            var selectedPlayer = DataManager.Instance.Players[selectedCharacterId - 1];
            m_InstantiatedPlayer = Instantiate(selectedPlayer.gameObject) as GameObject;
            m_InstantiatedPlayer.transform.position = new Vector3(0, 2, 0);
            m_InstantiatedPlayer.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
            m_InstantiatedPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
            m_InstantiatedPlayer.tag = "Player";
            Projector blobProjector = m_InstantiatedPlayer.GetComponentInChildren<Projector>();
            int qualityLevel = QualitySettings.GetQualityLevel();
            if (qualityLevel < 1)
            {
                QualitySettings.shadows=ShadowQuality.Disable;
                blobProjector.gameObject.SetActive(true);
            }
            else
            {
                QualitySettings.shadows=ShadowQuality.HardOnly;
                blobProjector.gameObject.SetActive(false);
            }
            //      m_InstantiatedPlayer.layer = LayerMask.NameToLayer("Player");
            var wornOutfit =
                PlayerData.PlayerProfile.WornOutfitPerPlayer;
            if (!wornOutfit.ContainsKey(selectedPlayer.PlayerID))
                wornOutfit.Add(selectedPlayer.PlayerID, 1);
            var selectedOufitId = wornOutfit[selectedPlayer.PlayerID];

            if (selectedOufitId == 0)
                return null;
            Dictionary<int, Dictionary<int, int>> wornSkinPerOutfitPerPlayer = PlayerData.PlayerProfile.WornSkinPerOutfitPerPlayer;
            if (wornSkinPerOutfitPerPlayer == null)
            {
                Debug.Log("worn skin per outfit per player dictionary is null");
                return null;
            }
            if (!wornSkinPerOutfitPerPlayer.ContainsKey(selectedPlayer.PlayerID))
            {
                wornSkinPerOutfitPerPlayer.Add(selectedPlayer.PlayerID,new Dictionary<int, int>());
            }
            Dictionary<int, int> wornSkinPeroutfit = wornSkinPerOutfitPerPlayer[selectedPlayer.PlayerID];
            if (wornSkinPeroutfit == null)
            {

                return null;
            }
            if (!wornSkinPeroutfit.ContainsKey(selectedOufitId))
            {
                wornSkinPeroutfit.Add(selectedOufitId, -1);
            }
            int selectedOutfitSkinID = wornSkinPeroutfit[selectedOufitId];
            //wear selected outfit
            OutfitSkins selectedOutfitWithSkins = selectedPlayer.PlayerAvailableOutFitsWithSkins[selectedOufitId - 1];
            Outfit selectedSkinOutfit;
            if (selectedOutfitSkinID <= 0)
            {
                selectedSkinOutfit = selectedOutfitWithSkins.AvailableSkinsPerOutfit[0];
            }
            else
            {
                selectedSkinOutfit = selectedOutfitWithSkins.AvailableSkinsPerOutfit[selectedOutfitSkinID];
            }
            var outfit = selectedSkinOutfit.gameObject;
            selectedSkinOutfit.Initialize(outfit, m_InstantiatedPlayer.GetComponent<Player>());
            selectedSkinOutfit.ChangeClothes();
            Util.SetLayerRecursively(m_InstantiatedPlayer, LayerMask.NameToLayer("Player"));
            //initialize character abilities

            var ability = selectedSkinOutfit.OutfitAbility;
            Dictionary<int,Dictionary<int,int>> noofUpgradesPeroutfitDictionary = PlayerData.PlayerProfile.NoofOutfitUpgradedPerPlayer;
            if (noofUpgradesPeroutfitDictionary == null)
            {
                return null;
            }
            if (!noofUpgradesPeroutfitDictionary.ContainsKey(selectedCharacterId))
            {
                noofUpgradesPeroutfitDictionary.Add(selectedCharacterId,new Dictionary<int, int>());
            }
            Dictionary<int, int> noOfUpgradesPerOutfit = noofUpgradesPeroutfitDictionary[selectedCharacterId];
            if (noOfUpgradesPerOutfit == null) { return null; }
            if (!noOfUpgradesPerOutfit.ContainsKey(selectedOufitId))
            {
                noOfUpgradesPerOutfit.Add(selectedOufitId, 0);
            }
            int noOfUpgradesCompleted = noOfUpgradesPerOutfit[selectedOufitId];
            PlayerHealth playerHealth = m_InstantiatedPlayer.GetComponent<PlayerHealth>();
            switch (ability)
            {
                case OutfitAbility.None:
                    break;
                case OutfitAbility.LowDamage:

                    playerHealth.SetDamageMultiplier =
                        selectedSkinOutfit.DamageMultiplier -
                        noOfUpgradesCompleted * selectedSkinOutfit.DamageMultiplier *
                        selectedSkinOutfit.DamageDecreasePercentage / 100;
                    //Debug.Log("damage multiplier" +
                    //          (selectedOutfit.DamageMultiplier -
                    //           noOfUpgradesCompleted * selectedOutfit.DamageMultiplier *
                    //           selectedOutfit.DamageDecreasePercentage / 100));
                    break;
                case OutfitAbility.RegenerateHealth:
                    playerHealth.CanRegenerateHealth = true;
                    playerHealth.RegenerateInitialWaitTime =
                        selectedSkinOutfit.RegenerateInitialWaitTime -
                        noOfUpgradesCompleted * selectedSkinOutfit.RegenerateInitialWaitTime *
                        selectedSkinOutfit.RegenerateInitialDecreaseTimePercentage / 100;
                    playerHealth.RegenerateAmountOverTime = selectedSkinOutfit.RegenerateAmountOverTime +
                                                            noOfUpgradesCompleted *
                                                            selectedSkinOutfit.RegenerateAmountOverTime *
                                                            selectedSkinOutfit.RegenerateAmountIncreasePercentage / 100;

                  
                    break;
                case OutfitAbility.Revive:
                    playerHealth.CanPlayerRevive = true;
                    remainingRevives = selectedSkinOutfit.NoofReviveTimes;
                    ReviveEnergyAmountFactor = selectedSkinOutfit.ReviveEnergyAmount +
                                               noOfUpgradesCompleted *
                                               selectedSkinOutfit.ReviveEnergyIncreasePercentage *
                                               selectedSkinOutfit.ReviveEnergyAmount / 100;
                  

                    break;
                case OutfitAbility.Invincible:
                    playerHealth.IsInvincible = true;
                    playerHealth.InvincibilityTime = selectedSkinOutfit.InvincibleTime +
                                                                                       noOfUpgradesCompleted *
                                                                                       selectedSkinOutfit.InvincibleTime *
                                                                                       selectedSkinOutfit
                                                                                           .InvincibleIncreaseTimerPercentage /
                                                                                       100;

                    //Debug.Log("no of upgrades completed" + noOfUpgradesCompleted);
                    //Debug.Log("invincible time" + (selectedOutfit.InvincibleTime +
                    //                               noOfUpgradesCompleted *
                    //                               selectedOutfit.InvincibleTime *
                    //                               selectedOutfit
                    //                                   .InvincibleIncreaseTimerPercentage /
                    //                               100));
                    break;
            }

            //set and activate current weapon and animations
            var selectedWeaponID = PlayerData.PlayerProfile.CurrentSelectedWeapon;
            var playerShoot = m_InstantiatedPlayer.GetComponent<PlayerShoot>();
            var weapons
                = m_InstantiatedPlayer.GetComponentsInChildren<WeaponManagement>(true);
            if (weapons.Length > 0)
                switch (selectedWeaponID)
                {
                    case 1:
                        playerShoot.CurrentWeapon = weapons[0];
                        break;
                    case 2:
                        playerShoot.CurrentWeapon = weapons[1];
                        break;
                    case 3:
                        playerShoot.CurrentWeapon = weapons[2];
                        break;
                    case 4:
                        playerShoot.CurrentWeapon = weapons[3];
                        break;
                    case 5:
                        playerShoot.CurrentWeapon = weapons[4];
                        break;
                    case 6:
                        playerShoot.CurrentWeapon = weapons[5];
                        break;
                }
            return m_InstantiatedPlayer;
        }

        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            m_SceneName = scene.name;
            m_audioSource = GetComponent<AudioSource>();

            if (scene.name == "Home") //should change
            {
                if (GameManager.m_InstantiatedPlayer == null)
                {
                    InitializeSelectedCharacter();
                }
                

                if (m_InstantiatedPlayer)
                {
                    m_InstantiatedPlayer.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                    m_PlayerControl = m_InstantiatedPlayer.GetComponent<PlayerControl>();
                    m_PlayerRigidbody = m_InstantiatedPlayer.GetComponent<Rigidbody>();
                }

                //create pools
                UiManager.Instance.CreateUiPools();
                //set timeline controllers
                if (!Util.IsTutorialComplete())
                {
                    if (m_InstantiatedPlayer)
                    {
                        m_PlayerRigidbody.isKinematic = false;
                        m_InstantiatedPlayer.transform.localPosition = new Vector3(65.7f, 0, 0);
                    }

                    GameObject timelineControllergo = GameObject.Find("TimelineController");
                    TimelineController timelineController = timelineControllergo.GetComponent<TimelineController>();
                    timelineController.SetVirtualCameraFollowObject(m_InstantiatedPlayer.transform);
                    GameObject playerNeck = Util.FindGameObjectWithTag(m_InstantiatedPlayer, "Neck");
                    timelineController.SetLookUpCameraObject(playerNeck.transform);
                    DynamicTimeliineBinding dynamicTimelineBinding = timelineControllergo.GetComponent<DynamicTimeliineBinding>();
                    dynamicTimelineBinding.SetPlayer(m_InstantiatedPlayer);
                    dynamicTimelineBinding.ApplyOffsets();
                    timelineController.PlayCutScene();
                }

                //enable main camera

               // m_MainCamera = Camera.main;
                if (m_MainCamera && Util.IsTutorialComplete())
                {

                    m_MainCamera.gameObject.SetActive(true);

                    m_MainCamera.GetComponent<IntroSceneCameraMovement>().enabled = true;
                    m_MainCamera.transform.localEulerAngles = new Vector3(0, -90, 0);

                }
               
            }
            else if (scene.name == "missions")
            {
                MissionManager.Instance.DisplayActiveMissions();
            }

            if (UiManager.Instance)
            {
                if (UiManager.IsSceneCanvasInitialized)
                {
                    UiManager.Instance.UpdateSceneName("Stats");
                    UiManager.Instance.EnableSceneCanvas();
                    UiManager.Instance.AssignSceneCanvasButtonActions(scene.name);
                }
                //UiManager.Instance.CreateUiPools();
            }


            //change music based on scene
            if (Util.IsTutorialComplete())
            {
                AudioManager.Instance.PlaySound(m_audioSource,
                    m_IsGameActive ? GamePlayAudioClip : MenuAudioClip);
            }
 
            //destroy banner ad
   //         AdmobAdManager.Instance.DestroyBannerAd();
        }

        public void OnDeath()
        {
            
            PlayerHealth playerHealth = m_InstantiatedPlayer.GetComponent<PlayerHealth>();
            PlayerAnimation playerAnimation = m_InstantiatedPlayer.GetComponent<PlayerAnimation>();
            m_IsGameActive = false;            
            if (playerHealth.CanPlayerRevive)
            {
                if (remainingRevives > 0)
                {
                    remainingRevives--;                   
                    if (playerHealth)
                    {
                        playerHealth.ResetHealth(ReviveEnergyAmountFactor);
                        playerHealth.ActivateInvincibility(5);
                    }
                    playerAnimation.Revive();
                    playerHealth.IsDead = false;
                    m_IsGameActive = true;
                    return;
                }
                playerHealth.CanPlayerRevive = false;
            }
            var currentDeaths = PlayerData.CurrentGameStats.CurrentDeaths;
            //  Debug.Log("currentDeaths" + currentDeaths);
            UiManager.Instance.ShowGameOverCanvas(8f, currentDeaths,
                m_InstantiatedPlayer.GetComponent<Player>().PlayerName);
        }

        public void Restart()
        {
        }

        public void ResumeFromDeath(bool isFromVideo)
        {
            PlayerHealth playerHealth = m_InstantiatedPlayer.GetComponent<PlayerHealth>();
            PlayerControl playerControl = m_InstantiatedPlayer.GetComponent<PlayerControl>();
            PlayerAnimation playerAnim = m_InstantiatedPlayer.GetComponent<PlayerAnimation>();
            Vector3 playerPos =playerControl.GetPlayerCurrentPosition();
            playerPos+= new Vector3(5,0,0);
            playerControl.SetPlayerPosition(playerPos);
            UiManager.Instance.ShowPauseButton();
            if (!isFromVideo)
            {
                UiManager.Instance.StopHideGameObjectCoroutine();
                var currentDeaths = PlayerData.CurrentGameStats.CurrentDeaths;
                var currentDiamonds = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                currentDiamonds -= 10 * currentDeaths;
                if (currentDiamonds < 0)
                    return;
                UiManager.Instance.EnableBuywithDiamondsButton();                
                PlayerData.PlayerProfile.NoofDiamondsAvailable = currentDiamonds;
                UiManager.Instance.UpdateDiamonds(currentDiamonds);
                //close dialog
                UiManager.Instance.HideGameOverCanvas();
                if (playerHealth)
                {
                    playerHealth.ResetHealth();
                    playerHealth.ActivateInvincibility(5);
                }

                playerAnim.Revive();
                playerHealth.IsDead = false;
                m_IsGameActive = true;
                playerControl.IsResumeFromDeath = true;
            }
            else
            {
                UiManager.Instance.StopHideGameObjectCoroutine();
                UiManager.Instance.HideGameOverCanvas();
                if (playerHealth)
                {
                    Debug.Log("player health is not null");
                    playerHealth.ResetHealth();
                    playerHealth.ActivateInvincibility(5);
                }
                playerHealth.IsDead = false;
                playerAnim.Revive();
                m_IsGameActive = true;
                playerControl.IsResumeFromDeath = true;


            }
            PlayerData.SavePlayerData();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            //m_IsGameActive = false;
           // m_InstantiatedPlayer.GetComponent<PlayerControl>().MoveSpeedMultiplier = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            m_IsGameActive = true;
          //  m_InstantiatedPlayer.GetComponent<PlayerControl>().MoveSpeedMultiplier = m_MovementSpeed;
        }
    }
}

