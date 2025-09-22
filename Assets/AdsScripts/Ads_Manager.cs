using AppsFlyerSDK;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Sample.Analytics;
public enum AdLoadingStatus
{
    NotLoaded,
    Loading,
    Loaded,
    NoInventory
}
public enum AdType
{
    NoAds,
    Ads
}

public enum BannerSize
{
    Small,
    Smart,
    LeaderBoard,
    Adaptive
};

public enum BannerPos
{
    Top,
    TopLeft,
    TopRight,
    Bottom,
    BottomLeft,
    BottomRight
};

[Serializable]
public class AdmobId
{
    [Header("App Id")]
    public string ADMOB_APP_ID;

    [Header("Banner_Ad Id")]
    public string ADMOB_BANNER_AD_ID;

    [Header("Banner_Ad Id")]
    public string ADMOB_INTERTITIAL_AD_ID;

    [Header("AppOpen_Ad Id")]
    public string ADMOB_AppOpen_AD_ID;
}


public class Ads_Manager : MonoBehaviour
{
    public static Ads_Manager instance;
    int retryAttempt;
    [Header("InApp Ads Check")]
    public bool RemoveAds;
    [Space]
    [Header("Banner Ads ")]
    public bool isShowBanner = false;
    public static bool OppenApp_Not_Shown;
    public delegate void RewardUserDelegate();
    private static RewardUserDelegate NotifyReward;
    private static DateTime TimeForAppOpenAds;
    private static DateTime TimeForInterAds;
    public static bool firstOpen = true;
    public static bool interReward = false;

    [Header("AdUnit ID's")]
    public string MaxSdkKey = "Kqd0WkV087JpIBqZUm6EROpVelFiQ33aKsZSeiOKSy3t6L4r1EPxhqXRnqN0MDMUUOtos5nry1OMsFrdTMrL37";
    [Space]
    public string LovinBanner = "278353bf4df56b82";
    public string LovinBanner2 = "22a53361543807e5";
    public string LovinMRec = "0b804b6631b290b6";
    public string LovinInterstitial = "337cf4991f33e1bf";
    public string LovinRewarded = "e322476df6348b25";
    public string LovinAppOpen = "0af4145bdbe2106a";
    [Space]
    [Header("Set Banner Position")]
    public MaxSdkBase.BannerPosition _BannerPosition = MaxSdkBase.BannerPosition.TopCenter;
    public MaxSdkBase.BannerPosition _BannerPosition2 = MaxSdkBase.BannerPosition.TopCenter;
    public MaxSdkBase.AdViewPosition _MRecPosition = MaxSdkBase.AdViewPosition.BottomLeft;

    private bool isBannerShowing;
    private bool isBanner2Showing;
    private bool isMRecShowing;
    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    //..................Admob Setting........................
    [Header("Google Admob Network Setting")]
    public bool isAdmobBanner = false;
    public bool isAdmobAppOpen = false;

    public static AdType adsStatus;

    public static AdLoadingStatus smallBannerStatus = AdLoadingStatus.NotLoaded;
    public static AdLoadingStatus rAdStatus = AdLoadingStatus.NotLoaded;
    public static AdLoadingStatus iAdStatus = AdLoadingStatus.NotLoaded;

    AdPosition AdmobBannerPos;
    [Header("Banner Setting")]
    public BannerPos _AdmobBannerPos;
    public BannerSize _AdmobBannerSize;
    AdSize bannerSize;
    [Space]
    public AdmobId ADMOB_ID = new AdmobId();
    bool isAdmobInitialized = false, isAdmobBannerInitialized = false;

    #region Banner
    private BannerView banner;
    public static bool isSmallBannerLoaded = false;
    #endregion

    #region Interstitial
    private InterstitialAd interstitialAd;
    #endregion

    #region AppOpenAd
    [HideInInspector]
    private AppOpenAd appOpenAd;

    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;

    #endregion

    // Start is called before the first frame update
    [Header("RemoteConfi Values")]
    public long drainrotOccurance=2;
    public bool drainrotNeedAppOpen=true;

    public long drainrot_revenue_multiplier=1;
    public bool drainrot_needBanner=true;
    private int currentOccurrance = 1;

    void Awake()
    {
        Time.timeScale = 1;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        if (SystemInfo.systemMemorySize > 1024)
        {
            Debug.Log("GG >> System Memory is greater than 1024, Showing Ads");
            adsStatus = AdType.Ads;
        }
        else
        {
            Debug.Log("GG >> System Memory is less than or equal to 1024, Not Showing Ads");
            adsStatus = AdType.NoAds;
        }


        switch (_AdmobBannerSize)
        {
            case BannerSize.Small:
                bannerSize = AdSize.Banner;
                break;
            case BannerSize.Smart:
                bannerSize = AdSize.SmartBanner;
                break;
            case BannerSize.LeaderBoard:
                bannerSize = AdSize.Leaderboard;
                break;
            case BannerSize.Adaptive:
                AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                bannerSize = adaptiveSize;
                break;
        }
        SetBannerPos(_AdmobBannerPos);
    }

