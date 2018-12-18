using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;
using UnityEngine.UI;

public class LootCrates : MonoBehaviour {

    public int[] RandomCoinValues;
    public Text NoofLootBoxes;
    public Sprite CoinsSprite;
    public GameObject LootCratePrefab;
    public Canvas GiftBoxCanvas;

    public Button LootcrateButton;
    private GameObject _lootCratetHolder;
    private GameObject _lootName;
    private GameObject _lootImage;
    private int _noofAvailableLootBoxes;
    void Start()
    {        
        _lootCratetHolder = Util.FindGameObjectWithName(GiftBoxCanvas.gameObject, "LootCrateHolder");
        _lootName = Util.FindGameObjectWithName(GiftBoxCanvas.gameObject, "LootName");
        _lootImage = Util.FindGameObjectWithName(GiftBoxCanvas.gameObject, "LootImage");

        int tokensAvailable = PlayerData.PlayerProfile.NoofTokensAvailable;
        _noofAvailableLootBoxes = tokensAvailable / 100;
        NoofLootBoxes.text = _noofAvailableLootBoxes.ToString();
        if (_noofAvailableLootBoxes == 0)
        {
            LootcrateButton.interactable = false;
        }
    }

    public void OpenLootCrate()
    {
        int tokensAvailable = PlayerData.PlayerProfile.NoofTokensAvailable;
        _noofAvailableLootBoxes = tokensAvailable / 100;
        if (_noofAvailableLootBoxes > 0)
        {
            StartCoroutine(OpenLootCrateCoroutine());
        }


    }

    private IEnumerator OpenLootCrateCoroutine()
    {
        GameObject lootcrate = Instantiate(LootCratePrefab) as GameObject;
        lootcrate.SetActive(true);
        yield return new WaitForSeconds(1);
        lootcrate.GetComponentsInChildren<Animator>()[1].enabled = true;
        float time = 0;
        foreach (var clip in lootcrate.GetComponent<Animator>().runtimeAnimatorController.animationClips)
        {
            time += clip.length;
        }
        yield return new WaitForSeconds(time);
        //show Image of unlocked outfit
        GiftBoxCanvas.gameObject.SetActive(true);
        _lootCratetHolder.gameObject.SetActive(true);
        GiftBoxCanvas.GetComponentsInChildren<Button>(true)[1].onClick.AddListener(() => RemoveGiftBox(lootcrate));
        //call tyhe function
        ConvertTokensToCoins();
        yield return new WaitForEndOfFrame();
    }
    void RemoveGiftBox(GameObject go)
    {
        Destroy(go);
    }
    private void ConvertTokensToCoins()
    {
        int tokensAvailable = PlayerData.PlayerProfile.NoofTokensAvailable -= 100;
        _noofAvailableLootBoxes = tokensAvailable / 100;
        if (_noofAvailableLootBoxes > 0)
        {
            NoofLootBoxes.text = _noofAvailableLootBoxes.ToString();
        }
        else
        {
            LootcrateButton.interactable = false;
        }
        int random = Random.Range(0, RandomCoinValues.Length);
        //convert tokens to coins
        int convertedcoins = RandomCoinValues[random];
        int noOfCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
        noOfCoinsAvailable += convertedcoins;
        PlayerData.PlayerProfile.NoofCoinsAvailable = noOfCoinsAvailable;
        _lootName.GetComponentInChildren<Text>().text = convertedcoins + " Coins";
        _lootImage.GetComponentInChildren<Image>().sprite = CoinsSprite;
        //update ui
        UiManager.Instance.UpdateUi();

        //save data to file
        PlayerData.SavePlayerData();

    }

}
