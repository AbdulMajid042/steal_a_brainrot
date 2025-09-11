//using GoogleMobileAds.Api;

//public delegate void AdmobRewardUserDelegate(object sender, Reward args);

public enum Ads_Events
{
    Initializing,
    Initialized,

    LoadInterstitial,
    LoadRewardedAd,
    InterstitialAdLoaded,
    RewardedAdLoaded,

    ShowInterstitialAd,
    ShowRewardedAd,


    InterstitialAdWillDisplay,
    RewardedAdWillDisplay,
    SmallBannerWillDisplay,
    MediumBannerWillDisplay,

    InterstitialAdDisplayed,
    RewardedAdDisplayed,

    InterstitialAdNoInventory,
    RewardedAdNoInventory,

    RewardedAdStarted,

    RewardedAdReward,

    InterstitialAdClicked,
    RewardedAdClicked,

    InterstitialAdClosed,
    RewardedAdClosed,
    RewardedInterstitialAdClosed,

    AdaptersInitialized,
    AdaptersNotInitialized,

    LoadSmallBanner,

    LoadSmallAdaptiveBanner,

    LoadMediumBanner,

    ShowSmallBanner,

    ShowSmallAdaptiveBanner,
    ShowMediumBanner,

    SmallBannerLoaded,

    SmallAdaptiveBannerLoaded,
    MediumBannerLoaded,

    SmallBannerNoInventory,

    SmallAdaptiveBannerNoInventory,
    MediumBannerNoInventory,

    SmallBannerDisplayed,
    SmallAdaptiveBannerDisplayed,
    MediumBannerDisplayed,

    SmallBannerClicked,
    SmallAdaptiveBannerClicked,
    MediumBannerClicked,

    SmallBannerClosed,
    SmallAdaptiveBannerClosed,
    MediumBannerClosed,
    SmallBannerOflline, InterstialOffline, RewardOFline, AdapterNotReadyNoInternet, offlineInit,
    InterstitialAdFailedToShow, RewardedAdFailedToShow,
    InterstitialNotLoaded, RewardedAdNotLoaded, SmallBannerNotLoaded, MediumBannerNotLoaded,
    SmallBannerHide, MediumBannerHide,

    AppOpenLoadRequest, AppOpenLoaded, AppOpenNotLoaded, AppOpenClosed, AppOpenShowCall, AppOpenNoInventory,
    AppOpenDisplayed, AppOpenClicked, AppOpenFailedToShow, AppOpenWillDisplay,

}
