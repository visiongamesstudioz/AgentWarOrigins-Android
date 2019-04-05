using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndlessRunner
{
    public class UiManager : MonoBehaviour
    {
        public static UiManager Instance;


        public Player TutorialPlayer;

        public Slider HealthBar;
        public Slider CurrentLevelXpSlider;
        public Text CurrentLevel;
        public Text CurrentLevelXp;
        public Text CurrentTokens;
        public Text CurrentCoins;
        public Text CurrentDiamonds;
        public Text SceneName;
        public Canvas SceneCanvas;
        public Canvas GameMenuCanvas;
        public Canvas GameOverCanvas;
        public Text RemainingTimeTorevive;
        public Canvas SettingsCanvas;
        public Canvas PauseCanvas;
        public Button PauseButton;
        public GameObject DistanceTravelled;
       
        public GameObject CurrentKills;
        public GameObject TokensCollected;
        public Button BackButton;
        public Button Tokensbutton;
        public Button BuyCoinsButton;
        public Button BuyDiamondsButton;
        public Button WatchResumeVideoButton;
        public GameObject PauseMenu;
        public GameObject RefillAmmoHolder;
        public Button BuyAmmoButton;
        public GameObject XPTextPrefab;
        public Button ChestButton;
        public GameObject ChestBoxGameObject;
        public Button WatchVideo;
        public Text BonusReceivedText;
        public Image BonusReceivedDialog;
        public Sprite CoinSprite;
        public Sprite DiamondSprite;
        public Text CurrentandTotalAmmotext;
        public GameObject WeaponAmmoDisplay;
        public Text FreeText;
        public Text RemainingTimeForChestToOpenText;
        public RectTransform GameOverCanvasRectTransform;
        public Toggle PlayGameToggle;
        public Canvas TutorialCanvas;
        public Image SwipeUp;
        public Image SwipeDown;
        public Image SwipeLeft;
        public Image SwipeRight;
        public Image TapToShoot;
        public Image TapOnEnemies;
        public Image TutorialCompleted;
        public FixedJoystick FixedJoystick;
        public Button ZoomToWeapon;
        public LayerMask NothingLayerMask;

        private static Player m_Tutorialplayer;

        private readonly Color m_minHealthColor = Color.red;
        private readonly Color m_MaxHealthColor = Color.green;
        private static Image m_HealthBarColorImage;
        private static Coroutine hideGameObjectCoroutine;
        private Coroutine scaleUiObject;
        private static Button m_pauseButton;
        private static Button m_WatchResumeVideoButton;
        private static Canvas m_GameOverCanvas;
        private static Canvas m_GameMenuCanvas;
        private static Button m_GameMenuCanvasButton;
        private static Text m_RemainingTimeTorevive;
        private static Canvas m_PauseCanvas;
        private static GameObject m_PauseMenu;
        private static GameObject m_DistanceTravelled;
        private static Text m_distanceTravelledText;
        private static Text m_tokensCollectedText;
        private static Text m_CurrentKillsText;
        private static GameObject m_CurrentKills;
        private static GameObject m_TokensCollected;
        private static GameObject m_RefillAmmoHolder;
        private static Button m_BuyAmmoButton;
        private static RectTransform m_GameOverCanvasRectTransform;
        private static Slider m_HealthBar;
        private static RectTransform m_SettingsMenu;
        public static bool IsSceneCanvasInitialized;
        private static Button m_ChestButton;
        private static GameObject m_ChestBoxgameObject;
        private static Button m_WatchVideo;
        private static Text m_FreeText;
        private static Text m_remainingTimeForOpeningChest;
        private static Image m_BonusDialog;
        private static Text m_BonusReceived;
        private static Sprite m_CoinSprite;
        private static Sprite m_DiamondSprite;
        private static Toggle m_PlayGamesToggle;
        private static Text m_CurrentandTotalAmmotext;
        private static GameObject m_WeaponAmmoDisplay;
        private RectTransform SettingsMenu;
        private Canvas m_SceneCanvas;
        private GameObject m_TokensGameObject;
        private Button m_BuyCoinsButton;
        private Button m_BuyDiamondsButton;
     //   private static GameObject tutorial;
        private static PlayParticleSystems playParticleSystem;
      //  private static Button tutorialButton;
        private static RectTransform GameOverCanvasRT;
        private static GameObject m_XpTextPrefab;
        private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
        private static ToggleGroup m_GraphicstoggleGroup;
        private static GameObject m_ApplyGraphicsSettings;
        private static Text m_CurrentFpsLimitText;
        private static Text m_CurrentScreenResolution;
        //tutorial images
        private static Canvas m_TutorialCanvas;
        private static Image m_SwipeUp;
        private static Image m_SwipeDown;
        private static Image m_SwipeLeft;
        private static Image m_SwipeRight;
        private static Image m_TapToShoot;
        private static Image m_TapOnEnemies;
        private static Image m_TutorialCompleted;
        private static Text m_ShootText;
        private static Button m_AutoDetectButton;
        private static FixedJoystick m_FixedJoyStick;
        private static Button m_ZoomtoWeapon;
        private static LayerMask m_nothingLayerMask;
        private static bool isZoomToWeaponEnabled;
        public Camera mainCamera;
        private static Camera activeCamera;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }

            m_Tutorialplayer = TutorialPlayer;
            //joystick refernce
            m_FixedJoyStick = FixedJoystick;
            m_ZoomtoWeapon = ZoomToWeapon;
            m_nothingLayerMask = NothingLayerMask;
            m_HealthBar = HealthBar;

            m_HealthBarColorImage = HealthBar.GetComponentInChildren<Image>();
            m_PauseMenu = PauseMenu;
            m_RefillAmmoHolder = RefillAmmoHolder;
            m_BuyAmmoButton = BuyAmmoButton;
            m_pauseButton = PauseButton;
            m_GameMenuCanvas = GameMenuCanvas;
            m_GameOverCanvas = GameOverCanvas;
            m_WatchResumeVideoButton = WatchResumeVideoButton;
            m_RemainingTimeTorevive = RemainingTimeTorevive;
            m_PauseCanvas = PauseCanvas;
            m_DistanceTravelled = DistanceTravelled;
            m_distanceTravelledText = m_DistanceTravelled.GetComponentInChildren<Text>();
            m_TokensCollected = TokensCollected;
            m_tokensCollectedText = m_TokensCollected.GetComponentInChildren<Text>();
            m_CurrentKills = CurrentKills;
            m_CurrentKillsText = m_CurrentKills.GetComponentInChildren<Text>();
            m_GameOverCanvasRectTransform = GameOverCanvasRectTransform;
            m_ChestButton = ChestButton;
            m_ChestBoxgameObject = ChestBoxGameObject;
            m_WatchVideo = WatchVideo;
            m_FreeText = FreeText;
            m_remainingTimeForOpeningChest = RemainingTimeForChestToOpenText;
            m_BonusDialog = BonusReceivedDialog;
            m_BonusReceived = BonusReceivedText;
            m_CoinSprite = CoinSprite;
            m_DiamondSprite = DiamondSprite;
            m_CurrentandTotalAmmotext = CurrentandTotalAmmotext;
            m_WeaponAmmoDisplay = WeaponAmmoDisplay;


            m_XpTextPrefab = XPTextPrefab;

            //get componenets
            m_GameMenuCanvasButton = m_GameMenuCanvas.GetComponentInChildren<Button>();
         //   tutorial = Util.FindGameObjectWithName(GameMenuCanvas.gameObject, "Tutorial");

            playParticleSystem = m_GameMenuCanvas.GetComponentInChildren<PlayParticleSystems>();

            //if (tutorial)
            //{
            //    tutorialButton = tutorial.GetComponentInChildren<Button>();

            //}
            //assign tutorial static variables;
            m_TutorialCanvas = TutorialCanvas;
            m_SwipeUp = SwipeUp;
            m_SwipeDown = SwipeDown;
            m_SwipeLeft = SwipeLeft;
            m_SwipeRight = SwipeRight;
            m_TapToShoot = TapToShoot;
            m_TapOnEnemies = TapOnEnemies;
            m_ShootText = m_TapOnEnemies.GetComponentInChildren<Text>();
            m_TutorialCompleted = TutorialCompleted;

        }

        private void Start()
        {
          //  mainCamera=Camera.main;
            activeCamera = mainCamera;
            //   m_HealthBarColorImage.color = m_MaxHealthColor;

            //load vungle ads
            VungleAds.Instance.LoadAd(Util.VungleNonSkippablePlacementId);
            VungleAds.Instance.LoadAd(Util.VungleResumeVideoPlacementId);
            VungleAds.Instance.LoadAd(Util.VungleSkippablePlacementId);

        }

        void Update()
        {
            GameOverCanvas = m_GameOverCanvas;
            GameOverCanvasRectTransform = m_GameOverCanvasRectTransform;
            if (GameOverCanvasRectTransform && GameOverCanvas.gameObject.activeSelf)
            {
                float desiredScale = Mathf.Lerp(GameOverCanvasRectTransform.localScale.x, 1f, Time.deltaTime);

                GameOverCanvasRectTransform.localScale = new Vector3(desiredScale, desiredScale, 1);
            }
            //WatchVideo = m_WatchVideo;
            //if (WatchVideo)
            //{
            //    WatchVideo.interactable = Vungle.isAdvertAvailable(Util.VungleResumeVideoPlacementId);
            //}
        }
        //add all ui pools
        public void CreateUiPools()
        {
            CreateUiPool(m_XpTextPrefab,5);
        }

        public void CreateUiPool(GameObject prefab, int noOfInstances)
        {
            int prefabInstanceId = prefab.GetInstanceID();
            if (!poolDictionary.ContainsKey(prefabInstanceId))
            {
                poolDictionary.Add(prefabInstanceId, new Queue<GameObject>());
        
            }
            for (int i = 0; i < noOfInstances; i++)
            {
                GameObject newUiInstance = Instantiate(prefab) as GameObject;
                newUiInstance.SetActive(false);
                poolDictionary[prefabInstanceId].Enqueue(newUiInstance);
            }
        }
        public GameObject GetUiFromPool(GameObject uiGameObjectPrefab,int prefabId)
        {

            if (poolDictionary.ContainsKey(prefabId) && poolDictionary[prefabId].Count > 0)
            {
                GameObject objectFromPool = poolDictionary[prefabId].Dequeue();
                return objectFromPool;
            }
            GameObject newGameObject = Instantiate(uiGameObjectPrefab) as GameObject;
            newGameObject.SetActive(false);
            poolDictionary[prefabId].Enqueue(newGameObject);
            return newGameObject;

        }
        public void UpdateHealthBar(float healthAmount,float maxHealth)
        {
            HealthBar = m_HealthBar;
            HealthBar.maxValue = maxHealth;
            HealthBar.value = healthAmount;
            m_HealthBarColorImage = HealthBar.GetComponentInChildren<Image>();
            m_HealthBarColorImage.color = Color.Lerp(m_minHealthColor, m_MaxHealthColor,
                healthAmount / 100);
        }

        public void ShowHealthBar()
        {
            HealthBar = m_HealthBar;

            HealthBar.gameObject.SetActive(true);
            
        }
        public void HideHealthBar()
        {
            HealthBar = m_HealthBar;
            if (HealthBar)
            {
                HealthBar.gameObject.SetActive(false);
            }
        }
        public void UpdateButtonText(Button button, string message)
        {
            Text text = button.GetComponentInChildren<Text>();
            text.text = message;
        }

        public void UpdateCurrentLevelXpSlider(Slider slider, float currentLevelXp , float levelMaxValue)
        {
            slider.maxValue = levelMaxValue;
            slider.value = currentLevelXp;
        }
        public void UpdateCurrentLevelXpSlider(float currentLevelXp, float levelMaxValue)
        {
            UpdateCurrentLevelXpSlider(CurrentLevelXpSlider, currentLevelXp, levelMaxValue);
        }
        public void UpdateCurrentLevelXp(Text text, float currentLevelXp , float requiredXp)
        {
            text.text = currentLevelXp.ToString() + "/" + requiredXp.ToString();
        }
     
    
        public void UpdateCurrentLevelXp(float currentLevelXp, float requiredXp)
        {
            UpdateCurrentLevelXp(CurrentLevelXp,currentLevelXp,requiredXp);
        }
        public void UpdateCurrentLevelXp(float currentLevelXp)
        {
            var currentLevel = PlayerData.PlayerProfile.CurrentLevel;
            var requiredXptoComplete = DataManager.Instance.Levels[currentLevel - 1].XpRequiredToReachNextLevel;
            UpdateCurrentLevelXp(CurrentLevelXp, currentLevelXp, requiredXptoComplete);
        }
        public void UpdateCurrentLevel(int level)
        {
            UpdateCurrentLevel(CurrentLevel,level);

        }

        public void UpdateCurrentLevel(Text text, int level)
        {
            text.text = level.ToString();
        }

        public void UpdateCoins(int currentCoins)
        {

            UpdateCoins(CurrentCoins,currentCoins);
        }

        public void UpdateCoins(Text text, int coins)
        {
            text.text = coins.ToString();
        }

        public void UpdateTokens(int currentTokens)
        {
            UpdateTokens(CurrentTokens,currentTokens);
        }

        public void UpdateTokens(Text text ,int tokens)
        {
            text.text = tokens.ToString();
        }
        public void UpdateDiamonds(int currentDiamonds)
        {

            UpdateCoins(CurrentDiamonds, currentDiamonds);
        }

        public void UpdateDiamonds(Text text, int diamonds)
        {
            text.text = diamonds.ToString();
        }
        public void InitializeSceneCanvas()
        {
            m_SceneCanvas = Instantiate(SceneCanvas) as Canvas;
            m_SceneCanvas.gameObject.transform.SetParent(gameObject.transform);
            m_SceneCanvas.enabled = false;
            CurrentLevelXpSlider = m_SceneCanvas.GetComponentInChildren<Slider>();
            Text[] texts = m_SceneCanvas.GetComponentsInChildren<Text>();
            Button[] buttons = m_SceneCanvas.GetComponentsInChildren<Button>();

            BackButton = buttons[0];
            buttons[1].onClick.AddListener(ShowSettingsMenu);
            buttons[2].onClick.AddListener(ShowStatistics);
            SceneName = texts[0];
            CurrentLevelXp = texts[1];

            CurrentLevel = texts[3];
            //play sound
            

            Tokensbutton = buttons[3];
            BuyCoinsButton = buttons[4];
            BuyDiamondsButton = buttons[5];
            BuyCoinsButton.onClick.AddListener(() => SetCurrentStoreTypeClicked(0));
            BuyDiamondsButton.onClick.AddListener(() => SetCurrentStoreTypeClicked(1));
           
            BuyCoinsButton.onClick.AddListener(ResetAndLoadLevel);
            BuyDiamondsButton.onClick.AddListener(ResetAndLoadLevel);
            CurrentCoins = BuyCoinsButton.GetComponentInChildren<Text>();
            CurrentDiamonds = BuyDiamondsButton.GetComponentInChildren<Text>();
            CurrentTokens = Tokensbutton.GetComponentInChildren<Text>();
            BackButton.gameObject.SetActive(false);

            IsSceneCanvasInitialized = true;
        }
        void ResetAndLoadLevel()
        {
            StartCoroutine(ResetAndLoadEnumerator());
        }
        IEnumerator ResetAndLoadEnumerator()
        {
            CurrentGameStats currentGameStats = PlayerData.CurrentGameStats;
            Util.ResetCurrentGameStats(currentGameStats);
            yield return new WaitForSeconds(0.2f);
            GameObject sceneLoader = new GameObject { name = "SceneLoader" };
            sceneLoader.AddComponent<SceneLoader>();
            sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(5, true);
        }
        public void DisableSceneCanavas()
        {
            if (m_SceneCanvas)
            {
                m_SceneCanvas.enabled = false;         
            }
         
        }

        public void EnableSceneCanvas()
        {
            if (m_SceneCanvas)
            {
                m_SceneCanvas.enabled = true;
            }

        }
        public void DisableBuyCoinsButton()
        {
            if (m_BuyCoinsButton)
            {
                m_BuyCoinsButton.interactable = false;

            }
        }

        public void EnableBuyCoinsButton()
        {
            if (m_BuyCoinsButton)
            {
                m_BuyCoinsButton.interactable = true;

            }
        }
        public void DisableBuyDiamondsButton()
        {
            if (m_BuyDiamondsButton)
            {
                m_BuyDiamondsButton.interactable = false;
            }
        }

        public void EnableBuyDiamondsButton()
        {
            if (m_BuyDiamondsButton)
            {
                m_BuyDiamondsButton.interactable = true;
            }
        }
        public void ShowWeaponAmmoDisplay()
        {
            WeaponAmmoDisplay = m_WeaponAmmoDisplay;
            if (WeaponAmmoDisplay)
            {
                WeaponAmmoDisplay.SetActive(true);
            }
        }

        public void HideWeaponAmmoDisplay()
        {
            WeaponAmmoDisplay = m_WeaponAmmoDisplay;
            if (WeaponAmmoDisplay)
            {
                WeaponAmmoDisplay.SetActive(false);
            }
        }

        public void BuyAmmo()
        {
            int noOfCoinsAvailable= PlayerData.PlayerProfile.NoofCoinsAvailable;
            noOfCoinsAvailable -= 25;
            PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;
            GameObject player= GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();

                if (playerShoot)
                {
                    playerShoot.CurrentWeapon.BuyAmmo();
                }
            }
            HideRefillAmmoMenu(); 
            PlayerData.SavePlayerData();
        }
        public void UpdateWeaponAmmoDisplay(int currentAmmo, int totalAmmo)
        {
            CurrentandTotalAmmotext = m_CurrentandTotalAmmotext;
            UpdateWeaponAmmoDisplay(CurrentandTotalAmmotext,currentAmmo,totalAmmo);
        }

        public void UpdateWeaponAmmoDisplay(Text text, int currentAmmo, int totalAmmo)
        {
            text.text = currentAmmo + "/" + totalAmmo;
        }
        public void ShowSceneCanvas()
        {
            if (m_SceneCanvas)
            {
                if (!m_SceneCanvas.gameObject.activeSelf)
                {
                    m_SceneCanvas.gameObject.SetActive(true);
                }
            }

        }

        public void EnableMenuCanvas()
        {
         //   GameMenuCanvas = m_GameMenuCanvas;
            m_GameMenuCanvas.enabled = true;
            if (m_GameMenuCanvas.enabled)
            {
                AlternateActivateObjects alternateActivateObjects =
                    m_GameMenuCanvas.GetComponentInChildren<AlternateActivateObjects>(true);
                if (alternateActivateObjects)
                {
                    alternateActivateObjects.enabled = true;
                }
                if (playParticleSystem)
                {
                    playParticleSystem.enabled = true;
                }
            }

            //   tutorialButton.onClick.AddListener(Util.OnTutorialComplete);
            //    tutorial.SetActive(!Util.IsTutorialComplete());
            if (GameManager.Instance)
            {
                m_GameMenuCanvasButton.onClick.AddListener(GameManager.Instance.StartGame);

            }

        }
        public void InitializeSettingsCanvas()
        {
            Canvas settingsCanvas=Instantiate(SettingsCanvas) as Canvas;
            SettingsMenu = settingsCanvas.GetComponentInChildren<RectTransform>();
            m_SettingsMenu = SettingsMenu;

            settingsCanvas.gameObject.transform.SetParent(gameObject.transform);
            Toggle[] toggles = settingsCanvas.GetComponentsInChildren<Toggle>();
            toggles[0].onValueChanged.AddListener(OnMusicToggleChanged);
            toggles[0].isOn = AudioManager.IsMusicOn; //toggled based on prefs
            toggles[1].onValueChanged.AddListener(OnSoundFxToggleChanged);
            toggles[1].isOn = AudioManager.IsSounfFxOn;
            toggles[5].onValueChanged.AddListener(OnGoogleServicesToggleChanged);
            //should add for music and sound
            PlayGameToggle = toggles[5];
            CustomEventToggleGroup graphicsTogglegrp = Util.FindGameObjectWithComponent<CustomEventToggleGroup>(settingsCanvas.gameObject, "GraphicsToggleGroup");
            m_GraphicstoggleGroup = graphicsTogglegrp;
            int autoQuality = AutoQuality.Instance.detectQuality(false,false);
            if (autoQuality==0)
            {
                DisableGraphicsToggle(2);
            }
            Text graphicsText = Util.FindGameObjectWithName(SettingsMenu.gameObject, "GraphicsChangeText").GetComponentInChildren<Text>();
            graphicsText.text = "";
            int currentQuality = AutoQuality.Instance.getCacheContents();
            if (graphicsTogglegrp)
            {

                if (currentQuality > autoQuality)
                {
                    graphicsText.color = Color.red;
                    graphicsText.text = "* Higher quality on this device may result in unexpected behaviour";
                }

                graphicsTogglegrp.OnChange += OnGraphicsToggledChanged;
            }
            switch (currentQuality)
            {
                case 0:
                    SetGraphicsToggle(0);
                    break;
                case 1:
                    SetGraphicsToggle(1);
                    break;
                case 2:
                    SetGraphicsToggle(2);
                    break;
            }
            Button closeSettings = SettingsMenu.GetComponentsInChildren<Button>()[0];
            closeSettings.onClick.AddListener(HideSettingsMenu);
            Button autoDetectButton = SettingsMenu.GetComponentsInChildren<Button>()[1];
            m_AutoDetectButton = autoDetectButton;
            autoDetectButton.onClick.AddListener(OnAutoDetectClicked);

            //show privacy policy 


            m_PlayGamesToggle = PlayGameToggle;
#if !UNITY_STANDALONE
            toggles[5].isOn = SocialManager.Instance.IsUserAuthenticatedForPlayServices();
#endif
            SettingsMenu.gameObject.SetActive(false);


            //fps and resolution
            Slider[] fpsandresolutionSliders = SettingsMenu.GetComponentsInChildren<Slider>();
            Slider fpsSlider = fpsandresolutionSliders[0];
            Slider resolutionSlider = fpsandresolutionSliders[1];
            m_CurrentFpsLimitText = fpsSlider.GetComponentInChildren<Text>();
            m_CurrentScreenResolution = resolutionSlider.GetComponentInChildren<Text>();

            fpsSlider.onValueChanged.AddListener(ChangeCurrentFPSLimit);
            resolutionSlider.onValueChanged.AddListener(ChangeCurrentScreenResolution);
            int fpsLimit = 30;
            int resolutionPercentage = 100;
            if (PlayerPrefs.HasKey("FPSLimit"))

            {
                fpsLimit = PlayerPrefs.GetInt("FPSLimit");               
            }
            else
            {
                if (autoQuality < 2)
                {
                    fpsLimit = 30;
                    PlayerPrefs.SetInt("FPSLimit", fpsLimit);
                }
                else
                {
                    fpsLimit = 60;
                    PlayerPrefs.SetInt("FPSLimit", fpsLimit);
                }
            }
            if (PlayerPrefs.HasKey("ScreenResolution"))
            {
                resolutionPercentage = PlayerPrefs.GetInt("ScreenResolution");
            }
            else
            {
                //resolutionPercentage = 80;
                if (autoQuality < 1)
                {
                    resolutionPercentage = 60;
                    PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);
                } 
                else if(autoQuality==1)
                {
                    resolutionPercentage = 80;

                    PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);

                }
                else
                {
                    resolutionPercentage = 100;

                    PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);
                }
            }


            //change fps and resolution

            fpsSlider.value = fpsLimit;
            resolutionSlider.value = resolutionPercentage/10;
            ChangeCurrentFPSLimit((int)fpsLimit);
            ChangeCurrentScreenResolution((int)resolutionPercentage);

        }

      

        private void ChangeCurrentScreenResolution(float sliderValue)
        {
            int percentage = (int)sliderValue * 10;
            Util.SetResolution(percentage);
            m_CurrentScreenResolution.text = percentage.ToString();
            PlayerPrefs.SetInt("ScreenResolution", percentage);
            PlayerPrefs.Save();
        }
        private void ChangeCurrentScreenResolution(int percentage)
        {          
            Util.SetResolution(percentage);
            m_CurrentScreenResolution.text = percentage.ToString();
        }
        private void ChangeCurrentFPSLimit(float value)
        {
            Application.targetFrameRate = (int)value;
            m_CurrentFpsLimitText.text = value.ToString();
            PlayerPrefs.SetInt("FPSLimit", (int)value);
            PlayerPrefs.Save();
        }
        private void ChangeCurrentFPSLimit(int value)
        {

            Application.targetFrameRate = (int)value;
            m_CurrentFpsLimitText.text = value.ToString();
        }

        private void OnAutoDetectClicked()
        {
            SettingsMenu = m_SettingsMenu;
            int autoQuality= AutoQuality.Instance.detectQuality(false,false);
            Text graphicsText = Util.FindGameObjectWithName(SettingsMenu.gameObject, "GraphicsChangeText").GetComponentInChildren<Text>();
            m_ApplyGraphicsSettings = Util.FindGameObjectWithName(m_SettingsMenu.gameObject, "ApplyGraphicsChange");
            graphicsText.color = Color.white;
            switch (autoQuality)
            {
                case 0:
                    SetGraphicsToggle(0);
                    graphicsText.text = "* Based on your device graphics are set to better performance";
                    break;
                case 1:
                    SetGraphicsToggle(1);
                    graphicsText.text = "* Based on your device graphics are set to optimal";
                    break;
                case 2:
                    SetGraphicsToggle(2);
                    graphicsText.text = "* Based on your device graphics are set to better quality";

                    break;
                case 3:
                    SetGraphicsToggle(2);
                    graphicsText.text = "* Based on your device graphics are set to better quality";

                    break;
                case 4:
                    SetGraphicsToggle(2);
                    graphicsText.text = "* Based on your device graphics are set to better quality";

                    break;
                case 5:
                    SetGraphicsToggle(2);
                    graphicsText.text = "* Based on your device graphics are set to better quality";

                    break;
            }

            m_ApplyGraphicsSettings.SetActive(true);

        }

        private void SetGraphicsToggle(int v)
        {
            Toggle[] toggles = m_GraphicstoggleGroup.GetComponentsInChildren<Toggle>();
            toggles[v].isOn = true;
            
        }
        private void DisableGraphicsToggle(int v)
        {
            Toggle[] toggles = m_GraphicstoggleGroup.GetComponentsInChildren<Toggle>();
            toggles[v].interactable = false;
        }

        private void EnableGraphicsToggle(int v)
        {
            Toggle[] toggles = m_GraphicstoggleGroup.GetComponentsInChildren<Toggle>();
            toggles[v].interactable = true;
        }
        private void OnGraphicsToggledChanged(Toggle newActive)
        {
            int currentQuality = QualitySettings.GetQualityLevel();
            int selectedQuality=0; 
       
            m_ApplyGraphicsSettings = Util.FindGameObjectWithName(m_SettingsMenu.gameObject, "ApplyGraphicsChange");
            int autoQuality = AutoQuality.Instance.detectQuality(false,false);
            Text graphicsText = Util.FindGameObjectWithName(SettingsMenu.gameObject, "GraphicsChangeText").GetComponentInChildren<Text>();
            graphicsText.color = Color.white;
            int setQuality=0;
            switch (newActive.name)
            {
                case "Low":
                    setQuality = 0;
                    AutoQuality.Instance.writeQuality(setQuality);                    
                    graphicsText.text = "* Graphics are set to better performance. Changes apply after restart";
                    break;
                case "Medium":
                    setQuality = 1;
                    AutoQuality.Instance.writeQuality(setQuality);
                    graphicsText.text = "* Graphics are set to optimal. Changes apply after restart";
                    break;
                case "High":
                    setQuality = 2;
                    AutoQuality.Instance.writeQuality(setQuality);
                    graphicsText.text = "* Graphics are set to better quality. Changes apply after restart";
                    break;
            }
            if(setQuality > autoQuality)
            {
                graphicsText.color = Color.red;
                graphicsText.text = "* Changing to higher quality on this device may result in unexpected behaviour";
            }
            switch (newActive.name)
            {
                case "Low":
                    selectedQuality = 0;
                    break;
                case "Medium":
                    selectedQuality = 1;
                    break;
                case "High":
                    selectedQuality = 2;
                    break;
            }
            if (selectedQuality == currentQuality)
            {
                return;
            }
            if (!newActive.isOn)
            {
                return;
            }
            m_ApplyGraphicsSettings.SetActive(true);

        }


        public void UpdateSceneName(string sceneName)
        {
            UpdateSceneName(SceneName,sceneName);
        }
        public void UpdateSceneName(Text text, string sceneName)
        {
            text.text = sceneName;
        }
        public void ShowGameOverCanvas(float timeInSeconds ,int noOfdeaths , string charatername)
        {
            var currentDeaths = PlayerData.CurrentGameStats.CurrentDeaths;
            GameOverCanvas = m_GameOverCanvas;
            PauseButton = m_pauseButton;
            PauseButton.gameObject.SetActive(false);

            GameOverCanvas.gameObject.SetActive(true);
            Button[] buttons = GameOverCanvas.GetComponentsInChildren<Button>();
            Text charactername = GameOverCanvas.GetComponentInChildren<Text>();
            charactername.text = "SAVE" +  " " + charatername.ToUpper();
            buttons[0].GetComponentInChildren<Text>().text = (10 * noOfdeaths).ToString();
            var currentDiamonds = PlayerData.PlayerProfile.NoofDiamondsAvailable;
            currentDiamonds -= 10 * currentDeaths;
            if (currentDiamonds < 0)
            {
                DisableBuywithDiamondsButton();
            }
            else
            {
                EnableBuywithDiamondsButton();
            }

            if (AdmobAdManager.Instance.isRewardedVideoReady())
            {
                m_WatchResumeVideoButton.gameObject.SetActive(true);
            }
            else
            {
                m_WatchResumeVideoButton.gameObject.SetActive(false);
            }

            hideGameObjectCoroutine =  StartCoroutine(LoadSceneAfterDelay(timeInSeconds));
        }

        public void DisableBuywithDiamondsButton()
        {
            GameOverCanvas = m_GameOverCanvas;
            Button[] buttons = GameOverCanvas.GetComponentsInChildren<Button>();
            buttons[0].interactable =false;

        }
        public void EnableBuywithDiamondsButton()
        {
            GameOverCanvas = m_GameOverCanvas;
            Button[] buttons = GameOverCanvas.GetComponentsInChildren<Button>();
            buttons[0].interactable = true;

        }
        public void StopHideGameObjectCoroutine()
        {
            StopCoroutine(hideGameObjectCoroutine);
        }
        private IEnumerator LoadSceneAfterDelay(float timeInSeconds)
        {
            int totalTime = 8;

            GameOverCanvas = m_GameOverCanvas;
            RemainingTimeTorevive = m_RemainingTimeTorevive;
            while (totalTime > 0)
            {
                if (RemainingTimeTorevive)
                {
                    RemainingTimeTorevive.text = (totalTime -= 1).ToString();
                }
                yield return new WaitForSeconds(1);
            }
            //StartCoroutine(ShowRemainingTimeToRevive());
            //   yield return new WaitForSeconds(timeInSeconds);
            if (totalTime <= 0)
            {
                if (GameOverCanvas)
                {
                    GameOverCanvas.gameObject.SetActive(false);
                }
                GameObject sceneLoader = new GameObject { name = "SceneLoader" };
                sceneLoader.AddComponent<SceneLoader>();
                sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(6, false);
            }

        }

        public void HideGameOverCanvas()
        {
            GameOverCanvas = m_GameOverCanvas;
            GameOverCanvas.gameObject.SetActive(false);
            StopCoroutine(hideGameObjectCoroutine);
            
        }

        public IEnumerator ScaleUiGameObject(RectTransform rectTransform, Vector3 desiredScale)
        {

            float currentTime = 0.0f;
            float time = 2f;
            if (!rectTransform) yield break;
            do
            {
                rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, desiredScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= time);
        }
        public IEnumerator ScaleUiGameObject(RectTransform rectTransform, Vector3 desiredScale,bool moveUp ,Action callback)
        {

            float currentTime = 0.0f;
            float time = 2f;
            if (!rectTransform) yield break;
            do
            {
                rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, desiredScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= time);

            if (moveUp)
            {
                rectTransform.gameObject.GetComponent<Animator>().enabled = true;
            }
            callback();
        }

        public void ResetScaleUi(RectTransform rectTransform)
        {
            rectTransform.localScale=new Vector3(2,2,2);
        }

        public void ShowPauseButton()
        {
            PauseButton = m_pauseButton;
            PauseButton.gameObject.SetActive(true);
        }

        public void ShowPauseMenu()
        {
            PauseButton = m_pauseButton;
            PauseMenu = m_PauseMenu;
            PauseButton.gameObject.SetActive(false);
            PauseMenu.SetActive(true);
            GameManager.Instance.PauseGame();

        }
        public void HidePauseMenu()
        {
            PauseButton = m_pauseButton;
            PauseMenu = m_PauseMenu;
            PauseButton.gameObject.SetActive(true);
            PauseMenu.SetActive(false);
            GameManager.Instance.ResumeGame();
        }

        public void ShowRefillAmmoMenu()
        {
            RefillAmmoHolder = m_RefillAmmoHolder;
            BuyAmmoButton = m_BuyAmmoButton;
            RefillAmmoHolder.SetActive(true);
            Animator refilAnimator = RefillAmmoHolder.GetComponent<Animator>();
            if (refilAnimator)
            {
                refilAnimator.SetBool("close",false);
            }

            if (PlayerData.PlayerProfile.NoofCoinsAvailable < 25)
            {
                BuyAmmoButton.interactable = false;

            }
            else
            {
                BuyAmmoButton.interactable = true;
            }
        }

        public void HideRefillAmmoMenu()
        {
            RefillAmmoHolder = m_RefillAmmoHolder;
            if (RefillAmmoHolder)
            {
                RefillAmmoHolder.SetActive(false);
                Animator refilAnimator = RefillAmmoHolder.GetComponent<Animator>();
                if (refilAnimator && RefillAmmoHolder.activeSelf)
                {
                    refilAnimator.SetBool("close", true);
                }
            }
        
        }
        public void ShowXPText(Vector3 playerPos, float amount,XPType xpType)
        {
            PauseCanvas = m_PauseCanvas;
            GameObject xpText= GetUiFromPool(m_XpTextPrefab, m_XpTextPrefab.GetInstanceID());
            if (xpText == null)
            {
                xpText=Instantiate(m_XpTextPrefab) as GameObject;
            }
            if (xpText)
            {
                if (xpText.GetComponent<Text>())
                {
                    if (xpType == XPType.ObstacleDodged)
                    {
                        xpText.GetComponent<Text>().text = "[Near Miss] " + "+" + amount + "XP";
                    }
                    else if (xpType == XPType.KilledEnemy)
                    {
                        xpText.GetComponent<Text>().text = "[Enemy Killed] " + "+" + amount + "XP";
                    }
                    else if(xpType==XPType.DestroyedDrone)
                    {
                        xpText.GetComponent<Text>().text = "[Enemy Drone Destroyed] " + "+" + amount + "XP";

                    }

                }

                if (Camera.main)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(playerPos);
                    xpText.transform.SetParent(PauseCanvas.transform, false);
                    xpText.transform.position = position;
                    xpText.SetActive(true);
                }
                
               
            }

    
        }


        public void ExitGame()
        {
#if UNITY_ANDROID
            if (SocialManager.Instance.IsUserAuthenticatedForPlayServices())
            {
                SocialManager.Instance.SaveDataToGpgCloud(true);

            }
            else
            {
                Application.Quit();
            }
#endif
        }

        public void AssignSceneCanvasButtonActions(string sceneName)
        {
            GameObject sceneLoader = new GameObject {name = "SceneLoader"};
            sceneLoader.AddComponent<SceneLoader>();
            if (sceneName == "game_play_scene" || sceneName== "Home")
            {
                if (BackButton)
                {
                    BackButton.gameObject.SetActive(false);

                }
            }
            else if (sceneName=="Game_over")
            {
                if (BackButton)
                {
                    BackButton.gameObject.SetActive(false);

                }
            }
            else
            {
                if (BackButton)
                {
                    
                    BackButton.gameObject.SetActive(true);
                    BackButton.onClick.RemoveAllListeners();
                    if (PlayerData.CurrentGameStats!=null)
                    {
                        BackButton.onClick.AddListener(() => Util.ResetCurrentGameStats(PlayerData.CurrentGameStats));
                    }                    
                    BackButton.onClick.AddListener(() => sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(2,true)); 
                }
            
            }
            if (BuyCoinsButton && BuyDiamondsButton)
            {
                if (sceneName == "Store" || sceneName =="GameOver")
                {
                    BuyCoinsButton.interactable = false;
                    BuyDiamondsButton.interactable = false;
                }
                else
                {
                    BuyCoinsButton.interactable = true;
                    BuyDiamondsButton.interactable = true;
                }
                BuyCoinsButton.onClick.RemoveAllListeners();
                BuyDiamondsButton.onClick.RemoveAllListeners();
                BuyCoinsButton.onClick.AddListener(() => SetCurrentStoreTypeClicked(0));
                BuyDiamondsButton.onClick.AddListener(() => SetCurrentStoreTypeClicked(1));
                BuyCoinsButton.onClick.AddListener(() => sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(5,true));
                BuyDiamondsButton.onClick.AddListener(() => sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(5,true));


            }
        }

        void SetCurrentStoreTypeClicked(int value)
        {
            PlayerData.CurrentGameStats.CurrentStoreTypeClicked = value;
        }
        public void ShowSettingsMenu()
        {
            SceneCanvas.enabled = false;
            int currentQuality = AutoQuality.Instance.getCacheContents();
            int autoQuality = AutoQuality.Instance.detectQuality(false,false);
            SettingsMenu = m_SettingsMenu;
            SettingsMenu.gameObject.SetActive(true);
            if (GameManager.Instance)
            {
                if (GameManager.Instance.IsGameActive())
                {
                    DisableGraphicsToggle(0);
                    DisableGraphicsToggle(1);
                    DisableGraphicsToggle(2);
                    m_AutoDetectButton.interactable = false;
                }
                else
                {

                    EnableGraphicsToggle(0);
                    EnableGraphicsToggle(1);
                    EnableGraphicsToggle(2);
                    if (autoQuality == 0)
                    {
                        DisableGraphicsToggle(2);
                    }
                    m_AutoDetectButton.interactable = true;
                }
            }

            switch (currentQuality)
            {
                case 0:
                    SetGraphicsToggle(0);
                    break;
                case 1:
                    SetGraphicsToggle(1);
                    break;
                case 2:
                    SetGraphicsToggle(2);
                    break;
            }

            //track appsflyer custom event
            AppsFlyerStartUp.Instance.TrackCustomEvent(AFInAppEvents.VIEW_SETTINGS);
            FirebaseInitializer.Instance.LogClickEvent(AFInAppEvents.VIEW_SETTINGS);

        }

        public void HideSettingsMenu()
        {

            SettingsMenu = m_SettingsMenu;
            SettingsMenu.gameObject.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                return;
            }
            EnableSceneCanvas();
        }

        public void OnTimedChestClicked()
        {
            DisableChestButton();
            ChestTimer.Instance.TimedChestClicked();
        }

        public void EnableChestButton()
        { 
            ChestButton = m_ChestButton;    
            ChestButton.interactable = true;
        }

        //public IEnumerator PlayChestBoxAnimation()
        //{
        //    ChestBoxGameObject = m_ChestBoxgameObject;
        //    if (ChestBoxGameObject!=null)
        //    {
        //        Animator chestAnim = ChestBoxGameObject.GetComponent<Animator>();
        //        ChestBoxGameObject.SetActive(true);
        //        yield return new WaitForSeconds(2);
        //        if (chestAnim!=null)
        //        {
        //            if (chestAnim.gameObject.activeSelf)
        //            {
        //                if (chestAnim.runtimeAnimatorController != null)
        //                {
        //                    chestAnim.SetBool("ChestReady", true);
        //                }
        //            }
        //        }
           
               
        //    }              
        //}

        //public IEnumerator StopChestBoxAnimation()
        //{
        //    ChestBoxGameObject = m_ChestBoxgameObject;
        //    if (ChestBoxGameObject != null)
        //    {
        //        Animator chestAnim = ChestBoxGameObject.GetComponent<Animator>();
        //        if (chestAnim)
        //        {
        //            if (chestAnim.runtimeAnimatorController != null)
        //            {
        //                chestAnim.SetBool("ChestReady", false);
        //            }
        //        }
        //        yield return null;
        //    }
        //}
        public void DisableChestButton()
        {
            ChestButton = m_ChestButton;
            ChestButton.interactable = false;
        }

        public void HideChestButton()
        {
            ChestButton = m_ChestButton;
            ChestButton.gameObject.SetActive(false);
        }
        public bool IsChestButtonInteractable()
        {
            ChestButton = m_ChestButton;
           return ChestButton.IsInteractable();
        }

        public void HideFreeText()
        {
            FreeText = m_FreeText;
            if (FreeText)
            {
                FreeText.gameObject.SetActive(false);
            }
        }

        public void ShowFreetext()
        {
            FreeText = m_FreeText;
            if (FreeText)
            {
                FreeText.gameObject.SetActive(true);
            }
        }
        public void UpdateRemainingTimeForChest(string remainingTime)
        {
           
            RemainingTimeForChestToOpenText = m_remainingTimeForOpeningChest;
            if (RemainingTimeForChestToOpenText)
            {
                
                RemainingTimeForChestToOpenText.text = remainingTime;
            }
        }

        public void UpdateBonusReceivedText(string message , Rewardtype rewardtype)
        {
            BonusReceivedText = m_BonusReceived;
            BonusReceivedText.text = message;
            CoinSprite = m_CoinSprite;
            DiamondSprite = m_DiamondSprite;
            if (rewardtype == Rewardtype.Coins)
            {
                BonusReceivedText.GetComponentInChildren<Image>().sprite = CoinSprite;

            }
            else if (rewardtype==Rewardtype.Diamonds)
            {
                BonusReceivedText.GetComponentInChildren<Image>().sprite = DiamondSprite;
            }
        }

 
        public void ShowBonusReceivedDialog()
        {
            BonusReceivedDialog = m_BonusDialog;
            BonusReceivedDialog.gameObject.SetActive(true);
        }

        public void CloseBonusReceivedDialog()
        {
            BonusReceivedDialog = m_BonusDialog;
            BonusReceivedDialog.gameObject.SetActive(false);

        }
        public void OnGoogleServicesToggleChanged(bool value)
        {
#if !UNITY_STANDALONE
            if (value)
            {
                SocialManager.Instance.AuthenticateUser();
            }
            else
            {
                SocialManager.Instance.SignoutPlayServices();
            }
#endif

        }

        public void OnMusicToggleChanged(bool isOn)
        {
            if (isOn)
            {
                AudioManager.Instance.UnMuteGameMusic();
                PlayerPrefs.SetInt("IsMusicOn",1);
            }
            else
            {

                AudioManager.Instance.MuteGameMusic();
                PlayerPrefs.SetInt("IsMusicOn", 0);
            }
            PlayerPrefs.Save();
        }

        public void OnSoundFxToggleChanged(bool isOn)
        {
            if (isOn)
            {
                AudioManager.Instance.UnMuteSoundFx();
                PlayerPrefs.SetInt("IsSoundFxOn", 1);

            }
            else
            {
                AudioManager.Instance.MuteSoundFx();
                PlayerPrefs.SetInt("IsSoundFxOn", 0);

            }
            PlayerPrefs.Save();
        }
        public void OnLeaderboardClicked()
        {
            SocialManager.Instance.ShowLeaderBoardUi();
        }

        public void UpdatePlayGamesToggle(bool isOn)
        {
            PlayGameToggle = m_PlayGamesToggle;
            PlayGameToggle.isOn = isOn;
        }

        public void UpdateUi()
        {
            int currentLevel = PlayerData.PlayerProfile.CurrentLevel;
            int currentLevelXp = PlayerData.PlayerProfile.CurrentLevelXp;
            int requiredXptoComplete = DataManager.Instance.Levels[currentLevel - 1].XpRequiredToReachNextLevel;
            int currentCoins = PlayerData.PlayerProfile.NoofCoinsAvailable;
            int currentDiamonds = PlayerData.PlayerProfile.NoofDiamondsAvailable;
            int currentTokens = PlayerData.PlayerProfile.NoofTokensAvailable;
            UpdateCurrentLevel(currentLevel);
            UpdateCurrentLevelXp(currentLevelXp, requiredXptoComplete);
            UpdateCurrentLevelXpSlider(currentLevelXp, requiredXptoComplete);
            UpdateCoins(currentCoins);
            UpdateTokens(currentTokens);
            UpdateDiamonds(currentDiamonds);
            
        }

        private IEnumerator ShowRemainingTimeToRevive()
        {
            int totalTime=8;
            RemainingTimeTorevive = m_RemainingTimeTorevive;
            while (totalTime>0)
            {
                if (RemainingTimeTorevive)
                {
                    RemainingTimeTorevive.text = (totalTime -= 1).ToString();
                }
                yield return new WaitForSeconds(1);
            }

        }
        //shows statistics of player
        public void ShowStatistics()
        {
            if (m_SceneCanvas != null)
            {
                GameObject statisticsPanel = Util.FindGameObjectWithName(m_SceneCanvas.gameObject, "StatisticsPanel");
                statisticsPanel.SetActive(true);

            }
            //track appsflyer event
            AppsFlyerStartUp.Instance.TrackCustomEvent(AFInAppEvents.VIEW_STATS);
            //log firebase view event
            FirebaseInitializer.Instance.LogClickEvent(AFInAppEvents.VIEW_STATS);
        }

        public void OnRateButtonClicked()
        {
            AppsFlyerStartUp.Instance.TrackCustomEvent(AFInAppEvents.CLICK_RATE_NOW);
            FirebaseInitializer.Instance.LogClickEvent(AFInAppEvents.CLICK_RATE_NOW);
            Util.RateGame();
        }

        public void OnReviewLaterClicked()
        {
            Util.ReviewLater();
        }
        

        //tutorial mrthods

        public void ShowSwipeUp()
        {
            m_SwipeUp.enabled = true;
        }
        public void HideSwipeUp()
        {
            m_SwipeUp.enabled = false;

        }
        public void ShowSwipeDown()
        {
            m_SwipeDown.enabled = true;
        }
        public void HideSwipeDown()
        {
            m_SwipeDown.enabled = false;
        }
        public void ShowSwipeLeft()
        {
            m_SwipeLeft.enabled = true;
        }
        public void HideSwipeLeft()
        {
            m_SwipeLeft.enabled = false;
        }
        public void ShowSwipeRight()
        {
            m_SwipeRight.enabled = true;

        }
        public void HideSwipeRight()
        {
            m_SwipeRight.enabled = false;

        }
        public Image ShowTapToShoot()
        {
            m_TapToShoot.enabled = true;
            return m_TapToShoot;
        }
        public void HideTapToShoot()
        {
            m_TapToShoot.enabled = false;
        }

        public void ShowIndicatorText(string text)
        {
            m_ShootText.text = text;
            m_TapOnEnemies.gameObject.SetActive(true);
        }
        public void HideTapOnEnemiesText()
        {
            m_TapOnEnemies.gameObject.SetActive(false);
        }
        public void ShowTutorialCompletedImage()
        {
            m_TutorialCompleted.gameObject.SetActive(true); 
        }
        public void HideTutorialCompletedImage()
        {
            m_TutorialCompleted.gameObject.SetActive(false);
        }
        public void HideTutorial()
        {
            HideSwipeUp();
            HideSwipeDown();
            HideSwipeLeft();
            HideSwipeRight();
            HideTapToShoot();
            HideTapOnEnemiesText();

        }
        public IEnumerator OnTutorialCompleted()
        {
            //log appsflyer event
            Dictionary<string, string> TutorialCompletiondEvent = new Dictionary<string, string>();
            TutorialCompletiondEvent.Add(AFInAppEvents.SUCCESS, "true");
            TutorialCompletiondEvent.Add(AFInAppEvents.CONTENT_TITLE, "Getting started");
            AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.TUTORIAL_COMPLETION,TutorialCompletiondEvent);
            //log firebase event
            FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventTutorialComplete);

            PlayerPrefs.SetInt("IsTutorialComplete", 1);
            PlayerPrefs.Save();
            ShowTutorialCompletedImage();
            yield return new WaitForSeconds(3);

            GameObject sceneloader = GameObject.Find("SceneLoader");

            sceneloader.GetComponent<SceneLoader>().LoadSceneAsync(2, false, m_TutorialCanvas);
        }
        public void OnTutorialSkipped()
        {
            PlayerPrefs.SetInt("IsTutorialComplete", 1);
            PlayerPrefs.Save();
            GameObject sceneloader = GameObject.Find("SceneLoader");

            sceneloader.GetComponent<SceneLoader>().LoadSceneAsync(2, false, m_TutorialCanvas);
        }

        public void SkipTutorial()
        {
            OnTutorialSkipped();
        }

        public void EnableJoyStick()
        {
            m_FixedJoyStick.gameObject.SetActive(true);
        }

        public void EnableJoyStickAnimator()
        {
            m_FixedJoyStick.GetComponent<Animator>().enabled = true;
        }

        public void DisableJoyStickAnimator()
        {
            m_FixedJoyStick.GetComponent<Animator>().enabled = false;
        }

        public void DisableJoyStick()
        {
            m_FixedJoyStick.gameObject.SetActive(false);
        }

        public Camera GetActiveCamera()
        {
            return activeCamera;
        }

        public void EnableZoomToWeaponButton()
        {
            m_ZoomtoWeapon.gameObject.SetActive(true);
        }

        public void DisableZoomToWeaponButton()
        {
            m_ZoomtoWeapon.gameObject.SetActive(false);
        }
        public void ZoomToWeapons()
        {
            isZoomToWeaponEnabled = !isZoomToWeaponEnabled;
            GameObject player= GameManager.m_InstantiatedPlayer;

            if (player == null)
            {
                //get tutorial player
                player = m_Tutorialplayer.gameObject;
            }

            if (player)
            {
                PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
                Camera zoomToWeaponCamera = playerShoot.CurrentWeapon.ZoomToWeaponCamera;

                zoomToWeaponCamera.transform.localPosition = playerShoot.CurrentWeapon.DefaultCamTransform;

                if (isZoomToWeaponEnabled)
                {
                    //enable zoom camera and disable main cam
                    Camera.main.cullingMask = NothingLayerMask;
                    zoomToWeaponCamera.gameObject.SetActive(true);
                    activeCamera = zoomToWeaponCamera;
                    m_FixedJoyStick.gameObject.SetActive(false);
                }
                else
                {

                    zoomToWeaponCamera.gameObject.SetActive(false);
                    Camera.main.cullingMask = ~(1 << 5);  //5 for UI layer
                    activeCamera = Camera.main;
                    m_FixedJoyStick.gameObject.SetActive(true);
                }
            }
     

        }

        public void SetisZoomToWeaponEnabled(bool value)
        {
            isZoomToWeaponEnabled = value;
            GameObject player = GameManager.m_InstantiatedPlayer;
            if (player == null)
            {
                player = m_Tutorialplayer.gameObject;
            }
            if (player)
            {
                PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();
                Camera zoomToWeaponCamera = playerShoot.CurrentWeapon.ZoomToWeaponCamera;
                zoomToWeaponCamera.gameObject.SetActive(value);
            }
  
            activeCamera = Camera.main;
            Camera.main.cullingMask = ~(1 << 5);  //5 for UI layer
        }

        public void UpdateDistanceTravelled(float value)
        {
            m_distanceTravelledText.text = value.ToString();
        }

        public void UpdateCurrentKills(int value)
        {
            m_CurrentKillsText.text = value.ToString();
        }

        public void UpdateTokensCollected(int value)
        {
            m_tokensCollectedText.text = value.ToString();
        }

        public void ShowCurrentStatsInPauseCanvas()
        {
            m_DistanceTravelled.SetActive(true);
            m_CurrentKills.SetActive(true);
            m_TokensCollected.SetActive(true);
        }

        public void ChangeCameraCullingMask()
        {
            

        }
    }

}

public enum XPType
{
    ObstacleDodged,
    KilledEnemy,
    DestroyedDrone
}

