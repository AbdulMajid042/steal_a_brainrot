using Firebase.Analytics;
using Firebase.Sample.Analytics;
using SolarEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MaxSdkBase;
using static SolarEngine.Analytics;
//using AppLovinMax.Scripts.IntegrationManager.Editor;
//using AppLovinMax.Scripts.IntegrationManager.Editor;
public enum AdType
{
    NOAds,
    Ads
}

public enum SolarAdType
{
    Other,
    Rewarded_Video_Ads,
    Splash_Ads,
    Interstitial_Ads,
    Full_Screen_Videos,
    Banner_Ads,
    Mrec_Ads,
    AppOpen_Ads,
}
public enum AdLoadingStatus
{
    NotLoaded,
    Loading,
    Loaded,
    NoInventory
}
public class AdsManagerWrapper : MonoBehaviour
{
    public delegate void RewardUserDelegate();
    public static AdsManagerWrapper Instance;
    public static AdType adsStatus;
    public String SdkKey;
    [Space]
    public String InterID;
    public String RewardedID;
    public String BannerID;
    public String RectBannerID;

    //[Space]
    //[Header("App IDs")]
    //[SerializeField] private string appId = "YOUR_ADMOB_APP_ID";

    //[Header("Ad Unit IDs")]
    //[SerializeField] private string appOpenAdId = "YOUR_APPOPEN_AD_ID";
    //[SerializeField] private string bannerAdId = "YOUR_BANNER_AD_ID";
    //[SerializeField] private string mrecAdId = "YOUR_MREC_AD_ID";
    //[SerializeField] private string interstitialAdId = "YOUR_INTERSTITIAL_AD_ID";
    //[SerializeField] private string rewardedAdId = "YOUR_REWARDED_AD_ID";










    


    //[Space]





    //  public String AdaptiveBannerID;
    public String AppOpenID;
    public static event Action onInitializeEvent;
    bool IfCanTryToInitializeAgain = true;
    public string[] TestDevices;
    [HideInInspector] public bool ADsDisable;
    [HideInInspector] public bool isWatchAdPanel = true;
    [HideInInspector] public long startAppOpenTime = 60;
    [Space]
    [HideInInspector] public long idleAdTime = 60;
    [HideInInspector] public long InterGraceTime = 30;
    [HideInInspector] public long loadingAdTextTime = 1;
   /* [HideInInspector]*/ public long quizScreenCount = 4;
    [HideInInspector] public bool thirdSlotlock = true;
    [HideInInspector] public bool fourthSlotlock = true;
    [HideInInspector] public bool showIapLoading = true;
    [HideInInspector] public bool enableQuiz = false;
    [HideInInspector] public bool enableSelection = true;
    /*[HideInInspector] */public bool appOpenAfterIad = true;
   /* [HideInInspector] */public long appOpenCount = 2;
    [HideInInspector] public bool runAdsTimer = false;
    public GameObject checkInternetMsg;
    [HideInInspector] public float lastRemoveAdsTime;
    [HideInInspector] public float lastUnlockAllTime;
    [HideInInspector] public bool hasUnlocked5Guns;
    [HideInInspector] public bool welcomInter = false;
    [HideInInspector] public bool canShowCheckInternetPanel = false;

    [HideInInspector] public long IAP_Attempts = 2; // break Point Attempts For Premium
    [HideInInspector] public int remainingAttempts = 0;
    public GameObject inappPremium,noThanksBtn;







    #region Fazool Data

    public static AdLoadingStatus iAdStatus = AdLoadingStatus.NotLoaded;

    public static AdLoadingStatus smallBannerStatus = AdLoadingStatus.NotLoaded;

    public static AdLoadingStatus mediumBannerStatus = AdLoadingStatus.NotLoaded;

    public static AdLoadingStatus rAdStatus = AdLoadingStatus.NotLoaded;

    public static AdLoadingStatus appOpenAdStatus = AdLoadingStatus.NotLoaded;
    #region Private Members
    private int retryAttemptIAD, retryAttempt;
    private bool isAppLovinInitiallized = false, isSBannerDisplayed = false, isMBannerDisplayed = false;
    private bool isSmallBannerLoaded = false, isMediumBannerLoaded = false;
    public static bool ForeGroundedAD = false;
    private static RewardUserDelegate NotifyReward;
    #endregion
    public static bool InterstitialShowd = false;
    public static bool RewardVideoShowed = false;
    //#region CP
    ////private bool isCP_MRec_Showing = false;
    ////private bool isCP_SM_Showing = false;
    ////private int CP_SM_lastShownIndex = -1;
    ////private int CP_Interstitial_lastShownIndex = -1;
    ////[HideInInspector] public GameObject[] CPMRec;
    ////[HideInInspector] public GameObject[] CPSmallBanner;
    ////[HideInInspector] public GameObject[] CPinter;
    ////private int CP_MRec_lastShownIndex = -1;
    //#endregion
    [HideInInspector] public GameObject loadingscreen, RewardLoadingPanel;
    #endregion 
    #region SolarEngine
    void SE_Initialize()
    {

        Debug.Log("[unity] init click");

        String AppKey = "b9f994c9186a3c0e";

        SEConfig seConfig = new SEConfig();
        RCConfig rcConfig = new RCConfig();
        //SEConfig isDebugModel = new SEConfig();
        seConfig.logEnabled = true;
        //seConfig.isDebugModel = true;
        rcConfig.enable = true;
        SolarEngine.Analytics.SESDKInitCompletedCallback initCallback = new SolarEngine.Analytics.SESDKInitCompletedCallback(initSuccessCallback);
        seConfig.initCompletedCallback = initCallback;
        SolarEngine.Analytics.preInitSeSdk(AppKey);
        SolarEngine.Analytics.initSeSdk(AppKey, seConfig, rcConfig);

    }
    
    public void OpenPremium()
    {
        StartCoroutine(OpenPremiumIAP());
    }
    IEnumerator OpenPremiumIAP()
    {
        inappPremium.SetActive(true);
        noThanksBtn.SetActive(false);
        remainingAttempts = (int)IAP_Attempts;
        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("premium_panel_shown");
        yield return new WaitForSeconds(2);
        noThanksBtn.SetActive(true);   
    }
    public void ShowCustomInterstitial()
    {
        if (AdsManagerWrapper.Instance)
        {
            if (AdsManagerWrapper.Instance.isIdleTimeCompleted)
            {
                StartCoroutine(AdsManagerWrapper.Instance.ShowInterAdWithFadeIn());
            }
        }
    }
    public void trackAdClick(MaxSdkBase.AdInfo maxAd, SolarAdType format)
    {
        Debug.Log("[unity] trackAdClick click");

        AdClickAttributes AdClickAttributes = new AdClickAttributes();
        AdClickAttributes.ad_platform = maxAd.NetworkName;
        AdClickAttributes.mediation_platform = "MAX";
        AdClickAttributes.ad_id = maxAd.AdUnitIdentifier;
        AdClickAttributes.ad_type = (int)format;
        AdClickAttributes.checkId = "123";
        SolarEngine.Analytics.trackAdClick(AdClickAttributes);

    }

