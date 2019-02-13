using System;
using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowCharacters : MonoBehaviour
{
    public Vector3 FirstAgentShowPosition=new Vector3(5, -52, 50);
    public Vector3 AgentShowAngle= new Vector3(0f, 200f, 0f);
    public Vector3 AgentLocalScale = new Vector3(40, 40, 40);
    public float DistanceBetweenShowAgents = 120;
    private int m_CurrentDisplayingPlayerId;
    public GameObject UnlockCharacterPanel;
    public GameObject SelectCharacterPanel;
    public Sprite CoinSprite;
    public Sprite DiamondSprite;
    public Sprite CoinButton;
    public Sprite DiamondButton;
    public Text PlayernameText;
    public Text PlayerDescriptionText;
    private CharacterScroller m_Hs;
    public RectTransform CharacterPanel;
    private ShowOutfits m_ShowOutfits;
    private bool m_CanRotate;

    private void Awake()
    {
        m_Hs = GetComponent<CharacterScroller>();

    }


    // Use this for initialization
    void Start ()
	{

	    m_Hs =GetComponent<CharacterScroller>();
	    m_ShowOutfits = GetComponent<ShowOutfits>();
        int i = 0;
        //instantiate all characters 
        foreach (var player in DataManager.Instance.Players)
        {
            Player instance = Instantiate(player) as Player;
            Animator animator = instance.GetComponent<Animator>();
            if (animator.runtimeAnimatorController == null)
            {
                animator.runtimeAnimatorController = player.PlayerAnimatorController;
            }
            instance.transform.position = FirstAgentShowPosition + new Vector3(DistanceBetweenShowAgents * i, 0, 0);
            InputControl inputControl = instance.GetComponent<InputControl>();
            Destroy(inputControl);
            instance.transform.eulerAngles =AgentShowAngle;
            instance.transform.localScale = AgentLocalScale;
            instance.transform.SetParent(CharacterPanel, true);
            if (m_ShowOutfits)
            {
                m_ShowOutfits.instantiatePlayers.Add(instance);
            }
           
          //  instantiatePlayers.Add(instance);
            i++;

        }
        //display character details
        DisplayCurrentCharacterDetails();
        //show outfits
        m_ShowOutfits.ShowPlayerOutfits();
	   // UpdateUI();
	}

    public void DisplayCurrentCharacterDetails()
    {
        m_CurrentDisplayingPlayerId = m_Hs.GetCurrentPositionSelected();
        Player player = GetCurrentPlayerDisplayed();
        //need to change according to playerprofile
        List<int> unlockedPlayers = PlayerData.PlayerProfile.UnlockedPlayerList;

        PlayernameText.text = player.PlayerName;
        PlayerDescriptionText.text = player.PlayerDescription;
        if (unlockedPlayers.Contains(player.PlayerID))
        {
           int currentSelectedPlayer=  PlayerData.PlayerProfile.CurrentSelectedPlayer;
            UnlockCharacterPanel.gameObject.SetActive(false);
            SelectCharacterPanel.gameObject.SetActive(true);
            SelectCharacterPanel.GetComponentInChildren<Text>().text = currentSelectedPlayer == player.PlayerID ? "Selected" : "Select";
        }
        else
        {

            Button unlockButton = UnlockCharacterPanel.GetComponentInChildren<Button>();
            //assign cost
            unlockButton.GetComponentInChildren<Text>().text = player.PlayerCost.ToString();
            Image[] images = unlockButton.GetComponentsInChildren<Image>();
            if (player.PlayerLockType == LockType.Coins)
            {
                images[0].sprite = CoinButton;
                images[1].sprite = CoinSprite;
            }
            else
            {
                images[0].sprite = DiamondButton;
                images[1].sprite = DiamondSprite;
            }

            UnlockCharacterPanel.gameObject.SetActive(true);
            SelectCharacterPanel.gameObject.SetActive(false);
        }
    }

    public void BuyCharacter()
    {
        List<int> unlockedPlayers = PlayerData.PlayerProfile.UnlockedPlayerList;
        GameObject mCanvas = GameObject.Find("Canvas");
        //load text components
        //Text coins=GameObject.FindGameObjectWithTag("Coins").GetComponent<Text>();
        //Text diamonds=GameObject.FindGameObjectWithTag("Diamonds").GetComponent<Text>();
        var transactionImage = Util.FindGameObjectWithName(mCanvas, "TransactionFailedImage");

        Player player = GetCurrentPlayerDisplayed();
        LockType playerLockType = player.PlayerLockType;
        var currentLevel = PlayerData.PlayerProfile.CurrentLevel;
        if (currentLevel < player.LevelRequiredToUnlock)
        {
            transactionImage.gameObject.SetActive(true);
            var texts = transactionImage.GetComponentsInChildren<Text>();
            texts[0].text = "Reach Level " + player.LevelRequiredToUnlock + " to unlock the agent"; //message
            texts[1].text = "Level " + player.LevelRequiredToUnlock + " Required"; //message
            return;
        }
        if (!unlockedPlayers.Contains(player.PlayerID))
        {
            switch (playerLockType)
            {
                case LockType.Coins:
                    int nofCoinsAvaliable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                    if (nofCoinsAvaliable < player.PlayerCost)
                    {
                        //open dialog to buy
                        transactionImage.gameObject.SetActive(true);
                        var texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (player.PlayerCost - nofCoinsAvaliable) + " more coins to buy the outfit"; //message
                        texts[1].text = "Not enough coins"; //message
                        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 1;
                       // GameManager.Instance.LoadLevel(4);
                    }
                    else
                    {
                        nofCoinsAvaliable -= player.PlayerCost;
                        unlockedPlayers.Add(player.PlayerID);
                        PlayerData.PlayerProfile.NoofCoinsAvailable = nofCoinsAvaliable;
                        PlayerData.PlayerProfile.UnlockedPlayerList = unlockedPlayers;
                        //set text
                        UiManager.Instance.UpdateCoins(nofCoinsAvaliable);
                       // CoinsAvailable.text = nofCoinsAvaliable.ToString();
                        UnlockCharacterPanel.gameObject.SetActive(false);
                        SelectCharacterPanel.gameObject.SetActive(true);
                        SelectCharacterPanel.GetComponentInChildren<Text>().text = "Select";

                        m_ShowOutfits.ShowPlayerOutfits();

                        //track appsflyerrich event
                        Dictionary<string,string> events=new Dictionary<string, string>();
                        events.Add(AFInAppEvents.CONTENT_TYPE,"agent");
                        events.Add(AFInAppEvents.CONTENT_ID,player.PlayerID.ToString());
                        events.Add(AFInAppEvents.CONTENT_TITLE, player.PlayerName);
                        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.BOUGHT_AGENT,events);

                        Parameter[] virtualcurrencyparameters =
                        {
                            new Parameter(FirebaseAnalytics.ParameterItemName, player.PlayerName),
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Coins)),
                            new Parameter(FirebaseAnalytics.ParameterValue,player.PlayerCost),
                            new Parameter(FirebaseAnalytics.ParameterContentType, "agent"),
                        };

                        FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventSpendVirtualCurrency, virtualcurrencyparameters);
                    }
                  
                    break;

                case LockType.Diamonds:
                    int noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                    if (noofDiamondsAvailable < player.PlayerCost)
                    {
                        //open dialog to buy
                        transactionImage.gameObject.SetActive(true);
                        var texts = transactionImage.GetComponentsInChildren<Text>();
                        texts[0].text = "Requires " + (player.PlayerCost - noofDiamondsAvailable) + " more diamonds to buy the outfit"; //message
                        texts[1].text = "Not enough diamonds"; //message
                        Debug.Log("not enough diamonds availabe");
                       PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 2;
                      //  GameManager.Instance.LoadLevel(4);  //5 for store
                    }
                    else
                    {
                        noofDiamondsAvailable -= player.PlayerCost;
                        unlockedPlayers.Add(player.PlayerID);
                        PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;
                        PlayerData.PlayerProfile.UnlockedPlayerList = unlockedPlayers;


                        //set text
                        UiManager.Instance.UpdateDiamonds(noofDiamondsAvailable);
                       // DiamondsAvailable.text = noofDiamondsAvailable.ToString();
                        UnlockCharacterPanel.gameObject.SetActive(false);

                        SelectCharacterPanel.gameObject.SetActive(true);
                        SelectCharacterPanel.GetComponentInChildren<Text>().text = "Select";
                        m_ShowOutfits.ShowPlayerOutfits();
                        //track appsflyerrich event
                        Dictionary<string, string> events = new Dictionary<string, string>();
                        events.Add(AFInAppEvents.CONTENT_TYPE, "agent");
                        events.Add(AFInAppEvents.CONTENT_ID, player.PlayerID.ToString());
                        events.Add(AFInAppEvents.CONTENT_TITLE, player.PlayerName);
                        AppsFlyerStartUp.Instance.TrackRichEvent(AFInAppEvents.BOUGHT_AGENT, events);

                        //log firebase events
                        Parameter[] virtualcurrencyparameters =
                        {
                            new Parameter(FirebaseAnalytics.ParameterItemName, player.PlayerName),
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Diamonds)),
                            new Parameter(FirebaseAnalytics.ParameterValue,player.PlayerCost),
                            new Parameter(FirebaseAnalytics.ParameterContentType, "agent"),
                        };

                       FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventSpendVirtualCurrency,virtualcurrencyparameters);
                       
                    }
                    break;
            }
        }
        else
        {
         //   Debug.Log("player already bought");
        }
        //save to database
        PlayerData.SavePlayerData();

    }

    public void UpdatePlayerName(string playerName)
    {
        PlayernameText.text = playerName;
    }
    public void SetCurrentCharacterSelected()
    {
        Player player = GetCurrentPlayerDisplayed();
        PlayerData.PlayerProfile.CurrentSelectedPlayer = player.PlayerID;
        SelectCharacterPanel.GetComponentInChildren<Text>().text = "Selected";

        PlayerData.SavePlayerData();
    }
    //private void UpdateUI()
    //{
    //    CoinsAvailable.text = playerProfile.NoofCoinsAvailable.ToString();
    //    DiamondsAvailable.text = playerProfile.NoofDiamondsAvailable.ToString();

    //}
    private Player GetCurrentPlayerDisplayed()
    {
        return DataManager.Instance.Players[m_CurrentDisplayingPlayerId];
    }

}
