using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using EndlessRunner;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BluePrintGiftBox : MonoBehaviour
{
    public GameObject GiftboxPrefab;
    public int GiftBoxCost;
    public int RewardCoinsCount;
    public int RewardDiamondsCount;
    public Sprite CoinsSprite;
    public Sprite DiamondsSprite;
    private List<BluePrintItem> bluePrintItems;
    private Canvas _giftBoxCanvas;
    private GameObject _bluePrintHolder;
    private GameObject _bluePrintReceived;
    private GameObject _bluePrintName;
    private GameObject _bluePrintImage;
    private GameObject _transactionImage;
    private Canvas mainCanvas;
    void Start()
    {
        mainCanvas = GameObject.Find("Canvas").GetComponentInChildren<Canvas>();
        bluePrintItems=new List<BluePrintItem>();
        bluePrintItems = DataManager.Instance.BluePrintItems;
        _giftBoxCanvas = GetComponentInChildren<Canvas>(true);
        _bluePrintHolder = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BlueprintHolder");
        _bluePrintReceived = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BluePrintReceived");
        _bluePrintName = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BlueprintName");
        _bluePrintImage = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BlueprintImage");
        _transactionImage = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject ,"TransactionFailedImage");
    }

    public void BuyGiftBox()
    {
        int noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
        if (noofDiamondsAvailable < GiftBoxCost)
        {
            _giftBoxCanvas.gameObject.SetActive(true);
            _bluePrintHolder.SetActive(false);
          //  _giftBoxCanvas.renderMode=RenderMode.ScreenSpaceCamera;
            mainCanvas.renderMode=RenderMode.ScreenSpaceOverlay;
           mainCanvas.sortingOrder = -1;
            //no enough diamonds available
            _transactionImage.SetActive(true);
            var texts = _transactionImage.GetComponentsInChildren<Text>();
            texts[0].text = "Requires " + (GiftBoxCost - noofDiamondsAvailable) +
                            " more diamonds to buy a gift box";
            //message
            texts[1].text = "Not enough Diamonds"; //message
            return;
        }
        else
        {
            //
            noofDiamondsAvailable -= GiftBoxCost;
            PlayerData.PlayerProfile.NoofDiamondsAvailable = noofDiamondsAvailable;
            UiManager.Instance.UpdateUi();
            PlayerData.SavePlayerData();
            OpenGiftBox();
        }
    }
    public void OpenGiftBox()
    {

        StartCoroutine(OpenGiftBoxCoroutine());
    }

    IEnumerator OpenGiftBoxCoroutine()
    {
        //instantiate the giftbox prefab

        GameObject giftbox = Instantiate(GiftboxPrefab) as GameObject;
        giftbox.SetActive(true);
        yield return new WaitForSeconds(1);
        giftbox.GetComponentsInChildren<Animator>()[1].enabled = true;
        float time=0;
        foreach (var clip in giftbox.GetComponent<Animator>().runtimeAnimatorController.animationClips)
        {
            time += clip.length;
        }
        yield return new WaitForSeconds(time);
        //show Image of unlocked outfit
        _giftBoxCanvas.gameObject.SetActive(true);
        _bluePrintHolder.gameObject.SetActive(true);
        _giftBoxCanvas.GetComponentInChildren<Button>(true).onClick.AddListener(() => RemoveGiftBox(giftbox));
        //call tyhe function
        RewardRandomBlueprint();
        yield return new WaitForEndOfFrame();
    }

    void RemoveGiftBox(GameObject go)
    {
       Destroy(go);
    }
    void RewardRandomBlueprint()
    {
        _bluePrintReceived = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BluePrintReceived");
        _bluePrintName = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BlueprintName");
        _bluePrintImage = Util.FindGameObjectWithName(_giftBoxCanvas.gameObject, "BlueprintImage");
        //no of items for gift box
        float itemWeights = 0;
        for (int i = 0; i < bluePrintItems.Count; i++)
        {
            itemWeights += bluePrintItems[i].DropChance;
        }
        float randomValue = Random.Range(0, itemWeights);
        for (int i = 0; i < bluePrintItems.Count; i++)
        {
            if (randomValue <= bluePrintItems[i].DropChance)
            {
                //we found a outfit blueprint
                if (bluePrintItems[i].ItemType == BluePrintItemType.OutfitSkins)
                {
                    //get unlocked blueprints for outfit
                    Dictionary<int, Dictionary<int, int>> noofBluePrintsUnlockedPerOutfitPerPlayer = PlayerData.PlayerProfile.UnlockedBlueprintsPeroutfit;
                    if (noofBluePrintsUnlockedPerOutfitPerPlayer == null)
                        return;
                    if (!noofBluePrintsUnlockedPerOutfitPerPlayer.ContainsKey(bluePrintItems[i].PlayerId))
                        noofBluePrintsUnlockedPerOutfitPerPlayer.Add(bluePrintItems[i].PlayerId, new Dictionary<int, int>());

                    Dictionary<int, int> noOfBluePrintsUnlockedPerOutfitDict = noofBluePrintsUnlockedPerOutfitPerPlayer[bluePrintItems[i].PlayerId];
                    if (!noOfBluePrintsUnlockedPerOutfitDict.ContainsKey(bluePrintItems[i].ItemId))
                        noOfBluePrintsUnlockedPerOutfitDict.Add(bluePrintItems[i].ItemId, 0);
                    //get outfit from outfit ID for specific player;
                    OutfitSkins outfit =
                        DataManager.Instance.Players[bluePrintItems[i].PlayerId - 1].PlayerAvailableOutFitsWithSkins[
                            bluePrintItems[i].ItemId - 1];
                    
                    int value = Random.Range(0, outfit.AvailableSkinsPerOutfit[0].NoofBluePrints + 1); //+ 1 to include last blueprint

                    if (value < noOfBluePrintsUnlockedPerOutfitDict[bluePrintItems[i].ItemId])
                    {
                        //already unlocked the blueprint
                        //convert unlocked outfit to coins or diamonds;
                        int randomCoinsOrDiamonds = Random.Range(0, 2);

                        switch (randomCoinsOrDiamonds)
                        {
                            case 0:
                                int noOfCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                                noOfCoinsAvailable += RewardCoinsCount;
                                PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;
                                _bluePrintReceived.GetComponentInChildren<Text>().text="Coins Received";
                                _bluePrintName.GetComponentInChildren<Text>().text = RewardCoinsCount+" Coins";
                                _bluePrintImage.GetComponentInChildren<Image>().sprite = CoinsSprite;
                                break;
                            case 1:
                                int noOfDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                                noOfDiamondsAvailable += RewardDiamondsCount;
                                PlayerData.PlayerProfile.NoofDiamondsAvailable = noOfDiamondsAvailable;
                                _bluePrintReceived.GetComponentInChildren<Text>().text = "Diamonds Received";
                                _bluePrintName.GetComponentInChildren<Text>().text = RewardDiamondsCount +" Diamonds";
                                _bluePrintImage.GetComponentInChildren<Image>().sprite = DiamondsSprite;
                                break;
                        }
                    }
                    else
                    {
                        //doesnot contain the blueprint
                        int noOfBlueprintsUnlockedPerOutfit = noOfBluePrintsUnlockedPerOutfitDict[bluePrintItems[i].ItemId] += 1;
                        if (noOfBluePrintsUnlockedPerOutfitDict[bluePrintItems[i].ItemId] > outfit.AvailableSkinsPerOutfit[0].NoofBluePrints)
                        {
                            //can add blue prints for skins also
                            noOfBluePrintsUnlockedPerOutfitDict[bluePrintItems[i].ItemId] = outfit.AvailableSkinsPerOutfit[0].NoofBluePrints;
                            //convert blueprint to coins or diamonds
                            int randomCoinsOrDiamonds = Random.Range(0, 2);

                            switch (randomCoinsOrDiamonds)
                            {
                                case 0:
                                    int noOfCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                                    noOfCoinsAvailable += RewardCoinsCount;
                                    PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;
                                    _bluePrintReceived.GetComponentInChildren<Text>().text = "Coins Received";
                                    _bluePrintName.GetComponentInChildren<Text>().text = RewardCoinsCount + " Coins";
                                    _bluePrintImage.GetComponentInChildren<Image>().sprite = CoinsSprite;
                                    break;
                                case 1:
                                    int noOfDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                                    noOfDiamondsAvailable += RewardDiamondsCount;
                                    PlayerData.PlayerProfile.NoofDiamondsAvailable = noOfDiamondsAvailable;

                                    _bluePrintReceived.GetComponentInChildren<Text>().text = "Diamonds Received";
                                    _bluePrintName.GetComponentInChildren<Text>().text = RewardDiamondsCount + " Diamonds";
                                    _bluePrintImage.GetComponentInChildren<Image>().sprite = DiamondsSprite;
                                    break;
                            }
                        }
                        else
                        {
                            //show show unlocked with animation
                            _bluePrintReceived.GetComponentInChildren<Text>().text = "BluePrint Received";
                            _bluePrintName.GetComponentInChildren<Text>().text = outfit.AvailableSkinsPerOutfit[0].OutfitName + "(" + noOfBlueprintsUnlockedPerOutfit + " OF " + outfit.AvailableSkinsPerOutfit[0].NoofBluePrints + ")";
                            _bluePrintImage.GetComponentInChildren<Image>().sprite = bluePrintItems[i].BluePrintImage;
                        }
                    }
                    PlayerData.PlayerProfile.UnlockedBlueprintsPeroutfit = noofBluePrintsUnlockedPerOutfitPerPlayer;
                }
                //we received a blueprint of weapon 
                else if (bluePrintItems[i].ItemType == BluePrintItemType.Weapon)
                {
                    Dictionary<int, int> noofBluePrintsUnlockedPerWeaponDictionary = PlayerData.PlayerProfile.NoOfBluePrintsUnlockedPerWeapon;
                    if (noofBluePrintsUnlockedPerWeaponDictionary == null)
                        return;
                    if (!noofBluePrintsUnlockedPerWeaponDictionary.ContainsKey(bluePrintItems[i].ItemId))
                        noofBluePrintsUnlockedPerWeaponDictionary.Add(bluePrintItems[i].ItemId, 0);
                    int noOfBluePrintsUnlockedPerWeapon = noofBluePrintsUnlockedPerWeaponDictionary[bluePrintItems[i].ItemId];
                    Weapon weapon = DataManager.Instance.Weapons[bluePrintItems[i].ItemId - 1];

                    var value = Random.Range(0,
                        weapon.NoofBluePrints + 1);
                    if (value < noOfBluePrintsUnlockedPerWeapon)
                    {
                        //already unlocked the blueprint of that weapon or required level not reached
                        int randomCoinsOrDiamonds = Random.Range(0, 2);

                        switch (randomCoinsOrDiamonds)
                        {
                            case 0:
                                int noOfCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                                noOfCoinsAvailable += RewardCoinsCount;
                                PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;
                                _bluePrintReceived.GetComponentInChildren<Text>().text = "Coins Received";
                                _bluePrintName.GetComponentInChildren<Text>().text = RewardCoinsCount + " Coins";
                                _bluePrintImage.GetComponentInChildren<Image>().sprite = CoinsSprite;
                                break;
                            case 1:
                                int noOfDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                                noOfDiamondsAvailable += RewardDiamondsCount;
                                PlayerData.PlayerProfile.NoofDiamondsAvailable = noOfDiamondsAvailable;

                                _bluePrintReceived.GetComponentInChildren<Text>().text = "Diamonds Received";
                                _bluePrintName.GetComponentInChildren<Text>().text = RewardDiamondsCount + " Diamonds";
                                _bluePrintImage.GetComponentInChildren<Image>().sprite = DiamondsSprite;
                                break;
                        }
                    }
                    else
                    {
                        noOfBluePrintsUnlockedPerWeapon=noofBluePrintsUnlockedPerWeaponDictionary[bluePrintItems[i].ItemId] += 1;
                        //already received all blueprints
                        if (noofBluePrintsUnlockedPerWeaponDictionary[bluePrintItems[i].ItemId] > weapon.NoofBluePrints)
                        {
                            noofBluePrintsUnlockedPerWeaponDictionary[bluePrintItems[i].ItemId] = weapon.NoofBluePrints;
                            //convert blueprint to coins or diamonds
                            int randomCoinsOrDiamonds = Random.Range(0, 2);

                            switch (randomCoinsOrDiamonds)
                            {
                                case 0:
                                    int noOfCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                                    noOfCoinsAvailable += RewardCoinsCount;
                                    PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;
                                    _bluePrintReceived.GetComponentInChildren<Text>().text = "Coins Received";
                                    _bluePrintName.GetComponentInChildren<Text>().text = RewardCoinsCount + " Coins";
                                    _bluePrintImage.GetComponentInChildren<Image>().sprite = CoinsSprite;
                                    break;
                                case 1:
                                    int noOfDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                                    noOfDiamondsAvailable += RewardDiamondsCount;
                                    PlayerData.PlayerProfile.NoofDiamondsAvailable = noOfDiamondsAvailable;

                                    _bluePrintReceived.GetComponentInChildren<Text>().text = "Diamonds Received";
                                    _bluePrintName.GetComponentInChildren<Text>().text = RewardDiamondsCount + " Diamonds";
                                    _bluePrintImage.GetComponentInChildren<Image>().sprite = DiamondsSprite;
                                    break;
                            }

                        }
                        else
                        {
                            //show the dialog
                            //show show unlocked with animation
                            _bluePrintReceived.GetComponentInChildren<Text>().text = "BluePrint Received";
                            _bluePrintName.GetComponentInChildren<Text>().text = weapon.WeaponName + "(" + noOfBluePrintsUnlockedPerWeapon + " OF" + weapon.NoofBluePrints+" )";
                            _bluePrintImage.GetComponentInChildren<Image>().sprite = bluePrintItems[i].BluePrintImage;
                        }
              
                    }
                    PlayerData.PlayerProfile.NoOfBluePrintsUnlockedPerWeapon = noofBluePrintsUnlockedPerWeaponDictionary;

                }
                else if(bluePrintItems[i].ItemType==BluePrintItemType.Coins)
                {
                    int noOfCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                    noOfCoinsAvailable += bluePrintItems[i].Coins;
                    PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;

                    _bluePrintReceived.GetComponentInChildren<Text>().text = "Coins Received";
                    _bluePrintName.GetComponentInChildren<Text>().text = bluePrintItems[i].Coins + " Coins";
                    _bluePrintImage.GetComponentInChildren<Image>().sprite = bluePrintItems[i].BluePrintImage;
                }
                else if (bluePrintItems[i].ItemType == BluePrintItemType.Diamonds)
                {
                    int noOfDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                    noOfDiamondsAvailable += RewardDiamondsCount;
                    PlayerData.PlayerProfile.NoofDiamondsAvailable = noOfDiamondsAvailable;

                    _bluePrintReceived.GetComponentInChildren<Text>().text = "Diamonds Received";
                    _bluePrintName.GetComponentInChildren<Text>().text = bluePrintItems[i].Diamonds + " Diamonds";
                    _bluePrintImage.GetComponentInChildren<Image>().sprite = bluePrintItems[i].BluePrintImage;
                }
                //update ui
                UiManager.Instance.UpdateUi();
                //save data to file
                PlayerData.SavePlayerData();
                return;
            }
            randomValue -= bluePrintItems[i].DropChance;

        }
      
    }
}

public enum BluePrintItemType
{
    OutfitSkins,
    Weapon,
    Coins,
    Diamonds
}

[Serializable]
public class BluePrintItem
{
    public BluePrintItemType ItemType;
    public int ItemId; //to which id the the blueprint should be added
                       //  [HideInInspector]
    public int PlayerId; //if the item type is outfit
    public int Coins;
    public int Diamonds;
    public Sprite BluePrintImage;
    public float DropChance;
}
