using System;
using System.Collections.Generic;
using System.ComponentModel;
using EndlessRunner;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public bool ShowNotificationOnComplete;
    public int noOfActiveMissions;
    public Sprite CoinSprite;
    public Sprite CoinButton;
    public Sprite DiamondSprite;
    public Sprite DiamondButton;
    private List<int> m_CompletedMissions = new List<int>();
    public List<Mission> m_ActiveMissions = new List<Mission>();
    private readonly List<GameObject> m_ActiveMissionPanels = new List<GameObject>();
    private readonly List<GameObject> m_completedMissionPanels = new List<GameObject>();
    private List<Mission> m_ToDisplayMissions;
    public GameObject MissionPanel;
    public GameObject CompletedMissionPanel;


    public static MissionManager Instance;

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
        m_CompletedMissions = PlayerData.PlayerProfile.CompletedMissions;

        // List<Mission> activeMissions = GetActiveMissions();
        //  DisplayActiveMissions();
        //   Debug.Log(m_ActiveMissions.Count);

        //add listeners
        //  EventManager.StartListening();
        GetActiveMissions();
        //      DisplayActiveMissions();
        foreach (var t in m_ActiveMissions)
        {
            var t1 = t;
            EventManager.StartListening(t.MissionTitle, () => OnMissionComplete(t1));
        }

    }

    private void OnDisable()
    {
        foreach (var t in m_ActiveMissions)
        {
            var t1 = t;
            EventManager.StopListening(t.MissionTitle, () => OnMissionComplete(t1));
        }
    }

    private void OnMissionComplete(Mission mission)
    {
        m_CompletedMissions = PlayerData.PlayerProfile.CompletedMissions;

        if (!m_CompletedMissions.Contains(mission.MissionId))
            m_CompletedMissions.Add(mission.MissionId);
        //add completed mission to player profile
        PlayerData.PlayerProfile.CompletedMissions = m_CompletedMissions;
        //add reward based on mission type
        AwardRewardToPlayer(mission);

        //display notification
        //  NotifyPlayerWithMissionCompletion(mission);
        //update xp of player
        if (PlayerData.PlayerProfile.CurrentLevel <= DataManager.Instance.MaxLevels)
        {
            AddRewardXp(mission);

        }

        //updates UI
        UiManager.Instance.UpdateUi();
        //save to database
  //      PlayerData.SavePlayerData();
        EventManager.StopListening(mission.MissionTitle, () => OnMissionComplete(mission));
        //show notification to player
        if (ShowNotificationOnComplete)
        {
            NotifyPlayerWithMissionCompletion(mission);

        }
        GetActiveMissions();

        foreach (var t in m_ActiveMissions)
        {
            var t1 = t;
            EventManager.StartListening(t.MissionTitle, () => OnMissionComplete(t1));
        }
        //track appsflyer rich event
        Dictionary<string, string> missionCompleteEvent = new Dictionary<string, string>();
        missionCompleteEvent.Add(AFInAppEvents.CONTENT_TYPE, "missions");
        missionCompleteEvent.Add(AFInAppEvents.CONTENT_ID, mission.MissionId.ToString());
        missionCompleteEvent.Add(AFInAppEvents.CONTENT_TITLE, mission.MissionTitle);
        missionCompleteEvent.Add("af_mission_type", Enum.GetName(typeof(MissionType), mission.MissionType));
           
        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.MISSION_COMPLETED, missionCompleteEvent);

        //log firebase event
        Parameter unlockAchievementParameters = new Parameter(FirebaseAnalytics.ParameterAchievementId, mission.MissionTitle);
        
        FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventUnlockAchievement, unlockAchievementParameters);
    }

    public void SkipMission(Mission mission)
    {
        m_CompletedMissions = PlayerData.PlayerProfile.CompletedMissions;
        var mCanvas = GameObject.Find("Canvas");
        //load text components
        //Text coins=GameObject.FindGameObjectWithTag("Coins").GetComponent<Text>();
        //Text diamonds=GameObject.FindGameObjectWithTag("Diamonds").GetComponent<Text>();
        var transactionImage = Util.FindGameObjectWithName(mCanvas, "TransactionFailedImage");
        switch (mission.SkipType)
        {
            case LockType.Coins:
                var nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;

                if (nofCoinsAvaliable < mission.SkippableAmount)
                {
                    transactionImage.SetActive(true);
                    var texts = transactionImage.GetComponentsInChildren<Text>();
                    texts[0].text = "Requires " + (mission.SkippableAmount - nofCoinsAvaliable) + " more coins to skip the mission";
                    //message
                    texts[1].text = "Not enough coins"; //message
                }
                else
                {
                    nofCoinsAvaliable -= (int) mission.SkippableAmount;
                    if (!m_CompletedMissions.Contains(mission.MissionId))
                        m_CompletedMissions.Add(mission.MissionId);
                    PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;
                    PlayerData.PlayerProfile.CompletedMissions = m_CompletedMissions;
                    //add reward based on mission type
                    AwardRewardToPlayer(mission);

                    //display notification
                    //  NotifyPlayerWithMissionCompletion(mission);
                    //update xp of player
                    if (PlayerData.PlayerProfile.CurrentLevel <= DataManager.Instance.MaxLevels)
                    {
                        AddRewardXp(mission);

                    }
                }
                break;

            case LockType.Diamonds:
                var noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;

                if (noofDiamondsAvailable < mission.SkippableAmount)
                {
                    //open dialog to buy
                    transactionImage.SetActive(true);
                    var texts = transactionImage.GetComponentsInChildren<Text>();
                    texts[0].text = "Requires " + (mission.SkippableAmount - noofDiamondsAvailable) + " more diamonds to skip the mission";
                    //message
                    texts[1].text = "Not enough diamonds"; //message
                }
                else
                {
                    noofDiamondsAvailable -= (int) mission.SkippableAmount;
                    if (!m_CompletedMissions.Contains(mission.MissionId))
                        m_CompletedMissions.Add(mission.MissionId);
                    PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;

                    //add reward based on mission type
                    AwardRewardToPlayer(mission);

                    //display notification
                    //   NotifyPlayerWithMissionCompletion(mission);
                    //update xp of player

                    if (PlayerData.PlayerProfile.CurrentLevel <= DataManager.Instance.MaxLevels)
                    {
                        AddRewardXp(mission);

                    }

                    //show notification to player
                    if (ShowNotificationOnComplete)
                    {
                        NotifyPlayerWithMissionCompletion(mission);

                    }
                }

                break;
        }
        //save to database
        PlayerData.SavePlayerData();
        //
        UiManager.Instance.UpdateUi();
        //display new active missionns
        DisplayActiveMissions();
        //
    }

    private void AwardRewardToPlayer(Mission mission)
    {
        switch (mission.RewardType)
        {
            case Rewardtype.Unlockoutfit:
                var unlockedOutfitsPerPlayerDictionary = PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer;
                var selectedCharacter = PlayerData.PlayerProfile.CurrentSelectedPlayer;

                if (selectedCharacter == 0)
                    selectedCharacter = 1;
                var selectedPlayer = DataManager.Instance.Players[selectedCharacter - 1];
                if (!unlockedOutfitsPerPlayerDictionary.ContainsKey(selectedPlayer.PlayerID))
                    unlockedOutfitsPerPlayerDictionary.Add(selectedPlayer.PlayerID, new List<int>());
                var unlockedOutfitsList = unlockedOutfitsPerPlayerDictionary[selectedPlayer.PlayerID];
                if (!unlockedOutfitsList.Contains(mission.RewardAmountorObjectId))
                {
                    unlockedOutfitsList.Add(mission.RewardAmountorObjectId);
                    //indicate user 
                }
                PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary;
                break;
            case Rewardtype.UnlockWeapon:

                var unlockedWeaponsList = PlayerData.PlayerProfile.UnlockedWeaponsList;

                if (!unlockedWeaponsList.Contains(mission.RewardAmountorObjectId))
                {
                    unlockedWeaponsList.Add(mission.RewardAmountorObjectId);
                    Debug.Log("unlocked weapon" + mission.RewardAmountorObjectId);
                    //indicate user 
                }
                //add unlocked weapons to playerdata
                PlayerData.PlayerProfile.UnlockedWeaponsList = unlockedWeaponsList;
                break;
            case Rewardtype.Coins:
                PlayerData.PlayerProfile.NoofCoinsAvailable += mission.RewardAmountorObjectId;
                //later
                break;
            case Rewardtype.Diamonds:
                //later

                PlayerData.PlayerProfile.NoofDiamondsAvailable += mission.RewardAmountorObjectId;
                break;
                case Rewardtype.BluePrintReceived:
                break;
        }
    }

    private void AddRewardXp(Mission mission)
    {
        PlayerData.PlayerProfile.PlayerXp += mission.XpEarned;
        PlayerData.CurrentGameStats.CurrentXpEarned += mission.XpEarned;
        PlayerData.PlayerProfile.CurrentLevelXp += mission.XpEarned;
        while (PlayerData.PlayerProfile.CurrentLevelXp >=
               DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel).XpRequiredToReachNextLevel)
        {
            PlayerData.PlayerProfile.CurrentLevelXp -=
                DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel).XpRequiredToReachNextLevel;
            PlayerData.PlayerProfile.CurrentLevel++;
        }
        GameObject canvas = GameObject.Find("Canvas");
        {
            if (canvas != null)
            {
                GameObject levelUpdisplay = Util.FindGameObjectWithName(canvas, "LevelUpDisplay");
                if (levelUpdisplay != null)
                {
                    
                    LevelUpDisplay levelUpDisplay = levelUpdisplay.GetComponent<LevelUpDisplay>();
                    if (levelUpDisplay)
                    {
                        levelUpDisplay.DisplayLevelUp();
                    }
                }
              
            }
        }

    }

    private void NotifyPlayerWithMissionCompletion(Mission mission)
    {
        if (!NotifyData.IsNotificationAddedToQueue(mission))
        {
            NotifyData.AddNew(mission.MissionTitle + "\n" + "<color=#add8e6ff>" + "+ " + +mission.XpEarned + "XP".ToString() + "</color>", mission.MissionSprite);
        }
    }

    private void DestroyActiveMissions()
    {
        foreach (var t in m_ActiveMissionPanels)
            Destroy(t.gameObject);

        m_ActiveMissions.Clear();
    }

    private void DestroyCompletedMissionPanels()
    {
        foreach (var t in m_completedMissionPanels)
            Destroy(t.gameObject);
    }

    public List<Mission> GetActiveMissions()
    {
        m_ActiveMissions.Clear();
        var i = 0;

        foreach (var mission in DataManager.Instance.Missions)
        {
            if (i >= noOfActiveMissions)
                break;
            if (!m_CompletedMissions.Contains(mission.MissionId))
            {
                if (mission.MissionType != MissionType.None)
                {
                    i++;
                    m_ActiveMissions.Add(mission);
                }
            }
        }
        return m_ActiveMissions;
    }

    //public void DisplayActiveMissionsType2()
    //{
    //    DestroyActiveMissions();

    //    var i = 0;
    //    foreach (var mission in DataManager.Instance.Missions)
    //    {
    //        if (i >= 3)
    //            break;
    //        if (m_CompletedMissions.Contains(mission.MissionId))
    //            continue;

    //        if (MissionPanel)
    //        {
    //            var temp = Instantiate(MissionPanel);
    //            //  Mission mission = DataManager.Instance.GetMission(lastCompletedMissionId + i);
    //            temp.transform.SetParent(ActiveMissionsPanelParent);
    //            temp.transform.localPosition = new Vector3(temp.transform.localPosition.z, temp.transform.localPosition.y, 0);
    //            temp.transform.localScale = Vector3.one;
    //            // temp.transform.GetChild(0).GetComponent<Image>().sprite = mission.MissionSprite;
    //            var images = temp.GetComponentsInChildren<Image>(true);

    //            //images[1].sprite = Resources.Load<Sprite>(mission.MissionSpritePath);
    //            //images[2].sprite = Resources.Load<Sprite>(mission.RewardSpritePath);
    //            images[1].sprite = mission.MissionSprite;
    //            images[3].sprite = (mission.RewardSprite);
    //            var texts = temp.GetComponentsInChildren<Text>();

    //            texts[0].text = mission.MissionTitle;
    //            texts[1].text = mission.MisionDescription;

    //            if (mission.RewardType==Rewardtype.None|| mission.RewardType == Rewardtype.Coins || mission.RewardType == Rewardtype.Diamonds)
    //            {
    //                if (mission.RewardAmountorObjectId > 0)
    //                {
    //              //      Debug.Log("misiion rewaud amount" + mission.RewardAmountorObjectId);

    //                    texts[3].text = "X " + mission.RewardAmountorObjectId;
    //                    images[3].gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    Debug.Log("misiion rewaud amount" + mission.RewardAmountorObjectId);
    //                    texts[3].text = string.Empty;
    //                    images[3].gameObject.SetActive(false);
    //                }
    //            }                    
    //            else
    //                texts[3].text = string.Empty;
    //            var skipbutton = temp.GetComponentInChildren<Button>();
    //            if (mission.IsSkippable)
    //            {
    //                skipbutton.gameObject.SetActive(true);
    //                if (mission.SkipType == LockType.Coins)
    //                {
    //                    var buttonImages = skipbutton.GetComponentsInChildren<Image>();
    //                    buttonImages[0].sprite = CoinButton;
    //                    buttonImages[1].sprite = CoinSprite;
    //                }
    //                else
    //                {
    //                    var buttonImages = skipbutton.GetComponentsInChildren<Image>();
    //                    buttonImages[0].sprite = DiamondButton;
    //                    buttonImages[1].sprite = DiamondSprite;
    //                }
    //                var buttonTexts = skipbutton.GetComponentsInChildren<Text>();
    //                buttonTexts[1].text = mission.SkippableAmount.ToString();
    //            }
    //            //temp.transform.GetChild(1).GetComponent<Text>().text = mission.MissionTitle;
    //            //temp.transform.GetChild(2).GetComponent<Text>().text = mission.MisionDescription;

    //            skipbutton.onClick.AddListener(() => SkipMission(mission));

    //            m_ActiveMissionPanels.Add(temp);
    //        }

    //        i++;
    //        m_ActiveMissions.Add(mission);
    //    }
    //}

    public void DisplayActiveMissions()
    {
        DestroyActiveMissions();
     //   DestroyCompletedMissions();
        var activeMissions = GetActiveMissions();
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        var objects = canvas.GetComponentsInChildren<VerticalLayoutGroup>(true);
        if (activeMissions.Count == 0)
            objects[0].gameObject.GetComponentInChildren<Text>(true).gameObject.SetActive(true);
        else
            objects[0].gameObject.GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        foreach (var mission in activeMissions)

            if (MissionPanel)
            {

                var temp = Instantiate(MissionPanel);
                //  Mission mission = DataManager.Instance.GetMission(lastCompletedMissionId + i);

                temp.transform.SetParent(objects[0].gameObject.transform);
                temp.transform.localPosition = new Vector3(temp.transform.localPosition.z, temp.transform.localPosition.y, 0);
                temp.transform.localScale=Vector3.one;
                // temp.transform.GetChild(0).GetComponent<Image>().sprite = mission.MissionSprite;
                var images = temp.GetComponentsInChildren<Image>(true);
                //Debug.Log(mission.MissionSpritePath);
                //Debug.Log(mission.RewardSpritePath);

                //images[1].sprite = Resources.Load<Sprite>(mission.MissionSpritePath);
                //images[2].sprite = Resources.Load<Sprite>(mission.RewardSpritePath);
                images[1].sprite = mission.MissionSprite;
                images[3].sprite = mission.RewardSprite;
                var texts = temp.GetComponentsInChildren<Text>();

                texts[0].text = mission.MissionTitle;
                texts[1].text = mission.MisionDescription;
                texts[3].text = "X " + mission.XpEarned;

                if (mission.RewardType == Rewardtype.None || mission.RewardType == Rewardtype.Coins ||
                    mission.RewardType == Rewardtype.Diamonds)
                {
                    if (mission.RewardAmountorObjectId > 0)
                    {
                        //      Debug.Log("misiion rewaud amount" + mission.RewardAmountorObjectId);

                        texts[4].text = "X " + mission.RewardAmountorObjectId;
                        images[3].gameObject.SetActive(true);
                    }
                    else
                    {

                        texts[4].gameObject.SetActive(false);
                        images[3].gameObject.SetActive(false);
                    }
                }
                else
                {
                    texts[3].gameObject.SetActive(false);
                        
                }
                GameObject skipDetails = Util.FindGameObjectWithName(temp, "SkipDetails");
                var skipbutton = temp.GetComponentInChildren<Button>();
                if (mission.IsSkippable)
                {
                    skipbutton.gameObject.SetActive(true);
                    if (mission.SkipType == LockType.Coins)
                    {
                        var buttonImages = skipbutton.GetComponentsInChildren<Image>();
                        buttonImages[0].sprite = CoinButton;
                        buttonImages[1].sprite = CoinSprite;
                    }
                    else
                    {
                        var buttonImages = skipbutton.GetComponentsInChildren<Image>();
                        buttonImages[0].sprite = DiamondButton;
                        buttonImages[1].sprite = DiamondSprite;
                    }
                    Text skipamount = skipbutton.GetComponentInChildren<Text>();
                    skipamount.text = mission.SkippableAmount.ToString();
                }
                else
                {
                    foreach (var go in skipDetails.GetComponentsInChildren<RectTransform>())
                    {
                        go.gameObject.SetActive(false);
                    }
                }
                //temp.transform.GetChild(1).GetComponent<Text>().text = mission.MissionTitle;
                //temp.transform.GetChild(2).GetComponent<Text>().text = mission.MisionDescription;

                skipbutton.onClick.AddListener(() => SkipMission(mission));
                skipbutton.onClick.AddListener(()=>PlayAudio(skipbutton.GetComponent<AudioSource>()));
                m_ActiveMissionPanels.Add(temp);
            }
    }

    public void PlayAudio(AudioSource audioSource)
    {
        AudioManager.Instance.PlaySound(audioSource);
    }
    public void DisplayCompletedMissions()
    {
        DestroyCompletedMissionPanels();
        //  DestroyActiveMissions();
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject completedMissionsScrollView = Util.FindGameObjectWithName(canvas, "CompletedMissionsPanel");
        VerticalLayoutGroup contentHolder = completedMissionsScrollView.GetComponentInChildren<VerticalLayoutGroup>(true);
        Text noCompletedMissionsText = completedMissionsScrollView.GetComponentInChildren<Text>(true);
        m_CompletedMissions = PlayerData.PlayerProfile.CompletedMissions;

        if (m_CompletedMissions.Count > 0)
        {
            noCompletedMissionsText.gameObject.SetActive(false);
        }
        else
        {
            noCompletedMissionsText.gameObject.SetActive(true);

        }
        foreach (var mission in DataManager.Instance.Missions)
        {
            if (m_CompletedMissions.Contains(mission.MissionId))
            {
                if (CompletedMissionPanel)
                {
                    GameObject temp = Instantiate(CompletedMissionPanel) as GameObject;
                    temp.transform.SetParent(contentHolder.transform);
                    temp.transform.localPosition = new Vector3(temp.transform.localPosition.z, temp.transform.localPosition.y, 0);
                    temp.transform.localScale = Vector3.one;
                    // temp.transform.GetChild(0).GetComponent<Image>().sprite = mission.MissionSprite;
                    var images = temp.GetComponentsInChildren<Image>(true);

                    //images[1].sprite = Resources.Load<Sprite>(mission.MissionSpritePath);
                    //images[2].sprite = Resources.Load<Sprite>(mission.RewardSpritePath);
                    images[1].sprite = mission.MissionSprite;
                    images[3].sprite = mission.RewardSprite;
                    var texts = temp.GetComponentsInChildren<Text>();

                    texts[0].text = mission.MissionTitle;
                    texts[1].text = mission.MisionDescription;
                    texts[3].text = "X " + mission.XpEarned;

                    if (mission.RewardType == Rewardtype.None || mission.RewardType == Rewardtype.Coins ||
                        mission.RewardType == Rewardtype.Diamonds)
                    {
                        if (mission.RewardAmountorObjectId > 0)
                        {
                            texts[4].text = "X " + mission.RewardAmountorObjectId;
                            images[3].gameObject.SetActive(true);
                        }
                        else
                        {

                            texts[4].gameObject.SetActive(false);
                            images[3].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        texts[3].gameObject.SetActive(false);

                    }
                    m_completedMissionPanels.Add(temp);
                }
            }

        }

    }

}