    public void trackIAP(ProductsAttributes productsAttributes)
    {

        SolarEngine.Analytics.trackIAP(productsAttributes);
        Debug.Log("Track IAP");

    }
    public void trackIAP(string productName, string productId, int quantity, string currency, string orderId, string payType, double amount)
    {
        Debug.Log("[unity] trackIAP click");

        ProductsAttributes productsAttributes = new ProductsAttributes();
        productsAttributes.product_name = productName;
        productsAttributes.product_id = productId;
        productsAttributes.product_num = quantity;
        productsAttributes.currency_type = currency;
        productsAttributes.order_id = orderId;
        productsAttributes.pay_type = payType;
        productsAttributes.pay_amount = amount;

        // Optional fields
        // productsAttributes.fail_reason = "fail_reason"; // Only if failed
        // productsAttributes.paystatus = SEConstant_IAP_PayStatus.SEConstant_IAP_PayStatus_success;

        SolarEngine.Analytics.trackIAP(productsAttributes);
        Debug.Log("Track IAP");
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        Debug.Log("Solar AD Impression " + impressionData.AdFormat);
        SolarAdType(impressionData.AdFormat);
        Debug.Log("[unity] trackAdImpression click");
        ImpressionAttributes impressionAttributes = new ImpressionAttributes();
        impressionAttributes.ad_platform = impressionData.NetworkName;
        impressionAttributes.ad_appid = "ad_appid";
        impressionAttributes.mediation_platform = "MAX";
        impressionAttributes.ad_id = impressionData.AdUnitIdentifier;
        SolarAdType adType = SolarAdType(impressionData.AdFormat);
        Debug.Log("Ad Type ::::::::::::::::::::::::::" + adType);
        //impressionAttributes.ad_type = impressionData.AdFormat;
        impressionAttributes.ad_type = (int)adType;
        impressionAttributes.ad_ecpm = impressionData.Revenue * 1000.00;
        impressionAttributes.currency_type = "USD";
        impressionAttributes.is_rendered = true;
        //SolarEngine.Analytics.trackIAI(impressionAttributes);
        SolarEngine.Analytics.trackAdImpression(impressionAttributes);
        Debug.Log("Track IAAAPP");

    }
    public SolarAdType SolarAdType(string adFormat)
    {
        Debug.Log("Solar Ad Type Format : " + adFormat);
        switch (adFormat)
        {
            case "INTER":
                return global::SolarAdType.Interstitial_Ads;
            case "REWARDED":
                return global::SolarAdType.Rewarded_Video_Ads;
            case "BANNER":
                return global::SolarAdType.Banner_Ads;
            case "MREC":
                return global::SolarAdType.Mrec_Ads;
            case "APPOPEN":
                return global::SolarAdType.AppOpen_Ads;
            default:
                throw new ArgumentException($"Unsupported ad format: {adFormat}");
        }
    }
    private void initSuccessCallback(int code)
    {
        Debug.Log("SEUnity:initSuccessCallback  code : " + code);
        Debug.Log("Rbiaaaaaaaa Testing.........////");

    }

    #endregion
    #region Init
    private void Awake()
    {
        lastInterstitialTime = Time.unscaledTime - INTERSTITIAL_COOLDOWN;
        lastAppOpenAdTime = Time.unscaledTime - APP_OPEN_COOLDOWN;
        lastRewardedTime = Time.unscaledTime - 999f;







        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Instance = this;
        DontDestroyOnLoad(this.gameObject);


        if (Instance != null && Instance != this)
        {

            Debug.Log("Object is destroying ");
            Destroy(this.gameObject);
            return;
        }

    }
    private void Start()
    {

        SE_Initialize();
        PlayerPrefs.SetInt("firstQuiz", 0);
        timeForInterstitialAd = idleAdTime;
        solidAdTime = timeForInterstitialAd+InterGraceTime;

        welcomInter = false;
        //StartCoroutine(ShowDelayInterstitial());
        Invoke(nameof(SetAppOpenTime), 5);
        if (Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork && Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            Constant.LogDesignEvent("User_Without_Internet");

        }
        PreferenceManager.PrivacyPolicyAgreeStatus = 1;
        if (PreferenceManager.PrivacyPolicyAgreeStatus == 1)
            Initialize();                                                      //TO CHECK THAT INETERNET

        //if (Data.RemoveAds == 1)
        //{
        //    adsStatus = AdType.NOAds;
        //}

        if (PlayerPrefs.GetInt("RemoveAds") == 1)
        {
            adsStatus = AdType.NOAds;
        }

        //OnAppOpenAdShown();
        //OnInterstitialShown();
        //OnRewardedVideoShown();



    }

    void SetAppOpenTime()
    {
        OnAdClosed((int)startAppOpenTime);

        isIdleTimeCompleted = false;
        //if (/*SoundAPPManager.ins.*/timeForInterstitialAd <= 15)
        {
            /*SoundAPPManager.ins.*/
            
            timeForInterstitialAd = idleAdTime;
            solidAdTime = timeForInterstitialAd + InterGraceTime;
        }


    }

    public void BuyUnlockEverything()
    {
        AdsManagerWrapper.ForeGroundedAD = true;
        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("premium_panel_clicked");


        IAP_Manager.Instance.Unlock_All_Guns();
    }
    public void BackOnline()
    {
        //if(!isAppLovinInitiallized)
            //Initialize();
        ReTryToInitialize();
        //ShowSmallBanner();
    }
    public void Initialize()
    {
        Logging.Log("GG >> AppLovin:AttemptingInit");
        AdsLogsHelper.Logs(Ads_Events.Initializing);
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                isAppLovinInitiallized = true;
                Logging.Log("GG >> AppLovin:Initialized");
                AdsLogsHelper.Logs(Ads_Events.Initialized);
                InitializeSmallBannerAds(MaxSdkBase.BannerPosition.TopCenter);               // just for testing
                InitializeMRecAds(MaxSdkBase.AdViewPosition.BottomLeft);               // just for testing
                InitializeInterstitialAds();               // just for testing
                InitializeRewardedAds();                  // just for testing
                InitializeAppOpenAds();                   // just for testing
                if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("applovin_Initialized");
            };

