using Firebase.Sample.Analytics;
using GoogleMobileAds.Api;
//using GoogleMobileAds.Editor;
using SolarEngine;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class AdmobeAdsManager : MonoBehaviour
{
    public static AdmobeAdsManager instance;

    // === Ad Unit IDs ===
    [Header("Ad Unit IDs")]
    public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";   // Test ID
    public string interstitialAdUnitId2 = "ca-app-pub-3940256099942544/8691691433";  // Test ID
    public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";       // Test ID
    public string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";         // Test ID
    public string mrecAdUnitId = "ca-app-pub-3940256099942544/6300978111";           // Same banner size but placed differently
    public string appOpenAdUnitId = "ca-app-pub-3940256099942544/3419835294";        // Test ID

    // === Ad References ===
    private InterstitialAd interstitialAd;
    private InterstitialAd interstitialAd2;
    private RewardedAd rewardedAd;
    private BannerView bannerView;
    private BannerView mrecView;
    private AppOpenAd appOpenAd;

    void Start()
    {
        instance = this;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Object is destroying ");
            Destroy(gameObject);
        }

        Invoke(nameof(Init), 0.1f);
    }

    private void Init()
    {
        MobileAds.Initialize(initStatus =>
        {
            if (initStatus == null)
            {
                Debug.LogError("AdMob failed to initialize.");
                return;
            }

            Debug.Log("AdMob initialized successfully.");
            if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("admob_Initialized");

            LoadInterstitial();
            LoadInterstitial2();
            LoadRewarded();
            LoadAppOpenAd();
            LoadBanner(AdPosition.Top);
            //bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        });
    }

    #region Interstitial 1
    private void LoadInterstitial()
    {
        Debug.Log("Loading Interstitial 1...");
        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Failed to load interstitial 1: " + error.GetMessage().ToString());
                    if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("inter1_failed_admob_med" + error.GetMessage().ToString());
                    return;
                }

                Debug.Log("Interstitial 1 loaded.");
                interstitialAd = ad;
                interstitialAd.OnAdFullScreenContentClosed += HandleAdClosed;
                interstitialAd.OnAdPaid += (AdValue adValue) =>
                {
                    OnAdmobRevenuePaid(interstitialAdUnitId, "INTER", adValue);
                };
            });
    }

    private void HandleAdClosed()
    {
        Debug.Log("Interstitial 1 closed, loading again...");
        Invoke(nameof(EnableForeGround), 2.5f);
        LoadInterstitial();
    }
    #endregion

    #region Interstitial 2
    public void LoadInterstitial2()
    {
        Debug.Log("Loading Interstitial 2...");
        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId2, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Failed to load interstitial 2: " + error);
                    if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("inter2_failed_admob_med" + error.ToString());

                    return;
                }

                Debug.Log("Interstitial 2 loaded.");
                interstitialAd2 = ad;
                interstitialAd2.OnAdFullScreenContentClosed += HandleAdClosed2;

                interstitialAd.OnAdPaid += (AdValue adValue) =>
                {
                    OnAdmobRevenuePaid(interstitialAdUnitId, "INTER", adValue);
                };
            });

    }
    public bool IsInterstitial2Ready()
    {
        return interstitialAd2 != null;
    }
    public void ShowInterstitial2()
    {
        AdsManagerWrapper.Instance.ResetTimer();
        AdsManagerWrapper.Instance.interstitialCount++;
        Debug.Log("Interstitial 2 is showing ");

        if (!PreferenceManager.GetAdsStatus() || AdsManagerWrapper.adsStatus == AdType.NOAds)
        {
            Debug.Log("Interstitial 2 is no Ads zone ");
            return;
        }


        if (IsInterstitial2Ready())
        {
            if (AdsManagerWrapper.Instance)
            {
                AdsManagerWrapper.ForeGroundedAD = true;
                AdsManagerWrapper.Instance.ResetTimer();
            }
            Debug.Log("admob_backup_interstitial_shown");
            interstitialAd2.Show();
            if (GRS_FirebaseHandler.Instance)
                GRS_FirebaseHandler.Instance.LogEventPlay("admob_backup_interstitial_shown");
        }

        else
        {
            Debug.Log("Interstitial 2 not ready, loading...");
            //AdsManagerWrapper.Instance.ShowInterstitial();
            LoadInterstitial2();
        }
    }
    public void ShowFallBackAdmobInterstitial()
    {
        AdsManagerWrapper.Instance.ResetTimer();
        AdsManagerWrapper.Instance.interstitialCount++;

        if (!PreferenceManager.GetAdsStatus() || AdsManagerWrapper.adsStatus == AdType.NOAds)
        {
            return;
        }


        if (IsInterstitial2Ready())
        {
            if (AdsManagerWrapper.Instance)
            {
                AdsManagerWrapper.ForeGroundedAD = true;
                AdsManagerWrapper.Instance.ResetTimer();
            }
            interstitialAd2.Show();
            if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("admobmed_fallbackInterAd_shown");
        }
        else
        {
            Debug.Log("Interstitial 2 not ready, loading...");
            //AdsManagerWrapper.Instance.ShowInterstitial();
            LoadInterstitial2();
        }
    }
    private void HandleAdClosed2()
    {
        Debug.Log("Interstitial 1 closed, loading again...");
        Invoke(nameof(EnableForeGround), 2.5f);
        AdsManagerWrapper.Instance.OnInterstitialShown();
        AdsManagerWrapper.Instance.isIdleTimeCompleted = false;
        AdsManagerWrapper.Instance.timeForInterstitialAd = AdsManagerWrapper.Instance.idleAdTime;
        AdsManagerWrapper.Instance.solidAdTime = AdsManagerWrapper.Instance.timeForInterstitialAd + AdsManagerWrapper.Instance.InterGraceTime;
        if (AdsManagerWrapper.Instance.GetAppOpenRemainingTime() <= 15)
        {
            AdsManagerWrapper.Instance.OnAdClosed(15);
        }
        LoadInterstitial2();
    }
    #endregion

    #region Rewarded
    public void LoadRewarded()
    {
        Debug.Log("Loading Rewarded...");
        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load rewarded: " + error.GetMessage().ToString());
                return;
            }

            Debug.Log("Rewarded loaded.");
            rewardedAd = ad;

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("AdMob Rewarded closed, reloading...");

                LoadRewarded();  // auto reload after use
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                Debug.LogError("AdMob Rewarded failed to show: " + err);
                if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("rewarded_failed_admob_med" + error.ToString());
                LoadRewarded();
            };
            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                OnAdmobRevenuePaid(rewardedAdUnitId, "REWARDED", adValue);
            };
        });
    }

    void OnRewardedAdComplete()
    {
        if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("admob_rewarded_ad_completed");
        AdsManagerWrapper.Instance.isIdleTimeCompleted = false;
        if (AdsManagerWrapper.Instance.timeForInterstitialAd <= 15)
        {
            AdsManagerWrapper.Instance.timeForInterstitialAd = 15;
            AdsManagerWrapper.Instance.solidAdTime = AdsManagerWrapper.Instance.timeForInterstitialAd + AdsManagerWrapper.Instance.InterGraceTime;
        }

        if (AdsManagerWrapper.Instance.GetAppOpenRemainingTime() <= 15)
        {
            AdsManagerWrapper.Instance.OnAdClosed(15);
        }


        Time.timeScale = 1;
        Debug.Log("Admob Rewarded Video Completed Rewarded");
    }
    public bool IsAdmobRewardedReady()
    {
        return rewardedAd != null;
    }

    public void ShowRewarded(Action onRewardEarned = null)
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("User earned reward: " + reward.Amount);
                if (GRS_FirebaseHandler.Instance)
                    GRS_FirebaseHandler.Instance.LogEventPlay("admob_backup_rewarded_shown");
                onRewardEarned?.Invoke();
                OnRewardedAdComplete();
            });
        }
        else
        {
            Debug.Log("Rewarded not ready, loading...");
            LoadRewarded();
        }
    }
    #endregion

    #region Banner
    private bool isBannerLoaded = false;
    public void LoadBanner(AdPosition adPosition = AdPosition.Bottom)
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, adPosition);

        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("AdMob Banner Loaded ");

            isBannerLoaded = true;
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.Log("AdMob Banner Failed : " + error);
            if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("ban_failed_admob_med" + error.ToString());
            if (AdsManagerWrapper.Instance) AdsManagerWrapper.Instance.ShowMaxBanner();
            AdsManagerWrapper.Instance.currentBanner = AdsManagerWrapper.BannerSource.Max;
            isBannerLoaded = false;
        };

        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("AdMob Banner closed by user.");
        };
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            OnAdmobRevenuePaid(bannerAdUnitId, "BANNER", adValue);
        };

        AdRequest request = new AdRequest();

        bannerView.LoadAd(request);
    }

    public bool IsBannerReady()
    {
        return isBannerLoaded;
    }

    public void ShowBanner()
    {
        if (bannerView != null/* && isBannerLoaded*/)
        {
            bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Top);
            bannerView.Show();
            if (GRS_FirebaseHandler.Instance)
                GRS_FirebaseHandler.Instance.LogEventPlay("admob_backup_banner_shown");

        }
        else
        {
            Debug.LogWarning("AdMob Banner not ready, call LoadBanner first.");
        }
    }

    public void HideBanner()
    {
        Debug.Log("Hide Admobe Banner Called");
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }
    #endregion

    #region MREC
    public void ShowMREC()
    {
        if (mrecView != null)
        {
            mrecView.Destroy();
            mrecView = null;
        }

        mrecView = new BannerView(mrecAdUnitId, AdSize.MediumRectangle, AdPosition.BottomLeft);

        // Subscribe to events
        mrecView.OnBannerAdLoaded += () =>
        {
            Debug.Log("MREC loaded successfully.");
            if (GRS_FirebaseHandler.Instance)
                GRS_FirebaseHandler.Instance.LogEventPlay("admob_backup_mrec_shown");
        };

        mrecView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("MREC failed to load: " + error);

            //AdsManagerWrapper.Instance.ShowMaxMediumBanner();
            GRS_FirebaseHandler.Instance.LogEventPlay("admob_mrec_failed_" + error.GetMessage());
        };
        mrecView.OnAdPaid += (AdValue adValue) =>
        {
            OnAdmobRevenuePaid(mrecAdUnitId, "MREC", adValue);
        };

        // Load the ad
        AdRequest request = new AdRequest();
        mrecView.LoadAd(request);
        mrecView.Show();
        Debug.Log("MREC requested.");
    }
    //public void ShowMREC()
    //{
    //    if (mrecView != null)
    //    {
    //        mrecView.Destroy();
    //    }

    //    mrecView = new BannerView(mrecAdUnitId, AdSize.MediumRectangle, AdPosition.Center);
    //    AdRequest request = new AdRequest();
    //    mrecView.LoadAd(request);
    //    Debug.Log("MREC shown.");
    //}

    public void HideMediumBanner()
    {
        if (mrecView != null) mrecView.Destroy();
    }
    #endregion

    #region AppOpen

    private void LoadAppOpenAd()
    {
        Debug.Log("Loading AppOpenAd...");

        AdRequest request = new AdRequest();

        AppOpenAd.Load(appOpenAdUnitId, request,
            (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Failed to load AppOpenAd: " + error);
                    if (GRS_FirebaseHandler.Instance)
                        GRS_FirebaseHandler.Instance.LogEventPlay("appOpen_failed_admob_med" + error.ToString());
                    return;
                }

                Debug.Log("AppOpenAd loaded successfully.");
                appOpenAd = ad;

                // Fired when ad is shown (opened)
                appOpenAd.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("AppOpenAd Shown");
                    if (GRS_FirebaseHandler.Instance)
                        GRS_FirebaseHandler.Instance.LogEventPlay("admob_appopen_shown");
                    //AppOpenShownEvent();
                };

                // Fired when ad is closed
                appOpenAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("AppOpenAd closed. Reloading...");
                    AppOpenCloseEvent();
                };

                // Fired when ad revenue is paid
                appOpenAd.OnAdPaid += (AdValue adValue) =>
                {
                    OnAdmobRevenuePaid(appOpenAdUnitId, "APPOPEN", adValue);
                };
            });
    }

    void AppOpenCloseEvent()
    {
        //AdsManagerWrapper.Instance.ShowSmallBanner();
        LoadAppOpenAd();

        AdsManagerWrapper.Instance.isIdleTimeCompleted = false;
        if (AdsManagerWrapper.Instance.timeForInterstitialAd <= 15)
        {
            AdsManagerWrapper.Instance.timeForInterstitialAd = 15;
            AdsManagerWrapper.Instance.solidAdTime = AdsManagerWrapper.Instance.timeForInterstitialAd + AdsManagerWrapper.Instance.InterGraceTime;
        }

        AdsManagerWrapper.Instance.OnAdClosed((int)AdsManagerWrapper.Instance.startAppOpenTime);
    }

    public bool IsAppOpenAdReady()
    {
        return appOpenAd != null;
    }
    public void ShowAppOpenAd()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Show();
        }
        else
        {
            Debug.Log("AppOpenAd not ready. Loading...");
            LoadAppOpenAd();
        }
    }
    #endregion

    void EnableForeGround()
    {
        AdsManagerWrapper.ForeGroundedAD = true;
    }

    #region Interstitial 1 Show
    public bool IsInterstitialReady()
    {
        return interstitialAd != null;
    }

    public void ShowInterstitial()
    {
        if (!PreferenceManager.GetAdsStatus() || AdsManagerWrapper.adsStatus == AdType.NOAds)
        {
            return;
        }

        if (IsInterstitialReady())
        {
            if (AdsManagerWrapper.Instance)
            {
                AdsManagerWrapper.ForeGroundedAD = true;
                AdsManagerWrapper.Instance.ResetTimer();
            }
            if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.Instance.LogEventPlay("admob_med_int_shown");
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready, loading...");
            LoadInterstitial();
        }
    }
    #endregion
    private void OnAdmobRevenuePaid(string adUnitId, string adFormat, AdValue adValue)
    {
        double revenue = adValue.Value / 1000000.0; // micros  USD
        string currency = adValue.CurrencyCode;     // usually "USD"

        // --- Firebase logging (optional, same as MAX) ---
        var impressionParameters = new[]
        {
        new Firebase.Analytics.Parameter("ad_platform", "AdMob"),
        new Firebase.Analytics.Parameter("ad_source", "AdMob"), // Or map network name if available
        new Firebase.Analytics.Parameter("ad_unit_name", adUnitId),
        new Firebase.Analytics.Parameter("ad_format", adFormat),
        new Firebase.Analytics.Parameter("value", revenue),
        new Firebase.Analytics.Parameter("currency", currency),
    };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_Admob", impressionParameters);
        // --- Solar Engine ---
        ImpressionAttributes impressionAttributes = new ImpressionAttributes();
        impressionAttributes.ad_platform = "AdMob"; // platform name
        impressionAttributes.ad_appid = "ca-app-pub-6263347419612757~4821112022"; // replace with real app id
        impressionAttributes.mediation_platform = "AdMob"; // since not mediated by MAX
        impressionAttributes.ad_id = adUnitId;
        impressionAttributes.ad_type = (int)AdsManagerWrapper.Instance.SolarAdType(adFormat); // convert AdMob format  Solar enum
        impressionAttributes.ad_ecpm = revenue * 1000.0; // Solar expects eCPM
        impressionAttributes.currency_type = currency;
        impressionAttributes.is_rendered = true;

        SolarEngine.Analytics.trackAdImpression(impressionAttributes);

        Debug.Log($"[AdMob  SolarEngine] Ad: {adFormat}, Revenue: {revenue} {currency}");
    }


    //private SolarAdType SolarAdType(string adFormat)
    //{
    //    switch (adFormat.ToLower())
    //    {
    //        case "banner": return AdsManagerWrapper.Instance.SolarAdType.BANNER;
    //        case "mrec": return SolarAdType.MREC;
    //        case "interstitial": return SolarAdType.INTERSTITIAL;
    //        case "rewarded": return SolarAdType.REWARDED;
    //        case "appopen": return SolarAdType.APP_OPEN;
    //        default: return SolarAdType.UNKNOWN;
    //    }
    //}
}
