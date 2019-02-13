using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpDisplay : MonoBehaviour {

    public GameObject LevelUpDisplayGo;
    public Text LevelUpText;
    public Text LevelReachedValue;
    public Text XpEarnedForLevel;
    public GameObject AvailableRewardPrefab;
    public GameObject RewardDisplayContent;
    public GameObject NowAvailable;
    public GameObject RewardsDisplayPanel;
    private AudioSource m_AudioSource;
    private List<GameObject> rewardSprites = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }
	
    public void DisplayLevelUp()
    {

        if (GameManager.LevelStarted < PlayerData.PlayerProfile.CurrentLevel && PlayerData.PlayerProfile.CurrentLevel <= DataManager.Instance.MaxLevels)
        {
            GameManager.LevelStarted++;
            LevelUpDisplayGo.SetActive(true);
            StartCoroutine(UiManager.Instance.ScaleUiGameObject(LevelUpText.rectTransform, new Vector3(1, 1, 1), true,
                delegate
                {
                    LevelReachedValue.text = "Reached Level " + GameManager.LevelStarted;
                    LevelReachedValue.gameObject.SetActive(true);
                    //    AudioManager.Instance.PlaySound(m_AudioSource);
                    Level currentLevelReached = DataManager.Instance.GetLevel(GameManager.LevelStarted);
                    //foreach (var rewardImageSprite in currentLevelReached.RewardsSprite)
                    //{
                    //    if (currentLevelReached.RewardsSprite.Length > 0)
                    //    {
                    //        NowAvailable.SetActive(true);
                    //        RewardsDisplayPanel.SetActive(true);
                    //        GameObject reward = Instantiate(AvailableRewardPrefab) as GameObject;
                    //        reward.transform.SetParent(RewardDisplayContent.transform);
                    //        reward.transform.localScale = new Vector3(1, 1, 1);
                    //        RectTransform rectTransform = reward.GetComponent<RectTransform>();
                    //        rectTransform.localPosition = new Vector3(rectTransform.localPosition.z, rectTransform.localPosition.y, 0);
                    //        Image rewardImage = reward.GetComponentInChildren<Image>();
                    //        rewardImage.sprite = rewardImageSprite;
                    //        rewardSprites.Add(reward);
                    //    }

                    //}
                    if (currentLevelReached.RewardsList.Count > 0)
                    {
                        NowAvailable.SetActive(true);
                        RewardsDisplayPanel.SetActive(true);

                        foreach (Reward reward in currentLevelReached.RewardsList)
                        {

                            GameObject rewardgo = Instantiate(AvailableRewardPrefab) as GameObject;
                            rewardgo.transform.SetParent(RewardDisplayContent.transform,true);
                            rewardgo.transform.localScale = new Vector3(1, 1, 1);
                            RectTransform rectTransform = rewardgo.GetComponent<RectTransform>();
                            Text text = rewardgo.GetComponentInChildren<Text>();
                            text.text = reward.RewardAmountOrId.ToString();
                            rectTransform.localPosition = new Vector3(rectTransform.localPosition.z, rectTransform.localPosition.y, 0);
                            rectTransform.localEulerAngles= Vector3.zero;
                            Image rewardImage = rewardgo.GetComponentsInChildren<Image>()[1];
                            rewardImage.sprite = reward.RewardSprite;
                            rewardSprites.Add(rewardgo);

                            switch (reward.RewardType)
                            {
                                case Rewardtype.Xp:

                                    PlayerData.PlayerProfile.PlayerXp += reward.RewardAmountOrId;
                                    PlayerData.PlayerProfile.CurrentLevelXp += reward.RewardAmountOrId;
                                    break;
                                case Rewardtype.Coins:
                                    int noofCoinsAvailable = PlayerData.PlayerProfile.NoofCoinsAvailable;
                                    noofCoinsAvailable += reward.RewardAmountOrId;
                                    PlayerData.PlayerProfile.NoofCoinsAvailable = noofCoinsAvailable;
                                    UiManager.Instance.UpdateCoins(noofCoinsAvailable);
                                    break;
                                case Rewardtype.Diamonds:
                                    int noofDiamondsAvailable = PlayerData.PlayerProfile.NoofDiamondsAvailable;
                                    noofDiamondsAvailable += reward.RewardAmountOrId;
                                    PlayerData.PlayerProfile.NoofCoinsAvailable = noofDiamondsAvailable;
                                    UiManager.Instance.UpdateDiamonds(noofDiamondsAvailable);
                                    break;
                                default:
                                    if (text)
                                    {
                                        text.gameObject.SetActive(false);
                                    }
                                    HorizontalLayoutGroup horizontalLayoutGroup =
                                        rewardgo.GetComponent<HorizontalLayoutGroup>();
                                    if (horizontalLayoutGroup)
                                    {
                                        horizontalLayoutGroup.childControlWidth = true;

                                    }

                                    break;

                            }
                        }
                    }
                  
                }));

            //log firebase level up event
            Parameter[] levelUpParameter =
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, GameManager.LevelStarted)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelUp,levelUpParameter);

        }
        PlayerData.SavePlayerData();
    }
    public void ResetLevelUpTextScale()
    {
        UiManager.Instance.ResetScaleUi(LevelUpText.rectTransform);
        foreach (var rewardgo in rewardSprites)
        {
            Destroy(rewardgo);

        }
        rewardSprites.Clear();
        LevelUpDisplayGo.SetActive(false);
        LevelReachedValue.gameObject.SetActive(false);
        NowAvailable.SetActive(false);
        RewardsDisplayPanel.SetActive(false);
    }


    public void OnContinueClicked()
    {
        if (GameManager.LevelStarted < PlayerData.PlayerProfile.CurrentLevel)
        {
            DisplayLevelUp();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