            MaxSdk.SetSdkKey(SdkKey);
            //  MaxSdk.SetTestDeviceAdvertisingIdentifiers(TestDevices);
            MaxSdk.InitializeSdk();
        }
        else
        {
            Logging.Log("GG >> AppLovin:NoInternet");
            AdsLogsHelper.Logs(Ads_Events.AdapterNotReadyNoInternet);
        }
    }

    public void ReTryToInitialize()
    {
        if (IfCanTryToInitializeAgain)
        {
            IfCanTryToInitializeAgain = false;
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork | Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
                {
                    isAppLovinInitiallized = true;
                    Logging.Log("GG >> AppLovin:Initialized");
                    AdsLogsHelper.Logs(Ads_Events.Initialized);
                    InitializeSmallBannerAds(MaxSdkBase.BannerPosition.TopCenter);               // just for testing
                    InitializeMRecAds(MaxSdkBase.AdViewPosition.BottomLeft);               // just for testing
                    InitializeInterstitialAds();               // just for testing
                    InitializeRewardedAds();               // just for testing
                    // InitializeAppOpenAds();               // this line was already commented
                };

                MaxSdk.SetSdkKey(SdkKey);
                MaxSdk.SetTestDeviceAdvertisingIdentifiers(TestDevices);
                MaxSdk.InitializeSdk();

            }
            else
            {
                Logging.Log("GG >> AppLovin:NoInternet");
                AdsLogsHelper.Logs(Ads_Events.AdapterNotReadyNoInternet);
            }

            Invoke(nameof(cantryAgain), 20f);
        }

    }

    void cantryAgain()
    {
        IfCanTryToInitializeAgain = true;
    }
    #endregion
    #region AppOpen



    int appOpenCooldown = 60;
    private float lastAdClosedTime = -1f;
    public bool canShowAppOpen = false;
    private Coroutine appOpenTimerCoroutine;
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                return;


            if (!ForeGroundedAD)
            {
                if (canShowAppOpen)
                {
                    //if (AdCooldownManager.Instance.CanShowAppOpenAd())
                    //if (CanShowAppOpenAd())
                    {
                        if (IsAppOpenAdReady())
                        {
                            HideSmallBannerEvent();
                            Invoke("showbannerAfterAppopen", 2.5f);
                        }
                        ShowAppOpenAdIfReady();
                    }
                }
            }
            else
                ForeGroundedAD = false;
        }
    }

    public void OnAdClosed(int time)
    {
        lastAdClosedTime = Time.realtimeSinceStartup;
        canShowAppOpen = false;
        if (appOpenTimerCoroutine != null)
            StopCoroutine(appOpenTimerCoroutine);
        appOpenTimerCoroutine = StartCoroutine(StartAppOpenTimer(time));
    }

    public IEnumerator StartAppOpenTimer(int time)
    {
        yield return new WaitForSeconds(time);
        canShowAppOpen = true;
    }
    public float GetAppOpenRemainingTime()
    {
        if (lastAdClosedTime < 0)
            return 0f;

        float elapsed = Time.realtimeSinceStartup - lastAdClosedTime;
        return Mathf.Max(0f, appOpenCooldown - elapsed);
    }
    void showbannerAfterAppopen()
    {
        // ShowSmallBanner();
    }
    public void LoadAppOpenAd()
    {
        if (!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || appOpenAdStatus == AdLoadingStatus.Loading || IsAppOpenAdReady() || adsStatus == AdType.NOAds)
            return;
        appOpenAdStatus = AdLoadingStatus.Loading;
        Logging.Log("GG >> AppLovin:AppOpen:LoadRequest");
        AdsLogsHelper.Logs(Ads_Events.AppOpenLoadRequest);
        MaxSdk.LoadAppOpenAd(AppOpenID);
    }
    private bool showAdmobAppOpenNext = true;
    public void ShowAppOpenAdIfReady()
    {
        if (!isAppLovinInitiallized)
            GRS_FirebaseHandler.Instance.LogEventPlay("appOpen_call_offline");

        if (!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || adsStatus == AdType.NOAds)
            return;

        //if (showAdmobAppOpenNext)
        //{
            //if (AdmobeAdsManager.instance != null && AdmobeAdsManager.instance.IsAppOpenAdReady())
            {
                Debug.Log("Showing AdMob AppOpen Ad");
                ForeGroundedAD = true;
                AdmobeAdsManager.instance.ShowAppOpenAd();
                //GRS_FirebaseHandler.Instance.LogEventPlay("admob_appopen_shown");
            }
        //    else
        //    {
        //        Debug.Log("AdMob AppOpen not loaded, fallback to MAX");
        //        ShowMaxAppOpenAd();
        //    }
        //}
        //else
        //{
        //    ShowMaxAppOpenAd();
        //}

        //showAdmobAppOpenNext = !showAdmobAppOpenNext;
    }
    private void ShowMaxAppOpenAd()
    {
        if (IsAppOpenAdReady())
        {
            Logging.Log("GG >> AppLovin:AppOpen:WillDisplay");
            AdsLogsHelper.Logs(Ads_Events.AppOpenWillDisplay);
            ForeGroundedAD = true;

            MaxSdk.ShowAppOpenAd(AppOpenID);
            OnAppOpenAdShown();
            GRS_FirebaseHandler.Instance.LogEventPlay("appopen_ad_shown");
        }
        else
        {
            Logging.Log("GG >> AppLovin:AppOpen:NotLoaded");
            AdsLogsHelper.Logs(Ads_Events.AppOpenNotLoaded);
            LoadAppOpenAd();
            GRS_FirebaseHandler.Instance.LogEventPlay("Appopen_Notloaded");
        }
    }
    //public void ShowAppOpenAdIfReady()
    //{

    //    if (!isAppLovinInitiallized) GRS_FirebaseHandler.Instance.LogEventPlay("appOpen_call_offline");


    //    if (!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || adsStatus == AdType.NOAds)
    //        return;
    //    Logging.Log("GG >> AppLovin:AppOpen:Showcall");
    //    AdsLogsHelper.Logs(Ads_Events.AppOpenShowCall);
    //    if (IsAppOpenAdReady())
    //    {
    //        Logging.Log("GG >> AppLovin:AppOpen:WillDisplay");

    //        AdsLogsHelper.Logs(Ads_Events.AppOpenWillDisplay);
    //        ForeGroundedAD = true;
    //        MaxSdk.ShowAppOpenAd(AppOpenID);
    //        //AdCooldownManager.Instance.ShowAppOpenAd();
    //        OnAppOpenAdShown();

    //        GRS_FirebaseHandler.Instance.LogEventPlay("appopen_ad_shown");

    //    }
    //    else
    //    {
    //        Logging.Log("GG >> AppLovin:AppOpen:NotLoaded");
    //        AdsLogsHelper.Logs(Ads_Events.AppOpenNotLoaded);
    //        LoadAppOpenAd();
    //        GRS_FirebaseHandler.Instance.LogEventPlay("Appopen_Notloaded");

    //    }
    //}
    #endregion
    #region BindEAdvents
    public void InitializeSmallBannerAds(MaxSdkBase.BannerPosition AdPosition)
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        //MaxSdk.SetBannerWidth(BannerID,468);


        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAllAdsRevenuePaidEvent;
        // MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        // MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
        LoadSmallBanner(AdPosition);
    }

    public void InitializeMRecAds(MaxSdkBase.AdViewPosition AdPosition)
    {
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAllAdsRevenuePaidEvent;
        // MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        //MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
        // MRECs are sized to 300x250 on phones and tablets
        LoadMediumBanner(AdPosition);
    }
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAllAdsRevenuePaidEvent;
        // Load the first interstitial
        LoadInterstitial();
    }
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAllAdsRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }
    public void InitializeAppOpenAds()
    {
        MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
        MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFailedEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;
        MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenClickedEvent;
        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenAdFailedToDisplayEvent;
        MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAllAdsRevenuePaidEvent;
        LoadAppOpenAd();
    }
    #endregion
    #region SmallBannerAdsHandler
    private void OnBannerAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        smallBannerStatus = AdLoadingStatus.Loaded;
        Logging.Log("GG >> AppLovin:smallBanner:Loaded");
        AdsLogsHelper.Logs(Ads_Events.SmallBannerLoaded);
        isSmallBannerLoaded = true;
        //ShowSmallBanner();
        // if (isCP_SM_Showing) { ShowSmallBanner(); }
    }
    private void OnBannerAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        smallBannerStatus = AdLoadingStatus.NoInventory;
        Logging.Log("GG >> AppLovin:smallBanner:NoInventory");
        AdsLogsHelper.Logs(Ads_Events.SmallBannerNoInventory);
        ShowSmallBanner();
        currentBanner = BannerSource.AdMob;
        //  isSmallBannerLoaded = false;
    }
    private void OnBannerAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        Logging.Log("GG >> AppLovin:smallBanner:Clicked");
        AdsLogsHelper.Logs(Ads_Events.SmallBannerClicked);
        SolarAdType adType = SolarAdType(arg2.AdFormat);
        trackAdClick(arg2, adType);
    }
    #endregion
    #region MediumBannerAdEvents
    private void OnMRecAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        mediumBannerStatus = AdLoadingStatus.Loaded;
        Logging.Log("GG >> AppLovin:mediumBanner:Loaded");
        AdsLogsHelper.Logs(Ads_Events.MediumBannerLoaded);
        isMediumBannerLoaded = true;
    }
    private void OnMRecAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        mediumBannerStatus = AdLoadingStatus.NoInventory;
        Logging.Log("GG >> AppLovin:mediumBanner:NoInventory");
        AdsLogsHelper.Logs(Ads_Events.MediumBannerNoInventory);
        // isMediumBannerLoaded = false;
    }
    private void OnMRecAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        Logging.Log("GG >> AppLovin:mediumBanner:Clicked");
        AdsLogsHelper.Logs(Ads_Events.MediumBannerClicked);
        
        SolarAdType adType = SolarAdType(arg2.AdFormat);
        trackAdClick(arg2, adType);
    }
    #endregion
    #region InterstitialAds
    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttemptIAD = 0;
        Logging.Log("GG >> AppLovin:iad:Loaded");
        AdsLogsHelper.Logs(Ads_Events.InterstitialAdLoaded);
        iAdStatus = AdLoadingStatus.Loaded;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        iAdStatus = AdLoadingStatus.NoInventory;
        retryAttemptIAD++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptIAD));
        Logging.Log("GG >> AppLovin:iad:NoInventory");
        AdsLogsHelper.Logs(Ads_Events.InterstitialAdNoInventory);
        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        iAdStatus = AdLoadingStatus.NotLoaded;
        Logging.Log("GG >> AppLovin:iad:Displayed");
        AdsLogsHelper.Logs(Ads_Events.InterstitialAdDisplayed);
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        iAdStatus = AdLoadingStatus.NotLoaded;
        Logging.Log("GG >> AppLovin:iad:FailedToShow");
        AdsLogsHelper.Logs(Ads_Events.InterstitialAdFailedToShow);
        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("Interstial_max_failedToShow");
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Logging.Log("GG >> AppLovin:iad:Clicked");
        AdsLogsHelper.Logs(Ads_Events.InterstitialAdClicked);
        SolarAdType adType = SolarAdType(adInfo.AdFormat);
        trackAdClick(adInfo, adType);
    }
    int appOpenCounter = 1;
    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        iAdStatus = AdLoadingStatus.NotLoaded;
        // Interstitial ad is hidden. Pre-load the next ad.
        Logging.Log("GG >> AppLovin:iad:Closed");
        AdsLogsHelper.Logs(Ads_Events.InterstitialAdClosed);
        /* AdsManagerWrapper.Instance.*/
        OnInterstitialShown();

        if (appOpenAfterIad)
        {
            if (appOpenCounter >= appOpenCount)
            {
                AdmobeAdsManager.instance.ShowAppOpenAd();
                appOpenCount = 1;
            }
            else { appOpenCount++; }
        }
        
        isIdleTimeCompleted = false;
        //ResetTimer();
        timeForInterstitialAd = idleAdTime;
        solidAdTime = timeForInterstitialAd+InterGraceTime;

        if (GetAppOpenRemainingTime() <= 15)
        {
            OnAdClosed(15);
        }
        LoadInterstitial();
    }
    #endregion
    #region RewardedAdsHandler
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        rAdStatus = AdLoadingStatus.Loaded;
        // Reset retry attempt
        Logging.Log("GG >> AppLovin:rad:Loaded");
        AdsLogsHelper.Logs(Ads_Events.RewardedAdLoaded);
        retryAttempt = 0;
        RewardVideoLoaded();
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        rAdStatus = AdLoadingStatus.NoInventory;
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Logging.Log("GG >> AppLovin:rad:NoInventory");
        AdsLogsHelper.Logs(Ads_Events.RewardedAdNoInventory);
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        rAdStatus = AdLoadingStatus.NotLoaded;
        Logging.Log("GG >> AppLovin:rad:Displayed");
        AdsLogsHelper.Logs(Ads_Events.RewardedAdDisplayed);

        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.OnRewardedVideoDisplayEvent();

    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Logging.Log("GG >> AppLovin:rad:FailedToShow");
        rAdStatus = AdLoadingStatus.NotLoaded;
        AdsLogsHelper.Logs(Ads_Events.RewardedAdFailedToShow);
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Logging.Log("GG >> AppLovin:rad:Clicked");
        AdsLogsHelper.Logs(Ads_Events.RewardedAdClicked);
        SolarAdType adType = SolarAdType(adInfo.AdFormat);
        trackAdClick(adInfo, adType);
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        rAdStatus = AdLoadingStatus.NotLoaded;
        // Rewarded ad is hidden. Pre-load the next ad
        Logging.Log("GG >> AppLovin:rad:Closed");
        AdsLogsHelper.Logs(Ads_Events.RewardedAdClosed);
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        Logging.Log("GG >> AppLovin:rad:GiveRewardToUser");

        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.RewardedVideoComppleteEvent();




        AdsLogsHelper.Logs(Ads_Events.RewardedAdReward);
        //if (SoundAPPManager.ins)
        {
            isIdleTimeCompleted = false;
            if (/*SoundAPPManager.ins.*/timeForInterstitialAd <= 15)
            {
                /*SoundAPPManager.ins.*/
                timeForInterstitialAd = 15;
                solidAdTime = timeForInterstitialAd+InterGraceTime;
            }
        }

        //AdCooldownManager.Instance.OnRewardedAdCompleted();
        OnRewardedVideoShown();
        if (GetAppOpenRemainingTime() <= 15)
        {
            OnAdClosed(15);
        }

        NotifyReward();

        Time.timeScale = 1;
        Debug.Log("Rewarded Video Completed Rewarded");






    }

    private void OnAllAdsRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        //if (Constant.firebaseInitialized)
        { // Ad revenue paid. Use this callback to track user revenue.
            double revenue = impressionData.Revenue;
            var impressionParameters = new[] {
        new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
        new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
        new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
        new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
        new Firebase.Analytics.Parameter("value", revenue),
        new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_max", impressionParameters);
        }
        Dictionary<string, string> additionalParams = new Dictionary<string, string>();

        Debug.Log("Solar AD Impression " + impressionData.AdFormat);
        SolarAdType(impressionData.AdFormat);
        Debug.Log("[unity] trackAdImpression click");
        ImpressionAttributes impressionAttributes = new ImpressionAttributes();
        impressionAttributes.ad_platform = impressionData.NetworkName;
        impressionAttributes.ad_appid = "ad_appid";
        impressionAttributes.mediation_platform = "MAX";
        impressionAttributes.ad_id = impressionData.AdUnitIdentifier;
        SolarAdType adType = SolarAdType(impressionData.AdFormat);
        Debug.Log("Ad Type ::::::::::::::::::::::::::" + adType);
        //impressionAttributes.ad_type = impressionData.AdFormat;
        impressionAttributes.ad_type = (int)adType;
        impressionAttributes.ad_ecpm = impressionData.Revenue * 1000.00;
        impressionAttributes.currency_type = "USD";
        impressionAttributes.is_rendered = true;
        //SolarEngine.Analytics.trackIAI(impressionAttributes);
        SolarEngine.Analytics.trackAdImpression(impressionAttributes);
        Debug.Log("Track IAAAPP");
    }

    void RewardVideoLoaded()
    {
        Logging.Log("Reward video Ready to Show");
        if (Reward_Video_Loading.ins)
        {
            Reward_Video_Loading.ins.Loading_Time = 0.5f;
        }
    }
    #endregion
    #region AppOpen Ads Handler

    private void OnAppOpenLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        appOpenAdStatus = AdLoadingStatus.Loaded;
        Logging.Log("GG >> AppLovin:AppOpen:Loaded");
        AdsLogsHelper.Logs(Ads_Events.AppOpenLoaded);
        //if (!Constant.firstAppOpenShown)
        //{
        //    Constant.firstAppOpenShown = true;
        //    ShowAppOpenAdIfReady();
        //}
    }

    private void OnAppOpenLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        appOpenAdStatus = AdLoadingStatus.NoInventory;
        Logging.Log("GG >> AppLovin:AppOpen:NoInventory");
        AdsLogsHelper.Logs(Ads_Events.AppOpenNoInventory);
    }

    private void OnAppOpenDisplayedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        appOpenAdStatus = AdLoadingStatus.NotLoaded;
        Logging.Log("GG >> AppLovin:AppOpen:Displayed");
        AdsLogsHelper.Logs(Ads_Events.AppOpenDisplayed);
    }

    private void OnAppOpenClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        Logging.Log("GG >> AppLovin:AppOpen:Clicked");
        AdsLogsHelper.Logs(Ads_Events.AppOpenClicked);
        SolarAdType adType = SolarAdType(arg2.AdFormat);
        trackAdClick(arg2, adType);
    }

    private void OnAppOpenAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo arg2, MaxSdkBase.AdInfo arg3)
    {
        Logging.Log("GG >> AppLovin:AppOpen:FailedToShow");
        AdsLogsHelper.Logs(Ads_Events.AppOpenFailedToShow);
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        appOpenAdStatus = AdLoadingStatus.NotLoaded;
        Logging.Log("GG >> AppLovin:AppOpen:Closed");
        AdsLogsHelper.Logs(Ads_Events.AppOpenClosed);
        ShowSmallBanner();
        LoadAppOpenAd();

        isIdleTimeCompleted = false;
        if (/*SoundAPPManager.ins.*/timeForInterstitialAd <= 15)
        {
            /*SoundAPPManager.ins.*/
            timeForInterstitialAd = 15;
            solidAdTime = timeForInterstitialAd + InterGraceTime    ;
        }


        //OnAdClosed(60);
        OnAdClosed((int)startAppOpenTime);
    }
    #endregion
    #region Load Ads
    public void LoadSmallBanner(MaxSdkBase.BannerPosition AdPosition)
    {
        if ((!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || smallBannerStatus == AdLoadingStatus.Loading || IsSmallBannerReady() || adsStatus == AdType.NOAds) && !Application.isEditor)
            return;
        Logging.Log("GG >> AppLovin:smallbanner:LoadRequest");
        AdsLogsHelper.Logs(Ads_Events.LoadSmallBanner);
        //MaxSdk.LoadBanner(BannerID);
        smallBannerStatus = AdLoadingStatus.Loading;
        //MaxSdkUtils.GetAdaptiveBannerHeight(-1);
        MaxSdk.CreateBanner(BannerID, AdPosition);

        MaxSdk.SetBannerExtraParameter(BannerID, "adaptive_banner", "false");
    }
    public void LoadMediumBanner(MaxSdkBase.AdViewPosition AdPosition)
    {
        if ((!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || mediumBannerStatus == AdLoadingStatus.Loading || IsMediumBannerReady() || adsStatus == AdType.NOAds) && !Application.isEditor)
            return;
        mediumBannerStatus = AdLoadingStatus.Loading;
        Logging.Log("GG >> AppLovin:mediumbanner:LoadRequest");
        AdsLogsHelper.Logs(Ads_Events.LoadMediumBanner);
        MaxSdk.CreateMRec(RectBannerID, AdPosition);
        //MaxSdk.LoadMRec(RectBannerID);
    }
    public void LoadInterstitial()
    {
        if (!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || iAdStatus == AdLoadingStatus.Loading || IsInterstitialAdReady() || adsStatus == AdType.NOAds)
            return;
        iAdStatus = AdLoadingStatus.Loading;
        Logging.Log("GG >> AppLovin:iad:LoadRequest"); ;
        AdsLogsHelper.Logs(Ads_Events.LoadInterstitial);
        MaxSdk.LoadInterstitial(InterID);
    }
    public void LoadRewardedAd()
    {
        if (!isAppLovinInitiallized || rAdStatus == AdLoadingStatus.Loading || IsRewardedAdReady())
            return;
        rAdStatus = AdLoadingStatus.Loading;
        Logging.Log("GG >> AppLovin:rad:LoadRequest");
        AdsLogsHelper.Logs(Ads_Events.LoadRewardedAd);
        MaxSdk.LoadRewardedAd(RewardedID);
    }
    public bool IsSmallBannerReady()
    {
        return isSmallBannerLoaded;
    }
    public bool IsMediumBannerReady()
    {
        return isMediumBannerLoaded;
    }
    public bool IsInterstitialAdReady()
    {
        return MaxSdk.IsInterstitialReady(InterID);
    }
    public bool IsRewardedAdReady()
    {
        return MaxSdk.IsRewardedAdReady(RewardedID);
    }
    public bool IsAppOpenAdReady()
    {
        return MaxSdk.IsAppOpenAdReady(AppOpenID);
    }
    #endregion
    #region Show Ads
    [HideInInspector] public enum BannerSource { None, AdMob, Max }
    [HideInInspector] public BannerSource currentBanner = BannerSource.None;
    private bool showAdmobBannerNext = true; // toggle for alternating

    public void ShowSmallBanner()
    {

        Debug.Log("Show small banner called ");

        if (!PreferenceManager.GetAdsStatus() || adsStatus == AdType.NOAds)
            return;

        if (!isAppLovinInitiallized)
        {
            GRS_FirebaseHandler.Instance.LogEventPlay("banner_call_offline");
            return;
        }

        // Pick random 0 or 1 (50% chance each)
        //bool showAdmobBanner = UnityEngine.Random.Range(0, 2) == 0;
        HideCurrentBanner();
        //if (showAdmobBanner)
        //{
        //    if (AdmobeAdsManager.instance != null /*&& AdmobeAdsManager.instance.IsBannerReady()*/)
        //    {
        //        Debug.Log("Showing AdMob Small Banner");
        //        //AdmobeAdsManager.instance.ShowBanner();
        //        AdmobeAdsManager.instance.LoadBanner();
        //        currentBanner = BannerSource.AdMob;
        //        GRS_FirebaseHandler.Instance.LogEventPlay("admob_banner_shown");
        //    }
        //    //else
        //    //{
        //    //    Debug.Log("AdMob Banner not ready, fallback to MAX");
        //    //    ShowMaxBanner();
        //    //    currentBanner = BannerSource.Max;
        //    //}
        //}
        //else
        //{
            ShowMaxBanner();
            currentBanner = BannerSource.Max;
        //}

        //showAdmobBannerNext = !showAdmobBannerNext;
    }
    private void HideCurrentBanner()
    {
        HideBothBanners();
        //switch (currentBanner)
        //{
        //    case BannerSource.AdMob:
        //        if (AdmobeAdsManager.instance != null)
        //        {
        //            AdmobeAdsManager.instance.HideBanner();
        //            Debug.Log("AdMob Banner Hidden");
        //        }
        //        break;

        //    case BannerSource.Max:
        //        HideMaxBanner(); // implement this if not already
        //        Debug.Log("MAX Banner Hidden");
        //        break;
        //}

        currentBanner = BannerSource.None;
    }
    public void HideBothBanners()
    {
        HideMaxBanner();
        AdmobeAdsManager.instance.HideBanner();
    }
    public void ShowMaxBanner()
    {
        if (isAppLovinInitiallized)
            MaxSdk.HideBanner(BannerID);

        Logging.Log("GG >> AppLovin:smallbanner:Showcall");
        if (IsSmallBannerReady() || Application.isEditor)
        {
            if (GRS_FirebaseHandler.Instance)
                GRS_FirebaseHandler.Instance.LogEventPlay("banner_ad_shown");

            Logging.Log("GG >> AppLovin:smallbanner:WillDisplay");
            isSBannerDisplayed = true;
            MaxSdk.ShowBanner(BannerID);
        }
        else
        {
            if(AdmobeAdsManager.instance)
                AdmobeAdsManager.instance.ShowBanner();
            //Logging.Log("GG >> AppLovin:smallBanner:NotLoaded");
        }
    }
    //public void ShowSmallBanner(/*MaxSdkBase.BannerPosition AdPositio*/)
    //{
    //    if (!PreferenceManager.GetAdsStatus() || adsStatus == AdType.NOAds)
    //        return;

    //    if (!isAppLovinInitiallized)
    //    {
    //        GRS_FirebaseHandler.Instance.LogEventPlay("banner_call_offline");
    //        return;
    //    }
    //    if (isAppLovinInitiallized)
    //    { MaxSdk.HideBanner(BannerID); }

    //    Logging.Log("GG >> AppLovin:smallbanner:Showcall");
    //    //    AdsLogsHelper.Logs(Ads_Events.ShowSmallBanner);
    //    if (IsSmallBannerReady() || Application.isEditor)
    //    {
    //        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("banner_ad_shown");
    //        Logging.Log("GG >> AppLovin:smallbanner:WillDisplay");
    //        //   AdsLogsHelper.Logs(Ads_Events.SmallBannerWillDisplay);
    //        isSBannerDisplayed = true;
    //        MaxSdk.ShowBanner(BannerID);
    //    }
    //    else
    //    {
    //        Logging.Log("GG >> AppLovin:smallBanner:NotLoaded");
    //        //  AdsLogsHelper.Logs(Ads_Events.SmallBannerNotLoaded);
    //    }
    //}
    private bool showAdmobMediumBannerNext = false; // toggle flag

    public void ShowMediumBanner()
    {
        if (!PreferenceManager.GetAdsStatus() || adsStatus == AdType.NOAds)
            return;

        if (!isAppLovinInitiallized)
        {
            GRS_FirebaseHandler.Instance.LogEventPlay("Mrec_call_offline");
            return;
        }

        // Always hide existing banners before showing new one
        MaxSdk.HideMRec(RectBannerID);
        if (AdmobeAdsManager.instance != null)
            AdmobeAdsManager.instance.HideMediumBanner();

        Debug.Log("Medium banner requested");

        // Flip a coin (random 50/50 chance) OR use toggle flag
        bool useAdmob = UnityEngine.Random.Range(0, 2) == 0;
        // OR if you prefer toggle sequence, replace with:
        // bool useAdmob = showAdmobMediumBannerNext;

        //if (useAdmob)
        //{
        //    if (AdmobeAdsManager.instance != null /*&& AdmobeAdsManager.instance.IsMediumBannerReady()*/)
        //    {
        //        Debug.Log("Showing AdMob Medium Banner");
        //        AdmobeAdsManager.instance.ShowMREC();
        //        GRS_FirebaseHandler.Instance.LogEventPlay("admob_mrec_shown");
        //    }
        //    else
        //    {
        //        Debug.Log("AdMob Medium Banner not ready, fallback to MAX");
        //        ShowMaxMediumBanner();
        //    }
        //}
        //else
        //{
            ShowMaxMediumBanner();
        //}

        // showAdmobMediumBannerNext = !showAdmobMediumBannerNext;
    }

    public void ShowMaxMediumBanner()
    {
        if (IsMediumBannerReady() || Application.isEditor)
        {
            Logging.Log("GG >> AppLovin:mediumBanner:WillDisplay");
            isMBannerDisplayed = true;
            MaxSdk.ShowMRec(RectBannerID);
        }
        else
        {
            if (AdmobeAdsManager.instance != null)
            {
                Debug.Log("Fallback: Showing AdMob MREC");
                AdmobeAdsManager.instance.ShowMREC();
            }
                Logging.Log("GG >> AppLovin:mediumBanner:NotLoaded");
        }
    }


    //public void ShowMediumBanner(/*MaxSdkBase.AdViewPosition AdPosition*/)
    //{
    //    if (!PreferenceManager.GetAdsStatus() || adsStatus == AdType.NOAds)
    //        return;


    //    if (!isAppLovinInitiallized) GRS_FirebaseHandler.Instance.LogEventPlay("Mrec_call_offline");



    //    if (isAppLovinInitiallized)
    //    { MaxSdk.HideMRec(RectBannerID); }


    //    Debug.Log("Medium banner shown");
    //    Logging.Log("GG >> AppLovin:mediumBanner:Showcall");
    //    // Debug.Log("GG >> AppLovin:mediumBanner:Showcall");
    //    //  AdsLogsHelper.Logs(Ads_Events.ShowMediumBanner);
    //    if (IsMediumBannerReady() || Application.isEditor)
    //    {
    //        Logging.Log("GG >> AppLovin:mediumBanner:WillDisplay");
    //        //   Debug.Log("GG >> AppLovin:mediumBanner:WillDisplay");
    //        //  AdsLogsHelper.Logs(Ads_Events.MediumBannerWillDisplay);
    //        isMBannerDisplayed = true;
    //        MaxSdk.ShowMRec(RectBannerID);
    //    }
    //    else
    //    {
    //        Logging.Log("GG >> AppLovin:mediumBanner:NotLoaded");
    //        //   Debug.Log("GG >> AppLovin:mediumBanner:NotLoaded");
    //        //   AdsLogsHelper.Logs(Ads_Events.MediumBannerNotLoaded);
    //        //ShowCrossPermotion_MediumBanner();
    //    }
    //}

    bool isInternetAvailable;
    public bool IsInternetConnection()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable ||
            Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            isInternetAvailable = true;
        }
        else
            isInternetAvailable = false;

        return isInternetAvailable;
    }

    public IEnumerator ShowInterAdWithFadeIn()
    {
        //if (IsInterstitialAdReady())
        //{
            //if (!startFading)
            //{
            //    startFading = true;
            //    StartCoroutine(FadeInPopup(interstitialPopup.GetComponent<CanvasGroup>(), 1.5f));
            //    interstitialPopup.SetActive(true);
            //    adTimer.gameObject.SetActive(true);

            //}
        //}

        //adTimer.text = "AD IS LOADING...2 ";
        yield return new WaitForSeconds(0.01f);
        //adTimer.text = "AD IS LOADING...1 ";
        //yield return new WaitForSeconds(0.5f);
        //ShowInterstitial();
        ShowInterstitialAlternate();
        interstitialPopup.SetActive(false);
        adTimer.gameObject.SetActive(false);
        startFading = false;
    }

    private static bool showAdmobNext = true;

    public void ShowInterstitialAlternate()
    {

        //ShowInterstitial();

        //if (showAdmobNext)
        //{
        //    Debug.Log("Showing AdMob Interstitial");
        //    if (AdmobeAdsManager.instance != null && AdmobeAdsManager.instance.IsInterstitial2Ready())
        //    {

        //        AdmobeAdsManager.instance.ShowInterstitial2();
        //    }
        //    else
        //    {
        //        Debug.Log("AdMob interstitial not ready, fallback to MAX");
        //        ShowInterstitial();
        //        AdmobeAdsManager.instance.LoadInterstitial2();
        //    }
        //}
        //else
        //{
        Debug.Log("Showing MAX Interstitial");
        if (IsInterstitialAdReady())
        {
            ShowInterstitial();
        }
        else
        {
            Debug.Log("MAX interstitial not ready, fallback to AdMob");
            if (AdmobeAdsManager.instance != null/* && AdmobeAdsManager.instance.IsInterstitial2Ready()*/)
            {
                AdmobeAdsManager.instance.ShowInterstitial2();
            }
            LoadInterstitial();
        }
        //}

        //showAdmobNext = !showAdmobNext;

    }

    public void ShowFallBackInterstitialAlternate()
    {
        //ShowFallBackInterstitial();
        //if (showAdmobNext)
        //{
        //    Debug.Log("Showing AdMob Interstitial");
        //    if (AdmobeAdsManager.instance != null && AdmobeAdsManager.instance.IsInterstitial2Ready())
        //    {

        //        AdmobeAdsManager.instance.ShowInterstitial2();
        //    }
        //    else
        //    {
        //        Debug.Log("AdMob interstitial not ready, fallback to MAX");
        //        ShowFallBackInterstitial();
        //        AdmobeAdsManager.instance.LoadInterstitial2();
        //    }
        //}
        //else
        //{
            Debug.Log("Showing MAX Interstitial");
            if (IsInterstitialAdReady())
            {
                ShowFallBackInterstitial();
            }
            else
            {
                Debug.Log("MAX interstitial not ready, fallback to AdMob");
                if (AdmobeAdsManager.instance != null/* && AdmobeAdsManager.instance.IsInterstitial2Ready()*/)
                {
                    AdmobeAdsManager.instance.ShowInterstitial2();
                }
                LoadInterstitial();
            }
        //}

        //showAdmobNext = !showAdmobNext;
    }




    public void ShowInterstitial()
    {
        ResetTimer();
        interstitialCount++;

        if (interstitialCount == 2)
        {
            lastRemoveAdsTime = Time.time;
        }

        if (!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || adsStatus == AdType.NOAds)
        {
            if (!isAppLovinInitiallized)
            {
                ReTryToInitialize();              //     TO CHECK INTERNET CONNECTION
                GRS_FirebaseHandler.Instance.LogEventPlay("interstitial_call_Offline");
                return;
            }
        }
        Logging.Log("GG >> AppLovin:iad:Showcall");
        AdsLogsHelper.Logs(Ads_Events.ShowInterstitialAd);
        if (IsInterstitialAdReady())
        {
            Logging.Log("GG >> AppLovin:iad:WillDisplay");
            AdsLogsHelper.Logs(Ads_Events.InterstitialAdWillDisplay);
            ForeGroundedAD = true;
            MaxSdk.ShowInterstitial(InterID);
            InterstitialShowd = true;

            GRS_FirebaseHandler.Instance.LogEventPlay("interstitial_ad_shown");
        }
        else
        {
            Logging.Log("GG >> AppLovin:iad:NotLoaded");
            AdsLogsHelper.Logs(Ads_Events.InterstitialNotLoaded);
            LoadInterstitial();
            //GRS_FirebaseHandler.Instance.LogEventPlay("interstitial not loaded");
        }

    }



    public void ShowFallBackInterstitial()
    {
        ResetTimer();
        interstitialCount++;
        if (interstitialCount == 2)
        {
            lastRemoveAdsTime = Time.time;
        }
        if (!PreferenceManager.GetAdsStatus() || !isAppLovinInitiallized || adsStatus == AdType.NOAds)
        {
            if (!isAppLovinInitiallized)
            {
                ReTryToInitialize();              //     TO CHECK INTERNET CONNECTION
                GRS_FirebaseHandler.Instance.LogEventPlay("fallback_iad_call_Offline");
            }
            return;
        }
        //solidAdTime = timeForInterstitialAd+30;
        Logging.Log("GG >> AppLovin:iad:Showcall");
        AdsLogsHelper.Logs(Ads_Events.ShowInterstitialAd);

        if (IsInterstitialAdReady())
        {
            Logging.Log("GG >> AppLovin:iad:WillDisplay");
            AdsLogsHelper.Logs(Ads_Events.InterstitialAdWillDisplay);
            ForeGroundedAD = true;
            MaxSdk.ShowInterstitial(InterID);
            InterstitialShowd = true;

            GRS_FirebaseHandler.Instance.LogEventPlay("fallback_interstitial_shown");
            //FirebaseAnalytics.LogEvent();

        }
        else
        {
            Logging.Log("GG >> AppLovin:iad:NotLoaded");
            AdsLogsHelper.Logs(Ads_Events.InterstitialNotLoaded);
            LoadInterstitial();
            //GRS_FirebaseHandler.Instance.LogEventPlay("interstitial not loaded");
        }
    }
   



    public void ResetTimer()
    {
        isIdleTimeCompleted = false;
        timeForInterstitialAd = idleAdTime;
        solidAdTime = timeForInterstitialAd + InterGraceTime;

        if (GetAppOpenRemainingTime() <= 15)
        {
            OnAdClosed(15);
        }
    }

    //IEnumerator ShowDelayInterstitial(float expected_time)
    //{
    //    if ()
    //    {
    //        yield return new WaitForSeconds(expected_time);
    //        ShowInterstitial();
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(expected_time + 10f);
    //        ShowInterstitial();
    //    }

    //}
    private bool showAdMobNextRewarded = false; // toggle between MAX & AdMob
    //private RewardUserDelegate NotifyReward;

    public void ShowRewardedVideo(RewardUserDelegate _delegate)
    {
        if (!IsInternetConnection())
        {
            GRS_FirebaseHandler.Instance.LogEventPlay("Rewarded_call_Offline");
            StartCoroutine(CheckInternetConnection());
            return;
        }

        NotifyReward = _delegate;
        AdsLogsHelper.Logs(Ads_Events.ShowRewardedAd);

        //if (showAdMobNextRewarded)
        //{
        //    if (AdmobeAdsManager.instance.IsAdmobRewardedReady())
        //    {
        //        Debug.Log("Showing AdMob Rewarded Ad...");
        //        AdmobeAdsManager.instance.ShowRewarded(() =>
        //        {
        //            Debug.Log("AdMob: User rewarded");
        //            NotifyReward?.Invoke();
        //        });
        //        showAdMobNextRewarded = false; // Next time show MAX
        //        return;
        //    }
        //}
        //else
        //{
        //    if (IsRewardedAdReady())
        //    {
        //        Debug.Log("Showing MAX Rewarded Ad...");
        //        ForeGroundedAD = true;
        //        MaxSdk.ShowRewardedAd(RewardedID);
        //        RewardVideoShowed = true;
        //        showAdMobNextRewarded = true; // Next time show AdMob
        //        return;
        //    }
        //}

        if (IsRewardedAdReady())
        {
            Debug.Log("Fallback: Showing MAX Rewarded");
            MaxSdk.ShowRewardedAd(RewardedID);
            showAdMobNextRewarded = true;
            ForeGroundedAD = true;
            AdmobeAdsManager.instance.LoadRewarded();
        }
        else if (AdmobeAdsManager.instance.IsAdmobRewardedReady())
        {
            ForeGroundedAD = true;
            Debug.Log("Fallback: Showing AdMob Rewarded");
            AdmobeAdsManager.instance.ShowRewarded(() =>
            {
                NotifyReward?.Invoke();
            });
            showAdMobNextRewarded = false;
        }
        else
        {
            Debug.Log("No rewarded ads ready. Reloading...");
            AdsLogsHelper.Logs(Ads_Events.RewardedAdNotLoaded);

            LoadRewardedAd();                  // MAX load
            AdmobeAdsManager.instance.LoadRewarded(); // AdMob load
        }
    }


    //public void ShowRewardedVideo(RewardUserDelegate _delegate)
    //{
    //    if (!isAppLovinInitiallized || !IsInternetConnection())
    //    {
    //        GRS_FirebaseHandler.Instance.LogEventPlay("Rewarded_call_Offline");

    //        StartCoroutine(CheckInternetConnection());

    //        return;
    //    }
    //    Logging.Log("GG >> AppLovin:rad:Showcall");
    //    AdsLogsHelper.Logs(Ads_Events.ShowRewardedAd);


    //    if (IsRewardedAdReady())
    //    {
    //        NotifyReward = _delegate;

    //        Constant.LogDesignEvent("Show Rewarded AD");
    //        Logging.Log("GG >> AppLovin:rad:WillDisplay");
    //        AdsLogsHelper.Logs(Ads_Events.RewardedAdWillDisplay);
    //        ForeGroundedAD = true;
    //        MaxSdk.ShowRewardedAd(RewardedID);
    //        RewardVideoShowed = true;
    //    }
    //    else
    //    {
    //        Logging.Log("GG >> AppLovin:rad:NotLoaded");
    //        AdsLogsHelper.Logs(Ads_Events.RewardedAdNotLoaded);
    //        LoadRewardedAd();
    //    }
    //}


    #endregion
    #region HideBanners
    public void HideSmallBannerEvent()
    {


        HideBothBanners();





        if (isSBannerDisplayed)
        {
            Logging.Log("GG >> AppLovin:smallBanner:Hide");
            AdsLogsHelper.Logs(Ads_Events.SmallBannerHide);
        }
        isSBannerDisplayed = false;
        MaxSdk.HideBanner(BannerID);
        //Hide_CP_SmallBanner();
    }

    void HideMaxBanner()
    {
        isSBannerDisplayed = false;
        MaxSdk.HideBanner(BannerID);
    }
    public void HideMediumBanner()
    {

        if (isMBannerDisplayed)
        {
            Logging.Log("GG >> AppLovin:mediumBanner:Hide");
            AdsLogsHelper.Logs(Ads_Events.MediumBannerHide);
        }
        isMBannerDisplayed = false;
        MaxSdk.HideMRec(RectBannerID);
        AdmobeAdsManager.instance.HideMediumBanner();
        //Hide_CP_MediumBanner();
    }
    #endregion




    #region
    private float lastInterstitialTime = -999f;
    private float lastAppOpenAdTime = -999f;
    private float lastRewardedTime = -999f;
    private float DelayedAD = -999f;

    private const float INTERSTITIAL_COOLDOWN = 10f; // 2 minutes
    private const float APP_OPEN_COOLDOWN = 5f;
    private const float DelayedAD_COOLDOWN = 20f;
    private const float REWARDED_BLOCK_DURATION = 10f;
    private const float INTERSTITIAL_BLOCK_AFTER_REWARDED = 10f;
    private const float APPOPEN_BLOCK_AFTER_INTERSTITIAL = 15f;
    private const float DelayedAD_BLOCK_AFTER_INTERSTITIAL = 15f;


    private const float AUTO_INTERSTITIAL_TIMEOUT = 30f;
    private bool autoInterstitialTriggered = false;

    public bool CanShowInterstitial()
    {

        float now = Time.unscaledTime;
        Debug.Log("Last intrertitial time " + lastInterstitialTime/*(now - lastInterstitialTime >= INTERSTITIAL_COOLDOWN)*/);

        if (now - lastRewardedTime < INTERSTITIAL_BLOCK_AFTER_REWARDED)
            return false;

        return (now - lastInterstitialTime >= INTERSTITIAL_COOLDOWN);
    }

    public bool CanShowAppOpenAd()
    {
        float now = Time.unscaledTime;

        if (now - lastInterstitialTime < APPOPEN_BLOCK_AFTER_INTERSTITIAL)
            return false;

        if (now - lastRewardedTime < REWARDED_BLOCK_DURATION)
            return false;

        return (now - lastAppOpenAdTime >= APP_OPEN_COOLDOWN);
    }

    public bool CanShowInterstitialOnDelayed()
    {

        float now = Time.unscaledTime;
        Debug.Log("DelayedAD_COOLDOWN " + DelayedAD/*(now - lastInterstitialTime >= INTERSTITIAL_COOLDOWN)*/);

        if (now - DelayedAD < DelayedAD_BLOCK_AFTER_INTERSTITIAL)
            return false;

        return (now - DelayedAD >= DelayedAD_COOLDOWN);
    }

    public void OnInterstitialShown()
    {
        Debug.Log("lastRewardedTime" + lastRewardedTime);
        lastInterstitialTime = Time.unscaledTime;
    }

    public void OnAppOpenAdShown()
    {
        Debug.Log("lastAppOpenAdTime" + lastAppOpenAdTime);
        lastAppOpenAdTime = Time.unscaledTime;
    }

    public void OnInterstitialDelayedADShown()
    {
        Debug.Log("DelayedAD" + DelayedAD);
        DelayedAD = Time.unscaledTime;
    }

    public void OnRewardedVideoShown()
    {

        lastRewardedTime = Time.unscaledTime;
    }



    private void Update()
    {
        //if (!isIdleTimeCompleted) { IdleInterstitialLogic(); }
        if(PlayerPrefs.GetInt("RemoveAds")==0&&runAdsTimer)
            IdleInterstitialLogic();
        //float now = Time.unscaledTime;

        //if (!autoInterstitialTriggered &&
        //    now - lastInterstitialTime >= AUTO_INTERSTITIAL_TIMEOUT &&
        //    CanShowInterstitial())
        //{
        //    Debug.Log("Auto showing interstitial after 120 seconds.");
        //    ShowInterstitial(); // Replace with your ad show logic
        //    OnInterstitialShown();
        //    autoInterstitialTriggered = true;
        //}
    }

    #endregion


    #region Idle Ad
    //public float showInterstialAfterTheTime = 180f;
    /*[HideInInspector] */
    public float timeForInterstitialAd;
    public float solidAdTime = 20f;
    public GameObject interstitialPopup/*, adBreakPopup*/;
    public Text adTimer;
    //public bool canShowAd = true;
    [HideInInspector] public bool isIdleTimeCompleted = false;
    bool startFading = false;
    [HideInInspector] public int interstitialCount = 0;
    void IdleInterstitialLogic()
    {
        solidAdTime -= Time.deltaTime;
        if (!isIdleTimeCompleted)
        {
        timeForInterstitialAd -= Time.deltaTime;
            if (timeForInterstitialAd < 6)
            {
                if (timeForInterstitialAd <= 2)
                {
                    isIdleTimeCompleted = true;
                    Debug.Log("Interstitial Showed");
                }
            }
        }
        if (solidAdTime < loadingAdTextTime+1)
        {
            if (IsInterstitialAdReady())
            {
                if (!startFading)
                {
                    startFading = true;
                    StartCoroutine(FadeInPopup(interstitialPopup.GetComponent<CanvasGroup>(), 1.5f));
                    interstitialPopup.SetActive(true);
                    adTimer.gameObject.SetActive(true);

                }
            }
            adTimer.text = "AD IS LOADING... " + ((int)solidAdTime).ToString();
            if (solidAdTime < 1)
            {
                solidAdTime = idleAdTime + InterGraceTime;
                interstitialPopup.SetActive(false);
                adTimer.gameObject.SetActive(false);
                startFading = false;
                ShowFallBackInterstitialAlternate();

            }
        }
        else
        {
            if (!isIdleTimeCompleted)
            {
                interstitialPopup.SetActive(false);
                adTimer.gameObject.SetActive(false);
            }
        }
    }
    IEnumerator FadeInPopup(CanvasGroup canvasGroup, float duration)
    {
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    IEnumerator CheckInternetConnection()
    {
        checkInternetMsg.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        //checkInternetMsg.SetActive(false);
    }
    public void OpenWifiSetting()
    {
        
    }

    #endregion
}
