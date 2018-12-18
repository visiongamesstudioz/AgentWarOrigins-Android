using System.Collections.Generic;
using System.IO;
using EndlessRunner;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Outfit : MonoBehaviour
{
    public int OutfitID;
    public bool IsSkin;
    public int SkinID;
    public string OutfitName;
    public string OutfitDesc;
    public int LevelToUnlock;
    public Sprite OutfitImage;
    public Sprite CoinSprite;
    public Sprite DiamondSprite;
    public Sprite CoinButton;
    public Sprite DiamondButton;
    public Sprite DefaultButton;
    public int Cost;

    public LockType LockType;
    public GameObject OutfitGo;
    public OutfitAbility OutfitAbility;

    public int NoofBluePrints;
    public int NoofUpdrages;

    public List<LockType> UpgradeLockTypes; //should be ewal to no of upgrades 

    public List<int> UpgradeCosts; //should be equal to no of upgrades
    public float DamageMultiplier;
    public int DamageDecreasePercentage;
    public int NoofReviveTimes;
    public float ReviveEnergyAmount;
    public float ReviveEnergyIncreasePercentage;
    public float RegenerateInitialWaitTime;
    public float RegenerateInitialDecreaseTimePercentage;
    public float RegenerateAmountOverTime;

    public float RegenerateAmountIncreasePercentage;
    public float InvincibleTime;
    public float InvincibleIncreaseTimerPercentage;
    [HideInInspector]
    public GameObject BuyOutfitGameObject;
    [HideInInspector]
    public Button WearOutfitButton;
    [HideInInspector]
    public GameObject UpgradeOutfitGameObject;
    [HideInInspector]
    public Text UpgradeNote;
    [HideInInspector]
    public Text OutfitDescText;
    [HideInInspector]
    public Text OutfitNameText;
    [HideInInspector]
    public GameObject BuyPanel;

    private Text m_BuyOutfitOrSkinText;
    private Text m_WearOutfitText;
    private GameObject m_SourceBody;

    private GameObject m_Outfit;
    private List<GameObject> m_Extras=new List<GameObject>();
    private Stitcher m_Sticher;
    private bool isBought;
    private bool m_IsOutfitInstantiated;
    private Player m_DisplayedPlayer;
    private int m_CurrentDisplayedOutfitId;
    private GameObject m_Canvas;
    private Button m_BuyOutfitButton;
    private MissionManager m_MissionManager;
    private readonly List<Mission> unlockOutfitMissions = new List<Mission>();
    private readonly List<Mission> unlockSkinMissions = new List<Mission>();
    private void Awake()

    {
        m_Canvas = GameObject.FindGameObjectWithTag("Canvas");

    }

    private void Start()
    {
    }


    public void Initialize(GameObject outfit, Player sourcePlayer)
    {
        m_Extras.Clear();
        m_Sticher = new Stitcher();
        m_DisplayedPlayer = sourcePlayer;
        m_SourceBody = sourcePlayer.gameObject;
        if (!m_IsOutfitInstantiated)
        {
            OutfitGo = Instantiate(outfit);
            OutfitGo.SetActive(false);
            m_IsOutfitInstantiated = true;
        }
        else
        {
            OutfitGo = outfit;
        }
        var skinnedMesh = OutfitGo.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var child in skinnedMesh)
        {
            if (child.gameObject.tag == "Extras")
            {
                m_Extras.Add(child.gameObject);
            }
            else if (child.gameObject.tag.Equals("Outfit"))
                m_Outfit = child.gameObject;
        }
            
    }

    public void ChangeClothes()
    {
        m_MissionManager = MissionManager.Instance;
        RemovePreviousClothes();
        m_CurrentDisplayedOutfitId = OutfitID;

        //AddClothes(m_Tops);
        //AddClothes(m_Bottoms);
        //AddClothes(m_Shoes);
        //AddClothes(m_Bandana);
        //AddClothes(m_BackPack);
        //AddClothes(m_ArmorShirt);
        //AddClothes(m_ArmorPacks);
        AddClothes(m_Outfit);
        //add all extra clothing 
        for (int i = 0; i < m_Extras.Count; i++)
        {
            AddClothes(m_Extras[i]);
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
            return;

        m_Canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (m_Canvas == null)
            return;
        BuyPanel = Util.FindGameObjectWithName(m_Canvas, "BuyPanel");
        BuyOutfitGameObject = Util.FindGameObjectWithName(m_Canvas, "BuyOutfit");
        m_BuyOutfitOrSkinText = BuyOutfitGameObject.GetComponentInChildren<Text>();
        WearOutfitButton = Util.FindGameObjectWithTag(m_Canvas, "WearOutfit").GetComponent<Button>();
        m_WearOutfitText = WearOutfitButton.GetComponentInChildren<Text>();

        OutfitNameText = Util.FindGameObjectWithTag(m_Canvas, "OutfitName").GetComponent<Text>();
        OutfitDescText = Util.FindGameObjectWithTag(m_Canvas, "OutfitDesc").GetComponent<Text>();
        UpgradeNote = Util.FindGameObjectWithName(m_Canvas, "UpgradeNote").GetComponent<Text>();
        UpgradeOutfitGameObject = Util.FindGameObjectWithName(m_Canvas, "UpgradePanel");

        if (BuyOutfitGameObject == null)
            return;
        if (WearOutfitButton == null)
            return;

        m_BuyOutfitButton = BuyOutfitGameObject.GetComponentInChildren<Button>();
        m_BuyOutfitButton.onClick.RemoveAllListeners();
        m_BuyOutfitButton.onClick.AddListener(BuyOutfit);
        WearOutfitButton.onClick.RemoveAllListeners();
        WearOutfitButton.onClick.AddListener(WearOutfitOrSkin);
        UpgradeOutfitGameObject.GetComponentInChildren<Button>(true).onClick.RemoveAllListeners();
        UpgradeOutfitGameObject.GetComponentInChildren<Button>(true).onClick.AddListener(UpgradeOutfit);
        ShowUpgradeNote();
        Player currentDisplayedPlayer = GetCurrentPlayerDisplayed();
        List<int> unlockedPlayers = PlayerData.PlayerProfile.UnlockedPlayerList;
        OutfitDescText.text = OutfitDesc;
        OutfitNameText.text = OutfitName;
        if (IsSkin)
        {
           
            if (m_WearOutfitText)
            {
                m_WearOutfitText.text = "Wear Skin";
            }
            if (m_BuyOutfitOrSkinText)
            {
                m_BuyOutfitOrSkinText.text = "Buy Skin";
            }
            var unlockedOutfitsPerPlayerDictionary = PlayerData.PlayerProfile.UnlockedSkinsPerOutfit;
            if (unlockedOutfitsPerPlayerDictionary == null)
                return;
            if (!unlockedOutfitsPerPlayerDictionary.ContainsKey(currentDisplayedPlayer.PlayerID))
                unlockedOutfitsPerPlayerDictionary.Add(currentDisplayedPlayer.PlayerID, new Dictionary<int, List<int>>());
            Dictionary<int,List<int>> unlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary[currentDisplayedPlayer.PlayerID];

            if (!unlockedOutfitsPerPlayer.ContainsKey(m_CurrentDisplayedOutfitId))
            {
                unlockedOutfitsPerPlayer.Add(m_CurrentDisplayedOutfitId,new List<int>());
            }
            else if(unlockedPlayers.Contains(currentDisplayedPlayer.PlayerID))
            {
                //show upgrade panel if locked or not
                if (NoofUpdrages > 0)
                {
                    //    UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                    ShowUpgradeOutfitPanel();

                }
                else
                {
                    //     UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                    UpgradeOutfitGameObject.SetActive(false);

                }
            }
            else
            {
                UpgradeOutfitGameObject.SetActive(false);

            }
            List<int> unlockedSkinsPerOutfit = unlockedOutfitsPerPlayer[m_CurrentDisplayedOutfitId];

            if (!unlockedSkinsPerOutfit.Contains(SkinID))
            {
                BuyOutfitGameObject.gameObject.SetActive(true);
             //   UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 0;
                BuyPanel.SetActive(true);
                WearOutfitButton.gameObject.SetActive(false);
          
                UpgradeOutfitGameObject.SetActive(false);
                m_BuyOutfitButton.GetComponentInChildren<Text>().text = Cost.ToString();
                var images = m_BuyOutfitButton.GetComponentsInChildren<Image>();
                switch (LockType)
                {
                    case LockType.Coins:
                        images[0].sprite = CoinButton;
                        images[1].sprite = CoinSprite;
                        break;
                    case LockType.Diamonds:
                        images[0].sprite = DiamondButton;
                        images[1].sprite = DiamondSprite;
                        break;
                }
            }
            else
            {
                BuyPanel.SetActive(true);
                WearOutfitButton.gameObject.SetActive(true);
                BuyOutfitGameObject.gameObject.SetActive(false);
     
  
            }
        
        }
        else
        {

            if (m_WearOutfitText)
            {
                m_WearOutfitText.text = "Wear Outfit";
            }
            if (m_BuyOutfitOrSkinText)
            {
                m_BuyOutfitOrSkinText.text = "Buy Outfit";
            }
            var unlockedOutfitsPerPlayerDictionary = PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer;
            if (unlockedOutfitsPerPlayerDictionary == null)
                return;
            if (!unlockedOutfitsPerPlayerDictionary.ContainsKey(currentDisplayedPlayer.PlayerID))
                unlockedOutfitsPerPlayerDictionary.Add(currentDisplayedPlayer.PlayerID, new List<int>());
            List<int> unlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary[currentDisplayedPlayer.PlayerID];
            //show outfit desc and name

            //outfit not bought
            if (!unlockedOutfitsPerPlayer.Contains(m_CurrentDisplayedOutfitId))
            {

                BuyOutfitGameObject.gameObject.SetActive(true);
            //    UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 0;
                BuyPanel.SetActive(true);
                WearOutfitButton.gameObject.SetActive(false);
                UpgradeOutfitGameObject.SetActive(false);
                m_BuyOutfitButton.GetComponentInChildren<Text>().text = Cost.ToString();
                var images = m_BuyOutfitButton.GetComponentsInChildren<Image>();
                switch (LockType)
                {
                    case LockType.Coins:
                        images[0].sprite = CoinButton;
                        images[1].sprite = CoinSprite;
                        break;
                    case LockType.Diamonds:
                        images[0].sprite = DiamondButton;
                        images[1].sprite = DiamondSprite;
                        break;
                    case LockType.Blueprints:
                        BuyPanel.SetActive(false);
                        int noOfBluePrintsUnlocked = GetNumberOfBluePrintsUnlockedPerOutfit(currentDisplayedPlayer.PlayerID);

                        UpgradeOutfitGameObject.GetComponent<Text>().text = "Blue Prints(" + (noOfBluePrintsUnlocked) + " OF "
     + NoofBluePrints + ")";
                        var upgradeSlider = UpgradeOutfitGameObject.GetComponentInChildren<Slider>(true);
                        var upgradeButton = UpgradeOutfitGameObject.GetComponentInChildren<Button>(true);
                        Image[] buySpriteImages = upgradeButton.GetComponentsInChildren<Image>(true);
                        buySpriteImages[0].sprite = DefaultButton;
                        buySpriteImages[1].gameObject.SetActive(false);
                        //for testing need to add through boxes
                        //addallblueprints(player.PlayerID);
                        // noOfBluePrintsUnlocked = GetNumberOfBluePrintsUnlockedPerOutfit(player.PlayerID);

                        if (noOfBluePrintsUnlocked < NoofBluePrints)
                        {
                            BuyOutfitGameObject.gameObject.SetActive(false);
                            WearOutfitButton.gameObject.SetActive(false);
                            UpgradeOutfitGameObject.SetActive(true);
                            upgradeButton.GetComponentInChildren<Text>().text = "Assemble";
                            upgradeButton.interactable = false;
                            upgradeSlider.value = (float)noOfBluePrintsUnlocked / NoofBluePrints;
                            noOfBluePrintsUnlocked = NoofUpdrages;

                        }
                        else
                        {
                            upgradeSlider.value = (float)noOfBluePrintsUnlocked / NoofBluePrints;
                            UpgradeOutfitGameObject.SetActive(true);
                            upgradeButton.GetComponentInChildren<Text>().text = "Assemble";
                            upgradeButton.interactable = true;
                            upgradeButton.onClick.RemoveAllListeners();
                            upgradeButton.onClick.AddListener(() => Assemble(currentDisplayedPlayer.PlayerID));
                        }
                        break;
                }
            }
            else
            {
                BuyPanel.SetActive(true);
                WearOutfitButton.gameObject.SetActive(true);
                BuyOutfitGameObject.gameObject.SetActive(false);

                if (NoofUpdrages > 0 && unlockedPlayers.Contains(currentDisplayedPlayer.PlayerID))
                {
               //     UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                    ShowUpgradeOutfitPanel();

                }
                else
                {
                //    UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                    UpgradeOutfitGameObject.SetActive(false);

                }
           
            }

            if (OutfitGo)
                OutfitGo.SetActive(false);
        }


        ////check if already worn the outfit

        var wornOutfit =
            PlayerData.PlayerProfile.WornOutfitPerPlayer;
        if (!wornOutfit.ContainsKey(currentDisplayedPlayer.PlayerID))
            wornOutfit.Add(currentDisplayedPlayer.PlayerID, 1);
        var selectedOufitId = wornOutfit[currentDisplayedPlayer.PlayerID];

        if (selectedOufitId == 0)
            return;

        var selectedOutfitSkinPerPlayerDictionary = PlayerData.PlayerProfile.WornSkinPerOutfitPerPlayer;
        if (!selectedOutfitSkinPerPlayerDictionary.ContainsKey(currentDisplayedPlayer.PlayerID))
            selectedOutfitSkinPerPlayerDictionary.Add(currentDisplayedPlayer.PlayerID, new Dictionary<int, int>());

        var slectedOutfitPerPlayer = selectedOutfitSkinPerPlayerDictionary[currentDisplayedPlayer.PlayerID];
        if (!slectedOutfitPerPlayer.ContainsKey(m_CurrentDisplayedOutfitId))
            slectedOutfitPerPlayer.Add(m_CurrentDisplayedOutfitId, new int());
        int selectedSkinPerOutfit = slectedOutfitPerPlayer[m_CurrentDisplayedOutfitId];
        int selectedPlayer = PlayerData.PlayerProfile.CurrentSelectedPlayer;
        if (selectedPlayer == currentDisplayedPlayer.PlayerID && selectedOufitId == OutfitID && selectedSkinPerOutfit == SkinID)
        {
          //  UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 0;
            BuyPanel.SetActive(true);
            WearOutfitButton.gameObject.SetActive(true);
            WearOutfitButton.GetComponentInChildren<Text>().text = "Equipped";
            BuyOutfitGameObject.gameObject.SetActive(false);
        }
        else
        {
            //OutfitCostButton.gameObject.SetActive(false);
         //   UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
            BuyPanel.SetActive(true);
            WearOutfitButton.gameObject.SetActive(true);
        }
    }

    private void ShowUpgradeNote()
    {
        switch (OutfitAbility)
        {
            case OutfitAbility.None:
                
                break;
            case OutfitAbility.LowDamage:
                
                UpgradeNote.text = "* Each Upgrade decreased the damage taken by " + DamageDecreasePercentage + " % ";
                break;
            case OutfitAbility.Invincible:
                UpgradeNote.text = "* Each Upgrade increases the invincible time by " + InvincibleIncreaseTimerPercentage + " % ";
                break;
            case OutfitAbility.RegenerateHealth:
                UpgradeNote.text = "* Each Upgrade regenerates health faster by " + RegenerateInitialDecreaseTimePercentage + " % ";
                break;
            case OutfitAbility.Revive:
                UpgradeNote.text = "* Each Upgrade increases revived health by " + ReviveEnergyIncreasePercentage + " % ";
                break;
        }

        if(NoofUpdrages > 0)
        {
            UpgradeNote.gameObject.SetActive(true);
        }
        else
        {
            UpgradeNote.gameObject.SetActive(false);
        }
    }

    //assemble current displayed outfit for player
    public void Assemble(int playerId)
    {
        m_Canvas = GameObject.Find("Canvas");

        //assemle all Parts
        Debug.Log("assembling all parts");
        var unlockedOutfitsPerPlayerDictionary = PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer;
        if (unlockedOutfitsPerPlayerDictionary == null)
            return;
        if (!unlockedOutfitsPerPlayerDictionary.ContainsKey(playerId))
            unlockedOutfitsPerPlayerDictionary.Add(playerId, new List<int>());
        // var unlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary[player.PlayerID];
        //outfit not bought
        List<int> unlockedOutfitsPerPlayer;
        if (unlockedOutfitsPerPlayerDictionary.TryGetValue(playerId, out unlockedOutfitsPerPlayer))
        {
            if (!unlockedOutfitsPerPlayer.Contains(m_CurrentDisplayedOutfitId))
            {
                unlockedOutfitsPerPlayer.Add(m_CurrentDisplayedOutfitId);
                BuyPanel.SetActive(true);
                BuyOutfitGameObject.gameObject.SetActive(false);
                WearOutfitButton.gameObject.SetActive(true);
                //assign back upgrade logic to the button
                UpgradeOutfitGameObject.GetComponentInChildren<Button>(true).onClick.RemoveAllListeners();
                UpgradeOutfitGameObject.GetComponentInChildren<Button>(true).onClick.AddListener(UpgradeOutfit);
                if (NoofUpdrages > 0)
                {
                 //   UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                    ShowUpgradeOutfitPanel();

                }
                else
                {
                //    UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                    UpgradeOutfitGameObject.SetActive(false);

                }
                PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary;
            }
            //display 
            GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject, "DisplayAssembleOrUpgradeOutfit");
            display.GetComponentInChildren<Text>().text = "Assembled Outfit";
            display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
            ParticleSystem particles = display.GetComponentInChildren<ParticleSystem>(true);
            //instantiate particles

            display.SetActive(true);
        }

        PlayerData.SavePlayerData();

    }
    public void BuyOutfit()
    {
        m_Canvas = GameObject.Find("Canvas");

        unlockOutfitMissions.Clear();
        unlockSkinMissions.Clear();
        if (m_MissionManager != null)
            foreach (var mission in m_MissionManager.GetActiveMissions())
            {
                if (mission.MissionType == MissionType.UnlockOutfit)
                    unlockOutfitMissions.Add(mission);
                if (mission.MissionType == MissionType.UnlockSkin)
                    unlockSkinMissions.Add(mission);
            }
        //load text components
        //Text coins=GameObject.FindGameObjectWithTag("Coins").GetComponent<Text>();
        //Text diamonds=GameObject.FindGameObjectWithTag("Diamonds").GetComponent<Text>();
        var transactionImage = Util.FindGameObjectWithName(m_Canvas, "TransactionFailedImage");
        var player = GetCurrentPlayerDisplayed();
        if (!PlayerData.PlayerProfile.UnlockedPlayerList.Contains(player.PlayerID))
        {
            //prompt player to buy character first
            transactionImage.gameObject.SetActive(true);
            var texts = transactionImage.GetComponentsInChildren<Text>();
            texts[0].text = "Should Unlock The Agent first"; //message
            texts[1].text = "Unlock Agent to buy the outfit"; //message
            return;
        }

        var currentLevel = PlayerData.PlayerProfile.CurrentLevel;
        if (currentLevel < LevelToUnlock)
        {
            transactionImage.gameObject.SetActive(true);
            var texts = transactionImage.GetComponentsInChildren<Text>();
            texts[0].text = "Reach Level " + LevelToUnlock + " to buy outfit"; //message
            texts[1].text = "Level " + LevelToUnlock + " Required"; //message
            return;
        }

        var unlockedOutfitsPerPlayerDictionary = PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer;
        if (unlockedOutfitsPerPlayerDictionary == null)
            return;
        if (!unlockedOutfitsPerPlayerDictionary.ContainsKey(player.PlayerID))
            unlockedOutfitsPerPlayerDictionary.Add(player.PlayerID, new List<int>());
        // var unlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary[player.PlayerID];
        //outfit not bought
        List<int> unlockedOutfitsPerPlayer;


        if (unlockedOutfitsPerPlayerDictionary.TryGetValue(player.PlayerID, out unlockedOutfitsPerPlayer))

            if (IsSkin)
            {
                if (m_WearOutfitText)
                {
                    m_WearOutfitText.text = "Wear Skin";
                }
                if (m_BuyOutfitOrSkinText)
                {
                    m_BuyOutfitOrSkinText.text = "Buy Skin";
                }

                if (!unlockedOutfitsPerPlayer.Contains(m_CurrentDisplayedOutfitId))
                {
                    transactionImage.gameObject.SetActive(true);
                    var texts = transactionImage.GetComponentsInChildren<Text>();
                    texts[0].text = "Should buy Outfit first"; //message
                    texts[1].text = "Unlock outfit first"; //message
                    return;
                }
                else
                {
                    Dictionary<int,Dictionary<int,List<int>>> unlockedOutfitsSkinsPerPlayerDictionary = PlayerData.PlayerProfile.UnlockedSkinsPerOutfit;
                    if (unlockedOutfitsSkinsPerPlayerDictionary == null)
                        return;
                    if (!unlockedOutfitsSkinsPerPlayerDictionary.ContainsKey(player.PlayerID))
                        unlockedOutfitsSkinsPerPlayerDictionary.Add(player.PlayerID, new Dictionary<int, List<int>>());
                    Dictionary<int, List<int>> unlockedOutfitsSkinPerPlayer =
                        unlockedOutfitsSkinsPerPlayerDictionary[player.PlayerID];

                    if (!unlockedOutfitsSkinPerPlayer.ContainsKey(m_CurrentDisplayedOutfitId))
                    {
                        unlockedOutfitsSkinPerPlayer.Add(m_CurrentDisplayedOutfitId, new List<int>());
                    }
                    List<int> unlockedSkinsPerOutfit = unlockedOutfitsSkinPerPlayer[m_CurrentDisplayedOutfitId];

                    switch (LockType)
                    {
                        case LockType.Coins:
                            var nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                            if (nofCoinsAvaliable < Cost)
                            {
                                transactionImage.SetActive(true);
                                var texts = transactionImage.GetComponentsInChildren<Text>();
                                texts[0].text = "Requires " + (Cost - nofCoinsAvaliable) +
                                                " more coins to buy the skin";
                                //message
                                texts[1].text = "Not enough coins"; //message
                                BuyOutfitGameObject.gameObject.SetActive(true);
                                //hide model
                                Camera.main.depth = -1;
                                PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                            }
                            else
                            {
                                nofCoinsAvaliable -= Cost;
                                unlockedSkinsPerOutfit.Add(SkinID);
                                unlockedOutfitsSkinPerPlayer[m_CurrentDisplayedOutfitId] = unlockedSkinsPerOutfit;
                                unlockedOutfitsSkinsPerPlayerDictionary[player.PlayerID] = unlockedOutfitsSkinPerPlayer;
                                PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;

                                PlayerData.PlayerProfile.UnlockedSkinsPerOutfit =
                                    unlockedOutfitsSkinsPerPlayerDictionary;
                                WearOutfitButton.gameObject.SetActive(true);
                                BuyOutfitGameObject.gameObject.SetActive(false);
                                if (NoofUpdrages > 0)
                                    ShowUpgradeOutfitPanel();

                                //show unlocked 
                                GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject,
                                    "DisplayAssembleOrUpgradeOutfit");
                                display.GetComponentInChildren<Text>().text = "Unlocked Skin";
                                display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
                                display.SetActive(true);

                                //check for skin mission completion

                                if (unlockSkinMissions.Count > 0)
                                {
                                    foreach (var mission in unlockSkinMissions)
                                    {
                                        if (mission.PlayerId == player.PlayerID &&
                                            mission.AmountOrObjectIdToComplete == OutfitID && mission.SkinId==SkinID)
                                        {
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                        }
                                    }
                                }

                            }
                            //set text
                            UiManager.Instance.UpdateCoins(nofCoinsAvaliable);

                            break;

                        case LockType.Diamonds:
                            var noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                            if (noofDiamondsAvailable < Cost)
                            {
                                transactionImage.SetActive(true);
                                var texts = transactionImage.GetComponentsInChildren<Text>();
                                texts[0].text = "Require " + (Cost - noofDiamondsAvailable) +
                                                " more Diamonds to buy the skin"; //message
                                texts[1].text = "Not enough coins"; //message
                                BuyOutfitGameObject.gameObject.SetActive(true);
                                if (NoofUpdrages > 0)
                                    ShowUpgradeOutfitPanel();
                                //hide model
                                Camera.main.depth = -1;
                                PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 2;
                            }
                            else
                            {
                                noofDiamondsAvailable -= Cost;
                                unlockedSkinsPerOutfit.Add(SkinID);
                                unlockedOutfitsSkinPerPlayer[m_CurrentDisplayedOutfitId] = unlockedSkinsPerOutfit;
                                unlockedOutfitsSkinsPerPlayerDictionary[player.PlayerID] = unlockedOutfitsSkinPerPlayer;

                                PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;
                                PlayerData.PlayerProfile.UnlockedSkinsPerOutfit = unlockedOutfitsSkinsPerPlayerDictionary;
                                BuyOutfitGameObject.gameObject.SetActive(false);
                                WearOutfitButton.gameObject.SetActive(true);
                                if (NoofUpdrages > 0)
                                    ShowUpgradeOutfitPanel();


                                //show unlocked 
                                GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject,
                                    "DisplayAssembleOrUpgradeOutfit");
                                display.GetComponentInChildren<Text>().text = "Unlocked skin";
                                display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
                                display.SetActive(true);

                                Debug.Log("unlock skin missions" + unlockSkinMissions.Count);

                                //check for skin mission completion
                                if (unlockSkinMissions.Count > 0)
                                {

                                    foreach (var mission in unlockSkinMissions)
                                    {
                                        if (mission.PlayerId == player.PlayerID &&
                                            mission.AmountOrObjectIdToComplete == OutfitID && mission.SkinId == SkinID)
                                        {
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                        }
                                    }
                                }
                            }
                            UiManager.Instance.UpdateDiamonds(noofDiamondsAvailable);

                            break;

                    }

                }
            }
            else
            {
                if (m_WearOutfitText)
                {
                    m_WearOutfitText.text = "Wear Outfit";
                }
                if (m_BuyOutfitOrSkinText)
                {
                    m_BuyOutfitOrSkinText.text = "Buy Outfit";
                }
                if (!unlockedOutfitsPerPlayer.Contains(m_CurrentDisplayedOutfitId))
                {
                    switch (LockType)
                    {
                        case LockType.Coins:
                            var nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                            if (nofCoinsAvaliable < Cost)
                            {
                                transactionImage.SetActive(true);
                                var texts = transactionImage.GetComponentsInChildren<Text>();
                                texts[0].text = "Requires " + (Cost - nofCoinsAvaliable) +
                                                " more coins to buy the outfit";
                                //message
                                texts[1].text = "Not enough coins"; //message
                                BuyOutfitGameObject.gameObject.SetActive(true);
                                //hide model
                                Camera.main.depth = -1;
                                PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                            }
                            else
                            {
                                nofCoinsAvaliable -= Cost;
                                unlockedOutfitsPerPlayer.Add(m_CurrentDisplayedOutfitId);
                                PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;
                                PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary;
                                WearOutfitButton.gameObject.SetActive(true);
                                BuyOutfitGameObject.gameObject.SetActive(false);
                                if (NoofUpdrages > 0)
                                    ShowUpgradeOutfitPanel();

                                //show unlocked 
                                GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject,
                                    "DisplayAssembleOrUpgradeOutfit");
                                display.GetComponentInChildren<Text>().text = "Unlocked Outfit";
                                display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
                                display.SetActive(true);

                                //check for mission completion
                                Debug.Log("unlock oufit missions" + unlockOutfitMissions.Count);

                                if (unlockOutfitMissions.Count > 0)
                                {
                                    foreach (var mission in unlockOutfitMissions)
                                    {
                                        if (mission.PlayerId == player.PlayerID &&
                                            mission.AmountOrObjectIdToComplete == OutfitID)
                                        {
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                        }
                                    }
                                }
                            }
                            //set text
                            UiManager.Instance.UpdateCoins(nofCoinsAvaliable);

                            break;

                        case LockType.Diamonds:
                            var noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                            if (noofDiamondsAvailable < Cost)
                            {
                                transactionImage.SetActive(true);
                                var texts = transactionImage.GetComponentsInChildren<Text>();
                                texts[0].text = "Require " + (Cost - noofDiamondsAvailable) +
                                                " more Diamonds to buy the outfit"; //message
                                texts[1].text = "Not enough coins"; //message
                                BuyOutfitGameObject.gameObject.SetActive(true);
                                if (NoofUpdrages > 0)
                                    ShowUpgradeOutfitPanel();
                                //hide model
                                Camera.main.depth = -1;
                                PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 2;
                            }
                            else
                            {
                                noofDiamondsAvailable -= Cost;
                                unlockedOutfitsPerPlayer.Add(m_CurrentDisplayedOutfitId);
                                PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;
                                PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer = unlockedOutfitsPerPlayerDictionary;
                                BuyOutfitGameObject.gameObject.SetActive(false);
                                WearOutfitButton.gameObject.SetActive(true);
                                if (NoofUpdrages > 0)
                                    ShowUpgradeOutfitPanel();


                                //show unlocked 
                                GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject,
                                    "DisplayAssembleOrUpgradeOutfit");
                                display.GetComponentInChildren<Text>().text = "Unlocked Outfit";
                                display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
                                display.SetActive(true);

                                //check for mission completion
                                Debug.Log("unlock oufit missions" + unlockOutfitMissions.Count);

                                if (unlockOutfitMissions.Count > 0)
                                {
                                    foreach (var mission in unlockOutfitMissions)
                                    {
                                        if (mission.PlayerId == player.PlayerID &&
                                            mission.AmountOrObjectIdToComplete == OutfitID)
                                        {
                                            EventManager.TriggerEvent(mission.MissionTitle);
                                        }
                                    }
                                }
                            }
                            UiManager.Instance.UpdateDiamonds(noofDiamondsAvailable);

                            break;
                        case LockType.Blueprints:

                            break;
                    }
                }
                else
                    BuyOutfitGameObject.gameObject.SetActive(false);
            }

    
        //save to database
        PlayerData.SavePlayerData();
    }

    public void ShowUpgradeOutfitPanel()
    {
        #region "check for dictionary"

        var player = GetCurrentPlayerDisplayed();
        var upgradedOutfitDictionary = PlayerData.PlayerProfile.NoofOutfitUpgradedPerPlayer;
        if (upgradedOutfitDictionary == null)
            return;
        if (!upgradedOutfitDictionary.ContainsKey(player.PlayerID))
            upgradedOutfitDictionary.Add(player.PlayerID, new Dictionary<int, int>());
        var upgradedOutfitDictionaryPerPlayer = upgradedOutfitDictionary[player.PlayerID];
        if (upgradedOutfitDictionaryPerPlayer == null)
            return;
        if (!upgradedOutfitDictionaryPerPlayer.ContainsKey(m_CurrentDisplayedOutfitId))
            upgradedOutfitDictionaryPerPlayer.Add(m_CurrentDisplayedOutfitId, 0);
        var noofUpgradesompleted = upgradedOutfitDictionaryPerPlayer[m_CurrentDisplayedOutfitId];

        #endregion

        #region Update UpgradePanel
        UpgradeOutfitGameObject.GetComponent<Text>().text = "Upgrade (" + noofUpgradesompleted +" OF " + NoofUpdrages +")";
        if (noofUpgradesompleted < NoofUpdrages)
        {
            UpgradeOutfitGameObject.SetActive(true);
            var upgradeSlider = UpgradeOutfitGameObject.GetComponentInChildren<Slider>();
            var upgradeButton = UpgradeOutfitGameObject.GetComponentInChildren<Button>();
            upgradeButton.interactable = true;
            upgradeButton.GetComponentInChildren<Text>().text = UpgradeCosts[noofUpgradesompleted].ToString();
            upgradeSlider.value = (float) noofUpgradesompleted / NoofUpdrages;
            var images = upgradeButton.GetComponentsInChildren<Image>(true);
            if (UpgradeLockTypes[noofUpgradesompleted] ==
                LockType.Coins)
            {
                images[0].gameObject.SetActive(true);
                images[1].gameObject.SetActive(true);

                images[0].sprite = CoinButton;
                images[1].sprite = CoinSprite;
            }
            else if (UpgradeLockTypes[noofUpgradesompleted] == LockType.Diamonds)
            {
                images[0].gameObject.SetActive(true);
                images[1].gameObject.SetActive(true);

                images[0].sprite = DiamondButton;
                images[1].sprite = DiamondSprite;
            }
        }
        else
        {
            // UpgradeOutfitGameObject.SetActive(false);
            UpgradeOutfitGameObject.SetActive(true);

            Slider upgradeSlider = UpgradeOutfitGameObject.GetComponentInChildren<Slider>();
            upgradeSlider.value = (float)noofUpgradesompleted / NoofUpdrages;
            Button upgradeButton = UpgradeOutfitGameObject.GetComponentInChildren<Button>();
            Image[] images = upgradeButton.GetComponentsInChildren<Image>(true);
            if (images[1] != null)
            {
                images[1].gameObject.SetActive(false);
                images[0].sprite = DefaultButton;
            }
            upgradeButton.GetComponentInChildren<Text>().text = "Completed";

            upgradeButton.interactable = false;
        }

        #endregion
    }

    public void UpgradeOutfit()
    {
       // Debug.Log("upgrade outfir called");
        m_Canvas = GameObject.Find("Canvas");
        //load text components
        //Text coins=GameObject.FindGameObjectWithTag("Coins").GetComponent<Text>();
        //Text diamonds=GameObject.FindGameObjectWithTag("Diamonds").GetComponent<Text>();
        var transactionImage = Util.FindGameObjectWithName(m_Canvas, "TransactionFailedImage");
        var player = GetCurrentPlayerDisplayed();
        if (!PlayerData.PlayerProfile.UnlockedPlayerList.Contains(player.PlayerID))
        {
            //prompt player to buy character first
            transactionImage.gameObject.SetActive(true);
            var texts = transactionImage.GetComponentsInChildren<Text>();
            texts[0].text = "Should buy character first"; //message
            texts[1].text = "Unlock Charcter first"; //message
            return;
        }
        var upgradedOutfitDictionary = PlayerData.PlayerProfile.NoofOutfitUpgradedPerPlayer;

        if (upgradedOutfitDictionary == null)
        {
            Debug.Log("dictinay is null");
            return;
        }

        if (!upgradedOutfitDictionary.ContainsKey(player.PlayerID))
            upgradedOutfitDictionary.Add(player.PlayerID, new Dictionary<int, int>());
        var upgradedOutfitDictionaryPerPlayer = upgradedOutfitDictionary[player.PlayerID];
        if (upgradedOutfitDictionaryPerPlayer == null)
            return;
        if (!upgradedOutfitDictionaryPerPlayer.ContainsKey(m_CurrentDisplayedOutfitId))
            upgradedOutfitDictionaryPerPlayer.Add(m_CurrentDisplayedOutfitId, 0);
        var noofUpgradesompleted = upgradedOutfitDictionaryPerPlayer[m_CurrentDisplayedOutfitId];
        //more upgrades available
        if (noofUpgradesompleted < NoofUpdrages)
        {
          
            var upgradeLockType = UpgradeLockTypes[noofUpgradesompleted];

            switch (upgradeLockType)
            {
                case LockType.Coins:
                    var nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;


                    if (nofCoinsAvaliable < UpgradeCosts[noofUpgradesompleted])
                    {
                        transactionImage.SetActive(true);
                        var texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (UpgradeCosts[noofUpgradesompleted] - nofCoinsAvaliable) +
                                        " more coins to upgrade the outfit";
                        //message
                        texts[1].text = "Not enough coins"; //message
                        //hide model
                        Camera.main.depth = -1;
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                    }
                    else
                    {
                        
                        nofCoinsAvaliable -= UpgradeCosts[noofUpgradesompleted];
                        noofUpgradesompleted = upgradedOutfitDictionaryPerPlayer[m_CurrentDisplayedOutfitId] += 1;
                        PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;
                        PlayerData.PlayerProfile.NoofOutfitUpgradedPerPlayer = upgradedOutfitDictionary;
                        ShowUpgradeOutfitPanel();
                        BuyOutfitGameObject.gameObject.SetActive(false);
                        //display 
                        GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject, "DisplayAssembleOrUpgradeOutfit");
                        display.GetComponentInChildren<Text>().text = "Upgraded Outfit";
                        display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
                        display.SetActive(true);
                    }
                    break;
                case LockType.Diamonds:
                    var noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                    if (noofDiamondsAvailable < UpgradeCosts[noofUpgradesompleted])
                    {
                        transactionImage.SetActive(true);
                        var texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (UpgradeCosts[noofUpgradesompleted] - noofDiamondsAvailable) +
                                        " more diamonds to upgrade the outfit";
                        //message
                        texts[1].text = "Not enough coins"; //message
                        //hide model
                        Camera.main.depth = -1;
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                    }
                    else
                    {
                        noofDiamondsAvailable -= UpgradeCosts[noofUpgradesompleted];
                        noofUpgradesompleted = upgradedOutfitDictionaryPerPlayer[m_CurrentDisplayedOutfitId] += 1;
                        PlayerData.PlayerProfile.NoofCoinsAvailable = noofDiamondsAvailable;
                        PlayerData.PlayerProfile.NoofOutfitUpgradedPerPlayer = upgradedOutfitDictionary;
                        ShowUpgradeOutfitPanel();
                        BuyOutfitGameObject.gameObject.SetActive(false);
                        //display 
                        GameObject display = Util.FindGameObjectWithName(m_Canvas.gameObject, "DisplayAssembleOrUpgradeOutfit");
                        display.GetComponentInChildren<Text>().text = "Upgraded Outfit";
                        display.GetComponentsInChildren<Image>()[1].sprite = OutfitImage;
                        display.SetActive(true);
                    }
                    break;
            }
            UiManager.Instance.UpdateUi();
            PlayerData.SavePlayerData();
            //ShowUpgradeOutfitPanel();
        }
    }

    private void addallblueprints(int playerId)
    {
        var noofBluePrintsUnlockedPerOutfitPerPlayer = PlayerData.PlayerProfile.UnlockedBlueprintsPeroutfit;
        if (noofBluePrintsUnlockedPerOutfitPerPlayer == null)
            return;
        if (!noofBluePrintsUnlockedPerOutfitPerPlayer.ContainsKey(playerId))
            noofBluePrintsUnlockedPerOutfitPerPlayer.Add(playerId, new Dictionary<int, int>());
        var noOfBluePrintsUnlockedPerOutfitDict = noofBluePrintsUnlockedPerOutfitPerPlayer[playerId];
        if (!noOfBluePrintsUnlockedPerOutfitDict.ContainsKey(m_CurrentDisplayedOutfitId))
            noOfBluePrintsUnlockedPerOutfitDict.Add(m_CurrentDisplayedOutfitId, -1);
        var noOfBlueprintsUnlockedPerOutfit = noOfBluePrintsUnlockedPerOutfitDict[m_CurrentDisplayedOutfitId]+=NoofBluePrints;
        PlayerData.PlayerProfile.UnlockedBlueprintsPeroutfit = noofBluePrintsUnlockedPerOutfitPerPlayer;
        PlayerData.SavePlayerData();
    }
    private int GetNumberOfBluePrintsUnlockedPerOutfit(int playerId)
    {
        var noofBluePrintsUnlockedPerOutfitPerPlayer = PlayerData.PlayerProfile.UnlockedBlueprintsPeroutfit;
        if (noofBluePrintsUnlockedPerOutfitPerPlayer == null)
            return -1;
        if (!noofBluePrintsUnlockedPerOutfitPerPlayer.ContainsKey(playerId))
            noofBluePrintsUnlockedPerOutfitPerPlayer.Add(playerId, new Dictionary<int, int>());
        var noOfBluePrintsUnlockedPerOutfitDict = noofBluePrintsUnlockedPerOutfitPerPlayer[playerId];
        if (!noOfBluePrintsUnlockedPerOutfitDict.ContainsKey(m_CurrentDisplayedOutfitId))
            noOfBluePrintsUnlockedPerOutfitDict.Add(m_CurrentDisplayedOutfitId, 0);
        var noOfBlueprintsUnlockedPerOutfit = noOfBluePrintsUnlockedPerOutfitDict[m_CurrentDisplayedOutfitId];

        return noOfBlueprintsUnlockedPerOutfit;
    }

    private Player GetCurrentPlayerDisplayed()
    {
        return DataManager.Instance.Players[m_SourceBody.GetComponent<Player>().PlayerID - 1];
    }

    public void WearOutfitOrSkin()
    {
        Player player = GetCurrentPlayerDisplayed();
        var wornOutfitPerPlayer = new Dictionary<int, int>
        {
            {player.PlayerID, m_CurrentDisplayedOutfitId}
        };
        Dictionary<int, Dictionary<int, int>> wornSkinPerOutfitperPlayerDictionary =
               PlayerData.PlayerProfile.WornSkinPerOutfitPerPlayer;
        if (wornSkinPerOutfitperPlayerDictionary == null)
            return;
        if (!wornSkinPerOutfitperPlayerDictionary.ContainsKey(player.PlayerID))
            wornSkinPerOutfitperPlayerDictionary.Add(player.PlayerID, new Dictionary<int, int>());

        Dictionary<int, int> wornSkinPerOutfitDictionary = wornSkinPerOutfitperPlayerDictionary[player.PlayerID];
        if (wornSkinPerOutfitDictionary == null)
        {
            return;
        }
        if (!wornSkinPerOutfitDictionary.ContainsKey(m_CurrentDisplayedOutfitId))
        {
            wornSkinPerOutfitDictionary.Add(m_CurrentDisplayedOutfitId, -1);
        }
        if (IsSkin)
        {
           
            wornSkinPerOutfitDictionary[m_CurrentDisplayedOutfitId] = SkinID;
        }
        else
        {
            var transactionImage = Util.FindGameObjectWithName(m_Canvas, "TransactionFailedImage");
            if (!PlayerData.PlayerProfile.UnlockedPlayerList.Contains(player.PlayerID))
            {
                //prompt player to buy character first
                transactionImage.gameObject.SetActive(true);
                var texts = transactionImage.GetComponentsInChildren<Text>();
                texts[0].text = "Should buy character first"; //message
                texts[1].text = "Unlock Charcter first"; //message
                return;
            }
            wornSkinPerOutfitDictionary[m_CurrentDisplayedOutfitId] = -1;
        }

        PlayerData.PlayerProfile.WornSkinPerOutfitPerPlayer = wornSkinPerOutfitperPlayerDictionary;
        PlayerData.PlayerProfile.WornOutfitPerPlayer = wornOutfitPerPlayer;

        BuyPanel.SetActive(true);
    //    UpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 0;
        WearOutfitButton.gameObject.SetActive(true);
        WearOutfitButton.GetComponentInChildren<Text>().text = "Equipped";
        //save to database
        PlayerData.SavePlayerData();
    }

    private void AddClothes(GameObject clothing)
    {
        if (clothing == null)
            return;

        //  clothing = (GameObject) GameObject.Instantiate(clothing);
        //add material if outfit is invincible
        Outfitlist.PreviousOutfits.Add(OutfitAbility == OutfitAbility.Invincible
            ? m_Sticher.Stitch(clothing, m_SourceBody, true)
            : m_Sticher.Stitch(clothing, m_SourceBody));

        //   GameObject.Destroy(clothing);
    }


    private void RemovePreviousClothes()
    {
        for (var i = 0; i < Outfitlist.PreviousOutfits.Count; i++)
            Destroy(Outfitlist.PreviousOutfits[i]);
    }

    //public static GameObject FindGameObjectWithTag(GameObject parent, string tag)
    //{
    //    var trs = parent.GetComponentsInChildren<Transform>(true);
    //    foreach (var t in trs)
    //    {
    //        if (t.CompareTag(tag))
    //        {
    //            return t.gameObject;
    //        }

    //    }
    //    return null;
    //}

    //public static GameObject FindGameObjectWithName(GameObject parent, string name)
    //{
    //    var trs = parent.GetComponentsInChildren<Transform>(true);
    //    foreach (var t in trs)
    //    {
    //        if (t.name==name)
    //        {
    //            return t.gameObject;
    //        }
    //    }
    //    return null;
    //}
    private void SetIsOutfitInitialized()
    {
    }


    public static class Outfitlist
    {
        public static List<GameObject> PreviousOutfits = new List<GameObject>();
    }
}

public enum OutfitAbility
{
    None,
    LowDamage,
    RegenerateHealth,
    Invincible,
    Revive
}