    private void OnDisable()
    {
        DestroyBannerAd();
        smallBannerStatus = AdLoadingStatus.NotLoaded;
        isSmallBannerLoaded = false;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
        OppenApp_Not_Shown = false;
        interReward = false;

        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            RemoveAds = false;
            Debug.Log("Don't Purchase Remove ADs Yet");
        }
        else if (PlayerPrefs.GetInt("RemoveAds") == 1)
        {
            RemoveAds = true;
            Debug.Log("Purchase Remove ADs");
        }

        // Init MaxApplovin...............

                InitializeMaxlovin();
            
        
        /*InitializeMaxlovin();*/
    }

    public void AdsCheck()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            RemoveAds = false;
            Debug.Log("Don't Purchase Remove ADs Yet");
        }
        else if (PlayerPrefs.GetInt("RemoveAds") == 1)
        {
            RemoveAds = true;
            Debug.Log("Purchase Remove ADs");
        }
    }

    #region Max Initilization

    public void InitializeMaxlovin()
    {
        MaxSdk.SetHasUserConsent(true);

        Debug.Log("Initial Max AppLovin");

        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            Debug.Log("InitSuccess");
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (!RemoveAds)
                {
                    if (!isAdmobBanner)
                    {
                        InitializeSmallBanner();
                        InitializeSmallBanner2();
                    }
                    InitializeSmallBanner2();
                    if (!isAdmobAppOpen)
                    {
                        InitializeAppOppen();
                    }
                    InitializeMediumBanner();
                    InitializeInterstitialAds();
                }
                InitializeRewardedAds();
            }
        };

        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();

        LoadAllAds();
    }

    public void LoadAllAds()
    {
        if (!isAdmobBanner)
        {
            LoadSmallBanner();
           
        }
        LoadSmallBanner2();
        if (!isAdmobAppOpen)
        {
            LoadAppOpen();
        }
        LoadMediumBanner();
        LoadInterstitial();
        InitializeAdmob();
    }

    #endregion

    #region AppsFlyer Event Initilization

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[]
        {

                new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
                new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
                new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
                new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
                new Firebase.Analytics.Parameter("value", revenue * drainrot_revenue_multiplier),
                new Firebase.Analytics.Parameter("currency", "USD"), // All Applovin revenue is sent in USD
        };

        //if (FbAnalytics.Instance.firebaseInitialized)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        }

        Dictionary<string, string> additionalParams = new Dictionary<string, string>();
        additionalParams.Add(AFAdRevenueEvent.AD_UNIT, impressionData.AdUnitIdentifier);
        additionalParams.Add(AFAdRevenueEvent.AD_TYPE, impressionData.AdFormat);
        AppsFlyerAdRevenue.logAdRevenue(impressionData.NetworkName,
                                AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                                impressionData.Revenue * drainrot_revenue_multiplier,
                                "USD",
                                additionalParams);
    }

    private void OnAdmobAdRevenuePaidEvent(AdValue adValue, AdapterResponseInfo adapterInfo, string adUnitId, string AdName)
    {
        if (adValue == null || adapterInfo == null || string.IsNullOrEmpty(adUnitId))
        {
            Debug.Log("AdValue or AdapterResponseInfo is null");
            return;
        }

        double revenue = adValue.Value / 1_000_000.0;
        var impressionParameters = new[]
        {
            new Parameter("ad_platform", "AdMob"),
            new Parameter("ad_source", adapterInfo.AdSourceName),
            new Parameter("ad_unit_name", adUnitId),
            new Parameter("ad_format", AdName),
            new Parameter("value", revenue* drainrot_revenue_multiplier),
            new Parameter("currency", adValue.CurrencyCode)
        };

        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);

        Dictionary<string, string> additionalParams = new Dictionary<string, string>();
        additionalParams.Add(AFAdRevenueEvent.AD_UNIT, adUnitId);
        additionalParams.Add(AFAdRevenueEvent.AD_TYPE, AdName);
        AppsFlyerAdRevenue.logAdRevenue(adapterInfo.AdSourceName,
                                AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob,
                                (adValue.Value / 1_000_000.0)* drainrot_revenue_multiplier,
                                "USD",
                                additionalParams);
    }


    #endregion

    #region Banner Ad Methods
    public void InitializeSmallBanner()
    {
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        //LoadSmallBanner();
    }

    public void LoadSmallBanner()
    {
        Debug.Log("Initialized Banner");

        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(LovinBanner, _BannerPosition);
        MaxSdk.SetBannerExtraParameter(LovinBanner, "adaptive_banner", "false"); // For adaptive Set true

        // Set width for banners manual.
        MaxSdk.SetBannerWidth(LovinBanner, 300f);

    }

    public void ShowSmallBanner()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1) 
            return;

        if (Application.internetReachability != NetworkReachability.NotReachable && RemoveAds == false)
        {
            if (isBannerShowing)
            {
                MaxSdk.ShowBanner(LovinBanner);
                Debug.Log("Show Banner");
                //hideBannerPanel.SetActive(true); //by GH

            }
        }
    }

    public void HideSmallBanner()
    {
        MaxSdk.HideBanner(LovinBanner);
        Debug.Log("Hide Banner");
        //hideBannerPanel.SetActive(false); //by GH

    }

    public void ResetBannerPos(MaxSdk.BannerPosition pos)
    {
        MaxSdk.HideBanner(LovinBanner);
        MaxSdk.UpdateBannerPosition(LovinBanner, pos);
        MaxSdk.ShowBanner(LovinBanner);
        Debug.Log("Show Banner on New Position");
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if(GRS_FirebaseHandler.Instance)
        {
            GRS_FirebaseHandler.Instance.DesignEvent("banner_shown_right");
        }
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
        isBannerShowing = true;
        if (isShowBanner)
        {
            ShowSmallBanner();
            ShowSmallBanner2();
        }
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        isBannerShowing = false;
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
        OppenApp_Not_Shown = true;
    }

    #endregion

    #region Banner2 Ad Methods
    public void InitializeSmallBanner2()
    {
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent2;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent2;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent2;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        LoadSmallBanner2();
    }

    public void LoadSmallBanner2()
    {
        Debug.Log("Initialized Banner");

        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(LovinBanner2, _BannerPosition2);
        MaxSdk.SetBannerExtraParameter(LovinBanner2, "adaptive_banner", "false"); // For adaptive Set true

        // Set width for banners manual.
        MaxSdk.SetBannerWidth(LovinBanner2, 300f);
    }

    public void ShowSmallBanner2()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (!drainrot_needBanner)
            return;
        if (Application.internetReachability != NetworkReachability.NotReachable && RemoveAds == false)
        {
            if (isBanner2Showing)
            {
                MaxSdk.ShowBanner(LovinBanner2);
                Debug.Log("Show Banner2");
            }
        }
    }

    public void HideSmallBanner2()
    {
        MaxSdk.HideBanner(LovinBanner2);
        Debug.Log("Hide Banner2");
        //hideBannerPanel.SetActive(false); //by GH

    }

    public void ResetBanner2Pos(MaxSdk.BannerPosition pos)
    {
        MaxSdk.HideBanner(LovinBanner2);
        MaxSdk.UpdateBannerPosition(LovinBanner2, pos);
        MaxSdk.ShowBanner(LovinBanner2);
        Debug.Log("Show Banner2 on New Position");
    }

    private void OnBannerAdLoadedEvent2(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (GRS_FirebaseHandler.Instance)
        {
            GRS_FirebaseHandler.Instance.DesignEvent("banner_shown_left");
        }
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
        isBanner2Showing = true;
        if (isShowBanner)
        {
            ShowSmallBanner2();
        }
    }

    private void OnBannerAdFailedEvent2(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        isBanner2Showing = false;
    }

    private void OnBannerAdClickedEvent2(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
        OppenApp_Not_Shown = true;
    }

    #endregion

    #region MREC Ad Methods

    public void InitializeMediumBanner()
    {
        // Attach Callbacks
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        //LoadMediumBanner();
    }

    public void LoadMediumBanner()
    {
        Debug.Log("Initial MRec AppLovin");
        // MRECs are automatically sized to 300x250.
        MaxSdk.CreateMRec(LovinMRec, MaxSdkBase.AdViewPosition.BottomLeft);
    }

    public void ShowMediumBanner()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (Application.internetReachability != NetworkReachability.NotReachable && RemoveAds == false)
        {
            if (isMRecShowing)
            {
                Debug.Log("Show MRec AppLovin");
                MaxSdk.ShowMRec(LovinMRec);
            }
        }
    }
    public void HideMediumBanner()
    {
        Debug.Log("Hide MRec AppLovin");
        MaxSdk.HideMRec(LovinMRec);
    }

    public void ResetMediumBannerPos(MaxSdk.AdViewPosition pos)
    {
        MaxSdk.HideMRec(LovinMRec);
        MaxSdk.UpdateMRecPosition(LovinMRec, pos);
        MaxSdk.ShowMRec(LovinMRec);
        Debug.Log("Show MRec on New Position");
    }

    private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // MRec ad is ready to be shown.
        // If you have already called MaxSdk.ShowMRec(MRecAdUnitId) it will automatically be shown on the next MRec refresh.
        Debug.Log("MRec ad loaded");
        isMRecShowing = true;
        if (GRS_FirebaseHandler.Instance)
        {
            GRS_FirebaseHandler.Instance.DesignEvent("medium_banner_shown");
        }
    }

    private void OnMRecAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // MRec ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("MRec ad failed to load with error code: " + errorInfo.Code);
        isMRecShowing = false;
    }

    private void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MRec ad clicked");
        OppenApp_Not_Shown = true;
    }


    #endregion

    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        Debug.Log("Initial Inters AppLovin");
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialAdClickedEvent;


        // Load the first interstitial
       // LoadInterstitial();
    }

    public void LoadInterstitial()
    {
            if (Application.internetReachability != NetworkReachability.NotReachable && RemoveAds == false)
            {
                Debug.Log("Load Inters AppLovin");
                MaxSdk.LoadInterstitial(LovinInterstitial);
            }
        
    }

    public void ShowInterstitial()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (Application.internetReachability != NetworkReachability.NotReachable && RemoveAds == false)
        {
            //if (isDealayCompleteForInterAds())
            //{
                Debug.Log("Inters AppLovin Show Call");
                if (MaxSdk.IsInterstitialReady(LovinInterstitial))
                {
                    Debug.Log("Inters AppLovin Will Display");
                    MaxSdk.ShowInterstitial(LovinInterstitial);
                    
                }
                else
                {
                    Debug.Log("Inters AppLovin Not Loaded");
                    LoadInterstitial();
                }
            //}
        }
    }

    //public void ShowInterstitial(RewardUserDelegate _delegate)
    //{
    //    if (Application.internetReachability != NetworkReachability.NotReachable)
    //    {
    //        Debug.Log("Rewarded Inters AppLovin Show Call");
    //        if (MaxSdk.IsInterstitialReady(LovinInterstitial))
    //        {
    //            Debug.Log("Rewarded Inters AppLovin Will Display");
    //            interReward = true;
    //            NotifyReward = _delegate;
    //            MaxSdk.ShowInterstitial(LovinInterstitial);
    //        }
    //        else
    //        {
    //            Debug.Log("Rewarded Inters AppLovin Not Loaded");
    //            LoadInterstitial();
    //        }
    //    }
    //}

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown.
        Debug.Log("Interstitial loaded");

        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

        Invoke(nameof(LoadInterstitial), (float)retryDelay);

    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if(GRS_FirebaseHandler.Instance)
        {
            GRS_FirebaseHandler.Instance.DesignEvent("interstitial_shown_max");
        }
        // Interstitial ad is hidden. Pre-load the next ad
        if (interReward)
        {
            Debug.Log("Give Reward On Inter Dismissed");
            interReward = false;
            NotifyReward();
        }
        Debug.Log("Interstitial dismissed");
        AudioListener.pause = false;
        Time.timeScale = 1;
        LoadInterstitial();
        //    OppenApp_Not_Shown = true;
        if (drainrotNeedAppOpen && currentOccurrance%drainrotOccurance==0)
        {
            ShowAdmobAppOpenAd();
        }
        currentOccurrance++;
        //    EventManager.DofireResetTimeBreak();
    }


    private void OnInterstitialAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is Clicked
        Debug.Log("Interstitial Ad is Clicked");
        //OppenApp_Not_Shown = true;
     //   EventManager.DofireResetTimeBreak();
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        Debug.Log("Initial Reward AppLovin");
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedVideo();
    }

    public void LoadRewardedVideo()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("Load Reward AppLovin");
            MaxSdk.LoadRewardedAd(LovinRewarded);
        }
    }
    public bool ShowRewardedVideo()
    {
        //...........Rewarded Ad Calling.........................
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("Reward AppLovin Show Call");
            if (MaxSdk.IsRewardedAdReady(LovinRewarded))
            {
                Debug.Log("Reward AppLovin Will Display");
                
                MaxSdk.ShowRewardedAd(LovinRewarded);
                return true;
            }
            else
            {
                Debug.Log("Reward AppLovin Not Loaded");
                
                LoadRewardedVideo();
                return false;

            }
        }
        return false;
    }
    public void ShowRewardedVideoMethod()
    {
        ShowRewardedVideo(Temp);
    }
    void Temp()
    {

    }
    public void ShowRewardedVideo(RewardUserDelegate _delegate)
    {

        //...........Rewarded Ad Calling.........................
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("Reward AppLovin Show Call");
            if (MaxSdk.IsRewardedAdReady(LovinRewarded))
            {
                Debug.Log("Reward AppLovin Will Display");
                NotifyReward = _delegate;
                MaxSdk.ShowRewardedAd(LovinRewarded);
            }
            else
            {
                Debug.Log("Reward AppLovin Not Loaded");
                LoadRewardedVideo();

            }
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;
    }


    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);
        AudioListener.pause = false;
        Time.timeScale = 1;
        Invoke(nameof(LoadRewardedVideo), (float)retryDelay);

    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedVideo();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
        OppenApp_Not_Shown = true;
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (GRS_FirebaseHandler.Instance)
        {
            GRS_FirebaseHandler.Instance.DesignEvent("rewarded_video_shown");
        }
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("Rewarded ad dismissed");
        OppenApp_Not_Shown = true;
        AudioListener.pause = false;
        Time.timeScale = 1;
        LoadRewardedVideo();
        if (drainrotNeedAppOpen && currentOccurrance % drainrotOccurance == 0)
        {
            ShowAdmobAppOpenAd();
        }
        currentOccurrance++;
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        if (GRS_FirebaseHandler.Instance)
        {
            GRS_FirebaseHandler.Instance.DesignEvent("rewarded_video_completed");
        }
        // Rewarded ad was displayed and user should receive the reward     
        Debug.Log("Rewarded ad received reward");
        NotifyReward();
        AudioListener.pause = false;
        Time.timeScale = 1;
    }


    #endregion

    #region AppOppen Ad Methods

    private void InitializeAppOppen()
    {
        Debug.Log("Initial AppOpen AppLovin");
        //Initialize AppOppen AD
        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
        MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenClickedEvent;
        MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;

        //LoadAppOpen();
    }

    public void LoadAppOpen()
    {
       
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("Load AppOpen AppLovin");

                MaxSdk.LoadAppOpenAd(LovinAppOpen);
            }
        
    }

    public void ShowAppOpen()
    {
        if (PlayerPrefs.GetInt("RemoveAds")==1) return;
        
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("AppOpen AppLovin Show Call");
            if (MaxSdk.IsAppOpenAdReady(LovinAppOpen))
            {
                Debug.Log("Ready To Show AppOpen AppLovin");
                MaxSdk.ShowAppOpenAd(LovinAppOpen);
            }
            else
            {
                LoadAppOpen();
            }
        }
    }

    private void OnAppOpenLoadedEvent(string LovinAppOpen, MaxSdkBase.AdInfo adInfo)
    {
        // AppOpen ad is ready for you to show. MaxSdk.IsAppOpenAdReady(adUnitId) now returns 'true'
        if (firstOpen)
        {
            firstOpen = false;
            Debug.Log("FirstOpen AppOpen Ad");
            ShowAppOpen();    //..............For Application Focus Disable this line....................
        }
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        LoadAppOpen();
    }

    public void OnAppOpenClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("AppOpen AD is Clicked");
    }

    //..............For Application Focus Enable this....................
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            AudioListener.pause = false;
            Time.timeScale = 1;

            if (!firstOpen)
            {
                if (OppenApp_Not_Shown == true)
                {
                    OppenApp_Not_Shown = false;
                }
                else
                {
                    if (isDealayCompleteForAppOpenAds())
                    {
                        if (isAdmobAppOpen)
                        {
                            print(">>>>>show addmob app oppen");
                            ShowAdmobAppOpenAd();
                        }
                        else
                        {
                            ShowAppOpen();
                        }
                    }
                }
            }
        }
    }

    public bool isDealayCompleteForAppOpenAds()
    {
        double secondsPassed = (DateTime.UtcNow - TimeForAppOpenAds).TotalSeconds;
        if (secondsPassed > 5)
        {
            setTime1ForAds();
            return true;
        }
        return false;
    }

    public void setTime1ForAds()
    {
        TimeForAppOpenAds = DateTime.UtcNow;
    }

    public bool isDealayCompleteForInterAds()
    {
        //double secondsPassed = (DateTime.UtcNow - TimeForInterAds).TotalSeconds;
        //if (secondsPassed > 20)
        //{
        //    setTime1ForInterAds();
        //    return true;
        //}
        //return false;

        return true;
    }

    public void setTime1ForInterAds()
    {
        TimeForInterAds = DateTime.UtcNow;
    }

    #endregion

    #region Admob Network

    #region Initialize Admob

    void InitializeAdmob()
    {
        MobileAds.Initialize((initStatus) =>
        {
            Debug.Log("GG >> Admob:Initialized");

            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        Debug.Log("GG >> Adapter: " + status.Description + " not ready.Name=" + className);
                        break;
                    case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        Debug.Log("GG >> Adapter: " + className + " is initialized.");
#if UNITY_IOS
                        MediationAdapterConsent(className);
#endif

#if UNITY_ANDROID
                        MediationAdapterConsent(className);
#endif
                        break;
                }
            }

#if UNITY_EDITOR
            isAdmobInitialized = true;
            isAdmobBannerInitialized = true;
            if (isAdmobInitialized)
            {
                if (PlayerPrefs.GetInt("RemoveAds") != 1)
                {
                    Debug.Log("......Load calls");
                    if (isAdmobBanner && PlayerPrefs.GetInt("RemoveAds") == 0)
                    {
                        LoadAdmobSmallBanner();
                    }
                    if (isAdmobAppOpen && PlayerPrefs.GetInt("RemoveAds")==0)
                    {
                        LoadAdmobAppOpenAd();
                    }

                    LoadAdmobInterstitial();
                }
            }
#endif
        });

