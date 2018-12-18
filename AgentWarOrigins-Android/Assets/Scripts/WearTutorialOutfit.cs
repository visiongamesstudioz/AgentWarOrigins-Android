using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearTutorialOutfit : MonoBehaviour {

    private Player m_Player;

    private void Awake()
    {
        m_Player = GetComponent<Player>();
    }
    // Use this for initialization
    void Start () {
        OutfitSkins selectedOutfitWithSkins = m_Player.PlayerAvailableOutFitsWithSkins[0];
        Outfit selectedSkinOutfit = selectedOutfitWithSkins.AvailableSkinsPerOutfit[0];
        var outfit = selectedSkinOutfit.gameObject;
        selectedSkinOutfit.Initialize(outfit, m_Player);
        selectedSkinOutfit.ChangeClothes();
       // Util.SetLayerRecursively(m_Player.gameObject, LayerMask.NameToLayer("Player"));

    }
	
}
