using System;
using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class ShowWeapons : MonoBehaviour
{    //
    public GameObject BuyPanel;
    public AudioSource WeaponDetailAudioSource;
    public GameObject WeaponUpgradeOutfitGameObject;
    public Text UpgradeNote;
    public Text DamageText;
    public Text RateOfFire;
    public Text Range;
    public Text TotalAmmo;
    public Text ClipSize;
    public Text Spread;
    public Canvas Canvas;
    public Sprite DefaultBuyButton;
    public Sprite CoinSprite;
    public Sprite CoinButton;
    public Sprite DiamondSprite;
    public Sprite DiamondButton;
    public Sprite DefaultSprite;
    public Sprite SelectedSprite;
    public RectTransform WeaponPanel;
    public RectTransform WeaponsDisplayPanel;
    public RectTransform WeaponsRotatePanel;
    public Button WeaponsButton;
    public Button WeaponBuyButton;
    public Button WeaponEquipButton;
    private bool m_CanRotate;
    private List<Weapon> instantiatedWeapons=new List<Weapon>();
    private List<GameObject> instantiatedWeaponModels=new List<GameObject>();
    private WaitForSeconds waitTime=new WaitForSeconds(0.01f);
    private int m_CurrentWeaponDisplayedId;
    private List<Button> weaponButtons=new List<Button>();

    private MissionManager m_MissionManager;
    private readonly List<Mission> unlockWeaponMissions = new List<Mission>();
    private readonly List<Mission> upgradeWeaponMissions = new List<Mission>();
    private List<Mission> collectBluePrintsMissions=new List<Mission>();
    // Use this for initialization
    void Start ()
	{
        m_MissionManager=MissionManager.Instance;

        unlockWeaponMissions.Clear();
        upgradeWeaponMissions.Clear();
        if (m_MissionManager != null)
            foreach (var mission in m_MissionManager.GetActiveMissions())
            {
                if (mission.MissionType == MissionType.UnlockWeapon)
                    unlockWeaponMissions.Add(mission);
                if (mission.MissionType == MissionType.UpgradeWeapon)
                    upgradeWeaponMissions.Add(mission);
                if (mission.MissionType == MissionType.CollectBluePrints)
                {
                    collectBluePrintsMissions.Add(mission);
                }
            }
        int i = 0;
        List<int> unlockedWeapons = PlayerData.PlayerProfile.UnlockedWeaponsList;
	    Dictionary<int, int> noofUpgradesCompletedPerWeapon = PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted;
	    Dictionary<int,int> noOfBluePrintsUnlockedPerWeapon=PlayerData.PlayerProfile.NoOfBluePrintsUnlockedPerWeapon;
        int selectedWeapon = PlayerData.PlayerProfile.CurrentSelectedWeapon;
        weaponButtons.Clear();
        foreach (Weapon weapon in DataManager.Instance.Weapons)
        {
            GameObject instance = Instantiate(weapon.WeaponModel) as GameObject;

            instance.transform.position = Vector3.zero + new Vector3(20 , 0, 0);
            instance.transform.SetParent(WeaponPanel);
            instance.AddComponent<ObjectRotator>();
            instance.SetActive(false);
            instantiatedWeaponModels.Add(instance);
            instantiatedWeapons.Add(weapon);
            //instantiate buttons
            Button weaponsButton=Instantiate(WeaponsButton) as Button;
            weaponsButton.transform.SetParent(WeaponsDisplayPanel, false);
            WeaponsButton.transform.localScale = new Vector3(1, 1, 1);
            Weapon weapon1 = weapon;

            Image[] images = weaponsButton.GetComponentsInChildren<Image>();
            images[1].sprite = weapon.WeaponSprite;  //1 for weapon image sprite

            images[2].gameObject.SetActive(PlayerData.PlayerProfile.CurrentLevel < weapon.LevelRequiredToUnlock);
            if (unlockedWeapons.Contains(weapon.WeaponId))
            {
                images[2].gameObject.SetActive(false);
            }

            Text weaponnameText = weaponsButton.GetComponentInChildren<Text>();
            weaponnameText.text = weapon.WeaponName;
            weaponButtons.Add(weaponsButton);
            weaponsButton.onClick.AddListener(() => ChangeDisplayedWeapon(weapon1));
            var i1 = i;
            weaponsButton.onClick.AddListener(() =>ChangeSprite(i1));
            i++;
        }

        //show selected weapon
        ChangeDisplayedWeapon(instantiatedWeapons[selectedWeapon-1]);

        //check for already completed missions
	    if (unlockWeaponMissions.Count > 0)
	    {
            foreach (var mission in unlockWeaponMissions)
            {
                foreach (var weaponId in unlockedWeapons)
                {
                    if (mission.AmountOrObjectIdToComplete == weaponId)
                    {
                        EventManager.TriggerEvent(mission.MissionTitle);

                    }
                }          
            }
        }
        if (upgradeWeaponMissions.Count > 0)
        {
            foreach (var mission in upgradeWeaponMissions)
            {
                foreach (int key in noofUpgradesCompletedPerWeapon.Keys)
                {
                    int noofUpgrades = noofUpgradesCompletedPerWeapon[key];
                    if (mission.NoOfUpgrades == noofUpgrades)
                    {
                        EventManager.TriggerEvent(mission.MissionTitle);
                    }
                }
            }
        }
        //check for collect blueprints missions
	    if (collectBluePrintsMissions.Count > 0)
	    {
	        foreach (var mission in collectBluePrintsMissions)
	        {
                int key = mission.AmountOrObjectIdToComplete;
	            if (noOfBluePrintsUnlockedPerWeapon.ContainsKey(key))
	            {
	                int noOfBluePrintsCollected = noOfBluePrintsUnlockedPerWeapon[key];
	                if (mission.NoOfBluePrintsCollected == noOfBluePrintsCollected)
	                {
	                    EventManager.TriggerEvent(mission.MissionTitle);

	                }
                }
            }
	    }
    }

    void DisplayWeaponDetails(Weapon weapon)
    {
        m_CurrentWeaponDisplayedId = weapon.WeaponId;
        //display weapon
        instantiatedWeaponModels[m_CurrentWeaponDisplayedId - 1].SetActive(true);
        Dictionary<int,int> upgradedWeaponDictionary = PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted;
        if (upgradedWeaponDictionary == null)
        {
            return;
        }
        if (!upgradedWeaponDictionary.ContainsKey(weapon.WeaponId))
        {
            upgradedWeaponDictionary.Add(weapon.WeaponId,0);
        }
        int noOfUpgradesCompleted = upgradedWeaponDictionary[weapon.WeaponId];
        StartCoroutine(UpdateTextValueSlowly(DamageText, (int) Mathf.Ceil(weapon.HitscanDamageAmount + noOfUpgradesCompleted * weapon.HitscanDamageAmount* weapon.IncreasePercentage/100),true,5));
        StartCoroutine(UpdateTextValueSlowly(RateOfFire,  weapon.RateOfFire + noOfUpgradesCompleted * weapon.RateOfFire * weapon.IncreasePercentage/100,false));
        StartCoroutine(UpdateTextValueSlowly(Range, (int) Mathf.Ceil(weapon.WeaponRange + noOfUpgradesCompleted * weapon.WeaponRange * weapon.IncreasePercentage/100),true,10));
        StartCoroutine(UpdateTextValueSlowly(TotalAmmo, (int) Mathf.Ceil(weapon.TotalAmmo + noOfUpgradesCompleted*weapon.TotalAmmo*weapon.IncreasePercentage/100),true,10));
        StartCoroutine(UpdateTextValueSlowly(ClipSize, (int) Mathf.Ceil(weapon.ClipSize + noOfUpgradesCompleted * weapon.ClipSize * weapon.IncreasePercentage/100),false,10));
        StartCoroutine(UpdateTextValueSlowly(Spread, 100- (weapon.Spread-noOfUpgradesCompleted *weapon.Spread*weapon.IncreasePercentage/100)*100,false));
        WeaponBuyButton.GetComponentInChildren<Text>().text = instantiatedWeapons[m_CurrentWeaponDisplayedId - 1].WeaponCost.ToString();
        //play sound
        WeaponDetailAudioSource.Play((ulong)0.01f);
        DisableOtherObjects(instantiatedWeaponModels, m_CurrentWeaponDisplayedId - 1);
        ResetPositionOfOtherObjects(instantiatedWeaponModels, m_CurrentWeaponDisplayedId - 1);
        //display upgrade note
        UpgradeNote.text = "*Each Upgrade increases the values by " + weapon.IncreasePercentage + " %";


    }

    private void ChangeDisplayedWeapon(Weapon weapon)
    {
        int weaponId = weapon.WeaponId;
        DisplayWeaponDetails(weapon);
    
        WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>(true).onClick.RemoveAllListeners();
        WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>(true)
            .onClick.AddListener(() => UpgradeWeapon(weapon));
        var images = WeaponBuyButton.GetComponentsInChildren<Image>();
        WeaponUpgradeOutfitGameObject.SetActive(false);
        List<int> unlockedWeapons = PlayerData.PlayerProfile.UnlockedWeaponsList;
        if (!unlockedWeapons.Contains(weapon.WeaponId))
        {
            WeaponBuyButton.gameObject.SetActive(true);
            switch (weapon.LockType)
            {
                case LockType.Coins:
                    BuyPanel.SetActive(true);

                    images[0].sprite = CoinButton;
                    images[1].sprite = CoinSprite;
                    break;
                case LockType.Diamonds:
                    BuyPanel.SetActive(true);

                    images[0].sprite = DiamondButton;
                    images[1].sprite = DiamondSprite;
                    break;
                case LockType.Blueprints:
                    BuyPanel.SetActive(false);
                    var noOfBluePrintsUnlocked = GetNumberOfBluePrintsUnlockedPerWeapon(weapon.WeaponId);
                    WeaponUpgradeOutfitGameObject.GetComponent<Text>().text = "Blue Prints(" + noOfBluePrintsUnlocked + " OF "
 + weapon.NoofBluePrints + ")";
                    var upgradeSlider = WeaponUpgradeOutfitGameObject.GetComponentInChildren<Slider>(true);
                    var upgradeButton = WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>(true);
                    var buySpriteImages = upgradeButton.GetComponentsInChildren<Image>(true);
                    buySpriteImages[0].sprite = DefaultSprite;
                    buySpriteImages[1].gameObject.SetActive(false);
                    //for testing need to add through boxes
                    //addallblueprints(weapon);
                    //noOfBluePrintsUnlocked = GetNumberOfBluePrintsUnlockedPerWeapon(weapon.WeaponId);

                    if (noOfBluePrintsUnlocked < weapon.NoofBluePrints)
                    {
                        WeaponBuyButton.gameObject.SetActive(false);
                        WeaponEquipButton.gameObject.SetActive(false);
                        WeaponUpgradeOutfitGameObject.SetActive(true);
                        upgradeButton.GetComponentInChildren<Text>().text = "Assemble";
                        upgradeButton.interactable = false;
                        upgradeSlider.value = (float)noOfBluePrintsUnlocked / weapon.NoofBluePrints;
                        noOfBluePrintsUnlocked = weapon.NoofBluePrints;
                    }
                    else
                    {
                        upgradeSlider.value = (float)noOfBluePrintsUnlocked / weapon.NoofBluePrints;
                        WeaponUpgradeOutfitGameObject.SetActive(true);
                        upgradeButton.GetComponentInChildren<Text>().text = "Assemble";
                        upgradeButton.interactable = true;
                        upgradeButton.onClick.RemoveAllListeners();
                        upgradeButton.onClick.AddListener(() => Assemble(weapon));
                    }
                   break;
            }
        }
        else
        {
            int selectedWeapon = PlayerData.PlayerProfile.CurrentSelectedWeapon;
            BuyPanel.SetActive(true);
            WeaponBuyButton.gameObject.SetActive(false);

            //update weapon bought inf0
            if (selectedWeapon == weapon.WeaponId)
            {
                WeaponEquipButton.gameObject.SetActive(true);
                WeaponEquipButton.GetComponentInChildren<Text>().text = "Equipped";
            }
            else
            {
                WeaponEquipButton.gameObject.SetActive(true);
                WeaponEquipButton.GetComponentInChildren<Text>().text = "Equip";
            }
            ChangeSprite(selectedWeapon - 1);
            if (weapon.NoofUpdrages > 0)
            {
                ShowUpgradeWeaponPanel(weapon);

            }
            else
            {
                WeaponUpgradeOutfitGameObject.SetActive(false);

            }

        }
    }

    private void addallblueprints(Weapon weapon)
    {
        Dictionary<int,int> noofBluePrintsUnlockedPerWeapon = PlayerData.PlayerProfile.NoOfBluePrintsUnlockedPerWeapon;
        if (noofBluePrintsUnlockedPerWeapon == null)
            return;
        if (!noofBluePrintsUnlockedPerWeapon.ContainsKey(weapon.WeaponId))
            noofBluePrintsUnlockedPerWeapon.Add(weapon.WeaponId, -1);
        var noOfBlueprintsUnlockedPerOutfit = noofBluePrintsUnlockedPerWeapon[weapon.WeaponId] += weapon.NoofBluePrints;
        PlayerData.PlayerProfile.NoOfBluePrintsUnlockedPerWeapon = noofBluePrintsUnlockedPerWeapon;
        PlayerData.SavePlayerData();
    }

    private void Assemble(Weapon weapon)
    {
        //assemle all Parts
        var unlockedWeaponslist = PlayerData.PlayerProfile.UnlockedWeaponsList;
        if (unlockedWeaponslist == null)
            return;

        if (!unlockedWeaponslist.Contains(weapon.WeaponId))
        {
            unlockedWeaponslist.Add(weapon.WeaponId);
            BuyPanel.SetActive(true);
            WeaponBuyButton.gameObject.SetActive(false);
            WeaponEquipButton.gameObject.SetActive(true);
            WeaponEquipButton.GetComponentInChildren<Text>().text = "Equip";
            //assign back upgrade logic to the button
            WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>(true).onClick.RemoveAllListeners();
            WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>(true)
                .onClick.AddListener(() => UpgradeWeapon(weapon));
            if (weapon.NoofUpdrages > 0)
            {
           //     WeaponUpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                ShowUpgradeWeaponPanel(weapon);
            }
            else
            {
           //     WeaponUpgradeOutfitGameObject.GetComponentInParent<LayoutGroup>().padding.left = 50;
                WeaponUpgradeOutfitGameObject.SetActive(false);
            }
            PlayerData.PlayerProfile.UnlockedWeaponsList = unlockedWeaponslist;
            //display 
            GameObject display = Util.FindGameObjectWithName(Canvas.gameObject, "DisplayAssembleOrUpgradeWeapon");
            display.GetComponentInChildren<Text>().text = "Assembled Weapon";
            display.GetComponentsInChildren<Image>()[1].sprite = weapon.WeaponSprite;
            display.SetActive(true);
        }
        PlayerData.SavePlayerData();
    }

    private void UpgradeWeapon(Weapon weapon)
    {
        //m_Canvas = GameObject.Find("Canvas");
        //load text components
        //Text coins=GameObject.FindGameObjectWithTag("Coins").GetComponent<Text>();
        //Text diamonds=GameObject.FindGameObjectWithTag("Diamonds").GetComponent<Text>();

        unlockWeaponMissions.Clear();
        upgradeWeaponMissions.Clear();
        if (m_MissionManager != null)
            foreach (var mission in m_MissionManager.GetActiveMissions())
            {
                if (mission.MissionType == MissionType.UnlockWeapon)
                    unlockWeaponMissions.Add(mission);
                if (mission.MissionType == MissionType.UpgradeWeapon)
                    upgradeWeaponMissions.Add(mission);
            }
        var transactionImage = Util.FindGameObjectWithName(Canvas.gameObject, "TransactionFailedImage");
 
        var upgradedWeaponDictionary = PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted;

        if (upgradedWeaponDictionary == null)
        {
           // Debug.Log("dictinay is null");
            return;
        }

        if (!upgradedWeaponDictionary.ContainsKey(weapon.WeaponId))
            upgradedWeaponDictionary.Add(weapon.WeaponId, 0);
        var noofUpgradesompleted = upgradedWeaponDictionary[weapon.WeaponId];
        //more upgrades available
        if (noofUpgradesompleted < weapon.NoofUpdrages)
        {
            var upgradeLockType = weapon.UpgradeLockTypes[noofUpgradesompleted];

            switch (upgradeLockType)
            {
                case LockType.Coins:
                    var nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;           
                    if (nofCoinsAvaliable < weapon.UpgradeCosts[noofUpgradesompleted])
                    {
                        transactionImage.SetActive(true);
                        var texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (weapon.UpgradeCosts[noofUpgradesompleted] - nofCoinsAvaliable) +
                                        " more coins to upgrade the weapon";
                        //message
                        texts[1].text = "Not enough coins"; //message
                        //hide model
                        Camera.main.depth = -1;
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                    }
                    else
                    {

                        nofCoinsAvaliable -= weapon.UpgradeCosts[noofUpgradesompleted];

                        Parameter[] virtualcurrencyparameters =
                        {
                            new Parameter(FirebaseAnalytics.ParameterItemName, weapon.WeaponName),
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Coins)),
                            new Parameter(FirebaseAnalytics.ParameterValue,weapon.UpgradeCosts[noofUpgradesompleted]),
                            new Parameter(FirebaseAnalytics.ParameterContentType, "upgrade_weapon"),
                        };

                        FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventSpendVirtualCurrency, virtualcurrencyparameters);

                        noofUpgradesompleted = upgradedWeaponDictionary[weapon.WeaponId] += 1;
                        PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;
                        PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted = upgradedWeaponDictionary;
                        ShowUpgradeWeaponPanel(weapon);
                        WeaponBuyButton.gameObject.SetActive(false);
                        //display 
                        GameObject display = Util.FindGameObjectWithName(Canvas.gameObject, "DisplayAssembleOrUpgradeWeapon");
                        display.GetComponentInChildren<Text>().text = "Weapon Upgraded";
                        display.GetComponentsInChildren<Image>()[1].sprite = weapon.WeaponSprite;
                        display.SetActive(true);

                        //check for upgrade missions
                        if (upgradeWeaponMissions.Count > 0)
                        {
                            foreach (var mission in upgradeWeaponMissions)
                            {
                                if (mission.AmountOrObjectIdToComplete == weapon.WeaponId &&
                                    mission.NoOfUpgrades == noofUpgradesompleted)
                                {
                                    EventManager.TriggerEvent(mission.MissionTitle);
                                }
                            }
                        }
                        Dictionary<string, string> events = new Dictionary<string, string>();
                        events.Add(AFInAppEvents.CONTENT_TYPE, "upgrade_weapon");
                        events.Add(AFInAppEvents.CONTENT_ID, weapon.WeaponId.ToString());
                        events.Add(AFInAppEvents.CONTENT_TITLE, weapon.WeaponName);
                        events.Add(AFInAppEvents.NO_OF_UPGRADES,noofUpgradesompleted.ToString());
                        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.UPGRADED_WEAPON, events);


                      
                    }
                    break;
                case LockType.Diamonds:
                    var noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                    if (noofDiamondsAvailable < weapon.UpgradeCosts[noofUpgradesompleted])
                    {
                        transactionImage.SetActive(true);
                        var texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (weapon.UpgradeCosts[noofUpgradesompleted] - noofDiamondsAvailable) +
                                        " more diamonds to upgrade the weapon";
                        //message
                        texts[1].text = "Not enough coins"; //message
                        //hide model
                        Camera.main.depth = -1;
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                    }
                    else
                    {
                        noofDiamondsAvailable -= weapon.UpgradeCosts[noofUpgradesompleted];

                        //log firebase events
                        Parameter[] virtualcurrencyparameters =
                        {
                            new Parameter(FirebaseAnalytics.ParameterItemName, weapon.WeaponName),
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Diamonds)),
                            new Parameter(FirebaseAnalytics.ParameterValue,weapon.UpgradeCosts[noofUpgradesompleted]),
                            new Parameter(FirebaseAnalytics.ParameterContentType, "upgrade_weapon")
                        };

                        FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventSpendVirtualCurrency, virtualcurrencyparameters);

                        noofUpgradesompleted = upgradedWeaponDictionary[weapon.WeaponId] += 1;
                        PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;
                        PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted = upgradedWeaponDictionary;
                        ShowUpgradeWeaponPanel(weapon);
                        WeaponBuyButton.gameObject.SetActive(false);
                        //display 
                        GameObject display = Util.FindGameObjectWithName(Canvas.gameObject, "DisplayAssembleOrUpgradeWeapon");
                        display.GetComponentInChildren<Text>().text = "Weapon Upgraded";
                        display.GetComponentsInChildren<Image>()[1].sprite = weapon.WeaponSprite;
                        display.SetActive(true);

                        //check for upgrade missions
                        if (upgradeWeaponMissions.Count > 0)
                        {
                            foreach (var mission in upgradeWeaponMissions)
                            {
                                if (mission.AmountOrObjectIdToComplete == weapon.WeaponId &&
                                    mission.NoOfUpgrades == noofUpgradesompleted)
                                {
                                    EventManager.TriggerEvent(mission.MissionTitle);
                                }
                            }
                        }
                        Dictionary<string, string> events = new Dictionary<string, string>();
                        events.Add(AFInAppEvents.CONTENT_TYPE, "upgrade_weapon");
                        events.Add(AFInAppEvents.CONTENT_ID, weapon.WeaponId.ToString());
                        events.Add(AFInAppEvents.CONTENT_TITLE, weapon.WeaponName);
                        events.Add(AFInAppEvents.NO_OF_UPGRADES, noofUpgradesompleted.ToString());
                        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.UPGRADED_WEAPON, events);
                    }
                    break;
            }
            DisplayWeaponDetails(weapon);
            UiManager.Instance.UpdateUi();
            PlayerData.SavePlayerData();
            //ShowUpgradeOutfitPanel();
        }
    }

    private void ShowUpgradeWeaponPanel(Weapon weapon)
    {
        #region "check for dictionary"

        var upgradedWeaponDictionary = PlayerData.PlayerProfile.NoOfupgradesPerWeaponCompleted;

        if (upgradedWeaponDictionary == null)
        {
          //  Debug.Log("dictinay is null");
            return;
        }
        if (!upgradedWeaponDictionary.ContainsKey(weapon.WeaponId))
            upgradedWeaponDictionary.Add(weapon.WeaponId, 0);
        var noofUpgradesompleted = upgradedWeaponDictionary[weapon.WeaponId];
        #endregion

        #region Update UpgradePanel
        WeaponUpgradeOutfitGameObject.GetComponent<Text>().text = "Upgrade (" + noofUpgradesompleted + " OF " + weapon.NoofUpdrages + ")";
        if (noofUpgradesompleted < weapon.NoofUpdrages)
        {

            WeaponUpgradeOutfitGameObject.SetActive(true);
            var upgradeSlider = WeaponUpgradeOutfitGameObject.GetComponentInChildren<Slider>();
            var upgradeButton = WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>();
            upgradeButton.interactable = true;
            upgradeButton.GetComponentInChildren<Text>().text = weapon.UpgradeCosts[noofUpgradesompleted].ToString();
            upgradeButton.GetComponentInChildren<Text>().resizeTextForBestFit = false;

            upgradeSlider.value = (float)noofUpgradesompleted / weapon.NoofUpdrages;
            var images = upgradeButton.GetComponentsInChildren<Image>(true);
            if (weapon.UpgradeLockTypes[noofUpgradesompleted] ==
                LockType.Coins)
            {
                images[0].gameObject.SetActive(true);
                images[1].gameObject.SetActive(true);

                images[0].sprite = CoinButton;
                images[1].sprite = CoinSprite;
            }
            else if (weapon.UpgradeLockTypes[noofUpgradesompleted] == LockType.Diamonds)
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
            WeaponUpgradeOutfitGameObject.SetActive(true);
            Slider upgradeSlider = WeaponUpgradeOutfitGameObject.GetComponentInChildren<Slider>();
            upgradeSlider.value = (float)noofUpgradesompleted / weapon.NoofUpdrages;
            Button upgradeButton = WeaponUpgradeOutfitGameObject.GetComponentInChildren<Button>();
            Image[] images = upgradeButton.GetComponentsInChildren<Image>(true);
            if (images[1] != null)
            {
                images[1].gameObject.SetActive(false);

            }

            upgradeButton.GetComponentInChildren<Text>().text = "FULLY UPGRADED";
            upgradeButton.GetComponentInChildren<Text>().resizeTextForBestFit = true;
            upgradeButton.interactable = false;
        }

        #endregion
    }

    private int GetNumberOfBluePrintsUnlockedPerWeapon(int weaponID)
    {
        var noofBluePrintsUnlockedPerWeapon = PlayerData.PlayerProfile.NoOfBluePrintsUnlockedPerWeapon;
        if (noofBluePrintsUnlockedPerWeapon == null)
            return -1;
        if (!noofBluePrintsUnlockedPerWeapon.ContainsKey(weaponID))
            noofBluePrintsUnlockedPerWeapon.Add(weaponID, 0);
        var noOfBluePrintsUnlockedPerWeapon = noofBluePrintsUnlockedPerWeapon[weaponID];
        return noOfBluePrintsUnlockedPerWeapon;
    }
    public void BuyWeapon()
    {

        unlockWeaponMissions.Clear();
        upgradeWeaponMissions.Clear();
        if (m_MissionManager != null)
            foreach (var mission in m_MissionManager.GetActiveMissions())
            {
                if (mission.MissionType == MissionType.UnlockWeapon)
                    unlockWeaponMissions.Add(mission);
                if (mission.MissionType == MissionType.UpgradeWeapon)
                    upgradeWeaponMissions.Add(mission);
            }
        List<int> unlockedWeapons = PlayerData.PlayerProfile.UnlockedWeaponsList;
        Weapon weapon = instantiatedWeapons[m_CurrentWeaponDisplayedId - 1];
        int currentLevel = PlayerData.PlayerProfile.CurrentLevel;
        GameObject transactionImage = Util.FindGameObjectWithName(Canvas.gameObject, "TransactionFailedImage");
        if (currentLevel < weapon.LevelRequiredToUnlock)
        {
            transactionImage.gameObject.SetActive(true);
            var texts = transactionImage.GetComponentsInChildren<Text>();
            texts[0].text = "Reach Level " + weapon.LevelRequiredToUnlock + " to unlock the weapon"; //message
            texts[1].text = "Level " + weapon.LevelRequiredToUnlock + " Required"; //message
            return;
        }
        LockType weaponLockType = weapon.LockType;

        if (!unlockedWeapons.Contains(m_CurrentWeaponDisplayedId))
        {
            switch (weaponLockType)
            {
                case LockType.Coins:
                    int nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                    if (nofCoinsAvaliable < weapon.WeaponCost)
                    {
                       
                        transactionImage.SetActive(true);
                        Text[] texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (weapon.WeaponCost - nofCoinsAvaliable) + " more coins to unlock the weapon"; //message
                        texts[1].text = "Not enough coins"; //message
                        //hide model
                        Camera.main.depth = -1;
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                    }
                    else
                    {
                        nofCoinsAvaliable -= weapon.WeaponCost;
                        unlockedWeapons.Add(weapon.WeaponId);
                        PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;
                        PlayerData.PlayerProfile.UnlockedWeaponsList = unlockedWeapons;
                        //set text
                        UiManager.Instance.UpdateCoins(nofCoinsAvaliable);
                        WeaponBuyButton.gameObject.SetActive(false);
                        WeaponEquipButton.gameObject.SetActive(true);
                        WeaponEquipButton.GetComponentInChildren<Text>().text = "Equip";
                        if (weapon.NoofUpdrages > 0)
                            ShowUpgradeWeaponPanel(weapon);
                        GameObject display = Util.FindGameObjectWithName(Canvas.gameObject, "DisplayAssembleOrUpgradeWeapon");
                        display.GetComponentInChildren<Text>().text = "Weapon Unlocked";
                        display.GetComponentsInChildren<Image>()[1].sprite = weapon.WeaponSprite;
                        display.SetActive(true);

                        //check for missions completion
                        if (unlockWeaponMissions.Count > 0)
                        {
                            foreach (var mission in unlockWeaponMissions)
                            {
                                if (mission.AmountOrObjectIdToComplete == weapon.WeaponId)
                                {
                                    EventManager.TriggerEvent(mission.MissionTitle);
                                }
                            }
                        }
                        //track appsflyerrich event
                        Dictionary<string, string> events = new Dictionary<string, string>();
                        events.Add(AFInAppEvents.CONTENT_TYPE, "weapon");
                        events.Add(AFInAppEvents.CONTENT_ID, weapon.WeaponId.ToString());
                        events.Add(AFInAppEvents.CONTENT_TITLE, weapon.WeaponName);
                        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.BOUGHT_WEAPON, events);

                        Parameter[] virtualcurrencyparameters =
                        {
                            new Parameter(FirebaseAnalytics.ParameterItemName, weapon.WeaponName),
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Coins)),
                            new Parameter(FirebaseAnalytics.ParameterValue,weapon.WeaponCost),
                            new Parameter(FirebaseAnalytics.ParameterContentType, "weapon"),
                        };

                        FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventSpendVirtualCurrency, virtualcurrencyparameters);

                    }      
                    break;

                case LockType.Diamonds:
                    int noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                    if (noofDiamondsAvailable < weapon.WeaponCost)
                    {
                        transactionImage.SetActive(true);
                        Text[] texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (weapon.WeaponCost - noofDiamondsAvailable) + " more coins to unlock the weapon"; //message
                        texts[1].text = "Not enough diamonds"; //message
                        //open dialog to buy
                        //hide model
                        Camera.main.depth = -1;
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 2;
                    }
                    else
                    {
                        noofDiamondsAvailable -= weapon.WeaponCost;
                        unlockedWeapons.Add(weapon.WeaponId);
                        PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;
                        PlayerData.PlayerProfile.UnlockedWeaponsList = unlockedWeapons;


                        //set text
                        UiManager.Instance.UpdateDiamonds(noofDiamondsAvailable);
                        WeaponBuyButton.gameObject.SetActive(false);

                        WeaponEquipButton.gameObject.SetActive(true);
                        WeaponEquipButton.GetComponentInChildren<Text>().text = "Equip";
                        if (weapon.NoofUpdrages > 0)
                            ShowUpgradeWeaponPanel(weapon);
                        GameObject display = Util.FindGameObjectWithName(Canvas.gameObject, "DisplayAssembleOrUpgradeWeapon");
                        display.GetComponentInChildren<Text>().text = "Weapon Unlocked";
                        display.GetComponentsInChildren<Image>()[1].sprite = weapon.WeaponSprite;
                        display.SetActive(true);
                        //cehck for mission completion
                        if (unlockWeaponMissions.Count > 0)
                        {
                            foreach (var mission in unlockWeaponMissions)
                            {
                                if (mission.AmountOrObjectIdToComplete == weapon.WeaponId)
                                {
                                    EventManager.TriggerEvent(mission.MissionTitle);
                                }
                            }
                        }
                        ////track appsflyerrich event
                        Dictionary<string, string> events = new Dictionary<string, string>();
                        events.Add(AFInAppEvents.CONTENT_TYPE, "weapon");
                        events.Add(AFInAppEvents.CONTENT_ID, weapon.WeaponId.ToString());
                        events.Add(AFInAppEvents.CONTENT_TITLE, weapon.WeaponName);
                        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.BOUGHT_WEAPON, events);


                        Parameter[] virtualcurrencyparameters =
                        {
                            new Parameter(FirebaseAnalytics.ParameterItemName, weapon.WeaponName),
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Diamonds)),
                            new Parameter(FirebaseAnalytics.ParameterValue,weapon.WeaponCost),
                            new Parameter(FirebaseAnalytics.ParameterContentType, "weapon"),
                        };

                        FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventSpendVirtualCurrency, virtualcurrencyparameters);
                    }
                    break;
            }
        }
        else
        {
        }
        //save to database
        PlayerData.SavePlayerData();
    }


    private void ChangeSprite(int buttonId)

    {
        for (int i = 0; i < weaponButtons.Count; i++)
        {
            if (i == buttonId)
            {
                weaponButtons[i].GetComponent<Image>().sprite = SelectedSprite;
            }
            else
            {
                weaponButtons[i].GetComponent<Image>().sprite = DefaultSprite;

            }
        }
    }



    public void SetCurrentWeaponSelected()
    {
        Weapon weapon = instantiatedWeapons[m_CurrentWeaponDisplayedId - 1];
        PlayerData.PlayerProfile.CurrentSelectedWeapon = weapon.WeaponId;
        WeaponEquipButton.GetComponentInChildren<Text>().text = "Equipped";

       //save data
       PlayerData.SavePlayerData();
    }
    void DisableOtherObjects(List<GameObject> weapons ,int id)
    {

        for (int i = 0; i < DataManager.Instance.Weapons.Length; i++)
        {

            if (i == id)
            {
                weapons[i].SetActive(true);
                continue;
            }
            weapons[i].SetActive(false);

        }

    }

    void ResetPositionOfOtherObjects(List<GameObject> weapons , int id )
    {
        for (int i = 0; i < DataManager.Instance.Weapons.Length; i++)
        {

            if (i == id)
            {
                continue;
            }
            weapons[i].transform.position = new Vector3(10, 0, 0);

        }
    }
    IEnumerator UpdateTextValueSlowly(Text text , float toValue,bool animate )
    {
        float currentValue = float.Parse(text.text);
        float changeValue = Mathf.Abs(toValue - currentValue);

        if (animate && changeValue > 1)
        {
            while (currentValue < toValue)
            {
                if (currentValue < toValue)
                {
                    currentValue++;
                    text.text = currentValue.ToString();
                    yield return waitTime;
                }
                else
                {
                    currentValue--;
                    text.text = currentValue.ToString();
                    yield return waitTime;
                }
            }
        }
        else
        {
            text.text = toValue.ToString();
        }
        
    }
    IEnumerator UpdateTextValueSlowly(Text text, int toValue, bool animate,float inTime)
    {
        int currentValue = int.Parse(text.text);
        int changeValue =(int) (Mathf.Abs(toValue-currentValue) / inTime);
        if (animate && changeValue > 1)
        {
            while (currentValue < toValue)
            {
               
                    currentValue+=changeValue;
                    text.text = currentValue.ToString();
                    yield return waitTime;
            }
            while (currentValue > toValue )
            {
                currentValue -= changeValue;
                text.text = currentValue.ToString();
                yield return waitTime;
            }
           
            text.text = toValue.ToString();
        }
        else
        {
            text.text = toValue.ToString();
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetRotating(bool rotate)
    {
        m_CanRotate = rotate;
        //set object rotaing parameter of current displayed wepaon
        instantiatedWeaponModels[m_CurrentWeaponDisplayedId - 1].GetComponent<ObjectRotator>().IsRotating=m_CanRotate;

    }


}

