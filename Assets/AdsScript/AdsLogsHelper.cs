using UnityEngine;
public static class Logging
{
    static public void Log(object message)
    {
       // UnityEngine.Debug.Log(message);
    }
}
public class AdsLogsHelper : MonoBehaviour
{
    public static void Logs(Ads_Events log)
    {
        switch (log)
        {
            //Initalizing
            case Ads_Events.Initializing:
                LogEvent("AppLovin_Initializing");
                break;
            case Ads_Events.Initialized:
                LogEvent("AppLovin_Initialized");
                break;

            //Request
            case Ads_Events.LoadInterstitial:
                LogEvent("AppLovin_iAd_Request");
                break;
            case Ads_Events.LoadRewardedAd:
                LogEvent("AppLovin_rAd_Request");
                break;
            case Ads_Events.LoadSmallBanner:
                LogEvent("AppLovin_SB_Request");
                break;
            case Ads_Events.LoadSmallAdaptiveBanner:
                LogEvent("AppLovin_AB_Request");
                break;
            case Ads_Events.LoadMediumBanner:
                LogEvent("AppLovin_MB_Request");
                break;

            //LOADED
            case Ads_Events.InterstitialAdLoaded:
                LogEvent("AppLovin_iAd_Loaded");
                break;
            case Ads_Events.RewardedAdLoaded:
                LogEvent("AppLovin_rAd_Loaded");
                break;
            case Ads_Events.SmallBannerLoaded:
                LogEvent("AppLovin_SB_Loaded");
                break;

            case Ads_Events.SmallAdaptiveBannerLoaded:
                LogEvent("AppLovin_AB_Loaded");
                break;

            case Ads_Events.MediumBannerLoaded:
                LogEvent("AppLovin_MB_Loaded");
                break;

            //Show Call
            case Ads_Events.ShowInterstitialAd:
                LogEvent("AppLovin_iAd_ShowCall");
                break;


            case Ads_Events.ShowRewardedAd:
                LogEvent("AppLovin_rAd_ShowCall");
                break;

            case Ads_Events.ShowSmallBanner:
                LogEvent("AppLovin_SB_ShowCall");
                break;

            case Ads_Events.ShowSmallAdaptiveBanner:
                LogEvent("AppLovin_AB_ShowCall");
                break;

            case Ads_Events.ShowMediumBanner:
                LogEvent("AppLovin_MB_ShowCall");
                break;

            //Will Display
            case Ads_Events.InterstitialAdWillDisplay:
                LogEvent("AppLovin_iAd_WillDisplay");
                break;

            case Ads_Events.RewardedAdWillDisplay:
                LogEvent("AppLovin_rAd_WillDisplay");
                break;
            case Ads_Events.SmallBannerWillDisplay:
                LogEvent("AppLovin_smalllBanner_WillDisplay");
                break;
            case Ads_Events.MediumBannerWillDisplay:
                LogEvent("AppLovin_mediumBanner_WillDisplay");
                break;


            //Displayed
            case Ads_Events.InterstitialAdDisplayed:
                LogEvent("AppLovin_iAd_Displayed");
                break;



            case Ads_Events.RewardedAdDisplayed:
                LogEvent("AppLovin_rAd_Displayed");
                break;

                break;
            case Ads_Events.SmallBannerDisplayed:
                LogEvent("AppLovin_SB_Displayed");
                break;

            case Ads_Events.SmallAdaptiveBannerDisplayed:
                LogEvent("AppLovin_AB_Displayed");
                break;



            case Ads_Events.MediumBannerDisplayed:
                LogEvent("AppLovin_MB_Displayed");
                break;

            //Rewarded Ad Started
            case Ads_Events.RewardedAdStarted:
                LogEvent("AppLovin_rAd_Started");
                break;

            //Rewarded Ad Give Reward
            case Ads_Events.RewardedAdReward:
                LogEvent("AppLovin_rAd_Reward");
                break;

            case Ads_Events.RewardedAdFailedToShow:
                LogEvent("AppLovin_rAd_RewardFailedToShow");
                break;
            case Ads_Events.InterstitialAdFailedToShow:
                LogEvent("AppLovin_iAd_RewardFailedToShow");
                break;
            //No Inventory
            case Ads_Events.RewardedAdNoInventory:
                LogEvent("AppLovin_rAd_NoInventory");
                break;

            case Ads_Events.InterstitialAdNoInventory:
                LogEvent("AppLovin_iAd_NoInventory");
                break;
            case Ads_Events.SmallBannerNoInventory:
                LogEvent("AppLovin_SB_NoInventory");
                break;


            case Ads_Events.SmallAdaptiveBannerNoInventory:
                LogEvent("AppLovin_AB_NoInventory");
                break;




            case Ads_Events.MediumBannerNoInventory:
                LogEvent("AppLovin_MB_NoInventory");
                break;

            //Ad Close
            case Ads_Events.InterstitialAdClosed:
                LogEvent("AppLovin_iAd_Closed");
                break;


            case Ads_Events.RewardedAdClosed:
                LogEvent("AppLovin_rAd_Closed");
                break;


            //Ad Clicked
            case Ads_Events.InterstitialAdClicked:
                LogEvent("AppLovin_iAd_Clicked");
                break;
            case Ads_Events.RewardedAdClicked:
                LogEvent("AppLovin_rAd_Clicked");
                break;

            //Adapters Register
            case Ads_Events.AdaptersInitialized:
                LogEvent("AppLovin_Adapters_Initialized");
                break;

            //Adapters Not Register
            case Ads_Events.AdaptersNotInitialized:
                LogEvent("AppLovin_NotInitialized");
                break;
            case Ads_Events.InterstialOffline:
                Debug.Log("GG >> AppLovin_iad_ShowCall_Offline");

                break;
            case Ads_Events.AdapterNotReadyNoInternet:
                LogEvent("AppLovin_NotReadyNoInternet");
                break;
            case Ads_Events.offlineInit:
                LogEvent("AppLovin_OfflineInit");
                break;
            case Ads_Events.InterstitialNotLoaded:
                LogEvent("AppLovin_iad_NotLoaded");
                break;
            case Ads_Events.RewardedAdNotLoaded:
                LogEvent("AppLovin_iad_NotLoaded");
                break;
            case Ads_Events.SmallBannerNotLoaded:
                LogEvent("AppLovin_smallBanner_NotLoaded");
                break;
            case Ads_Events.MediumBannerNotLoaded:
                LogEvent("AppLovin_mediumBanner_NotLoaded");
                break;
            case Ads_Events.SmallBannerHide:
                LogEvent("AppLovin_SB_Hide");
                break;
            case Ads_Events.MediumBannerHide:
                LogEvent("AppLovin_MB_Hide");
                break;
            case Ads_Events.AppOpenLoadRequest:
                LogEvent("AppLovin_AppOpen_LoadRequest");
                break;
            case Ads_Events.AppOpenLoaded:
                LogEvent("AppLovin_AppOpen_Loaded");
                break;
            case Ads_Events.AppOpenNotLoaded:
                LogEvent("AppLovin_AppOpen_NotLoaded");
                break;
            case Ads_Events.AppOpenClosed:
                LogEvent("AppLovin_AppOpen_Closed");
                break;
            case Ads_Events.AppOpenShowCall:
                LogEvent("AppLovin_AppOpen_ShowCall");
                break;
            case Ads_Events.AppOpenNoInventory:
                LogEvent("AppLovin_AppOpen_NoInventory");
                break;
            case Ads_Events.AppOpenDisplayed:
                LogEvent("AppLovin_AppOpen_Displayed");
                break;
            case Ads_Events.AppOpenClicked:
                LogEvent("AppLovin_AppOpen_Clicked");
                break;
            case Ads_Events.AppOpenFailedToShow:
                LogEvent("AppLovin_AppOpen_FailedToShow");
                break;
            case Ads_Events.AppOpenWillDisplay:
                LogEvent("AppLovin_AppOpen_WillDisplay");
                break;
        }
    }

    public static void LogEvent(string log)
    {
      //  Constant.LogDesignEvent(log);
    }
}