[Serializable]
public class Mission
{
    public int MissionId;
    public string MissionTitle;
    public string MisionDescription;
    public bool IsMissionForOneRun;
    public MissionType MissionType;
    //only needed if missiontype is unlcok outfit or skin
    public int PlayerId;
    //only needed if mission type is unlock skin
    public int SkinId;
    //only needed if mission type is upgrade outfit or weapon
    public int NoOfUpgrades;
    public int AmountOrObjectIdToComplete;
    //only needed if mission type is collect prints
    public int NoOfBluePrintsCollected;
    public Rewardtype RewardType;
    public int RewardAmountorObjectId;
    public bool IsSkippable;
    public LockType SkipType;
    public float SkippableAmount;
    public int XpEarned;
    public Sprite MissionSprite;
    public Sprite RewardSprite;
    //   public Sprite MissionSprite;
    public override bool Equals(object obj)
    {
        return obj is Mission && Equals(MissionId, ((Mission) obj).MissionId);
    }

    public override int GetHashCode()
    {
        return MissionId.GetHashCode();
    }
}

public enum MissionType
{
    None,
    UnlockWeapon,
    UnlockOutfit,
    UnlockSkin,
    UpgradeOutfit,
    UpgradeWeapon,
    CollectBluePrints,
    KillEnemies,
    DodgeObstacle,
    DestroyObstacle,
    Die,
    TravelDistance,
    ExplodeVehicles,
    DestroyDrones,
    HackEnemyDrones,
    CollectTokens,

}

public enum Rewardtype
{
    None,
    UnlockWeapon,
    Unlockoutfit,
    BluePrintReceived,
    Coins,
    Diamonds,
    Xp,
    XpDoubler,
    UnlockCharacter,
    AllWeapons,
    AllCharacters
}