#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif
    }

    void MediationAdapterConsent(string AdapterClassname)
    {
        if (AdapterClassname.Contains("MobileAds"))
        {
            Debug.Log("GG >> Admob consent is send");
            isAdmobInitialized = true;
            isAdmobBannerInitialized = true;
            if (isAdmobInitialized)
            {
                //...........Ads Loading...............................................

                if (PlayerPrefs.GetInt("RemoveAds") != 1)
                {
                    if (isAdmobBanner)
                    {
                        LoadAdmobSmallBanner();
                    }

                    if (isAdmobAppOpen)
                    {
                        LoadAdmobAppOpenAd();
                    }

                    LoadAdmobInterstitial();

                }
            }
        }
    }

    public void SetBannerPos(BannerPos Pos)
    {
        switch (Pos)
        {
            case BannerPos.Top:
                AdmobBannerPos = AdPosition.Top;
                break;
            case BannerPos.TopLeft:
                AdmobBannerPos = AdPosition.TopLeft;
                break;
            case BannerPos.TopRight:
                AdmobBannerPos = AdPosition.TopRight;
                break;
            case BannerPos.Bottom:
                AdmobBannerPos = AdPosition.Bottom;
                break;
            case BannerPos.BottomLeft:
                AdmobBannerPos = AdPosition.BottomLeft;
                break;
            case BannerPos.BottomRight:
                AdmobBannerPos = AdPosition.BottomRight;
                break;
        }
    }

