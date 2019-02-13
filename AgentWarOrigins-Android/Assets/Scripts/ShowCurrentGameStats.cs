using System;
using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;
using UnityEngine.UI;

public class ShowCurrentGameStats : MonoBehaviour
{
    //constant fields
    public Text TokensCollected;
    public Text XpEarned;
    public Text DistanceTravelled;
    public Text Jumps;
    public Text Kills;
    public Text ObstaclesDodged;
    public Text Deaths;
    public Text DronesDestroyed;
    public Text VehiclesDestroyed;
    public GameObject LevelUpPrefab;
    //level up display
    //public GameObject LevelUpDisplay;
    //public Text LevelUpText;
    //public Text LevelReachedValue;
    //public Text XpEarnedForLevel;
    //public GameObject NowAvailable;
    //public GameObject AvailableRewardPrefab;
    //public GameObject RewardsDisplayPanel;
    //public GameObject RewardDisplayContent;

    //values
    public Text TokensCollectedAmount;
    public Text XpEarnedAmount;

    public Text DistanceTravelledAmount;
    public Text JumpsAmount;
    public Text KillsAmount;
    public Text ObstaclesDodgedAmount;
    public Text DeathsAmount;
    public Text DronesDestoyedValue;
    public Text VehiclesDestoyedValue;

    public Button PlayAgainButton;
    public Button DoubleXpButton;
    //rate us panel
    public GameObject RateUsPanel;
    public float MsToWaitToShowRatePanel;
    private ulong LastRateShowTime;

