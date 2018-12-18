using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowOutfits : MonoBehaviour
{
    public Button OutfitDisplayButton;
    public Sprite DefaultSprite;
    public Sprite SelectedSprite;
    //can add abilities later
    public GameObject InvisibleSkinContentHolder;

    public RectTransform InvisbleSkinsContent;
    public GameObject LessDamageSkinContentHolder;

    public RectTransform LessDamageSkinContent;
    public GameObject RegenSkinSkinContentHolder;

    public RectTransform RegenSkinContent;

    public RectTransform Panel;
    public RectTransform Canvas;
    private int m_CurrentDisplayingPlayerId;
    private CharacterScroller _mCharacterScroller;
    private int currentDisplayedoutfit;
    public List<Player> instantiatePlayers=new List<Player>();
    private List<Button> buttons=new List<Button>();

    private int noOfLessDamageSkins;
    private int noOfInvisibleSkins;
    private int noOfRegenSkins;

    private Player m_PreviousPlayerDisplayed;

    private readonly List<Mission> unlockOutfitMissions = new List<Mission>();
    private readonly List<Mission> unlockSkinMissions = new List<Mission>();
    private MissionManager m_MissionManager;

    void Start()
    {

        m_MissionManager=MissionManager.Instance;
        //   ShowPlayerOutfits();
        //check for mission completion

       
    }



    // Use this for initialization
    public void ShowPlayerOutfits ()
	{
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
        //remove effect to apply again
        if (m_PreviousPlayerDisplayed)
	    {
	        InvisibleEffect invisibleEffect = m_PreviousPlayerDisplayed.GetComponent<InvisibleEffect>();
	        if (invisibleEffect)
	        {
	            invisibleEffect.enabled = false;
	        }
	    }
        _mCharacterScroller = GetComponent<CharacterScroller>();
	    m_CurrentDisplayingPlayerId = _mCharacterScroller.GetCurrentPositionSelected();
	    Player instantiatePlayer = instantiatePlayers[m_CurrentDisplayingPlayerId];
	    m_PreviousPlayerDisplayed = instantiatePlayer;
	    List<OutfitSkins> playerOutfitList = instantiatePlayer.PlayerAvailableOutFitsWithSkins;
        //destroy previous buttons
        DestroyPreviosButtons();
        OutfitButtonsList.PreviousOutfitsButtons.Clear();
        //reset all skin numbers for different player
	    noOfLessDamageSkins = 0;
	    noOfInvisibleSkins = 0;
	    noOfRegenSkins = 0;
	    int outfitNo = 0;
        Dictionary<int, List<int>> listOfUnlockedOutfitsPerPlayerDic= PlayerData.PlayerProfile.UnlockedOutfitsPerPlayer;
	    Dictionary<int, Dictionary<int, List<int>>> unlockedSkinsPerOutfitPerPlayer =
	        PlayerData.PlayerProfile.UnlockedSkinsPerOutfit;
	    if (!listOfUnlockedOutfitsPerPlayerDic.ContainsKey(instantiatePlayer.PlayerID))
	    {
	        listOfUnlockedOutfitsPerPlayerDic.Add(instantiatePlayer.PlayerID,new  List<int>());
	    }
        List<int> unlockedOutiftPerPlayer = listOfUnlockedOutfitsPerPlayerDic[instantiatePlayer.PlayerID];
	    if (!unlockedSkinsPerOutfitPerPlayer.ContainsKey(instantiatePlayer.PlayerID))
	    {
	        unlockedSkinsPerOutfitPerPlayer.Add(instantiatePlayer.PlayerID,new Dictionary<int, List<int>>());
	    }
	    Dictionary<int, List<int>> unlockedListOfSkinsPerOutfitDic =
	        unlockedSkinsPerOutfitPerPlayer[instantiatePlayer.PlayerID];
        
        //wear select outfit with skin
        var wornOutfit =
            PlayerData.PlayerProfile.WornOutfitPerPlayer;
        if (!wornOutfit.ContainsKey(instantiatePlayer.PlayerID))
            wornOutfit.Add(instantiatePlayer.PlayerID, 1);
        var selectedOufitId = wornOutfit[instantiatePlayer.PlayerID];

	    if (selectedOufitId == 0)
	    {
	        selectedOufitId = 1;
	    }
        Dictionary<int, Dictionary<int, int>> wornSkinPerOutfitPerPlayer = PlayerData.PlayerProfile.WornSkinPerOutfitPerPlayer;
        if (wornSkinPerOutfitPerPlayer == null)
        {
            return;
        }
        if (!wornSkinPerOutfitPerPlayer.ContainsKey(instantiatePlayer.PlayerID))
        {
            wornSkinPerOutfitPerPlayer.Add(instantiatePlayer.PlayerID, new Dictionary<int, int>());
        }
        Dictionary<int, int> wornSkinPeroutfit = wornSkinPerOutfitPerPlayer[instantiatePlayer.PlayerID];
        if (wornSkinPeroutfit == null)
        {
            return;
        }
        if (!wornSkinPeroutfit.ContainsKey(selectedOufitId))
        {
            wornSkinPeroutfit.Add(selectedOufitId, -1);
        }
        int selectedOutfitSkinId = wornSkinPeroutfit[selectedOufitId];

        //wear selected outfit
        OutfitSkins selectedOutfitWithSkins = instantiatePlayer.PlayerAvailableOutFitsWithSkins[selectedOufitId - 1];
        Outfit selectedSkinOutfit;
        if (selectedOutfitSkinId <= 0)
        {
            selectedSkinOutfit = selectedOutfitWithSkins.AvailableSkinsPerOutfit[0];
        }
        else
        {
            selectedSkinOutfit = selectedOutfitWithSkins.AvailableSkinsPerOutfit[selectedOutfitSkinId];
        }


        WearOutfit(instantiatePlayer, selectedSkinOutfit);

        foreach (var outfitwithskins in playerOutfitList)
        {

            List<Outfit> skins = outfitwithskins.AvailableSkinsPerOutfit;

         

            foreach (var skin in skins)
            {
                Button outfitBtn = Instantiate(OutfitDisplayButton) as Button;
                switch (skin.OutfitAbility)
                {
                    case OutfitAbility.Invincible:
                        noOfInvisibleSkins++;
                        outfitBtn.transform.SetParent(InvisbleSkinsContent, false);

                        break;
                    case OutfitAbility.LowDamage:
                        noOfLessDamageSkins++;
                        outfitBtn.transform.SetParent(LessDamageSkinContent, false);

                        break;
                    case OutfitAbility.RegenerateHealth:
                        noOfRegenSkins++;
                        outfitBtn.transform.SetParent(RegenSkinContent, false);

                        break;
                    case OutfitAbility.Revive:
                        break;
                    default:
                        break;
                }             
         //       outfitBtn.transform.SetParent(Panel, false);
                outfitBtn.transform.localScale = new Vector3(1, 1, 1);
                OutfitButtonsList.PreviousOutfitsButtons.Add(outfitBtn.gameObject);
                //outfitBtn.GetComponentInChildren<Text>().text = outfit.OutfitID.ToString();	
                Image[] images = outfitBtn.GetComponentsInChildren<Image>(true);

                images[1].sprite = skin.OutfitImage;  //1 for outfit image sprite
                images[2].gameObject.SetActive(PlayerData.PlayerProfile.CurrentLevel < skin.LevelToUnlock);
                skin.Initialize(skin.gameObject, instantiatePlayer);
                outfitBtn.onClick.AddListener(skin.ChangeClothes);

                var no = outfitNo;
                outfitBtn.onClick.AddListener(() => ChangeSprite(no));
                outfitNo++;


            }
        }
        //enable and disable based on skins
	    InvisibleSkinContentHolder.SetActive(noOfInvisibleSkins > 0);
	    LessDamageSkinContentHolder.SetActive(noOfLessDamageSkins > 0);
	    RegenSkinSkinContentHolder.SetActive(noOfRegenSkins > 0);
	    //Dictionary<int, int> selectedOutfitPerPlayer = PlayerData.PlayerProfile.WornOutfitPerPlayer;
        //if (!selectedOutfitPerPlayer.ContainsKey(instantiatePlayer.PlayerID))
        //{
        //    selectedOutfitPerPlayer.Add(instantiatePlayer.PlayerID, 1);
        //}
        //int slectedOutfitPerPlayer = selectedOutfitPerPlayer[instantiatePlayer.PlayerID];
        //WearOutfit(instantiatePlayer, playerOutfitList[slectedOutfitPerPlayer - 1]);
        //int i = 0;
        //OutfitButtonsList.PreviousOutfitsButtons.Clear();
        //foreach (Outfit outfit in playerOutfitList)
        //{
        // Button outfitBtn= Instantiate(OutfitDisplayButton) as Button;
        //    outfitBtn.transform.SetParent(Panel, false);
        //    outfitBtn.transform.localScale = new Vector3(1, 1, 1);
        //    OutfitButtonsList.PreviousOutfitsButtons.Add(outfitBtn.gameObject);
        //    //outfitBtn.GetComponentInChildren<Text>().text = outfit.OutfitID.ToString();	
        //    Image[] images = outfitBtn.GetComponentsInChildren<Image>();

        // images[1].sprite = outfit.OutfitImage;  //1 for outfit image sprite
        //    images[2].gameObject.SetActive(PlayerData.PlayerProfile.CurrentLevel < outfit.LevelToUnlock);
        //    outfit.Initialize(outfit.gameObject,instantiatePlayer);
        //    outfitBtn.onClick.AddListener(outfit.ChangeClothes);
        //    var i1 = i;
        //    outfitBtn.onClick.AddListener(()=>ChangeSprite(i1));

        //    i++;
        //}
        //change selected outftt sprite
	    int changeSpriteButtonId = 0;
	    for (int i = 0; i < selectedOufitId-1; i++)
	    {
	        changeSpriteButtonId += instantiatePlayer.PlayerAvailableOutFitsWithSkins[i].AvailableSkinsPerOutfit.Count;
	    }
	    if (selectedOutfitSkinId != -1)
	    {
            changeSpriteButtonId += selectedOutfitSkinId;

        }

        ChangeSprite(changeSpriteButtonId); //change sprite of selected outfit per player

        //   WearDefaultOutfit(instantiatePlayer,playerOutfitList[0]);

        //check for mission completion
        if (unlockOutfitMissions.Count > 0)
        {
            foreach (var mission in unlockOutfitMissions)
            {
                if (mission.PlayerId != instantiatePlayer.PlayerID)
                {
                    continue;
                }      
                for (int i = 0; i < unlockedOutiftPerPlayer.Count; i++)
                {
                    if (mission.AmountOrObjectIdToComplete == unlockedOutiftPerPlayer[i])
                    {
                        EventManager.TriggerEvent(mission.MissionTitle);
                        break;
                    }
                }             
            }
        }
        if (unlockSkinMissions.Count > 0)
        {
            foreach (var mission in unlockSkinMissions)
            {

                if (mission.PlayerId != instantiatePlayer.PlayerID)
                {
                    continue;
                }
                foreach (int key in unlockedListOfSkinsPerOutfitDic.Keys)
                {
                    List<int> unlockedSkins = unlockedListOfSkinsPerOutfitDic[key];
                    if (mission.AmountOrObjectIdToComplete != key)
                    {

                        continue;
                    }
                    for (int i = 0; i < unlockedSkins.Count; i++)
                    {

                        if (mission.SkinId == unlockedSkins[i])
                        {

                            EventManager.TriggerEvent(mission.MissionTitle);
 
                        }
                    }

                }
            }
            
        }

    }


    private void ChangeSprite(int buttonId)

    {
        for (int i = 0; i < OutfitButtonsList.PreviousOutfitsButtons.Count; i++)
        {
            if (i == buttonId)
            {
                OutfitButtonsList.PreviousOutfitsButtons[i].GetComponent<Image>().sprite = SelectedSprite;
            }
            else
            {
                OutfitButtonsList.PreviousOutfitsButtons[i].GetComponent<Image>().sprite = DefaultSprite;

            }
        }
      
    }

    private Player GetCurrentPlayerDisplayed()
    {
        return DataManager.Instance.Players[m_CurrentDisplayingPlayerId];
    }

    private void DestroyPreviosButtons()
    {
        foreach (GameObject button in OutfitButtonsList.PreviousOutfitsButtons)
        {
            Destroy(button);
        }
    }
    public static class OutfitButtonsList
    {
        public static List<GameObject> PreviousOutfitsButtons = new List<GameObject>();
       
    }

   
    void WearOutfit(Player player, Outfit outfit)
    {
        outfit.Initialize(outfit.gameObject, player);
        outfit.ChangeClothes();
    }
}