#endregion


    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }

    #endregion

    #region BANNER ADS

    public bool IsBannerAdAvailable
    {
        get
        {
            return (banner != null);
        }

    }

    public bool IsAdmobSmallBannerReady()
    {
        return isSmallBannerLoaded;
    }

    public void LoadAdmobSmallBanner()
    {
        if (!isAdmobInitialized || IsAdmobSmallBannerReady() || smallBannerStatus == AdLoadingStatus.Loading || adsStatus == AdType.NoAds)
        {
            Debug.Log("GG >> Admob:smallBanner: No Request Generated");
            return;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {

                    Debug.Log("GG >> Admob:smallBanner:LoadRequest");
                    smallBannerStatus = AdLoadingStatus.Loading;
                    LoadAdmobBannerAd(ADMOB_ID.ADMOB_BANNER_AD_ID);
                
            }
        }
    }

    public void LoadAdmobBannerAd(string ID)
    {
        // Clean up banner before reusing
        if (banner != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        banner = new BannerView(ID, bannerSize, AdmobBannerPos);

        // Add Event Handlers
        banner.OnBannerAdLoaded += () =>
        {
            Debug.Log("GG >> Admob:smallBanner:Loaded.");
            smallBannerStatus = AdLoadingStatus.Loaded;
            isSmallBannerLoaded = true;
            if (isShowBanner)
            {
                banner.Show();
            }
        };
        banner.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.Log("GG >> Admob:smallBanner:NoInventory :: " + error.GetMessage());
            smallBannerStatus = AdLoadingStatus.NoInventory;
            isSmallBannerLoaded = false;
        };
        banner.OnAdImpressionRecorded += () =>
        {
            Debug.Log("GG >> Admob:smallBanner:Displayed");
            Debug.Log("GG >> Admob:smallBanner:Displayed recorded an impression");
        };
        banner.OnAdClicked += () =>
        {
            Debug.Log("GG >> Admob:smallBanner : Click.");
        };
        banner.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("GG >> Admob:smallBanner : Opening.");
        };
        banner.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("GG >> Admob:smallBanner : Closed.");
        };
        banner.OnAdPaid += (AdValue adValue) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Banner ad received a paid event.",
                                        adValue.CurrencyCode,
                                        adValue.Value);

            Debug.Log(msg);

            var responseInfo = banner.GetResponseInfo();
            OnAdmobAdRevenuePaidEvent(adValue, responseInfo.GetLoadedAdapterResponseInfo(), ID, "Banner");
        };

        // Load a banner ad
        AdRequest request = CreateAdRequest();
        banner.LoadAd(request);
       
    }

    public void ShowAdmobSmallBanner(BannerPos position)
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (!isAdmobBannerInitialized || !isAdmobInitialized || adsStatus == AdType.NoAds)
        {
            return;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {
                
                    SetBannerPos(position);
                    Debug.Log("GG >> Admob:smallBanner:ShowCall");
                    if (IsBannerAdAvailable)
                    {
                        Debug.Log("GG >> Admob:smallBanner:Hide Previous");
                        banner.Hide();
                        Debug.Log("GG >> Admob:smallBanner:WillDisplay");
                        banner.Show();
                        banner.SetPosition(AdmobBannerPos);
                    }
                    else
                    {
                        Debug.Log("GG >> Admob:smallBanner: No Ad Loaded");
                        LoadAdmobSmallBanner();
                    }
                }
           
        }
    }

    public void ShowAdmobSmallBanner()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (!isAdmobBannerInitialized || !isAdmobInitialized || adsStatus == AdType.NoAds)
        {
            return;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {
                
                    Debug.Log("GG >> Admob:smallBanner:ShowCall");
                    if (IsBannerAdAvailable)
                    {
                        Debug.Log("GG >> Admob:smallBanner:Hide Previous");
                        banner.Hide();
                        Debug.Log("GG >> Admob:smallBanner:WillDisplay");
                        banner.Show();
                        banner.SetPosition(AdmobBannerPos);
                    }
                    else
                    {
                        Debug.Log("GG >> Admob:smallBanner: No Ad Loaded");
                        LoadAdmobSmallBanner();
                    }
                }
            
        }
    }

    public void HideAdmobSmallBanner()
    {
        if (banner != null)
        {
            Debug.Log("GG >> Admob:smallBanner:Hide");
            banner.Hide();
        }
    }

    public void DestroyBannerAd()
    {
        if (banner != null)
        {
            Debug.Log("GG >> Admob:smallBanner : Destroyed");
            banner.Destroy();
            banner = null;
        }
    }

    #endregion

    #region INTERSTITIAL ADS

    public bool IsInterstitialAdAvailable
    {
        get
        {
            return (interstitialAd != null
                && interstitialAd.CanShowAd());
        }
    }

    public bool IsInterstitialAdReady()
    {
        if (interstitialAd != null)
            return interstitialAd.CanShowAd();
        else
            return false;
    }

    public void LoadAdmobInterstitial()
    {
        if (!isAdmobInitialized || IsInterstitialAdReady() || iAdStatus == AdLoadingStatus.Loading || adsStatus == AdType.NoAds)
        {
            return;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {
                Debug.Log("GG >> Admob: inter ad :LoadRequest");
                iAdStatus = AdLoadingStatus.Loading;
                LoadInterAd(ADMOB_ID.ADMOB_INTERTITIAL_AD_ID);
            }

        }
    }

    public void LoadInterAd(string ID)
    {
        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            DestroyInterstitialAd();
        }

        // Load an interstitial ad
        InterstitialAd.Load(ID, CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    iAdStatus = AdLoadingStatus.NoInventory;
                    Debug.Log("GG >> Admob:Interstitial Ad failed to load with error: " +
                        loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    iAdStatus = AdLoadingStatus.NoInventory;
                    Debug.Log("GG >> Admob:Interstitial Ad failed to load.");
                    return;
                }

                iAdStatus = AdLoadingStatus.Loaded;
                Debug.Log("GG >> Admob:Interstitial Ad loaded.");
                interstitialAd = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    iAdStatus = AdLoadingStatus.NotLoaded;
                    Debug.Log("GG >> Admob:Interstitial Ad open.");
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    iAdStatus = AdLoadingStatus.NotLoaded;
                    LoadAdmobInterstitial();
                    Debug.Log("GG >> Admob: Interstitial Ad closed.");
                    OppenApp_Not_Shown = true;
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("GG >> Admob:Interstitial Ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    Debug.Log("GG >> Admob:Interstitial Ad recorded a click.");
                    OppenApp_Not_Shown = true;
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("GG >> Admob:Interstitial Ad failed to show with error: " +
                                error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Interstitial ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    Debug.Log(msg);

                    var responseInfo = ad.GetResponseInfo();
                    OnAdmobAdRevenuePaidEvent(adValue, responseInfo.GetLoadedAdapterResponseInfo(), ID, "Interstitial");
                };
            });
    }

    public void ShowAdmobInterstitial()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (!isAdmobInitialized || adsStatus == AdType.NoAds)
        {
            return;
        }

        if (PlayerPrefs.GetInt("RemoveAds") != 1)
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {

                Debug.Log("GG >> Admob:iad:ShowCall");
            }
            else
            {
                Debug.Log("GG >> Admob:iad:ShowCall:Offline");
            }

            if (IsInterstitialAdAvailable)
            {
                Debug.Log("GG >> Admob:iad:WillDisplay");
                if (GRS_FirebaseHandler.Instance)
                {
                    GRS_FirebaseHandler.Instance.DesignEvent("interstitial_shown_admob");
                }
                interstitialAd.Show();
            }
            else
            {
                Debug.Log("GG >> Admob:Interstitial Ad No Inventory.");
            }
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            Debug.Log("GG >> Admob:Interstitial Ad Destroyed.");
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }

    #endregion

    #region APPOPEN ADS

    public bool IsAppOpenAdAvailable
    {
        get
        {
            return (appOpenAd != null
                    && appOpenAd.CanShowAd()
                    && DateTime.Now < appOpenExpireTime);
        }

    }

    public void LoadAdmobAppOpenAd()
    {
        if (!isAdmobInitialized || IsAppOpenAdAvailable || adsStatus == AdType.NoAds)
        {
            return;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {
                
                    Debug.Log("GG >> Admob: AppOpen Ad : LoadRequest");
                    LoadAdmobOpenAd(ADMOB_ID.ADMOB_AppOpen_AD_ID);
               
            }
        }
    }


    public void LoadAdmobOpenAd(string ID)
    {
        // destroy old instance.
        if (appOpenAd != null)
        {
            DestroyAppOpenAd();
        }

        // Create a new app open ad instance.
        AppOpenAd.Load(ID, CreateAdRequest(),
            (AppOpenAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("GG >> Admob: AppOpen Ad : failed to load with error: " +
                        loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("GG >> Admob: AppOpen Ad : failed to load : ");
                    return;
                }

                Debug.Log("GG >> Admob: AppOpen Ad : loaded. Please background the app and return.");
                this.appOpenAd = ad;
                this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("GG >> Admob: AppOpen Ad : opened : ");
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("GG >> Admob: AppOpen Ad : closed : ");
                    LoadAdmobAppOpenAd();        //EDIT FOR ONE TIME AD
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("GG >> Admob: App open ad recorded an impression : ");
                };
                ad.OnAdClicked += () =>
                {
                    Debug.Log("GG >> Admob: App open ad recorded a click : ");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("GG >> Admob: App open ad failed to show with error: " +
                        error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "App open ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);

                    Debug.Log(msg);

                    var responseInfo = ad.GetResponseInfo();
                    OnAdmobAdRevenuePaidEvent(adValue, responseInfo.GetLoadedAdapterResponseInfo(), ID, "AppOpen");
                };

                if (firstOpen)
                {
                    firstOpen = false;
                    Debug.Log("FirstOpen AppOpen Ad");
                    if (PlayerPrefs.GetInt("RemoveAds") == 1)
                        return;
                    ShowAdmobAppOpenAd();    //..............For Application Focus Disable this line....................
                }
            });
    }

    public void DestroyAppOpenAd()
    {
        if (this.appOpenAd != null)
        {
            this.appOpenAd.Destroy();
            this.appOpenAd = null;
        }
    }

    public void ShowAdmobAppOpenAd()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        if (!isAdmobInitialized || adsStatus == AdType.NoAds)
        {
            return;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {
                
                    Debug.Log("GG >> Admob: AppOpen Ad:ShowCall");
                    if (IsAppOpenAdAvailable)
                    {
                        Debug.Log("GG >> Admob: AppOpen Ad:WillDisplay");
                    if (GRS_FirebaseHandler.Instance)
                    {
                        GRS_FirebaseHandler.Instance.DesignEvent("appOpen_shown_admob");
                    }
                    appOpenAd.Show();
                    }
                    else
                    {
                        Debug.Log("GG >> Admob: AppOpen Ad: No Ad Loaded");
                        LoadAdmobAppOpenAd();
                    }
                }
            }
        
    }

    #endregion

#endregion

}