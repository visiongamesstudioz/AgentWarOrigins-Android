using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EndlessRunner;
public class LootCrate : MonoBehaviour {


    public int[] RandomCoinValues;
    private Button m_button;
    public Text NoofLootBoxes;
    void Start()
    {
        m_button = GetComponentInChildren<Button>();

        int tokensAvailable = PlayerData.PlayerProfile.NoofTokensAvailable;
        int noofAvailableLootBoxes = tokensAvailable / 10;
        NoofLootBoxes.text = noofAvailableLootBoxes.ToString();
        if (noofAvailableLootBoxes == 0)
        {
            m_button.interactable = false;
        }
    }


   
}