    private Vector3 DesiredScale;
    private float scale;
    private CurrentGameStats currentGameStats;
    private AudioSource m_AudioSource;
    private List<GameObject> rewardSprites=new List<GameObject>();
    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.25f);
    private GameObject m_player;
    private void Awake()
    {
        currentGameStats = PlayerData.CurrentGameStats;
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    private void Start()
    {
        if (PlayerPrefs.HasKey("LastRateShowTime"))
        {
            LastRateShowTime = ulong.Parse(PlayerPrefs.GetString("LastRateShowTime"));
        }
        else
        {
            LastRateShowTime = 0;
        }

      //  DoubleXpButton.interactable = Vungle.isAdvertAvailable(Util.VungleNonSkippablePlacementId);
        PlayAgainButton.onClick.AddListener(()=>Util.ResetCurrentGameStats(currentGameStats));
        UiManager.Instance.UpdateUi();

        InputControl inputcontrol;
        PlayerControl playercontrol;
        Animator playeranimator;
        m_player = GameManager.m_InstantiatedPlayer;
        if (m_player == null)
        {
            m_player= GameManager.Instance.InitializeSelectedCharacter();
        }
        inputcontrol = m_player.GetComponent<InputControl>();
        playercontrol = m_player.GetComponent<PlayerControl>();
        playeranimator = m_player.GetComponent<Animator>();
        if (playeranimator.runtimeAnimatorController == null)
        {
            playeranimator.runtimeAnimatorController = m_player.GetComponent<Player>().PlayerAnimatorController;
        }
        if (inputcontrol)
        {
            Destroy(inputcontrol);
        }

        if (playercontrol)
        {
            Destroy(playercontrol);
        }


       m_player.transform.position= new Vector3(0,-120,-125);
        m_player.transform.eulerAngles = new Vector3(0, 90, 0);
        m_player.transform.localScale = new Vector3(100, 100, 100);
        m_player.GetComponent<Animator>().applyRootMotion = false;
        m_player.SetActive(true);
        StartCoroutine(ShowCurrentStats());

    }

    private IEnumerator ShowCurrentStats()
    {
        //show curent game stats
        TokensCollected.gameObject.SetActive(true);
        TokensCollectedAmount.gameObject.SetActive(true);
        TokensCollectedAmount.text = currentGameStats.TokensCollected.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(TokensCollected.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);
        yield return waitForSeconds;
        XpEarned.gameObject.SetActive(true);
        XpEarnedAmount.gameObject.SetActive(true);
        XpEarnedAmount.text = currentGameStats.CurrentXpEarned.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(XpEarned.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);
        yield return waitForSeconds;
        DistanceTravelled.gameObject.SetActive(true);
        DistanceTravelledAmount.gameObject.SetActive(true);
        DistanceTravelledAmount.text = currentGameStats.CurrentDistanceTravelled.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(DistanceTravelled.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);

        //yield return waitForSeconds;
        //Jumps.gameObject.SetActive(true);
        //JumpsAmount.gameObject.SetActive(true);
        //JumpsAmount.text = currentGameStats.NoOfJumps.ToString();
        //StartCoroutine(UiManager.Instance.ScaleUiGameObject(Jumps.rectTransform, new Vector3(1, 1, 1)));
        //AudioManager.Instance.PlaySound(m_AudioSource);
        yield return waitForSeconds;
        ObstaclesDodged.gameObject.SetActive(true);
        ObstaclesDodgedAmount.gameObject.SetActive(true);
        ObstaclesDodgedAmount.text = currentGameStats.CurrentObstaclesDodged.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(ObstaclesDodged.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);

        yield return waitForSeconds;
        Kills.gameObject.SetActive(true);
        KillsAmount.gameObject.SetActive(true);
        KillsAmount.text = currentGameStats.CurrentKills.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(Kills.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);
    
        yield return waitForSeconds;
        Deaths.gameObject.SetActive(true);
        DeathsAmount.gameObject.SetActive(true);
        DeathsAmount.text = currentGameStats.CurrentDeaths.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(Deaths.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);
        yield return waitForSeconds;
        DronesDestroyed.gameObject.SetActive(true);
        DronesDestoyedValue.gameObject.SetActive(true);
        DronesDestoyedValue.text = currentGameStats.CurrentDronesDestroyed.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(DronesDestroyed.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);
        yield return waitForSeconds;
        VehiclesDestroyed.gameObject.SetActive(true);
        VehiclesDestoyedValue.gameObject.SetActive(true);
        VehiclesDestoyedValue.text = currentGameStats.CurrentEnemyVehiclesDestroyed.ToString();
        StartCoroutine(UiManager.Instance.ScaleUiGameObject(VehiclesDestroyed.rectTransform, new Vector3(1, 1, 1)));
        AudioManager.Instance.PlaySound(m_AudioSource);
        //yield return waitForSeconds;
        //CoinsCollected.gameObject.SetActive(true);
        //CoinsCollectedvalue.gameObject.SetActive(true);
        //CoinsCollectedvalue.text = (currentGameStats.CoinsCollected + Mathf.Max(1,(int)currentGameStats.CurrentXpEarned/100)).ToString(); //min 1 coins
        //StartCoroutine(UiManager.Instance.ScaleUiGameObject(CoinsCollected.rectTransform, new Vector3(1, 1, 1)));
        //AudioManager.Instance.PlaySound(m_AudioSource);

        //yield return waitForSeconds;
        //DiamondsCollected.gameObject.SetActive(true);
        //DiamondsCollectedValue.gameObject.SetActive(true);
        //DiamondsCollectedValue.text = (currentGameStats.DiamondsCollected + (Mathf.Max(0,(int)currentGameStats.CurrentXpEarned / 100))).ToString(); //min 0 diamonds
        //StartCoroutine(UiManager.Instance.ScaleUiGameObject(DiamondsCollected.rectTransform, new Vector3(1, 1, 1)));
        //AudioManager.Instance.PlaySound(m_AudioSource);

        yield return waitForSeconds;
        //if level up show level up 

        ShowLevelUpDisplay();
    //    DisplayLevelUp();

        //LevelUpDisplay levelUpDisplay = LevelUpPrefab.GetComponent<LevelUpDisplay>();
        //if (levelUpDisplay)
        //{
        //    levelUpDisplay.DisplayLevelUp();
        //}
        //show menu and retry
        PlayAgainButton.gameObject.SetActive(true);
        DoubleXpButton.gameObject.SetActive(true);
        if (currentGameStats.CurrentXpEarned > 0 && AdmobAdManager.Instance.isRewardedVideoReady())
        {
            DoubleXpButton.interactable = true;
        }
        else
        {
            DoubleXpButton.interactable = false;
        }

        //show rate us panel
        if (ShowRateUsPanel())
        {
            bool isAlreadyRated=false;
            if (PlayerPrefs.HasKey("AlreadyRated"))
            {
                isAlreadyRated = (PlayerPrefs.GetInt("AlreadyRated") == 1) ? true : false;
            }
            if (!isAlreadyRated)
            {
                RateUsPanel.SetActive(true);
            }
            else
            {
                RateUsPanel.SetActive(false);
            }
        }

        //show ad here
        AdmobAdManager.Instance.ShowInterstitial();


        //save data locally
        PlayerData.SavePlayerData();

        float longestDistanceTravelledinOneRun = PlayerData.PlayerProfile.LongestDistanceTravelledInOneRun;
        if (currentGameStats.CurrentDistanceTravelled > longestDistanceTravelledinOneRun)
        {
            longestDistanceTravelledinOneRun = currentGameStats.CurrentDistanceTravelled;
            PlayerData.PlayerProfile.LongestDistanceTravelledInOneRun = longestDistanceTravelledinOneRun;
        }
        //post to leaderboard
        SocialManager.Instance.PostToLeaderboard(GPGSIds.leaderboard_total_distance_travelled, PlayerData.PlayerProfile.TotalDistanceTravelled);
        SocialManager.Instance.PostToLeaderboard(GPGSIds.leaderboard_longest_distance_in_one_run,longestDistanceTravelledinOneRun);
        SocialManager.Instance.PostToLeaderboard(GPGSIds.leaderboard_total_kills, PlayerData.PlayerProfile.NoofEnemieskilled);

        //save data to cloud
        SocialManager.Instance.SaveDataToGpgCloud(false);
        //add to player profile
     //   AddToPlayerProfile(Mathf.Max(1, (int)currentGameStats.CurrentXpEarned / 100), (Mathf.Max(0, (int)currentGameStats.CurrentXpEarned / 1000)));
    }

    void AddToPlayerProfile(int coins,int diamonds)
    {
        // PlayerProfile playerProfile = SaveLoadManager.Instance.Load();
        PlayerData.PlayerProfile.NoofCoinsAvailable += coins;
        PlayerData.PlayerProfile.NoofDiamondsAvailable += diamonds;
        UiManager.Instance.UpdateCoins(PlayerData.PlayerProfile.NoofCoinsAvailable);
        UiManager.Instance.UpdateDiamonds(PlayerData.PlayerProfile.NoofDiamondsAvailable);
      
        //savenm data
        PlayerData.SavePlayerData();
    }

    //public void DisplayLevelUp()
    //{
        
    //    if (GameManager.LevelStarted < PlayerData.PlayerProfile.CurrentLevel)
    //    {
    //        GameManager.LevelStarted++;
    //        LevelUpDisplay.SetActive(true);
    //        StartCoroutine(UiManager.Instance.ScaleUiGameObject(LevelUpText.rectTransform, new Vector3(1, 1, 1), true,
    //            delegate
    //            {
    //                LevelReachedValue.text = "Reached Level " + GameManager.LevelStarted;
    //                LevelReachedValue.gameObject.SetActive(true);
    //            //    AudioManager.Instance.PlaySound(m_AudioSource);
    //                Level currentLevelReached = DataManager.Instance.GetLevel(GameManager.LevelStarted);
    //                foreach (var rewardImageSprite in currentLevelReached.RewardsSprite)
    //                {
    //                    if (currentLevelReached.RewardsSprite.Length > 0)
    //                    {
    //                        NowAvailable.SetActive(true);
    //                        RewardsDisplayPanel.SetActive(true);
    //                        GameObject reward = Instantiate(AvailableRewardPrefab) as GameObject;
    //                        reward.transform.SetParent(RewardDisplayContent.transform);
    //                        reward.transform.localScale = new Vector3(1, 1, 1);
    //                        RectTransform rectTransform = reward.GetComponent<RectTransform>();
    //                        rectTransform.localPosition = new Vector3(rectTransform.localPosition.z, rectTransform.localPosition.y, 0);
    //                        Image rewardImage = reward.GetComponentInChildren<Image>();
    //                        rewardImage.sprite = rewardImageSprite;
    //                        rewardSprites.Add(reward);
    //                    }

    //                }
    //            }));

    //    }

    public void ShowLevelUpDisplay()
    {
        LevelUpDisplay levelUpDisplay = LevelUpPrefab.GetComponent<LevelUpDisplay>();
        if (levelUpDisplay)
        {
            levelUpDisplay.DisplayLevelUp();
        }
    }

    bool ShowRateUsPanel()
    {
        var diff = (ulong)DateTime.Now.Ticks - LastRateShowTime;
        var millisec = diff / TimeSpan.TicksPerMillisecond;
        var secondsLeft = (MsToWaitToShowRatePanel - millisec) / 1000;
        if (secondsLeft < 0)
            return true;
        return false;
    }

    private void OnDestroy()
    {
        Destroy(m_player);
    }

}

    //public void ResetLevelUpTextScale()
    //{
    //    UiManager.Instance.ResetScaleUi(LevelUpText.rectTransform);
    //    foreach (var rewardgo in rewardSprites)
    //    {
    //        Destroy(rewardgo);

    //    }
    //    rewardSprites.Clear();
    //}



