using System;
using EndlessRunner;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public class AdmobAdManager : MonoBehaviour
{
    public static AdmobAdManager Instance;

    public static VideoType VideoType;

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private static RewardBasedVideoAd rewardBasedVideo;
    private float deltaTime = 0.0f;
    private static string outputMessage = string.Empty;

    public static string OutputMessage
    {
        set { outputMessage = value; }
    }

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

    public void Start()
    {
#if UNITY_ANDROID
        var appId = "ca-app-pub-2778622632232885~9878228409"; //our app id
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#else
        string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        rewardBasedVideo = RewardBasedVideoAd.Instance;

        // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        //request interstial ad
        RequestInterstitial();
        RequestRewardBasedVideo();
    }

    // Returns an ad request with custom ad targeting.
    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("E44A610ADA06A03FDAD2A217C0D3C1E2").AddTestDevice("6D6D3F9451B06131AD11971DA852CEA2").AddTestDevice("A5EC844662368BFEF1D57320E78A05AA")
            .AddTestDevice("4DF7FB7F1C624A480524C2AB65AC85EE")
            .AddKeyword("game")
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }

    public void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        var adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-2778622632232885/5248131529";  //this is real id
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (bannerView != null)
            bannerView.Destroy();

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.TopRight);

        // Register for ad events.
        bannerView.OnAdLoaded += HandleAdLoaded;
        bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
        bannerView.OnAdOpening += HandleAdOpened;
        bannerView.OnAdClosed += HandleAdClosed;
        bannerView.OnAdLeavingApplication += HandleAdLeftApplication;

        // Load a banner ad.
        bannerView.LoadAd(CreateAdRequest());
    }

    public void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        var adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-2778622632232885/9860549553";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (interstitial != null)
            interstitial.Destroy();

        // Create an interstitial.
        interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        interstitial.OnAdLoaded += HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
        interstitial.OnAdOpening += HandleInterstitialOpened;
        interstitial.OnAdClosed += HandleInterstitialClosed;
        interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;

        // Load an interstitial ad.
        interstitial.LoadAd(CreateAdRequest());
    }

    public void SetVideoType(int videoType)
    {
        VideoType = (VideoType)videoType;
    }


    public void RequestRewardBasedVideo()
    {
#if UNITY_EDITOR
        var adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-2778622632232885/6237017741";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        rewardBasedVideo.LoadAd(CreateAdRequest(), adUnitId);
    }

    public void ShowInterstitial()
    {
 
        if (interstitial == null)
        {
            RequestInterstitial();
        }
        if (interstitial.IsLoaded())
            interstitial.Show();
        else
            print("Interstitial is not ready yet");
    }

    public bool IsInterstitialReady()
    {
        return interstitial != null && interstitial.IsLoaded();
    }
    public void ShowRewardBasedVideo()
    {
        if (rewardBasedVideo.IsLoaded())
            rewardBasedVideo.Show();
        else
            print("Reward based video ad is not ready yet");
    }

    public bool isRewardedVideoReady()
    {
        return rewardBasedVideo!=null && rewardBasedVideo.IsLoaded();
    }
    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
   //     print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
    //    print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
   //     print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
    //    print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
   //     print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print(
            "HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        print("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        //request interstitial
        RequestInterstitial();
        print("HandleInterstitialClosed event received");
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        print("HandleInterstitialLeftApplication event received");
    }

    #endregion

    #region RewardBasedVideo callback handlers

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
   //     print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
  //      print(
 //           "HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
  //      print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
 //       print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
 //       print("HandleRewardBasedVideoClosed event received");
        RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, GoogleMobileAds.Api.Reward args)
    {
        var type = args.Type;
        var amount = args.Amount;
    //    print(
     //       "HandleRewardBasedVideoRewarded event received for " + amount + " " + type);


        switch (VideoType)
        {
            //double current xp earned;
            case VideoType.DoubleTokens:
                AppsFlyerStartUp.Instance.TrackCustomEvent(AFInAppEvents.CLICK_DOUBLETOKENS);
                FirebaseInitializer.Instance.LogClickEvent(AFInAppEvents.CLICK_DOUBLETOKENS);
                Parameter[] virtualcurrencyparameters =
                {
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Tokens"),
                            new Parameter(FirebaseAnalytics.ParameterValue,PlayerData.CurrentGameStats.TokensCollected),
                        };

                FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventEarnVirtualCurrency, virtualcurrencyparameters);
                PlayerData.PlayerProfile.NoofTokensAvailable += (int)PlayerData.CurrentGameStats.TokensCollected;
                PlayerData.CurrentGameStats.TokensCollected *= 2;

                GameObject showCurrentStats = GameObject.Find("CurrentGameStats");
                if (showCurrentStats != null)
                {
                    GameObject currenttokensCollected = Util.FindGameObjectWithName(showCurrentStats, "tokensCollectedValue");
                    currenttokensCollected.GetComponentInChildren<Text>().text =
                        PlayerData.CurrentGameStats.TokensCollected.ToString();
                }


                UiManager.Instance.UpdateUi();
                break;
            case VideoType.ResumeVideo:
                AppsFlyerStartUp.Instance.TrackCustomEvent(AFInAppEvents.INAPP_WATCH_VIDEO);
                FirebaseInitializer.Instance.LogClickEvent(AFInAppEvents.INAPP_WATCH_VIDEO);
                GameManager.Instance.ResumeFromDeath(true);

                break;
            case VideoType.AddDiamonds:
                //award reward tio player

                PlayerData.PlayerProfile.NoofDiamondsAvailable += 1;
                Parameter[] currencyparameters =
                {
                            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Enum.GetName(typeof(LockType),LockType.Diamonds)),
                            new Parameter(FirebaseAnalytics.ParameterValue,1),
                        };

                FirebaseInitializer.Instance.LogCustomEvent(FirebaseAnalytics.EventEarnVirtualCurrency, currencyparameters);
                if (UiManager.Instance)
                    UiManager.Instance.UpdateDiamonds(PlayerData.PlayerProfile.NoofDiamondsAvailable);
                break;
        }

        //reset video type to none
        VideoType = VideoType.None;
        //save player data
        PlayerData.SavePlayerData();

    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        print("HandleRewardBasedVideoLeftApplication event received");
    }

    #endregion

    public void DestroyBannerAd()
    {
        if (bannerView != null)
            bannerView.Destroy();
    }
    private void OnDestroy()
    {
        if (bannerView != null)
            bannerView.Destroy();
        if (interstitial != null)
            interstitial.Destroy();
    }
}